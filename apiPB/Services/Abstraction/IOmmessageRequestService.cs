using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface IOmmessageRequestService
    {
        /// <summary>
        /// Ritorna la lista delle informazioni della vista VwOmmessage, filtrata per MoId
        /// </summary>
        /// <param name="moIdRequestDtos">Richiesta di filtro per MoId</param>
        /// <returns>Lista di VwOmmessage</returns>
        List<OmmessageInfoRequestDto> GetOmmessageGroupedByMoid(MoIdRequestDto moIdRequestDtos);
    }
}