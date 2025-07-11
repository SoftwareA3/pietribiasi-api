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
        int MoId { get; set; }

        int Position { get; set; }

        string ExternalReferences { get; set; } = string.Empty; //id di riferimento della richiesta (tuo contatore)

        string ExternalProgram { get; set; } = "Pietribiasi App"; //nome applicazione che fa la richiesta a piacere
    }
}