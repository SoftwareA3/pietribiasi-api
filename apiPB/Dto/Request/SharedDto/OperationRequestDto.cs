using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// Dto per la richiesta di un'operazione di lavoro passando per attributo una operation (lavorazione).
    /// </summary>
    public class OperationRequestDto : MonoRequestDto
    {
        public required string Operation { get; set; }
    }
}