using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using System.Linq.Expressions;

namespace apiPB.Repository.Implementation
{
    public class MoStepRepository : IMoStepRepository, IGenericRepository<VwApiMostep>
    {
        private readonly ApplicationDbContext _context;

        public MoStepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiMostep> GetFiltered(Expression<Func<VwApiMostep, bool>> predicate, bool distinct = false)
        {
            var query = _context.VwApiMosteps.AsNoTracking().Where(predicate);
            
            if (distinct)
                query = query.Distinct();
            
            return query.ToList();
        }

        public IEnumerable<VwApiMostep> GetMostepWithJob(JobFilter filter)
        {
            
            return GetFiltered(m => m.Job == filter.Job, true);
        }

        public IEnumerable<VwApiMostep> GetMostepWithMono(MonoFilter filter)
        {
            return GetFiltered(m =>  m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate, true);
        }

        public IEnumerable<VwApiMostep> GetMostepWithOperation(OperationFilter filter)
        {
            return GetFiltered(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation && m.CreationDate == filter.CreationDate, true);
        }
    }
}