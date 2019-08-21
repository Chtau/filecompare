using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.JobService
{
    public interface IJobService
    {
        void Run();
        void StartJob(Features.Jobs.Models.Job job);
        void StopJob(Features.Jobs.Models.Job job);
    }
}
