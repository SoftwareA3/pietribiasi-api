using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using System.Linq.Expressions;

namespace apiPB.Repository.Implementation
{
    public class MostepsMocomponentRepository : IMostepsMocomponentRepository
    {
        private readonly ApplicationDbContext _context;

        public MostepsMocomponentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentJob(JobFilter filter)
        {
            return _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job).Distinct().ToList()
                ?? throw new Exception("Nessun risultato trovato per GetMostepsMocomponentJob in MostepsMocomponentRepository");
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentMono(MonoFilter filter)
        {
            return _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate).Distinct().ToList()
                ?? throw new Exception("Nessun risultato trovato per GetMostepsMocomponentMono in MostepsMocomponentRepository");
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentOperation(OperationFilter filter)
        {
            return _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation && m.CreationDate == filter.CreationDate).Distinct().ToList()
                ?? throw new Exception("Nessun risultato trovato per GetMostepsMocomponentOperation in MostepsMocomponentRepository");
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentBarCode(BarCodeFilter filter)
        {
            return _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation && m.BarCode == filter.BarCode).Distinct().ToList()
                ?? throw new Exception("Nessun risultato trovato per GetMostepsMocomponentBarCode in MostepsMocomponentRepository");
        }
    }
}