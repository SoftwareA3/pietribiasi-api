using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Request.Abstraction
{
    public interface IMoRequestService
    {
        IEnumerable<VwApiMoDto> GetVwApiMo(VwApiMoRequestDto request);
    }
}