using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class ViewOreRequestFilter : A3AppViewBaseFilter
    {
        public string? Job { get; set; }
        public string? Operation { get; set; }
        public string? Mono { get; set; }
    }
}