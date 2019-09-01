using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Features.Duplicates.ViewModels;

namespace Client.Features.Duplicates
{
    public class DuplicatesRepository : IDuplicatesRepository
    {
        private readonly Internal.ILogger _logger;
        private readonly DAL.IDBContext _dBContext;

        public DuplicatesRepository()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _dBContext = (DAL.IDBContext)Bootstrap.Instance.Services.GetService(typeof(DAL.IDBContext));
        }

        public async Task<bool> CheckDuplicateRemove(Guid duplicateId)
        {
            try
            {
                if (duplicateId != Guid.Empty)
                {
                    var count = await _dBContext.Instance.Table<JobService.Models.PathDuplicate>().CountAsync(y => y.DuplicateValueId == duplicateId);
                    if (count <= 1)
                    {
                        var items = await _dBContext.Instance.Table<JobService.Models.PathDuplicate>().Where(y => y.DuplicateValueId == duplicateId).ToListAsync();
                        foreach (var item in items)
                        {
                            await DeletePathDuplicate(item.DuplicateValueId, item.PathCompareValueId);
                        }
                        var dup = await _dBContext.Instance.Table<JobService.Models.DuplicateValue>().FirstOrDefaultAsync(x => x.Id == duplicateId);
                        if (dup != null)
                            await _dBContext.Instance.DeleteAsync(dup);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to perform CheckDuplicateRemove");
            }
            return false;
        }

        public async Task<bool> ClearDuplicates()
        {
            try
            {
                var count = await _dBContext.Instance.Table<JobService.Models.PathDuplicate>().CountAsync();
                if (count <= 1)
                {
                    var items = await _dBContext.Instance.Table<JobService.Models.PathDuplicate>().ToListAsync();
                    foreach (var item in items)
                    {
                        await DeletePathDuplicate(item.DuplicateValueId, item.PathCompareValueId);
                    }
                    var dups = await _dBContext.Instance.Table<JobService.Models.DuplicateValue>().ToListAsync();
                    foreach (var item in dups)
                    {
                        await _dBContext.Instance.DeleteAsync(item);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to perform ClearDuplicates");
            }
            return false;
        }

        public async Task<bool> DeletePathDuplicate(Guid duplicateValueId, Guid pathCompareValueId)
        {
            try
            {
                if (pathCompareValueId != Guid.Empty)
                {
                    var item = await _dBContext.Instance.Table<JobService.Models.PathDuplicate>().FirstOrDefaultAsync(x => x.DuplicateValueId == duplicateValueId && x.PathCompareValueId == pathCompareValueId);
                    if (item != null)
                        await _dBContext.Instance.DeleteAsync(item);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete DeletePath item");
            }
            return false;
        }

        public async Task<List<DuplicatesResult>> Duplicates()
        {
            return (from x in await _dBContext.Instance.Table<JobService.Models.DuplicateValue>().ToListAsync()
                   select new DuplicatesResult
                   {
                       DuplicateId = x.Id,
                       DuplicateValue = x.CompareValue,
                       DuplicateTitle = OnGetTitle(x.Id),
                       DuplicatesCount = _dBContext.Instance.Table<JobService.Models.PathDuplicate>().CountAsync(y => y.DuplicateValueId == x.Id).GetAwaiter().GetResult()
                   }).ToList();
        }

        public async Task<List<DuplicatesResultPath>> DuplicatesPaths(Guid duplicateValueId)
        {
            return (from x in await _dBContext.Instance.Table<JobService.Models.PathCompareValue>().ToListAsync()
                    join y in await _dBContext.Instance.Table<JobService.Models.PathDuplicate>().ToListAsync() on x.Id equals y.PathCompareValueId
                    where y.DuplicateValueId == duplicateValueId
                    select new DuplicatesResultPath
                    {
                        PathCompareValueId = x.Id,
                        Directory = x.Directory,
                        Extension = x.Extension,
                        FileName = x.FileName
                    }).ToList();
        }

        private string OnGetTitle(Guid duplicateValueId)
        {
            string title = "";
            Jobs.Models.Job job;
            var pathDup = _dBContext.Instance.Table<JobService.Models.PathDuplicate>().FirstOrDefaultAsync(y => y.DuplicateValueId == duplicateValueId).GetAwaiter().GetResult();
            if (pathDup != null)
            {
                if (pathDup.JobId != Guid.Empty)
                {
                    job = _dBContext.Instance.Table<Jobs.Models.Job>().FirstAsync(x => x.Id == pathDup.JobId).GetAwaiter().GetResult();
                }
                else
                {
                    job = new Jobs.Models.Job()
                    {
                        Name = "No JOB"
                    };
                }
                var pathDetail = _dBContext.Instance.Table<JobService.Models.PathCompareValue>().FirstAsync(x => x.Id == pathDup.PathCompareValueId).GetAwaiter().GetResult();
                title += $"Job:{job.Name} File:{pathDetail.FileName}";
            } else
            {
                title = duplicateValueId.ToString();
            }

            return title;
        }
    }
}