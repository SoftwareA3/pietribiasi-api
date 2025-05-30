using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO contenente l'ID di un lavoratore per sincronizzare i dati.
    /// </summary>
    public class WorkerIdSyncRequestDto
    {
        public int? WorkerId { get; set; }
    }
}