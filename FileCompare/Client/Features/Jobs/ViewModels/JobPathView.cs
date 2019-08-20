using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Jobs.ViewModels
{
    public class JobPathView
    {
        public Guid JobId { get; set; }
        public Guid JobCollectPathId { get; set; }
        public Guid CollectPathId { get; set; }
        public bool IncludeSubFolders { get; set; }
        public string Path { get; set; }
    }
}
