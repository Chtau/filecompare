using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Features.JobService.Models;

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

        public async Task<List<Models.PathCompareValue>> Gets(params string[] directorys)
        {
            var retVal = new List<Models.PathCompareValue>();
            foreach (var item in directorys)
            {
                var paths = await _dBContext.Instance.Table<Models.PathCompareValue>().Where(x => x.Directory == item.ToUpper()).ToListAsync();
                if (paths?.Count > 0)
                    retVal.AddRange(paths);
            }

            return retVal;
        }

        public async Task<List<Models.PathCompareValue>> GetsByDirectoryWithSubFolders(string directory)
        {
            return await _dBContext.Instance.Table<Models.PathCompareValue>().Where(x => x.Directory.StartsWith(directory)).ToListAsync();
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

        public async Task<DuplicateValue> CreateDuplicateValue(int compareValue)
        {
            try
            {
                var duplicate = new DuplicateValue
                {
                    CompareValue = compareValue,
                    Id = Guid.NewGuid(),
                };
                await _dBContext.Instance.InsertAsync(duplicate);
                return duplicate;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create new DuplicateValue item");
            }
            return null;
        }

        public async Task<bool> CreatePathDuplicate(Guid jobId, Guid duplicateValueId, string pathFullFileName)
        {
            try
            {
                var path = await Find(pathFullFileName.ToUpper());
                if (path != null)
                {
                    var pathDuplicate = new PathDuplicate
                    {
                        Id = Guid.NewGuid(),
                        JobId = jobId,
                        DuplicateValueId = duplicateValueId,
                        PathCompareValueId = path.Id
                    };
                    await _dBContext.Instance.InsertAsync(pathDuplicate);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create new PathDuplicate item");
            }
            return false;
        }

        public async Task<bool> ClearPathDuplicate(Guid jobId)
        {
            try
            {
                var pathDuplicates = await _dBContext.Instance.Table<Models.PathDuplicate>().Where(x => x.JobId == jobId).ToListAsync();
                foreach (var item in pathDuplicates)
                {
                    var duplicateValues = await _dBContext.Instance.Table<Models.DuplicateValue>().FirstOrDefaultAsync(x => x.Id == item.DuplicateValueId);
                    if (duplicateValues != null)
                        await _dBContext.Instance.DeleteAsync(duplicateValues);
                    await _dBContext.Instance.DeleteAsync(item);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to clear Duplicate items");
            }
            return false;
        }

        public async Task<bool> ClearCachePathCompareValues()
        {
            try
            {
                await _dBContext.Instance.DeleteAllAsync<Models.PathCompareValue>();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to clear PathCompareValue items");
            }
            return false;
        }

        public async Task<List<PathCompareValue>> GetAll()
        {
            return await _dBContext.Instance.Table<Models.PathCompareValue>().ToListAsync();
        }

        public async Task<bool> ClearCachePathCompareValues(PathCompareValue compareValue)
        {
            try
            {
                await _dBContext.Instance.DeleteAsync<Models.PathCompareValue>(compareValue);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete PathCompareValue items");
            }
            return false;
        }

        public async Task<bool> CheckCacheFileExists()
        {
            try
            {
                var cache = await GetAll();
                foreach (var item in cache)
                {
                    if (!Compare.AccessControl.File(item.FullFile))
                    {
                        await ClearCachePathCompareValues(item);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to execute CheckCacheFileExists");
            }
            return false;
        }

        public async Task<bool> ChangeDuplicateResultCache(DuplicateResultProgress duplicateResultProgress, List<DuplicateResultProgressIndex> indices)
        {
            try
            {
                var item = await GetDuplicateResultCache(duplicateResultProgress.JobId);
                if (item != null)
                {
                    item.DateTime = duplicateResultProgress.DateTime;
                    item.Cache = duplicateResultProgress.Cache;
                    await _dBContext.Instance.UpdateAsync(item);
                } else
                {
                    item = new DuplicateResultProgress
                    {
                        JobId = duplicateResultProgress.JobId,
                        DateTime = duplicateResultProgress.DateTime,
                        Cache = duplicateResultProgress.Cache
                    };
                    await _dBContext.Instance.InsertAsync(item);
                }
                var index = await GetDuplicateResultIndexCache(duplicateResultProgress.JobId);
                foreach (var val in indices)
                {
                    if (!index.Any(x => x.Value == val.Value && x.JobId == val.JobId))
                    {
                        await _dBContext.Instance.InsertAsync(
                            new DuplicateResultProgressIndex
                            {
                                Id = Guid.NewGuid(),
                                JobId = duplicateResultProgress.JobId,
                                Value = val.Value.ToUpper()
                            });
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to change ChangeDuplicateResultCache item");
            }
            return false;
        }

        public async Task<bool> RemoveDuplicateResultCache(Guid jobId)
        {
            try
            {
                var caches = await _dBContext.Instance.Table<Models.DuplicateResultProgress>().Where(x => x.JobId == jobId).ToListAsync();
                if (caches != null && caches.Count > 0)
                {
                    foreach (var item in caches)
                    {
                        await _dBContext.Instance.DeleteAsync<Models.DuplicateResultProgress>(item);
                    }
                }

                var cachesIndex = await _dBContext.Instance.Table<Models.DuplicateResultProgressIndex>().Where(x => x.JobId == jobId).ToListAsync();
                if (cachesIndex != null && cachesIndex.Count > 0)
                {
                    foreach (var item in cachesIndex)
                    {
                        await _dBContext.Instance.DeleteAsync<Models.DuplicateResultProgressIndex>(item);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete RemoveDuplicateResultCache items");
            }
            return false;
        }

        public async Task<DuplicateResultProgress> GetDuplicateResultCache(Guid jobId)
        {
            return await _dBContext.Instance.Table<Models.DuplicateResultProgress>().FirstOrDefaultAsync(x => x.JobId == jobId);
        }

        public async Task<List<DuplicateResultProgressIndex>> GetDuplicateResultIndexCache(Guid jobId)
        {
            return await _dBContext.Instance.Table<Models.DuplicateResultProgressIndex>().Where(x => x.JobId == jobId).ToListAsync();
        }
    }
}
