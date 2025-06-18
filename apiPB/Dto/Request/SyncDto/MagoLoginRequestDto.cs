using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per la richiesta di login a Mago. Vengono passati i parametri di accesso
    /// per ricevere un token di autenticazione con il quale effettuare le successive richieste.
    /// </summary>
    public class MagoLoginRequestDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Company { get; set; }
    }
}