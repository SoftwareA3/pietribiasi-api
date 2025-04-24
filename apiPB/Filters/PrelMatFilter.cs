using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class PrelMatFilter : A3AppFilter
    {   
        public short? Position { get; set; }

        public string? Component { get; set; }

        public string? UoM { get; set; }

        public string BarCode { get; set; } = null!;

        public int? PrelQty { get; set; }
    }
}