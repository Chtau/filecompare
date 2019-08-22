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
        private readonly IJobServiceRepository _jobServiceRepository;


        public CompareInAppHosting()
        {
            duplicates = new Compare.Duplicates();
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _repository = (Jobs.IJobRepository)Bootstrap.Instance.Services.GetService(typeof(Jobs.IJobRepository));
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
                            OnSaveCompareFiles(e.CompareFiles);
                        }
                    }
                    if (e.Progress == 100)
                    {
                        try
                        {
                            if (!isInSaveCompareFiles)
                            {
                                OnSaveCompareFiles(e.CompareFiles);
                            } else
                            {
                                Task.Run(async () =>
                                {
                                    do
                                    {
                                        if (isInSaveCompareFiles)
                                            await Task.Delay(100);
                                        if (!isInSaveCompareFiles)
                                            OnSaveCompareFiles(e.CompareFiles);
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

                var result = duplicates.Find();
            });
            return true;
        }

        public bool StopJob(Job job, JobConfiguration config)
        {
            return true;
        }

        private bool isInSaveCompareFiles = false;

        private void OnSaveCompareFiles(Dictionary<string, Compare.CompareValue> compareFiles)
        {
            isInSaveCompareFiles = true;
            try
            {

            } catch (Exception ex)
            {
                _logger.Error(ex);
            } finally
            {
                isInSaveCompareFiles = false;
            }
        }
    }
}
