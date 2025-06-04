using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace apiPB.Repository.Implementation
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiJob> GetJobs()
        {
            return _context.VwApiJobs.AsNoTracking().Distinct().ToList()
                ?? throw new Exception("Nessun risultato trovato per GetJobs in JobRepository");
        }
    }
}