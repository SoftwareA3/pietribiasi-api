using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class MagoLoginResponseDto
    {
        public string? Token { get; set; }
        public string? Subscription { get; set; }
        public bool? IsAdmin { get; set; }
    }
}