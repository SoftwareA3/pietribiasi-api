using apiPB.Repository.Abstraction;
using apiPB.Models;
using apiPB.Filters;
using apiPB.Data;

namespace apiPB.Repository.Implementation
{
    public class ActionMessageRepository : IActionMessageRepository
    {
        private readonly ApplicationDbContext _context;
        public ActionMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwOmActionMessage> GetActionMessagesByFilter(ImportedLogMessageFilter filter)
        {
            return _context.VwOmActionMessages
                .Where(x => x.Moid == filter.Moid
                            && x.RtgStep == filter.RtgStep
                            && x.Alternate == filter.Alternate
                            && x.AltRtgStep == filter.AltRtgStep
                            && (string.IsNullOrEmpty(filter.Mono) || x.Mono == filter.Mono)
                            && (string.IsNullOrEmpty(filter.Bom) || x.Bom == filter.Bom)
                            && (string.IsNullOrEmpty(filter.Variant) || x.Variant == filter.Variant)
                            && (string.IsNullOrEmpty(filter.Wc) || x.Wc == filter.Wc)
                            && (string.IsNullOrEmpty(filter.Operation) || x.Operation == filter.Operation)
                            && x.WorkerId == filter.WorkerId
                            && x.ActionType == filter.ActionType)   
                .OrderByDescending(x => x.ActionId)
                .ToList();
        }
    }
}