using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiMoStepsComponentRepository
    {
        IEnumerable<VwApiMoStepsComponent> GetVwApiMoStepsComponent(VwApiMoStepsComponentRequestFilter filter);
    }
}