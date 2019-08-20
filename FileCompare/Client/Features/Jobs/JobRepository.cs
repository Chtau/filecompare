﻿using System;
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
