using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IJobRepository
    {
        IEnumerable<VwApiJob> GetJobs();
        IEnumerable<VwApiMocomponent> GetMocomponent(MocomponentRequestFilter filter);
        IEnumerable<VwApiMostep> GetMostep(MostepRequestFilter filter);
        IEnumerable<VwApiMoStepsComponent> GetMoStepsComponent(MoStepsComponentRequestFilter filter);
        IEnumerable<VwApiMo> GetMo(MoRequestFilter filter);    
    }
}