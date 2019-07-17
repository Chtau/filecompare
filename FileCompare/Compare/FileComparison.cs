using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Compare
{
    class FileComparison : IFileCompare
    {

        public FileComparison()
        {
           
        }

        public int Similar(CompareValue srcCompareValue, CompareValue tarCompareValue)
        {
            int similarValue = 0;
            if (srcCompareValue.Hash?.ToLower() == tarCompareValue.Hash?.ToLower())
            {
                similarValue += 90;
            }
            if (srcCompareValue.Directory?.ToLower() == tarCompareValue.Directory?.ToLower())
            {
                similarValue += 5;
            }
            if (srcCompareValue.Extension?.ToLower() == tarCompareValue.Extension?.ToLower())
            {
                similarValue += 10;
            }
            if (srcCompareValue.FileName?.ToLower() == tarCompareValue.FileName?.ToLower())
            {
                similarValue += 25;
            }
            if (similarValue > 100)
                similarValue = 100;
            return similarValue;
        }

        private string OnGetMD5(string path)
        {
            if (AccessControl.File(path))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            return null;
        }

        public CompareValue CreateCompareValue(string path)
        {
            return new CompareValue
            {
                Hash = OnGetMD5(path),
                FileName = Path.GetFileNameWithoutExtension(path),
                Directory = Path.GetDirectoryName(path),
                Extension = Path.GetExtension(path)
            };
        }
    }
}
