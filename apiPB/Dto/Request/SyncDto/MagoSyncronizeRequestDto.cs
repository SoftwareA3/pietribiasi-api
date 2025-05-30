using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO che "attacca" un workerId alla richiesta di sincronizzazione in Mago.
    /// Questo DTO è necessario al fine di passare un unico DTO con il WorkerId dell'operatore che ha effettuato la sincronizzazione.
    /// Con questo è possibile stabilire chi ha effettuato la sincronizzazione.
    /// /// </summary>
    public class MagoSyncronizeRequestDto : MagoLoginRequestDto
    {
        public int? WorkerId { get; set; }
    }
}