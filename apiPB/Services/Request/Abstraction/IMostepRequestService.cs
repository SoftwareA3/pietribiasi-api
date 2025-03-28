using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Request.Abstraction
{
    public interface IMostepRequestService
    {
        IEnumerable<VwApiMostepDto> GetMostepByMoId(VwApiMostepRequestDto request);
    }
}