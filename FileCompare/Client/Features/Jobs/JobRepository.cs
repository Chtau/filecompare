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

        public async Task<bool> Delete(Job job)
        {
            try
            {
                if (job != null)
                {
                    var result1 = await GetJobCollect(job.Id);
                    if (result1 != null && result1.Count > 0)
                        result1.ForEach(async item =>
                        {
                            await _dBContext.Instance.DeleteAsync(item);
                        });
                    var result2 = await GetJobConfiguration(job.Id);
                    if (result2 != null)
                        await _dBContext.Instance.DeleteAsync(result2);
                    await _dBContext.Instance.DeleteAsync(job);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete Job item");
            }
            return false;
        }

        public async Task<bool> Delete(JobCollectPath jobCollectPath)
        {
            try
            {
                if (jobCollectPath != null)
                {
                    await _dBContext.Instance.DeleteAsync(jobCollectPath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete JobCollectPath item");
            }
            return false;
        }

        private async Task<List<Models.JobCollectPath>> GetJobCollect(Guid jobId)
        {
            return await _dBContext.Instance.Table<Models.JobCollectPath>().Where(x => x.JobId == jobId).ToListAsync();
        }

        public async Task<List<ViewModels.JobPathView>> GetJobCollectPath(Guid jobId)
        {
            return (from x in await _dBContext.Instance.Table<Models.JobCollectPath>().ToListAsync()
                   join y in await _dBContext.Instance.Table<Features.Folders.Models.CollectPath>().ToListAsync() on x.CollectPathId equals y.Id
                   where x.JobId == jobId
                   select new ViewModels.JobPathView
                   {
                       JobId = x.JobId,
                       CollectPathId = y.Id,
                       IncludeSubFolders = x.IncludeSubFolders,
                       JobCollectPathId = x.Id,
                       Path = y.Path
                   }).ToList();
        }

        public async Task<JobConfiguration> GetJobConfiguration(Guid jobId)
        {
            return await _dBContext.Instance.Table<Models.JobConfiguration>().Where(x => x.JobId == jobId).FirstOrDefaultAsync();
        }

        public async Task<JobCollectPath> GetJobPath(Guid id)
        {
            return await _dBContext.Instance.Table<Models.JobCollectPath>().FirstAsync(x => x.Id == id);
        }

        public async Task<List<Job>> GetJobs()
        {
            return await _dBContext.Instance.Table<Models.Job>().ToListAsync();
        }

        public async Task<List<Job>> GetJobsByState(JobState jobState)
        {
            return await _dBContext.Instance.Table<Models.Job>().Where(x => x.JobState == jobState).ToListAsync();
        }

        public async Task<Job> GetJobs(Guid jobId)
        {
            return await _dBContext.Instance.Table<Models.Job>().FirstAsync(x => x.Id == jobId);
        }

        public async Task<bool> Insert(Job job)
        {
            try
            {
                if (job != null)
                {
                    await _dBContext.Instance.InsertAsync(job);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to insert new Job item");
            }
            return false;
        }

        public async Task<bool> Insert(JobCollectPath jobCollectPath)
        {
            try
            {
                if (jobCollectPath != null)
                {
                    await _dBContext.Instance.InsertAsync(jobCollectPath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to insert new JobCollectPath item");
            }
            return false;
        }

        public async Task<bool> Insert(JobConfiguration jobConfiguration)
        {
            try
            {
                if (jobConfiguration != null)
                {
                    await _dBContext.Instance.InsertAsync(jobConfiguration);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to insert new JobConfiguration item");
            }
            return false;
        }

        public async Task<bool> Update(Job job)
        {
            try
            {
                if (job != null)
                {
                    await _dBContext.Instance.UpdateAsync(job);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update new Job item");
            }
            return false;
        }

        public async Task<bool> Update(JobConfiguration jobConfiguration)
        {
            try
            {
                if (jobConfiguration != null)
                {
                    await _dBContext.Instance.UpdateAsync(jobConfiguration);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update new JobConfiguration item");
            }
            return false;
        }

        public async Task<bool> Update(JobCollectPath jobCollectPath)
        {
            try
            {
                if (jobCollectPath != null)
                {
                    await _dBContext.Instance.UpdateAsync(jobCollectPath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update new JobCollectPath item");
            }
            return false;
        }
    }
}
