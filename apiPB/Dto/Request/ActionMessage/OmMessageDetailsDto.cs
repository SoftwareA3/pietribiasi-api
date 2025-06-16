using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per i dettagli dei messaggi di log associati alle operazioni.
    /// Questo DTO contiene informazioni generali riguardanti il tipo di messaggio, il testo dell'errore e la data
    /// </summary>
    public class OmMessageDetailsDto
    {
        public int? MessageId { get; set; }

        public string? MessageType { get; set; }

        public DateTime? MessageDate { get; set; }

        public string? MessageText { get; set; }
    }
}