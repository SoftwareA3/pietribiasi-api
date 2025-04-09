using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    public class MoStepsComponentDto
    {
        public string Job { get; set; } = null!;

        public short RtgStep { get; set; }

        public string Alternate { get; set; } = null!;

        public short AltRtgStep { get; set; }

        public short? Position { get; set; }

        public string? Component { get; set; }

        public string? Bom { get; set; }

        public string? Variant { get; set; }

        public string? ItemDesc { get; set; }

        public int Moid { get; set; }

        public string? Mono { get; set; }

        public DateTime? CreationDate { get; set; }

        public string? Uom { get; set; }

        public double? ProductionQty { get; set; }

        public double? ProducedQty { get; set; }

        public double? ResQty { get; set; }
    }
}