using System;
using System.Collections.Generic;
using System.Text;

namespace Compare
{
    interface IFileCompare
    {
        void Init(string path, string srcCompareValue);
        int Similar(string targetPath, string tarCompareValue);
        string GetSourceCompareValue();
        string GetTargetCompareValue();
        string CreateCompareValue(string path);
    }
}
