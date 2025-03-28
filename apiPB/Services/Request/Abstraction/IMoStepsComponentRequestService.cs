using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Request.Abstraction
{
    public interface IMoStepsComponentRequestService
    {
        IEnumerable<VwApiMoStepsComponentDto> GetMoStepsComponentByMoId(VwApiMoStepsComponentRequestDto request);
    }
}