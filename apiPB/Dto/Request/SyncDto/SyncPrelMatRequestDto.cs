using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per la richiesta di sincronizzazione dei materiali prelevati.
    /// I primi campi raggruppano le informazioni comuni dei prelievi,
    /// mentre ActionDetails contiene la lista di dettagli specifici di ciascun prelievo.
    /// </summary>
    public class SyncPrelMatRequestDto
    {
        public int? MoId { get; set; }
        public int? WorkerId { get; set; }
        public string? ExternalReferences { get; set; } = string.Empty;
        public List<SyncPrelMatDetailsRequestdto> ActionDetails { get; set; } = new List<SyncPrelMatDetailsRequestdto>();
        public string? ExternalProgram { get; set; } = "Pietribiasi App";
    }
}