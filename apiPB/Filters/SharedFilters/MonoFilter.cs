using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class MonoFilter : JobFilter
    {
        public required string Mono { get; set; }

        public required DateTime CreationDate { get; set; }
    }
}