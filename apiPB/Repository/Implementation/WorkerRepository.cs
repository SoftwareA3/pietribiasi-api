using apiPB.Models;
using apiPB.Data;
using apiPB.Repository.Abstraction;
using apiPB.Filters;
using Microsoft.EntityFrameworkCore;
using apiPB.Mappers.Dto;

namespace apiPB.Repository.Implementation
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiWorker> GetWorkers()
        {
            return _context.VwApiWorkers.AsNoTracking().ToList();
        }

        public VwApiWorker? GetWorkerByPassword(PasswordWorkersRequestFilter filter)
        {
            return _context.VwApiWorkers.AsNoTracking().FirstOrDefault(w => w.Password == filter.Password);
        }

        public Task CallStoredProcedure(WorkerIdAndValueRequestFilter filter)
        {
            Console.WriteLine("Calling stored procedure dbo.InsertWorkersFields: {0}, {1}", filter.WorkerId, filter.FieldValue);
            return _context.Database.ExecuteSqlRawAsync("EXEC dbo.InsertWorkersFields @WorkerId = {0}, @FieldValue = {1}", 
            filter.WorkerId, filter.FieldValue);
        }

        public async Task CreateOrUpdateLastLogin (PasswordWorkersRequestFilter filter)
        {
            var vwApiWorker = GetWorkerByPassword(filter);
            var workerIdRequestFilter = new WorkerIdAndValueRequestFilter
            {
                WorkerId = vwApiWorker?.WorkerId ?? throw new InvalidOperationException("Worker not found."),
                FieldValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            }; 
            await CallStoredProcedure(workerIdRequestFilter);
        }

        public IEnumerable<RmWorkersField> GetWorkersFieldsById(WorkerIdAndValueRequestFilter filter)
        {
            return _context.RmWorkersFields
            .Where(w => w.WorkerId == filter.WorkerId)
            .AsNoTracking()
            .ToList();
        }

        public RmWorkersField? GetLastWorkerFieldLine(WorkerIdAndValueRequestFilter filter)
        {
            // SELECT TOP 1 * FROM RM_WorkersFields WHERE WorkerID = {0} ORDER BY Line DESC
            return _context.RmWorkersFields
            .Where(w => w.WorkerId == filter.WorkerId)
            .OrderByDescending(w => w.Line)
            .AsNoTracking()
            .FirstOrDefault();
        }
    }
}