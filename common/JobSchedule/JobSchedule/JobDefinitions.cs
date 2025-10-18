using System;
using System.Collections.Generic;
using System.Text;

namespace JobSchedule
{
    public class JobDefinition
    {
        public string Name { get; set; }
        public string Schedule { get; set; }
        public Dictionary<string, object> Argument { get; set; }
    }
}
