using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Compare
{
    class FileComparison
    {
        private string sourcePath;
        private string md5Hash = null;

        public FileComparison(string path)
        {
            sourcePath = path;
            md5Hash = OnGetMD5(sourcePath);
        }

        public int Similar(string targetPath)
        {
            return md5Hash == OnGetMD5(targetPath) ? 100 : 0;
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
    }
}
