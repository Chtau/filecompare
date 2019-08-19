using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Folders
{
    public interface IFolderRepository
    {
        Task<List<Models.CollectPath>> GetPaths();
        Task<bool> Insert(Models.CollectPath collectPath);
        Task<bool> Delete(Models.CollectPath collectPath);
    }
}
