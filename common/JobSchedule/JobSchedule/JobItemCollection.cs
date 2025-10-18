using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobSchedule
{
    internal class JobItem
    {
        public string Schedule { get; set; }
        public Dictionary<string, object> Argument { get; set; }
    }

    internal class JobItemCollection
    {
        public List<JobItem> JobItems { get; set; } = new List<JobItem>();
    }
}
