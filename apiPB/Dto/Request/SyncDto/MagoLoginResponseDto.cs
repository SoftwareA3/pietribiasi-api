using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO di risposta per il login in Mago. Ritorna informazioni come il token di autenticazione.
    /// </summary>
    public class MagoLoginResponseDto
    {
        public string? Token { get; set; }
        public string? Subscription { get; set; }
        public bool? IsAdmin { get; set; }
    }
}