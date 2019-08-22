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

        public bool StartJob(Job job, JobConfiguration config)
        {
            throw new NotImplementedException();
        }

        public bool StopJob(Job job, JobConfiguration config)
        {
            throw new NotImplementedException();
        }
    }
}
