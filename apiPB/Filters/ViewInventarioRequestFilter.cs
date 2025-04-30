using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class ViewInventarioRequestFilter : A3AppViewBaseFilter
    {
        public string? Item { get; set; }
        public string? BarCode { get; set; }
    }
}