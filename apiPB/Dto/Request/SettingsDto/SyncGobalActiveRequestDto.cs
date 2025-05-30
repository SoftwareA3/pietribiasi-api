using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per la ricezione di una richiesta di sincronizzazione dello stato globale attivo.
    /// </summary>
    public class SyncGobalActiveRequestDto
    {
        public bool? SyncGlobalActive { get; set; }
    }
}