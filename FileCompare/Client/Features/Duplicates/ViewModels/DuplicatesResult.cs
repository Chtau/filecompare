﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Duplicates.ViewModels
{
    public class DuplicatesResult
    {
        public Guid DuplicateId { get; set; }

        public int DuplicateValue { get; set; }

        public int DuplicatesCount { get; set; }

        public string DuplicateTitle { get; set; }
    }
}
