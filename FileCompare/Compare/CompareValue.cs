﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Compare
{
    public class CompareValue
    {
        [Flags]
        public enum Types
        {
            None = 0,
            Hash = 1,
            FileName = 2,
            Directory = 4,
            Extension = 8,
        }

        public string Hash { get; set; }
        public string FileName { get; set; }
        public string Directory { get; set; }
        public string Extension { get; set; }
    }
}