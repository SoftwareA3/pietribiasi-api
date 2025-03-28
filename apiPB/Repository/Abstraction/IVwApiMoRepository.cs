using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiMoRepository
    {
        IEnumerable<VwApiMo> GetVwApiMo(VwApiMoRequestFilter filter);        
    }
}