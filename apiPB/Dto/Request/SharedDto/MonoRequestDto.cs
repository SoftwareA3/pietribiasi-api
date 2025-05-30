using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// Dto per la richiesta di un'operazione di lavoro passando per attributo Mono e la data di creazione.
    /// </summary>
    public class MonoRequestDto : JobRequestDto
    {
        public required string Mono { get; set; }

        public DateTime CreationDate { get; set; }
    }
}