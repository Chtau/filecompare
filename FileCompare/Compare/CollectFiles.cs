using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compare
{
    class CollectFiles
    {
        public async Task<IEnumerable<string>> Collect(string path, bool collectSubFolders)
        {
            return await Task.Run(() =>
            {
                return OnCollect(path, collectSubFolders);
            });
        }

        private IEnumerable<string> OnCollect(string path, bool collectSubFolders)
        {
            var result = new List<string>();
            if (AccessControl.Directory(path))
            {
                var files = Directory.EnumerateFiles(path);
                if (files.Any())
                    result.AddRange(files);
                if (collectSubFolders)
                {
                    var dir = Directory.EnumerateDirectories(path);
                    if (dir.Any())
                    {
                        foreach (var item in dir)
                        {
                            var dirFiles = OnCollect(item, collectSubFolders);
                            if (dirFiles.Any())
                                result.AddRange(dirFiles);
                        }
                    }
                }
            }
            return result;
        }
    }
}
