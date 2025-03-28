using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IWorkerRepository
    {
        IEnumerable<VwApiWorker> GetWorkers();
        VwApiWorker? GetWorkerByPassword(PasswordWorkersRequestFilter filter);
        Task CallStoredProcedure(WorkerIdRequestFilter filter);
        Task CreateOrUpdateLastLogin (PasswordWorkersRequestFilter filter);
        IEnumerable<RmWorkersField> GetWorkersFieldsById(WorkerIdRequestFilter filter);
        public RmWorkersField? GetLastWorkerFieldLine(WorkerIdRequestFilter filter);
    }
}