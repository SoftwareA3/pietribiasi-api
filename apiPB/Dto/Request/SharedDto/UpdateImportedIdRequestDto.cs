using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// Dto per la richiesta di aggiornamento aggiornamento del parametro che indica la sincronizzazione di un'operazione
    /// Passando l'Id dell'operazione e l'Id del lavoratore che ha esegito la sincronizzazione, vengono
    /// modificati i campi necessari per stabilire che l'operazione è stata sincronizzata.
    public class UpdateImportedIdRequestDto : WorkerIdSyncRequestDto
    {
        public int Id { get; set; }
    }
}