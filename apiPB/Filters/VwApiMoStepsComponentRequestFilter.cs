using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Filters
{
    public class VwApiMoStepsComponentRequestFilter
    {
        public string Job { get; set; } = null!;

        public short RtgStep { get; set; }

        public string Alternate { get; set; } = null!;

        public short AltRtgStep { get; set; }

        public short? Position { get; set; }

        public string? Component { get; set; }
    }
}