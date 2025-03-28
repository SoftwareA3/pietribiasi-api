using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiJobRepository
    {
        IEnumerable<VwApiJob> GetVwApiJobs();
    }
}