using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compare
{
    public class Duplicates
    {
        public class PrepareComareProgressItem
        {
            public decimal Progress { get; set; }
            public Dictionary<string, CompareValue> CompareFiles { get; set; }

            public PrepareComareProgressItem(decimal progress, Dictionary<string, CompareValue> compareFiles)
            {
                Progress = progress;
                CompareFiles = compareFiles;
            }
        }

        public List<string> Files { get; private set; }
        public Dictionary<string, CompareValue> CacheCompareValue { get; private set; }

        private readonly CollectFiles _collectFiles;

        public event EventHandler<string> ProcessFile;
        public event EventHandler<bool> PrepareCompareValues;
        public event EventHandler<decimal> PrepareCompareValuesProgress;
        public event EventHandler<PrepareComareProgressItem> PrepareCompareValuesProgressWithItems;

        private CompareValue.Types similarMinValue = CompareValue.Types.Hash;

        public Duplicates()
        {
            Files = new List<string>();
            _collectFiles = new CollectFiles();
            CacheCompareValue = new Dictionary<string, CompareValue>();
        }

        public void SetSimilarMinValue(CompareValue.Types value)
        {
            similarMinValue = value;
        }

        public void SetCache(Dictionary<string, CompareValue> cache)
        {
            CacheCompareValue = cache;
        }

        public async Task Collect(params string[] path)
        {
            Files = new List<string>();
            foreach (var item in path)
            {
                var colFiles = await _collectFiles.Collect(item);
                if (colFiles.Any())
                    Files.AddRange(colFiles);
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
                Parallel.For(0, Files.Count, (int index) =>
                {
                    ProcessFile?.Invoke(this, Files[index]);
                    var dup = OnCompareDuplicates(Files[index], index);
                    if (dup != null)
                        result.Add(dup);
                });
                return result;
            });
        }

        private void OnPrepareCompareValue()
        {
            var comp = new FileComparison();
            ConcurrentDictionary<string, CompareValue> compare = new ConcurrentDictionary<string, CompareValue>(CacheCompareValue);
            int itemCounter = 0;
            PrepareCompareValuesProgress?.Invoke(this, 0);
            Parallel.For(0, Files.Count, (int index) =>
            {
                itemCounter += 1;
                var progressValue = Math.Round(((decimal)itemCounter / (decimal)Files.Count * 100), 2);
                PrepareCompareValuesProgress?.Invoke(this, progressValue);
                if (!compare.ContainsKey(Files[index]))
                {
                    var compareValue = comp.CreateCompareValue(Files[index]);
                    if (compareValue != null)
                        compare.GetOrAdd(Files[index], compareValue);
                }
                PrepareCompareValuesProgressWithItems?.Invoke(this, new PrepareComareProgressItem(progressValue, new Dictionary<string, CompareValue>(compare)));
            });
            PrepareCompareValuesProgress?.Invoke(this, 100);
            PrepareCompareValuesProgressWithItems?.Invoke(this, new PrepareComareProgressItem(100, new Dictionary<string, CompareValue>(compare)));
            CacheCompareValue = new Dictionary<string, CompareValue>(compare);
        }

        private DuplicatesResult OnCompareDuplicates(string sourceFile, int fileStartIndex)
        {
            ConcurrentDictionary<string, CompareValue> compareCache = new ConcurrentDictionary<string, CompareValue>(CacheCompareValue);
            var result = new DuplicatesResult();
            var comp = new FileComparison();
            for (int i = fileStartIndex + 1; i < Files.Count; i++)
            {
                var similar = comp.Similar(compareCache.FirstOrDefault(x => x.Key == sourceFile).Value, compareCache.FirstOrDefault(x => x.Key == Files[i]).Value);
                if (similar.HasFlag(similarMinValue))
                    result.FileResults.Add(new DuplicatesResult.FileResult
                    {
                        CompareValue = similar,
                        FilePath = Files[i]
                    });
            }
            CacheCompareValue = new Dictionary<string, CompareValue>(compareCache);
            if (result.FileResults.Count > 0)
            {
                result.FileResults.Add(new DuplicatesResult.FileResult
                {
                    CompareValue = CompareValue.Types.Hash | CompareValue.Types.FileName | CompareValue.Types.Extension | CompareValue.Types.Directory,
                    FilePath = sourceFile
                });
                return result;
            }
            return null;
        }
    }
}
