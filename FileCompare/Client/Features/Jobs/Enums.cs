using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features.Jobs
{
    public enum JobType
    {
        Duplicates = 0
    }

    public enum JobState
    {
        Idle = 0,
        Starting = 1,
        Running = 2,
        Stopping = 3,
    }

    [Flags]
    public enum Day
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64
    }
}
