using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

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
            if (OnComparePartialFileName(srcCompareValue.FileName?.ToLower(), tarCompareValue.FileName?.ToLower(), 5))
            {
                similarValue |= CompareValue.Types.FileNamePartial;
            }
            return similarValue;
        }

        private bool OnComparePartialFileName(string fileName1, string fileName2, int maxDif)
        {
            if (!string.IsNullOrWhiteSpace(fileName1) && !string.IsNullOrWhiteSpace(fileName2))
            {
                string lName = fileName1;
                string sName = fileName2;
                if (fileName2.Length > fileName1.Length)
                {
                    lName = fileName2;
                    sName = fileName1;
                }
                var arLong = OnCreateCompareStringArray(lName);
                var arShot = OnCreateCompareStringArray(sName);
                
                var dif = arLong.Except(arShot);
                var difLong = lName.Length - dif.Count();
                if (difLong <= maxDif)
                    return true;

                // use different starting point for the [sName]
                sName = sName.Substring(1);
                arShot = OnCreateCompareStringArray(sName);
                dif = arLong.Except(arShot);
                difLong = lName.Length - dif.Count();
                if (difLong <= maxDif)
                    return true;
                sName = sName.Substring(1);
                arShot = OnCreateCompareStringArray(sName);
                dif = arLong.Except(arShot);
                difLong = lName.Length - dif.Count();
                if (difLong <= maxDif)
                    return true;
            }
            return false;
        }

        private List<string> OnCreateCompareStringArray(string value)
        {
            var retVal = new List<string>();
            for (int i = 0; i < value.Length;)
            {
                string val;
                if (value.Length > i + 3)
                    val = value.Substring(i, 3);
                else
                    val = value.Substring(i);
                retVal.Add(val);
                i += 3;
                if (i >= value.Length)
                    break;
            }
            return retVal;
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
