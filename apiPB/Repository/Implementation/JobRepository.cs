using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;

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
            return _context.VwApiJobs.AsNoTracking().ToList();
        }

        public IEnumerable<VwApiMocomponent> GetMocomponent(MocomponentRequestFilter filter)
        {
            return _context.VwApiMocomponents
            .Where(m => m.Job == filter.Job)
            .AsNoTracking();
        }

        public IEnumerable<VwApiMo> GetMo(MoRequestFilter filter)
        {
            return _context.VwApiMos
            .Where(j => j.Job == filter.Job 
                && j.RtgStep == filter.RtgStep
                && j.Alternate == filter.Alternate
                && j.AltRtgStep == filter.AltRtgStep)
            .AsNoTracking()
            .ToList();
        }

        public IEnumerable<VwApiMostep> GetMostep(MostepRequestFilter filter)
        {
            return _context.VwApiMosteps
            .Where(m => m.Job == filter.Job)
            .AsNoTracking()
            .ToList();
        }

        public IEnumerable<VwApiMoStepsComponent> GetMoStepsComponent(MoStepsComponentRequestFilter filter)
        {
            var query = _context.VwApiMoStepsComponents
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
    }
}