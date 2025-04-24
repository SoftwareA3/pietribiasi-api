using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class RegOreFilter
    {
        public int WorkerId { get; set; }

        public string? Job { get; set; }

        public short? RtgStep { get; set; }

        public string? Alternate { get; set; }

        public short? AltRtgStep { get; set; }

        public string Operation { get; set; } = null!;

        public string OperDesc { get; set; } = null!;

        public string Bom { get; set; } = null!;

        public string Variant { get; set; } = null!;

        public string? ItemDesc { get; set; }

        public int? Moid { get; set; }

        public string? Mono { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? Uom { get; set; }

        public double ProductionQty { get; set; }

        public double ProducedQty { get; set; }

        public double ResQty { get; set; }

        public string? Storage { get; set; }

        public string? Wc { get; set; }

        public long? WorkingTime { get; set; }
    }
}