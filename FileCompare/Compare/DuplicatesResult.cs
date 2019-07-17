using System;
using System.Collections.Generic;
using System.Text;

namespace Compare
{
    public class DuplicatesResult
    {
        public class FileResult
        {
            public string FilePath { get; set; }
            public CompareValue.Types CompareValue { get; set; }

            public override string ToString()
            {
                if (!string.IsNullOrWhiteSpace(FilePath))
                    return $"File: {FilePath};Value: {CompareValue.ToString()}";
                return base.ToString();
            }
        }

        public List<FileResult> FileResults { get; set; }

        public DuplicatesResult()
        {
            FileResults = new List<FileResult>();
        }

        public override string ToString()
        {
            if (FileResults.Count > 0)
            {
                return $"1.File:{FileResults[0].FilePath};Similar File: {FileResults.Count}";
            }
            return base.ToString();
        }
    }
}
