using apiPB.Models;
using apiPB.Filters;


namespace apiPB.Repository.Abstraction
{
    public interface IActionMessageRepository
    {
        IEnumerable<VwOmActionMessage> GetActionMessagesByFilter(ImportedLogMessageFilter filter);
    }
}