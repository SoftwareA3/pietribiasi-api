using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiWorkerRepository
    {
        IEnumerable<VwApiWorker> GetVwApiWorkers();
        VwApiWorker? GetVwApiWorkerByPassword(string password);
        Task CallStoredProcedure(int workerId);
    }
}