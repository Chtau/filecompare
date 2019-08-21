using Client.Features.Jobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.JobService
{
    public interface ICompare
    {
        event EventHandler<CompareProgressEventArgs> ReportProgress;
        void StartJob(Job job, JobConfiguration config);
        void StopJob(Job job, JobConfiguration config);
    }

    public class CompareProgressEventArgs : EventArgs
    {
        public Job Job { get; set; }
        public JobConfiguration JobConfiguration { get; set; }
        public List<object> ProgressData { get; set; }
        public List<string> ProgressInfoText { get; set; }

        public CompareProgressEventArgs(Job job, JobConfiguration jobConfiguration, List<object> progressData = null, List<string> progressInfoText = null)
        {
            Job = job;
            JobConfiguration = jobConfiguration;
            ProgressData = progressData ?? new List<object>();
            ProgressInfoText = progressInfoText ?? new List<string>();
        }
    }
}
