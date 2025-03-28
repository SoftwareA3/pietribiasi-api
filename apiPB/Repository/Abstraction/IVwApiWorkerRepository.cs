using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiWorkerRepository
    {
        IEnumerable<VwApiWorker> GetVwApiWorkers();
        VwApiWorker? GetVwApiWorkerByPassword(PasswordWorkersRequestFilter filter);
        Task CallStoredProcedure(int workerId);
    }
}