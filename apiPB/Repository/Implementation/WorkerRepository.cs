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

        public VwApiWorker? GetWorkerByPassword(WorkersFilter filter)
        {
            return _context.VwApiWorkers.AsNoTracking().FirstOrDefault(w => w.Password == filter.Password);
        }

        public Task CallStoredProcedure(WorkersFieldFilter filter)
        {
            Console.WriteLine("Calling stored procedure dbo.InsertWorkersFields: {0}, {1}", filter.WorkerId, filter.FieldValue);
            if (filter.WorkerId == null || filter.FieldValue == null)
            {
                throw new ArgumentNullException("WorkerId or FieldValue cannot be null.");
            }

            return _context.Database.ExecuteSqlRawAsync("EXEC dbo.InsertWorkersFields @WorkerId = {0}, @FieldValue = {1}", 
                filter.WorkerId, filter.FieldValue);
        }

        public async Task CreateOrUpdateLastLogin (WorkersFilter filter)
        {
            var vwApiWorker = GetWorkerByPassword(filter);
            var workersFieldFilter = new WorkersFieldFilter
            {
                WorkerId = vwApiWorker?.WorkerId ?? throw new InvalidOperationException("Worker not found."),
                FieldValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            }; 
            await CallStoredProcedure(workersFieldFilter);
        }

        public IEnumerable<RmWorkersField> GetWorkersFieldsById(WorkersFieldFilter filter)
        {
            return _context.RmWorkersFields
            .Where(w => w.WorkerId == filter.WorkerId)
            .AsNoTracking()
            .ToList();
        }

        public RmWorkersField? GetLastWorkerFieldLine(WorkersFieldFilter filter)
        {
            // SELECT TOP 1 * FROM RM_WorkersFields WHERE WorkerID = {0} ORDER BY Line DESC
            return _context.RmWorkersFields
            .Where(w => w.WorkerId == filter.WorkerId)
            .OrderByDescending(w => w.Line)
            .AsNoTracking()
            .FirstOrDefault();
        }

        public VwApiWorker GetWorkerByIdAndPassword(WorkersFilter filter)
        {
            return _context.VwApiWorkers
            .Where(w => w.WorkerId == filter.WorkerId && w.Password == filter.Password)
            .AsNoTracking()
            .FirstOrDefault() ?? throw new InvalidOperationException("Worker not found.");
        }
    }
}