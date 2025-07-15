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
        Task<DeleteMoComponentRequestDto> DeleteMoComponentAsync(MagoLoginResponseDto responseDto, DeleteMoComponentRequestDto request);
        
        Task<AddMoComponentRequestDto> AddMoComponentAsync(MagoLoginResponseDto responseDto, AddMoComponentRequestDto request);
    }
}