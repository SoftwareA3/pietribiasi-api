using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using System.Linq.Expressions;
using apiPB.Utils.Implementation;

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
            var query = _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job).Distinct().ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(MostepsMocomponentRepository), nameof(GetMostepsMocomponentJob));
            return query;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentMono(MonoFilter filter)
        {
            var query = _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate).Distinct().ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(MostepsMocomponentRepository), nameof(GetMostepsMocomponentMono));
            return query;
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentOperation(OperationFilter filter)
        {
            var query = _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job && m.Mono == filter.Mono && m.CreationDate == filter.CreationDate && m.Operation == filter.Operation).Distinct().ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(MostepsMocomponentRepository), nameof(GetMostepsMocomponentOperation));
            return query; 
        }

        public IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentBarCode(BarCodeFilter filter)
        {
            var query = _context.VwApiMostepsMocomponents.Where(m => m.Job == filter.Job
                && m.Mono == filter.Mono
                && m.CreationDate == filter.CreationDate
                && m.Operation == filter.Operation
                && m.Component == filter.Component
                && (string.IsNullOrEmpty(filter.BarCode) || m.BarCode == filter.BarCode)).ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(MostepsMocomponentRepository), nameof(GetMostepsMocomponentBarCode));
            return query;
        }
    }
}