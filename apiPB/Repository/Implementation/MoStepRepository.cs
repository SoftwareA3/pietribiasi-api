using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using System.Linq.Expressions;
using apiPB.Utils.Implementation;

namespace apiPB.Repository.Implementation
{
    public class MoStepRepository : IMoStepRepository
    {
        private readonly ApplicationDbContext _context;

        public MoStepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiMostep> GetMostepWithJob(JobFilter filter)
        {
            var query = _context.VwApiMosteps.Where(m => m.Job == filter.Job).Distinct().ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(MoStepRepository), nameof(GetMostepWithJob));
            return query;
        }

        public IEnumerable<VwApiMostep> GetMostepWithMono(MonoFilter filter)
        {
            var query = _context.VwApiMosteps.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate).Distinct().ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(MoStepRepository), nameof(GetMostepWithMono));
            return query;
        }

        public IEnumerable<VwApiMostep> GetMostepWithOperation(OperationFilter filter)
        {
            var query = _context.VwApiMosteps.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation).Distinct().ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(MoStepRepository), nameof(GetMostepWithOperation));
            return query; 
        }
    }
}