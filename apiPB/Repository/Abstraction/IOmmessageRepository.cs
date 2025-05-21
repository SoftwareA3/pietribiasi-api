using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IOmmessageRepository
    {
        /// <summary>
        /// Ritorna la lista delle ingormazioni della vista VwOmmessage, filtrata per MoId
        /// </summary>
        /// <returns>Lista di VwOmmessage</returns>
        List<VwOmmessage> GetOmmessagesFilteredByMoId(MoIdFilter moIdFilter);
    }
}