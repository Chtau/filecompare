﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Folders
{
    public class FolderRepository : IFolderRepository
    {
        private readonly Internal.ILogger _logger;
        private readonly DAL.IDBContext _dBContext;

        public FolderRepository()
        {
            _logger = (Internal.ILogger)Bootstrap.Instance.Services.GetService(typeof(Internal.ILogger));
            _dBContext = (DAL.IDBContext)Bootstrap.Instance.Services.GetService(typeof(DAL.IDBContext));
        }

        public async Task<List<Models.CollectPath>> GetPaths()
        {
            return await _dBContext.Instance.Table<Models.CollectPath>().ToListAsync();
        }

        public async Task<bool> Insert(Models.CollectPath collectPath)
        {
            try
            {
                if (collectPath != null)
                {
                    await _dBContext.Instance.InsertAsync(collectPath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to insert new CollectPath item");
            }
            return false;
        }

        public async Task<bool> Delete(Models.CollectPath collectPath)
        {
            try
            {
                if (collectPath != null)
                {
                    await _dBContext.Instance.DeleteAsync(collectPath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete CollectPath item");
            }
            return false;
        }

        public async Task UpdateFolders(string directory)
        {
            try
            {
                var paths = await _dBContext.Instance.Table<Models.CollectPath>().Where(x => x.Path.StartsWith(directory, StringComparison.OrdinalIgnoreCase)).ToListAsync();
                if (paths != null && paths.Count > 0)
                {
                    foreach (var item in paths)
                    {
                        if (string.Equals(item.Path, directory, StringComparison.OrdinalIgnoreCase))
                        {
                            item.TotalFilesFound += 1;
                            item.LastCheck = DateTime.Now;
                        } else
                        {
                            item.TotalFilesFound += 1;
                            item.SubFoldersFilesFound += 1;
                            item.LastCheck = DateTime.Now;
                        }
                    }
                    await _dBContext.Instance.UpdateAllAsync(paths);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to update Folders");
            }
        }
    }
}
