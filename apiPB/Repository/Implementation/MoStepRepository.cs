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

        public IEnumerable<VwApiMostep> GetMostep(MostepRequestFilter filter)
        {
            // FIXME: controllo sui parametri e sulla richiesta
            return _context.VwApiMosteps
            .Where(m => m.Job == filter.Job)
            .AsNoTracking()
            .ToList();
        }

        public IEnumerable<VwApiMostep> GetMostepWithOdp(MostepOdpRequestFilter filter)
        {
            return _context.VwApiMosteps
            .AsNoTracking()
            .Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate)
            .Distinct()
            .ToList();
        }

        public IEnumerable<VwApiMostep> GetMostepWithLavorazione(MostepLavorazioniRequestFilter filter)
        {
            return _context.VwApiMosteps
            .AsNoTracking()
            .Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation)
            .Distinct()
            .ToList();
        }
    }
}