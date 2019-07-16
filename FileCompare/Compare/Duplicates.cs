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
        private Dictionary<string, string> cacheCompareValue;

        public event EventHandler<string> ProcessFile;
        public event EventHandler<bool> PrepareCompareValues;

        public Duplicates()
        {
            files = new List<string>();
            _collectFiles = new CollectFiles();
            cacheCompareValue = new Dictionary<string, string>();
        }

        public async Task Collect(params string[] path)
        {
            files = new List<string>();
            cacheCompareValue = new Dictionary<string, string>();
            foreach (var item in path)
            {
                var colFiles = await _collectFiles.Collect(item);
                if (colFiles.Any())
                    files.AddRange(colFiles);
            }
        }

        public async Task<List<DuplicatesResult>> Find()
        {
            return await Task.Run(() =>
            {
                PrepareCompareValues?.Invoke(this, false);
                OnPrepareCompareValue();
                PrepareCompareValues?.Invoke(this, true);
                var result = new List<DuplicatesResult>();
                Parallel.For(0, files.Count, (int index) =>
                {
                    ProcessFile?.Invoke(this, files[index]);
                    var dup = OnCompareDuplicates(files[index], index);
                    if (dup != null)
                        result.Add(dup);
                });
                /*for (int i = 0; i < files.Count; i++)
                {
                    ProcessFile?.Invoke(this, files[i]);
                    var dup = OnCompareDuplicates(files[i], i);
                    if (dup != null)
                        result.Add(dup);
                }*/
                return result;
            });
        }

        private void OnPrepareCompareValue()
        {
            var comp = new FileComparison();
            for (int i = 0; i < files.Count; i++)
            {
                if (!cacheCompareValue.ContainsKey(files[i]))
                    cacheCompareValue.Add(files[i], comp.CreateCompareValue(files[i]));
            }
        }

        private DuplicatesResult OnCompareDuplicates(string sourceFile, int fileStartIndex)
        {
            var result = new DuplicatesResult();
            var comp = new FileComparison();
            comp.Init(sourceFile, cacheCompareValue.FirstOrDefault(x => x.Key == sourceFile).Value);
            for (int i = fileStartIndex + 1; i < files.Count; i++)
            {
                var similar = comp.Similar(files[i], cacheCompareValue.FirstOrDefault(x => x.Key == files[i]).Value);
                if (similar >= 90)
                    result.FileResults.Add(new DuplicatesResult.FileResult
                    {
                        CompareValue = similar,
                        FilePath = files[i]
                    });
                if (!cacheCompareValue.ContainsKey(files[i]))
                    cacheCompareValue.Add(files[i], comp.GetTargetCompareValue());
            }
            if (!cacheCompareValue.ContainsKey(sourceFile))
                cacheCompareValue.Add(sourceFile, comp.GetSourceCompareValue());
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
