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

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponent(MostepsMocomponentRequestFilter filter)
        {
            // FIXME
            var query = _context.VwApiMostepsMocomponents
            .AsNoTracking()
            .Where(m => m.Job == filter.Job && m.RtgStep == filter.RtgStep && m.Alternate == filter.Alternate && m.AltRtgStep == filter.AltRtgStep);

            if (filter.Position != null)
            {
                query = query.Where(m => m.Position == filter.Position);
            }

            if (filter.Component != null)
            {
                query = query.Where(m => m.Component == filter.Component);
            }

            var list = query.ToList();

            return list;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentDistinct(MostepsMocomponentRequestFilter filter)
        {
            // FIXME
            var query = _context.VwApiMostepsMocomponents
            .AsNoTracking()
            .Where(m => m.Job == filter.Job && m.RtgStep == filter.RtgStep && m.Alternate == filter.Alternate && m.AltRtgStep == filter.AltRtgStep)
            .Distinct();

            if (filter.Position != null)
            {
                query = query.Where(m => m.Position == filter.Position);
            }

            if (filter.Component != null)
            {
                query = query.Where(m => m.Component == filter.Component);
            }

            var list = query.ToList();

            return list;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentJobDistinct(MostepsMocomponentRequestFilter filter)
        {
            GetFiltered(m => m.Job == filter.Job, true);
            return _context.VwApiMostepsMocomponents;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentMonoDistinct(MostepsMocomponentRequestFilter filter)
        {
            GetFiltered(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate, true);
            return _context.VwApiMostepsMocomponents;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentOperationDistinct(MostepsMocomponentRequestFilter filter)
        {
            GetFiltered(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation, true);
            return _context.VwApiMostepsMocomponents;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentBarCodeDistinct(MostepsMocomponentRequestFilter filter)
        {
            GetFiltered(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation && m.BarCode == filter.BarCode, true);
            return _context.VwApiMostepsMocomponents;
        }
    }
}