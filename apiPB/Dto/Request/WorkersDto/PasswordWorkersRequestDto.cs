using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per la richiesta di una password per i lavoratori.
    /// Tramite la password vengono recuperate le altre informazioni necessarie all'autenticazione.
    /// </summary>
    public class PasswordWorkersRequestDto
    {
        public string Password { get; set; } = string.Empty;
    }
}