using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class BarCodeFilter : OperationFilter
    {
        public string? BarCode { get; set; } = string.Empty;
        public string? Component { get; set; } = string.Empty;
    }
}