using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;

namespace apiPB.Repository.Implementation
{
    public class MoStepRepository : IMoStepRepository
    {
        private readonly ApplicationDbContext _context;

        public MoStepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiMostep> GetMostepWithJob(MostepJobFilter filter)
        {
            // FIXME: controllo sui parametri e sulla richiesta
            return _context.VwApiMosteps
            .AsNoTracking()
            .Where(m => m.Job == filter.Job)
            .Distinct()
            .ToList();
        }

        public IEnumerable<VwApiMostep> GetMostepWithMono(MostepMonoFilter filter)
        {
            return _context.VwApiMosteps
            .AsNoTracking()
            .Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate)
            .Distinct()
            .ToList();
        }

        public IEnumerable<VwApiMostep> GetMostepWithOperation(MostepOperationFilter filter)
        {
            return _context.VwApiMosteps
            .AsNoTracking()
            .Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation)
            .Distinct()
            .ToList();
        }
    }
}