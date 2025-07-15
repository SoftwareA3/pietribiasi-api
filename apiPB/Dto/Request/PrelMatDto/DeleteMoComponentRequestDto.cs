using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per la richiesta di eliminazione di un componente da un ordine di produzione nel prelievo di materliali.
    /// </summary>
    public class DeleteMoComponentRequestDto
    {
        public int? WorkerId { get; set; } // ID del lavoratore che effettua la richiesta
        public int MoId { get; set; }

        public int Position { get; set; }

        public string ExternalReferences { get; set; } = string.Empty; //id di riferimento della richiesta (tuo contatore)

        public string ExternalProgram { get; set; } = "Pietribiasi App"; //nome applicazione che fa la richiesta a piacere
    }
}