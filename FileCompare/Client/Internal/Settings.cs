﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Globalize;

namespace Client.Internal
{
    public class Settings : ISettings
    {
        public Settings()
        {
            Culture = Localize.Language.English;
            DBPath = System.IO.Path.Combine(StaticFolders.GetUserFolder(), GlobalValues.DBFileName);
        }

        public Localize.Language Culture { get; set; }
        public string DBPath { get; set; }
    }
}