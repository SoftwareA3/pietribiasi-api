using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class MagoLoginRequestDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Company { get; set; }
    }
}