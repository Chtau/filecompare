using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.JobService
{
    public interface IJobServiceRepository
    {
        Task<bool> Delete(Models.PathCompareValue pathCompareValue);
        Task<List<Models.PathCompareValue>> Gets(params string[] directorys);
        Task<bool> Insert(Models.PathCompareValue pathCompareValue);
        Task<bool> Update(Models.PathCompareValue pathCompareValue);
        Task<List<Models.PathCompareValue>> GetsByDirectory(string directory);
        Task<List<Models.PathCompareValue>> GetsByDirectoryWithSubFolders(string directory);
        Task<Models.PathCompareValue> Find(string fullFileName);
        Task<Models.DuplicateValue> CreateDuplicateValue(int compareValue);
        Task<bool> CreatePathDuplicate(Guid jobId, Guid duplicateValueId, string pathFullFileName);
        Task<bool> ClearPathDuplicate(Guid jobId);
        Task<bool> ClearCachePathCompareValues();
        Task<bool> ClearCachePathCompareValues(Models.PathCompareValue compareValue);
        Task<List<Models.PathCompareValue>> GetAll();
        Task<bool> CheckCacheFileExists();
    }
}
