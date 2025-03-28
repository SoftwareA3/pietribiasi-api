using apiPB.Filters;
using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiMostepRepository
    {
        IEnumerable<VwApiMostep> GetVwApiMostep(VwApiMostepRequestFilter filter);
    }
}