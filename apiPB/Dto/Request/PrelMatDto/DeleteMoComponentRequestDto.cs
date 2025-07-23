using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per la richiesta di eliminazione di un componente da un ordine di produzione nel prelievo di materliali.
    /// </summary>
    public class DeleteMoComponentRequestDto
    {
        // Per semplicità il WorkerId è inserito per creare l'ID della richiesta di login, ma non utilizzato nel contesto della cancellazione.
        // Viene quindi ignorato nella serializzazione JSON per l'invio del payload a Mago.
        [JsonIgnore]
        public int? WorkerId { get; set; } // ID del lavoratore che effettua la richiesta
        public int MoId { get; set; }

        public int Position { get; set; }

        public string? ExternalReferences { get; set; } = string.Empty; //id di riferimento della richiesta (tuo contatore)

        public string? ExternalProgram { get; set; } = "Pietribiasi App"; //nome applicazione che fa la richiesta a piacere
    }
}