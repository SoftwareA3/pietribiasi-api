using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO contenente unicamente il token di autenticazione per Mago.
    /// </summary>
    public class MagoTokenRequestDto
    {
        public string? Token { get; set; }
    }
}