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
        public async Task<IEnumerable<string>> Collect(string path)
        {
            return await Task.Run(() =>
            {
                return OnCollect(path);
            });
        }

        private IEnumerable<string> OnCollect(string path)
        {
            var result = new List<string>();
            if (AccessControl.Directory(path))
            {
                var files = Directory.EnumerateFiles(path);
                if (files.Any())
                    result.AddRange(files);
                var dir = Directory.EnumerateDirectories(path);
                if (dir.Any())
                {
                    foreach (var item in dir)
                    {
                        var dirFiles = OnCollect(item);
                        if (dirFiles.Any())
                            result.AddRange(dirFiles);
                    }
                }
            }
            return result;
        }
    }
}
