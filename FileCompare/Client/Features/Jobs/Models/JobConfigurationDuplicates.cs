using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Jobs.Models
{
    public class JobConfigurationDuplicates
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        
        public Guid JobId { get; set; }

        public int CompareValueTypes { get; set; }
    }
}
