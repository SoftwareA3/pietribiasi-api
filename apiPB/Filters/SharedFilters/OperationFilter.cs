using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class OperationFilter : MonoFilter
    {
        public required string Operation { get; set; }
    }
}