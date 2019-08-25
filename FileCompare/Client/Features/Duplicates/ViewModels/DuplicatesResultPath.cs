using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Duplicates.ViewModels
{
    public class DuplicatesResultPath
    {
        public Guid PathCompareValueId { get; set; }

        public string FileName { get; set; }

        public string Directory { get; set; }

        public string Extension { get; set; }
        public bool Checked { get; set; }
    }
}
