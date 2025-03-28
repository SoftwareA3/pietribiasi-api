using apiPB.Data;
using apiPB.Filters;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace apiPB.Repository.Implementation
{
    public class VwApiMoRepository : IVwApiMoRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiMoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Restituisce tutte le informazioni della vista vw_api_mo
        // Parametri di ricerca: Job, RtgStep, Alternate, AltRtgStep
        public IEnumerable<VwApiMo> GetVwApiMo(VwApiMoRequestFilter filter)
        {
            return _context.VwApiMos
            .Where(j => j.Job == filter.Job 
                && j.RtgStep == filter.RtgStep
                && j.Alternate == filter.Alternate
                && j.AltRtgStep == filter.AltRtgStep)
            .AsNoTracking()
            .ToList();
        }
    }
}