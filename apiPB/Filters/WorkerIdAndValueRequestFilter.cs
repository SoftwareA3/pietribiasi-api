using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class WorkerIdAndValueRequestFilter
    {
        public int WorkerId { get; set; }
        public string FieldValue { get; set; } = string.Empty;
    }
}