using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.JobService.Models
{
    public class PathCompareValue
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public string FullFile { get; set; }

        public string Hash { get; set; }

        public string FileName { get; set; }

        public string Directory { get; set; }

        public string Extension { get; set; }

        public DateTime LastChange { get; set; }
    }
}
