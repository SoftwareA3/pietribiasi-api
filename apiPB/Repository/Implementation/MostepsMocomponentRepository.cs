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

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentJobDistinct(JobFilter filter)
        {
            return _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job).Distinct().ToList();
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentMonoDistinct(MonoFilter filter)
        {
            return _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate).Distinct().ToList();
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentOperationDistinct(OperationFilter filter)
        {
            return _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation && m.CreationDate == filter.CreationDate).Distinct().ToList();
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentBarCodeDistinct(BarCodeFilter filter)
        {
            return _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation && m.BarCode == filter.BarCode).Distinct().ToList();
        }
    }
}