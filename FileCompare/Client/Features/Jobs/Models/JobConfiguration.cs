using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Jobs.Models
{
    public class JobConfiguration
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public Day Days { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        public int MaxRuntimeMinutes { get; set; }

        public string FileExtensions { get; set; }

        public int MaxParallelism { get; set; } = 2;
    }
}
