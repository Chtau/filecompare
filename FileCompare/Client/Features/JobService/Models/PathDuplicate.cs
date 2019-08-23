using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.JobService.Models
{
    public class PathDuplicate
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public Guid DuplicateValueId { get; set; }

        public Guid PathCompareValueId { get; set; }
    }
}
