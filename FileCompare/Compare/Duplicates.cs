using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compare
{
    public class Duplicates
    {
        private List<string> files;
        private readonly CollectFiles _collectFiles;

        public Duplicates()
        {
            files = new List<string>();
            _collectFiles = new CollectFiles();
        }

        public async Task Collect(params string[] path)
        {
            files = new List<string>();
            foreach (var item in path)
            {
                var colFiles = await _collectFiles.Collect(item);
                if (colFiles.Any())
                    files.AddRange(colFiles);
            }
        }

        public List<DuplicatesResult> Find()
        {
            var result = new List<DuplicatesResult>();
            for (int i = 0; i < files.Count; i++)
            {
                var dup = OnCompareDuplicates(files[i], i);
                if (dup != null)
                    result.Add(dup);
            }
            return result;
        }

        private DuplicatesResult OnCompareDuplicates(string sourceFile, int fileStartIndex)
        {
            var result = new DuplicatesResult();
            var comp = new FileComparison(sourceFile);
            for (int i = fileStartIndex + 1; i < files.Count; i++)
            {
                var similar = comp.Similar(files[i]);
                if (similar >= 90)
                    result.FileResults.Add(new DuplicatesResult.FileResult
                    {
                        CompareValue = similar,
                        FilePath = files[i]
                    });
            }
            if (result.FileResults.Count > 0)
            {
                result.FileResults.Add(new DuplicatesResult.FileResult
                {
                    CompareValue = 100,
                    FilePath = sourceFile
                });
                return result;
            }
            return null;
        }
    }
}
