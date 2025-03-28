using Microsoft.EntityFrameworkCore;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using apiPB.Filters;


namespace apiPB.Repository.Implementation
{
    public class VwApiMocomponentRepository : IVwApiMocomponentRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiMocomponentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Restituisce tutte le informazioni della vista vw_api_mocomponents
        // Parametri: Job
        public IEnumerable<VwApiMocomponent> GetVwApiMocomponent(VwApiMocomponentRequestFilter filter)
        {
            return _context.VwApiMocomponents
            .Where(m => m.Job == filter.Job)
            .AsNoTracking();
        }
    }
}