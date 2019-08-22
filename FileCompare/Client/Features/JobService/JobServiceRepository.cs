﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.JobService
{
    public class JobServiceRepository : IJobServiceRepository
    {
        private readonly Internal.ILogger _logger;
        private readonly DAL.IDBContext _dBContext;

        public JobServiceRepository()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _dBContext = (DAL.IDBContext)Bootstrap.Instance.Services.GetService(typeof(DAL.IDBContext));
        }

        public async Task<bool> Delete(Models.PathCompareValue pathCompareValue)
        {
            try
            {
                if (pathCompareValue != null)
                {
                    await _dBContext.Instance.DeleteAsync(pathCompareValue);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete PathCompareValue item");
            }
            return false;
        }

        public async Task<Models.PathCompareValue> Find(string fullFileName)
        {
            return await _dBContext.Instance.Table<Models.PathCompareValue>().FirstOrDefaultAsync(x => x.FullFile == fullFileName.ToUpper());
        }

        public async Task<List<Models.PathCompareValue>> Gets()
        {
            return await _dBContext.Instance.Table<Models.PathCompareValue>().ToListAsync();
        }

        public async Task<List<Models.PathCompareValue>> GetsByDirectoryWithSubFolders(string directory)
        {
            return await _dBContext.Instance.Table<Models.PathCompareValue>().Where(x => x.Directory.StartsWith(directory.ToUpper())).ToListAsync();
        }

        public async Task<List<Models.PathCompareValue>> GetsByDirectory(string directory)
        {
            return await _dBContext.Instance.Table<Models.PathCompareValue>().Where(x => x.Directory == directory.ToUpper()).ToListAsync();
        }

        public async Task<bool> Insert(Models.PathCompareValue pathCompareValue)
        {
            try
            {
                if (pathCompareValue != null)
                {
                    await _dBContext.Instance.InsertAsync(pathCompareValue);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to insert new PathCompareValue item");
            }
            return false;
        }

        public async Task<bool> Update(Models.PathCompareValue pathCompareValue)
        {
            try
            {
                if (pathCompareValue != null)
                {
                    await _dBContext.Instance.UpdateAsync(pathCompareValue);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update new PathCompareValue item");
            }
            return false;
        }
    }
}
