using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Jobs.Models
{
    public class Job
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public JobType JobType { get; set; }
        public DateTime? LastExecuted { get; set; }
        public DateTime? NextExecution { get; set; }
        public JobState JobState { get; set; }
    }
}
