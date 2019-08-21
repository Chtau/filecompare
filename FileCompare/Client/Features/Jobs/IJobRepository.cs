using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Jobs
{
    public interface IJobRepository
    {
        Task<List<Models.Job>> GetJobs();
        Task<Models.Job> GetJobs(Guid jobId);
        Task<bool> Insert(Models.Job job);
        Task<bool> Delete(Models.Job job);
        Task<bool> Update(Models.Job job);

        Task<Models.JobConfiguration> GetJobConfiguration(Guid jobId);
        Task<bool> Insert(Models.JobConfiguration jobConfiguration);
        Task<bool> Update(Models.JobConfiguration jobConfiguration);

        Task<List<ViewModels.JobPathView>> GetJobCollectPath(Guid jobId);
        Task<Models.JobCollectPath> GetJobPath(Guid id);
        Task<bool> Insert(Models.JobCollectPath jobCollectPath);
        Task<bool> Delete(Models.JobCollectPath jobCollectPath);
        Task<bool> Update(Models.JobCollectPath jobCollectPath);
    }
}
