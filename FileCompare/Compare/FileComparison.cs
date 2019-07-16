using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Compare
{
    class FileComparison : IFileCompare
    {
        private string sourcePath;
        private string sourceMD5Hash = null;
        private string targetMD5Hash = null;

        public FileComparison()
        {
           
        }

        public void Init(string path, string srcCompareValue)
        {
            sourcePath = path;
            if (!string.IsNullOrWhiteSpace(srcCompareValue))
                sourceMD5Hash = srcCompareValue;
            else
                sourceMD5Hash = OnGetMD5(sourcePath);
        }

        public int Similar(string targetPath, string tarCompareValue)
        {
            if (!string.IsNullOrWhiteSpace(tarCompareValue))
                targetMD5Hash = tarCompareValue;
            else
                targetMD5Hash = OnGetMD5(targetPath);
            return sourceMD5Hash == targetMD5Hash ? 100 : 0;
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

        public string GetSourceCompareValue()
        {
            return sourceMD5Hash;
        }

        public string GetTargetCompareValue()
        {
            return targetMD5Hash;
        }

        public string CreateCompareValue(string path)
        {
            return OnGetMD5(path);
        }
    }
}
