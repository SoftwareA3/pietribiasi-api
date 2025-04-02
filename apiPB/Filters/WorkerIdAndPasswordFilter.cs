using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class WorkerIdAndPasswordFilter
    {
        public int WorkerId { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}