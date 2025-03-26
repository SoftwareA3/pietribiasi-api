using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto
{
    public class VwApiMostepDto
    {
        public string Job { get; set; } = null!;

        public short RtgStep { get; set; }

        public string Alternate { get; set; } = null!;

        public short AltRtgStep { get; set; }

        public string? Wc { get; set; }

        public string? Operation { get; set; }

    public string Storage { get; set; } = null!;
    }
}