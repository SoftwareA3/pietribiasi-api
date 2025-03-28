using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Services.Request.Abstraction
{
    public interface IWorkersRequestService
    {
        IEnumerable<WorkersFieldDto> GetWorkersFieldsById(WorkersFieldRequestDto request);
        WorkersFieldDto? GetLastWorkerFieldLine(WorkerDto request);
        IEnumerable<WorkerDto> GetWorkers();
        WorkerDto? GetWorkerByPassword(PasswordWorkersRequestDto request);
        Task CallStoredProcedure(WorkerDto request);
        Task<WorkersFieldDto?> UpdateOrCreateLastLogin(PasswordWorkersRequestDto request);
        
    }
}