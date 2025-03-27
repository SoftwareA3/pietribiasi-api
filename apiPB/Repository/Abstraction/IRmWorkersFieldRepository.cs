using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;
using apiPB.Repository.Abstraction;

namespace apiPB.Repository.Abstraction
{
    public interface IRmWorkersFieldRepository
    {
        IEnumerable<RmWorkersField> GetRmWorkersFieldsById(int workerId);
        RmWorkersField? GetLastWorkerFeldLine(int workerId);
    }
}