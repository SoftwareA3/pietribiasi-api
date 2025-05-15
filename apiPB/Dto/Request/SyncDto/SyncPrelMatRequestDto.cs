using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class SyncPrelMatRequestDto
    {
        public int? MoId { get; set; }
        public int? RtgStep { get; set; }
        public string? Alternate { get; set; } = string.Empty;
        public int? AltRtgStep { get; set; }
        public int? WorkerId { get; set; }
        public List<SyncPrelMatDetailsRequestdto> ActionDetails { get; set; } = new List<SyncPrelMatDetailsRequestdto>();
    }
}