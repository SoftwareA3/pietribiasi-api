using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class A3AppViewBaseFilter
    {
        public int? WorkerId { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public DateTime? DataImp { get; set; }
        public bool? Imported { get; set; } = false; // Default false
    }
}