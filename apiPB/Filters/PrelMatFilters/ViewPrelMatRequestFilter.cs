using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class ViewPrelMatRequestFilter : ViewOreRequestFilter
    {
        public string? Component { get; set; }
        public string? BarCode { get; set; }
    }
}