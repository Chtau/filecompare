using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Features.Jobs.Models;

namespace Client.Features.JobService
{
    public class CompareInAppHosting : ICompare
    {
        public event EventHandler<CompareProgressEventArgs> ReportProgress;

        private readonly Compare.Duplicates duplicates;
        private readonly Internal.ILogger _logger;
        private readonly Jobs.IJobRepository _repository;
        private readonly Folders.IFolderRepository _folderRepository;
        private readonly IJobServiceRepository _jobServiceRepository;


        public CompareInAppHosting()
        {
            duplicates = new Compare.Duplicates();
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (Jobs.IJobRepository)Bootstrap.Instance.Services.GetService(typeof(Jobs.IJobRepository));
            _folderRepository = (Folders.IFolderRepository)Bootstrap.Instance.Services.GetService(typeof(Folders.IFolderRepository));
            _jobServiceRepository = (IJobServiceRepository)Bootstrap.Instance.Services.GetService(typeof(IJobServiceRepository));
        }

        public bool StartJob(Job job, JobConfiguration config)
        {
            Task.Run(async () =>
            {
                int lastCompareFiles = 0;
                var paths = await _repository.GetJobCollectPath(job.Id);
                List<string> pathsToCollect = new List<string>();
                foreach (var item in paths)
                {
                    pathsToCollect.Add(item.Path);
                }
                await duplicates.Collect(pathsToCollect.ToArray());
                duplicates.PrepareCompareValuesProgressWithItems += (object sender, Compare.Duplicates.PrepareComareProgressItem e) =>
                {
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
                                            await OnSaveCompareFiles(e.CompareFiles);
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

                var result = await duplicates.Find();
                await OnComplete(result, job, config);
            });
            return true;
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
                        comp.Hash = item.Value.Hash;
                        comp.LastChange = DateTime.Now;
                        await _jobServiceRepository.Update(comp);
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
                        await _jobServiceRepository.CreatePathDuplicate(duplicateValue.Id, result.FilePath);
                    }
                }

                job.JobState = Jobs.JobState.Idle;
                await _repository.Update(job);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
