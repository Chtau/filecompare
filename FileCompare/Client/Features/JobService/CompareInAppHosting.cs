﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Features.Jobs;
using Client.Features.Jobs.Models;

namespace Client.Features.JobService
{
    public class CompareInAppHosting : ICompare
    {
        public event EventHandler<JobState> JobStateChanged;

        private readonly Compare.Duplicates duplicates;
        private readonly Internal.ILogger _logger;
        private readonly Jobs.IJobRepository _repository;
        private readonly Folders.IFolderRepository _folderRepository;
        private readonly IJobServiceRepository _jobServiceRepository;
        private readonly Duplicates.IDuplicatesManager _duplicatesManager;
        private readonly IMainManager _mainManager;


        public CompareInAppHosting()
        {
            duplicates = new Compare.Duplicates();
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (Jobs.IJobRepository)Bootstrap.Instance.Services.GetService(typeof(Jobs.IJobRepository));
            _folderRepository = (Folders.IFolderRepository)Bootstrap.Instance.Services.GetService(typeof(Folders.IFolderRepository));
            _jobServiceRepository = (IJobServiceRepository)Bootstrap.Instance.Services.GetService(typeof(IJobServiceRepository));
            _duplicatesManager = (Duplicates.IDuplicatesManager)Bootstrap.Instance.Services.GetService(typeof(Duplicates.IDuplicatesManager));
            _mainManager = (IMainManager)Bootstrap.Instance.Services.GetService(typeof(IMainManager));
            _mainManager.ApplicationClosing += _mainManager_ApplicationClosing;
        }

        private void _mainManager_ApplicationClosing(object sender, EventArgs e)
        {
            try
            {
                var task = Task.Run(async () =>
                {
                    var jobs = await _repository.GetJobsByState(JobState.Running);
                    if (jobs?.Count > 0)
                    {
                        for (int i = 0; i < jobs.Count; i++)
                        {
                            jobs[i].JobState = JobState.Idle;
                            await _repository.Update(jobs[i]);
                        }
                    }
                });
                task.Wait();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                
            }
        }

        public bool StartJob(Job job, JobConfiguration config)
        {
            Task.Run(async () =>
            {
                _mainManager.SetStatusBarInfoText("Job running");
                await _jobServiceRepository.ClearPathDuplicate(job.Id);
                int lastCompareFiles = 0;
                var paths = await _repository.GetJobCollectPath(job.Id);
                List<string> pathsToCollect = new List<string>();
                foreach (var item in paths)
                {
                    pathsToCollect.Add(item.Path);
                }
                var cache = new Dictionary<string, Compare.CompareValue>();
                var pathCompare = await _jobServiceRepository.Gets(pathsToCollect.ToArray());
                foreach (var item in pathCompare)
                {
                    cache.Add(item.FullFile, new Compare.CompareValue
                    {
                        Directory = item.Directory,
                        Extension = item.Extension,
                        FileName = item.FileName,
                        Hash = item.Hash
                    });
                }
                duplicates.SetCache(cache);

                duplicates.PrepareCompareValuesProgressWithItems += (object sender, Compare.Duplicates.PrepareComareProgressItem e) =>
                {
                    if (!isInSaveCompareFiles)
                    {
                        if (e.CompareFiles.Count > (lastCompareFiles + 20))
                        {
                            OnSaveCompareFiles(e.CompareFiles).GetAwaiter().GetResult();
                            _mainManager.SetStatusBarInfoText($"Job collect files ({e.Progress}%)");
                        }
                    }
                    if (e.Progress == 100)
                    {
                        try
                        {
                            _mainManager.SetStatusBarInfoText($"Job collect files ({e.Progress}%)");
                            if (!isInSaveCompareFiles)
                            {
                                OnSaveCompareFiles(e.CompareFiles).GetAwaiter().GetResult();
                            } else
                            {
                                Task.Run(async () =>
                                {
                                    do
                                    {
                                        if (isInSaveCompareFiles)
                                            await Task.Delay(100);
                                        if (!isInSaveCompareFiles)
                                        {
                                            await OnSaveCompareFiles(e.CompareFiles);
                                        }
                                    } while (!isInSaveCompareFiles);
                                });
                            }
                        } catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                    lastCompareFiles = e.CompareFiles.Count;
                };
                await duplicates.Collect(pathsToCollect.ToArray());
                await OnAfterCollect(job, config);
            });
            return true;
        }

        private async Task OnAfterCollect(Job job, JobConfiguration config)
        {
            _mainManager.SetStatusBarInfoText($"Job find duplicates");
            var result = await duplicates.Find();
            _mainManager.SetStatusBarInfoText($"Finish job");
            await OnComplete(result, job, config);
            _mainManager.SetStatusBarInfoText(null);
        }

        public bool StopJob(Job job, JobConfiguration config)
        {
            return true;
        }

        private bool isInSaveCompareFiles = false;

        private async Task OnSaveCompareFiles(Dictionary<string, Compare.CompareValue> compareFiles)
        {
            isInSaveCompareFiles = true;
            try
            {
                foreach (var item in compareFiles)
                {
                    var comp = await _jobServiceRepository.Find(item.Key.ToUpper());
                    if (comp != null)
                    {
                        // update
                        // we don't execute updates
                        /*comp.Hash = item.Value.Hash;
                        comp.LastChange = DateTime.Now;
                        await _jobServiceRepository.Update(comp);*/
                    } else
                    {
                        // insert
                        await _jobServiceRepository.Insert(new Models.PathCompareValue
                        {
                            Id = Guid.NewGuid(),
                            Directory = item.Value.Directory.ToUpper(),
                            Extension = item.Value.Extension.ToUpper(),
                            FileName = item.Value.FileName.ToUpper(),
                            FullFile = item.Key.ToUpper(),
                            Hash = item.Value.Hash,
                            LastChange = DateTime.Now
                        });
                    }
                    await _folderRepository.UpdateFolders(item.Value.Directory.ToUpper());
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
            } finally
            {
                isInSaveCompareFiles = false;
            }
        }

        private async Task OnComplete(List<Compare.DuplicatesResult> duplicatesResults, Job job, JobConfiguration config)
        {
            try
            {
                foreach (var item in duplicatesResults)
                {
                    Models.DuplicateValue duplicateValue = null;

                    foreach (var result in item.FileResults)
                    {
                        if (duplicateValue == null)
                        {
                            duplicateValue = await _jobServiceRepository.CreateDuplicateValue((int)result.CompareValue);
                        }
                        await _jobServiceRepository.CreatePathDuplicate(job.Id, duplicateValue.Id, result.FilePath);
                    }
                }

                job.JobState = Jobs.JobState.Idle;
                await _repository.Update(job);
                JobStateChanged?.Invoke(this, job.JobState);
                _duplicatesManager.RaiseRefreshData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
