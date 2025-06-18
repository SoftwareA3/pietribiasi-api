using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class ImportedLogMessageFilter
    {
        public int? Moid { get; set; }

        public short? RtgStep { get; set; }

        public string? Alternate { get; set; }

        public short? AltRtgStep { get; set; }

        public string? Mono { get; set; }

        public string? Bom { get; set; }

        public string? Variant { get; set; }

        public string? Wc { get; set; }

        public string? Operation { get; set; }

        public int? WorkerId { get; set; }
        public int? ActionType { get; set; }
    }
}