using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// Questo DTO raccoglie le informazioni comuni delle operazioni e i messaggi di log associati.
    /// </summary>
    public class ActionMessageListDto
    {
        public string? ActionType { get; set; }
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
        public List<ActionMessageDetailsDto>? ActionMessageDetails { get; set; } = new List<ActionMessageDetailsDto>();
        public List<OmMessageDetailsDto>? OmMessageDetails { get; set; } = new List<OmMessageDetailsDto>();
    }
}