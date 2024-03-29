﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client.Internal
{
    public static class StaticFolders
    {
        public static string GetUserFolder()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = Path.Combine(folder, GlobalValues.AppName);
            return GetFolder(folder);
        }

        public static string GetApplicationFolder()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static string GetFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return folder;
        }

        public static bool IsValidFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void OpenDirectory(string directory, string filename = null, string extension = null)
        {
            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (!extension.StartsWith("."))
                    extension = "." + extension;
            }
            string filePath;
            if (!string.IsNullOrWhiteSpace(filename))
            {
                if (!string.IsNullOrWhiteSpace(extension))
                    filePath = System.IO.Path.Combine(directory, filename + extension);
                else
                    filePath = System.IO.Path.Combine(directory, filename);
            } else
            {
                filePath = directory;
            }
            string argument = "/select, \"" + filePath + "\"";

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}
