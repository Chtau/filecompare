using System;
using System.Collections.Generic;
using System.Text;

namespace Compare
{
    interface IFileCompare
    {
        CompareValue.Types Similar(CompareValue srcCompareValue, CompareValue tarCompareValue);
        CompareValue CreateCompareValue(string path);
    }
}
