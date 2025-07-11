using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface IMagoMaterialsRequestService
    {
        Task<IEnumerable<DeleteMoComponentRequestDto>> DeleteMoComponentAsync(MagoLoginResponseDto responseDto, IEnumerable<DeleteMoComponentRequestDto> request);
    }
}