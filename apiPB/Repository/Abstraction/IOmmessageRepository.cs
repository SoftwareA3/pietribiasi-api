using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IOmmessageRepository
    {
        List<VwOmmessage> GetOmmessagesFilteredByMoId(MoIdFilter moIdFilter);
    }
}