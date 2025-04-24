using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class RegOreFilter : A3AppFilter
    {

        public string? Uom { get; set; }
        public long? WorkingTime { get; set; }
    }
}