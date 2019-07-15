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
            public int CompareValue { get; set; }
        }

        public List<FileResult> FileResults { get; set; }

        public DuplicatesResult()
        {
            FileResults = new List<FileResult>();
        }
    }
}
