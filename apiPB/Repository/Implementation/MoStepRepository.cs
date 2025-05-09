using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using System.Linq.Expressions;
using Azure;

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
            
            return _context.VwApiMosteps.Where(m => m.Job == filter.Job).Distinct().ToList();
        }

        public IEnumerable<VwApiMostep> GetMostepWithMono(MonoFilter filter)
        {
            return _context.VwApiMosteps.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate).Distinct().ToList();
        }

        public IEnumerable<VwApiMostep> GetMostepWithOperation(OperationFilter filter)
        {
            return _context.VwApiMosteps.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation).Distinct().ToList();
        }
    }
}