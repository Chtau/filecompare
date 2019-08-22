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
        private readonly Jobs.IJobRepository _repository;


        public CompareInAppHosting()
        {
            duplicates = new Compare.Duplicates();
            _repository = (Jobs.IJobRepository)Bootstrap.Instance.Services.GetService(typeof(Jobs.IJobRepository));
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
                duplicates.PrepareCompareValues += (object sender, bool e) =>
                {
                    /*if (e)
                        Console.WriteLine("Prepare compare value complete");
                    else
                        Console.WriteLine("Prepare compare value starting");*/
                };
                duplicates.PrepareCompareValuesProgress += (object sender, decimal e) =>
                {
                    //Console.WriteLine("Prepare compare value Progress:" + e);
                };
                duplicates.PrepareCompareValuesProgressWithItems += (object sender, Compare.Duplicates.PrepareComareProgressItem e) =>
                {
                    if (e.Progress == 100)
                    {
                        OnSaveCompareFiles(e.CompareFiles);
                    } else
                    {
                        if (e.CompareFiles.Count > (lastCompareFiles + 20))
                        {
                            OnSaveCompareFiles(e.CompareFiles);
                        }
                    }
                };
                duplicates.ProcessFile += (object sender, string e) =>
                {
                    //Console.WriteLine("Process file: " + e);
                };
                var result = duplicates.Find();
            });
            return true;
        }

        public bool StopJob(Job job, JobConfiguration config)
        {
            return true;
        }

        private void OnSaveCompareFiles(Dictionary<string, Compare.CompareValue> compareFiles)
        {

        }
    }
}
