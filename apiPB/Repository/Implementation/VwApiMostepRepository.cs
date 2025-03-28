using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;

namespace apiPB.Repository.Implementation
{
    public class VwApiMostepRepository : IVwApiMostepRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiMostepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Restituisce tutte le informazioni della vista vw_api_mosteps
        // Parametri: Job
        public IEnumerable<VwApiMostep> GetVwApiMostep(VwApiMostepRequestFilter filter)
        {
            return _context.VwApiMosteps
            .Where(m => m.Job == filter.Job)
            .AsNoTracking()
            .ToList();
        }
    }
}