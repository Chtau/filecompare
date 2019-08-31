using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Compare
{
    internal class FileComparison : IFileCompare
    {

        public CompareValue.Types Similar(CompareValue srcCompareValue, CompareValue tarCompareValue)
        {
            CompareValue.Types similarValue = CompareValue.Types.None;
            if (srcCompareValue.Hash?.ToLower() == tarCompareValue.Hash?.ToLower())
            {
                similarValue |= CompareValue.Types.Hash;
            }
            if (srcCompareValue.Directory?.ToLower() == tarCompareValue.Directory?.ToLower())
            {
                similarValue |= CompareValue.Types.Directory;
            }
            if (srcCompareValue.Extension?.ToLower() == tarCompareValue.Extension?.ToLower())
            {
                similarValue |= CompareValue.Types.Extension;
            }
            if (srcCompareValue.FileName?.ToLower() == tarCompareValue.FileName?.ToLower())
            {
                similarValue |= CompareValue.Types.FileName;
            }
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

        private FileInfo OnGetFileInfo(string path)
        {
            if (AccessControl.File(path))
            {
                return new System.IO.FileInfo(path);
            }
            return null;
        }

        public CompareValue CreateCompareValue(string path)
        {
            var fileInfo = OnGetFileInfo(path);
            return new CompareValue
            {
                Hash = OnGetMD5(path),
                FileName = Path.GetFileNameWithoutExtension(path),
                Directory = Path.GetDirectoryName(path),
                Extension = Path.GetExtension(path),
                FileCreated = fileInfo != null ? fileInfo.CreationTime : DateTime.Now,
                FileModified = fileInfo?.LastWriteTime,
                FileSize = fileInfo != null ? fileInfo.Length : 0,
            };
        }
    }
}
