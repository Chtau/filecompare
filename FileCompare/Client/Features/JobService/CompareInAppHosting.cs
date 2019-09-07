using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Features.Jobs;
using Client.Features.Jobs.Models;
using Newtonsoft.Json;

namespace Client.Features.JobService
{
    public class CompareInAppHosting : ICompare
    {
        private Dictionary<Job, CancellationTokenSource> jobTasks;

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
            jobTasks = new Dictionary<Job, CancellationTokenSource>();
            duplicates = new Compare.Duplicates();
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (Jobs.IJobRepository)Bootstrap.Instance.Services.GetService(typeof(Jobs.IJobRepository));
            _folderRepository = (Folders.IFolderRepository)Bootstrap.Instance.Services.GetService(typeof(Folders.IFolderRepository));
            _jobServiceRepository = (IJobServiceRepository)Bootstrap.Instance.Services.GetService(typeof(IJobServiceRepository));
            _duplicatesManager = (Duplicates.IDuplicatesManager)Bootstrap.Instance.Services.GetService(typeof(Duplicates.IDuplicatesManager));
            _mainManager = (IMainManager)Bootstrap.Instance.Services.GetService(typeof(IMainManager));
            _mainManager.ApplicationClosing += _mainManager_ApplicationClosing;
            _mainManager.ApplicationStarting += _mainManager_ApplicationStarting;
        }

        private void _mainManager_ApplicationStarting(object sender, EventArgs e)
        {
            OnResetJobStates();
        }

        private void _mainManager_ApplicationClosing(object sender, EventArgs e)
        {
            OnResetJobStates();
        }

        private void OnResetJobStates()
        {
            try
            {
                var task = Task.Run(async () =>
                {
                    duplicates.Cancel();
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
            var ts = new CancellationTokenSource();
            CancellationToken ct = ts.Token;
            var task = Task.Run(async () =>
            {
                var duplicateConfig = await _repository.JobConfigurationDuplicates(job.Id);
                if (duplicateConfig != null)
                    duplicates.SetSimilarMinValue((Compare.CompareValue.Types)duplicateConfig.CompareValueTypes);
                _mainManager.SetStatusBarInfoText("Job running");
                await _jobServiceRepository.ClearPathDuplicate(job.Id);
                int lastCompareFiles = 0;
                var paths = await _repository.GetJobCollectPath(job.Id);
                Dictionary<string, bool> pathsToCollect = new Dictionary<string, bool>();
                List<string> onlyPathsToCollect = new List<string>();
                List<Models.PathCompareValue> pathCompares = new List<Models.PathCompareValue>();
                foreach (var item in paths)
                {
                    pathsToCollect.Add(item.Path, item.IncludeSubFolders);
                    onlyPathsToCollect.Add(item.Path);
                    List<Models.PathCompareValue> items;
                    if (item.IncludeSubFolders)
                        items = await _jobServiceRepository.GetsByDirectoryWithSubFolders(item.Path);
                    else
                        items = await _jobServiceRepository.Gets(onlyPathsToCollect.ToArray());
                    if (items != null && items.Count > 0)
                        pathCompares.AddRange(items);
                }
                var cache = new Dictionary<string, Compare.CompareValue>();
                List<Models.PathCompareValue> invalidPathCompareItems = new List<Models.PathCompareValue>();
                foreach (var item in pathCompares)
                {
                    var cacheCompare = new Compare.CompareValue
                    {
                        Directory = item.Directory,
                        Extension = item.Extension,
                        FileName = item.FileName,
                        Hash = item.Hash,
                        FileCreated = item.FileCreated,
                        FileModified = item.FileModified,
                        FileSize = item.FileSize
                    };
                    if (OnIsValidCacheItem(cacheCompare))
                        cache.Add(item.FullFile, cacheCompare);
                    else
                        invalidPathCompareItems.Add(item);
                }
                if (invalidPathCompareItems.Count > 0)
                {
                    foreach (var item in invalidPathCompareItems)
                    {
                        await _jobServiceRepository.Delete(item);
                    }
                }
                duplicates.SetCache(cache);
                duplicates.Aborted += (object sender, EventArgs e) =>
                {
                    _mainManager.SetStatusBarInfoText(null);
                };
                decimal currentMaxPercent = 0;
                duplicates.PrepareCompareValuesProgressWithItems += (object sender, Compare.Duplicates.PrepareComareProgressItem e) =>
                {
                    if (e.Progress > currentMaxPercent)
                        currentMaxPercent = e.Progress;
                    _mainManager.SetStatusBarInfoText($"Job prepare files ({currentMaxPercent}%)");
                    if (!isInSaveCompareFiles)
                    {
                        if (e.CompareFiles.Count > (lastCompareFiles + 20))
                        {
                            OnSaveCompareFiles(e.CompareFiles).GetAwaiter().GetResult();
                        }
                    }
                    if (e.Progress == 100)
                    {
                        try
                        {
                            _mainManager.SetStatusBarInfoText($"Job prepare files (100%)");
                            if (!isInSaveCompareFiles)
                            {
                                OnSaveCompareFiles(e.CompareFiles, true).GetAwaiter().GetResult();
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
                                            await OnSaveCompareFiles(e.CompareFiles, true);
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
                foreach (var item in pathsToCollect)
                {
                    await duplicates.Collect(item.Value, item.Key);
                }
                await OnAfterCollect(job, config);
            }, ct);
            jobTasks.Add(job, ts);

            return true;
        }

        private bool OnIsValidCacheItem(Compare.CompareValue compareValue)
        {
            if (compareValue.FileSize != 0
                && !string.IsNullOrWhiteSpace(compareValue.Hash)
                && !string.IsNullOrWhiteSpace(compareValue.FileName)
                && compareValue.FileCreated != DateTime.MinValue
                && !string.IsNullOrWhiteSpace(compareValue.Extension)
                && !string.IsNullOrWhiteSpace(compareValue.Directory))
                return true;
            return false;
        }

        private async Task OnAfterCollect(Job job, JobConfiguration config)
        {
            _mainManager.SetStatusBarInfoText($"Job find duplicates");
            int maxPara = config.MaxParallelism;
            if (maxPara < 1)
                maxPara = Environment.ProcessorCount;
            duplicates.ProcessFileProgressWithItems += (object sender, Compare.Duplicates.ProcessFileProgressItem e) =>
            {
                _mainManager.SetStatusBarInfoText($"Job compare files ({e.Progress}%)");
                Task.Run(async () =>
                {
                    await OnSaveDuplicateResultProgress(job, e);
                });
            };
            duplicates.SetCache(await OnGetDuplicateResultProgress(job));
            var result = await duplicates.Find(maxPara);

            _mainManager.SetStatusBarInfoText($"Finish job");
            await OnComplete(result, job, config);
            _mainManager.SetStatusBarInfoText(null);
        }

        public bool StopJob(Job job, JobConfiguration config)
        {
            try
            {
                _mainManager.SetStatusBarInfoText($"Wait while stopping Job");
                if (jobTasks.Any(x => x.Key.Id == job.Id))
                {
                    duplicates.Cancel();
                    var key = jobTasks.First(x => x.Key.Id == job.Id).Key;
                    jobTasks[key]?.Cancel();
                    jobTasks.Remove(key);
                }
                _mainManager.SetStatusBarInfoText(null);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }

        private DateTime lastSaveDuplicateProgress = DateTime.Now;
        private async Task OnSaveDuplicateResultProgress(Job job, Compare.Duplicates.ProcessFileProgressItem e)
        {
            try
            {
                if (e.Progress == 100)
                {
                    await _jobServiceRepository.RemoveDuplicateResultCache(job.Id);
                }
                else
                {
                    if ((DateTime.Now - lastSaveDuplicateProgress).TotalMinutes > 5)
                    {
                        lastSaveDuplicateProgress = DateTime.Now;
                        var cache = new Models.DuplicateResultProgress
                        {
                            DateTime = DateTime.Now,
                            JobId = job.Id,
                            Cache = JsonConvert.SerializeObject(e)
                        };
                        await _jobServiceRepository.ChangeDuplicateResultCache(cache);
                    }
                }
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async Task<Compare.Duplicates.ProcessFileProgressItem> OnGetDuplicateResultProgress(Job job)
        {
            try
            {
                var cache = await _jobServiceRepository.GetDuplicateResultCache(job.Id);
                if (cache != null)
                {
                    var result = JsonConvert.DeserializeObject(cache.Cache, typeof(Compare.Duplicates.ProcessFileProgressItem));
                    if (result != null)
                        return (Compare.Duplicates.ProcessFileProgressItem)result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        private bool isInSaveCompareFiles = false;

        private async Task OnSaveCompareFiles(Dictionary<string, Compare.CompareValue> compareFiles, bool showStateInfo = false)
        {
            isInSaveCompareFiles = true;
            try
            {
                if (showStateInfo)
                    _mainManager.SetStatusBarInfoText($"Job save prepared files");
                int itemCount = 0;
                foreach (var item in compareFiles)
                {
                    itemCount++;
                    if (showStateInfo)
                        _mainManager.SetStatusBarInfoText($"Job save prepared files ({itemCount}/{compareFiles.Count})");
                    var comp = await _jobServiceRepository.Find(item.Key.ToUpper());
                    if (comp != null)
                    {
                        // update
                        // we don't execute updates
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
                            LastChange = DateTime.Now,
                            FileCreated = item.Value.FileCreated,
                            FileModified = item.Value.FileModified,
                            FileSize = item.Value.FileSize
                        });
                    }
                    await _folderRepository.UpdateFolders(item.Value.Directory.ToUpper());
                }
                if (showStateInfo)
                    _mainManager.SetStatusBarInfoText($"Job save prepared files complete");
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
                if (duplicatesResults != null)
                {
                    _mainManager.SetStatusBarInfoText($"Save compare result ({duplicatesResults.Count} Duplicates)");
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
                }
                job.JobState = Jobs.JobState.Idle;
                await _repository.Update(job);
                JobStateChanged?.Invoke(this, job.JobState);
                _duplicatesManager.RaiseRefreshData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            } finally
            {
                try
                {
                    if (jobTasks.Any(x => x.Key.Id == job.Id))
                    {
                        var key = jobTasks.First(x => x.Key.Id == job.Id).Key;
                        jobTasks.Remove(key);
                    }
                }
                catch (Exception ex1)
                {
                    _logger.Error(ex1);
                }
            }
        }
    }
}
