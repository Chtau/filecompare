using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Jobs.Models
{
    public class JobCollectPath
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Guid CollectPathId { get; set; }
        public bool IncludeSubFolders { get; set; }
    }
}
