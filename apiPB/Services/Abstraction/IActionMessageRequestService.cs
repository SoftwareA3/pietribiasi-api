using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Services.Abstraction
{
    public interface IActionMessageRequestService
    {
        ActionMessageListDto GetActionMessagesByFilter(ImportedLogMessageDto request);
    }
}