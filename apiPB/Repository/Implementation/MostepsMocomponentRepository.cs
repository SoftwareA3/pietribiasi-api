using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using System.Linq.Expressions;

namespace apiPB.Repository.Implementation
{
    public class MostepsMocomponentRepository : IMostepsMocomponentRepository, IGenericRepository<VwApiMostepsMocomponent>
    {
        private readonly ApplicationDbContext _context;

        public MostepsMocomponentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetFiltered(Expression<Func<VwApiMostepsMocomponent, bool>> predicate, bool distinct = false)
        {
            var query = _context.VwApiMostepsMocomponents.AsNoTracking().Where(predicate);

            if (distinct)
                query = query.Distinct();

            return query.ToList();
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentJobDistinct(JobFilter filter)
        {
            return GetFiltered(m => m.Job == filter.Job, true);
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentMonoDistinct(MonoFilter filter)
        {
            return GetFiltered(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate, true);
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentOperationDistinct(OperationFilter filter)
        {
            return GetFiltered(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation && m.CreationDate == filter.CreationDate, true);
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentBarCodeDistinct(BarCodeFilter filter)
        {
            return GetFiltered(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation && m.BarCode == filter.BarCode, true);
        }
    }
}