using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IRmWorkersFieldRepository
    {
        IEnumerable<RmWorkersField> GetRmWorkersFieldsById(int workerId);
        
        RmWorkersField? GetLastWorkerFieldLine(int workerId);
    }
}