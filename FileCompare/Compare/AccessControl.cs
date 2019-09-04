using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compare
{
    public static class AccessControl
    {
        public static bool Directory(string path)
        {
            try
            {
                var dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    if (System.IO.Directory.Exists(path))
                        return true;
                }
            } catch (Exception)
            { }
            return false;
        }

        public static bool File(string path)
        {
            try
            {
                var fInfo = new FileInfo(path);
                if (!fInfo.IsReadOnly)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.OpenRead(path);
                        return true;
                    }
                }
            } catch (Exception)
            {
                
            }
            return false;
        }
    }
}
