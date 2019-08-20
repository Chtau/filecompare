using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Features.Jobs.Models;

namespace Client.Features.Jobs
{
    public class JobRepository : IJobRepository
    {
        private readonly Internal.ILogger _logger;
        private readonly DAL.IDBContext _dBContext;

        public JobRepository()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _dBContext = (DAL.IDBContext)Bootstrap.Instance.Services.GetService(typeof(DAL.IDBContext));
        }

        public Task<bool> Delete(Job job)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(JobCollectPath jobCollectPath)
        {
            throw new NotImplementedException();
        }

        public async Task<List<JobCollectPath>> GetJobCollectPath(Guid jobId)
        {
            return await _dBContext.Instance.Table<Models.JobCollectPath>().Where(x => x.JobId == jobId).ToListAsync();
        }

        public async Task<JobConfiguration> GetJobConfiguration(Guid jobId)
        {
            return await _dBContext.Instance.Table<Models.JobConfiguration>().Where(x => x.JobId == jobId).FirstOrDefaultAsync();
        }

        public async Task<List<Job>> GetJobs()
        {
            return await _dBContext.Instance.Table<Models.Job>().ToListAsync();
        }

        public Task<bool> Insert(Job job)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Insert(JobCollectPath jobCollectPath)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Job job)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(JobConfiguration jobConfiguration)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(JobCollectPath jobCollectPath)
        {
            throw new NotImplementedException();
        }
    }
}
