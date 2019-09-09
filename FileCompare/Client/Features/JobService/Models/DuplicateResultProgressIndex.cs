using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.JobService.Models
{
    public class DuplicateResultProgressIndex
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public string Value { get; set; }
    }
}
