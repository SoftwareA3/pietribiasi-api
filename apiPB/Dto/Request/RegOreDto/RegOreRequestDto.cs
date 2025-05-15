using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class RegOreRequestDto : A3AppBaseRequestDto
    {
        public string? Uom { get; set; }
        public long? WorkingTime { get; set; }
    }
}