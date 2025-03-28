using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiMocomponentRepository
    {
        IEnumerable<VwApiMocomponent> GetVwApiMocomponent(VwApiMocomponentRequestFilter filter);
    }
}