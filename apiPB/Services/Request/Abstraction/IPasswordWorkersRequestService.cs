using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Request.Abstraction
{
    public interface IPasswordWorkersRequestService
    {
        VwApiWorkerDto? GetWorkerByPassword(PasswordWorkersRequestDto request);
    }
}