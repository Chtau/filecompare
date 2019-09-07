using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.JobService.Models
{
    public class DuplicateResultProgress
    {
        [PrimaryKey]
        public Guid JobId { get; set; }
        public string Cache { get; set; }
        public DateTime DateTime { get; set; }
    }
}
