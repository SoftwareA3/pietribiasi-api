using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class OmmessageInfoRequestDto
    {
        public int? Moid { get; set; }

        public List<OmmessageDetailsRequestDto> OmmessageDetails { get; set; } = new List<OmmessageDetailsRequestDto>();
    }
}