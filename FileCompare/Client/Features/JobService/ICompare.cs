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
        event EventHandler<Jobs.JobState> JobStateChanged;
        bool StartJob(Job job, JobConfiguration config);
        bool StopJob(Job job, JobConfiguration config);
    }
}
