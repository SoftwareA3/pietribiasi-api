using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Services.Request.Abstraction
{
    public interface IMocomponentRequestService
    {
        IEnumerable<VwApiMocomponentDto> GetVwApiMocomponent(VwApiMocomponentRequestDto request);
    }
}