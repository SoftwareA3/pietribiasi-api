using apiPB.Models;
using apiPB.Data;
using apiPB.Repository.Abstraction;
using apiPB.Filters;
using Microsoft.EntityFrameworkCore;
using apiPB.Utils.Implementation;

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
            var query = _context.VwApiWorkers.AsNoTracking().ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(WorkerRepository), nameof(GetWorkers));
            return query;
        }

        public VwApiWorker GetWorkerByPassword(PasswordWorkersRequestFilter filter)
        {
            return _context.VwApiWorkers.AsNoTracking().FirstOrDefault(w => w.Password == filter.Password)
                ?? throw new ArgumentNullException("Nessun risultato per GetWorkerByPassword in WorkerRepository.");
        }

        public Task CallStoredProcedure(WorkerIdAndValueRequestFilter filter)
        {
            Console.WriteLine("Calling stored procedure dbo.InsertWorkersFields: {0}, {1}", filter.WorkerId, filter.FieldValue);
            return _context.Database.ExecuteSqlRawAsync("EXEC dbo.InsertWorkersFields @WorkerId = {0}, @FieldValue = {1}", 
            filter.WorkerId, filter.FieldValue) ?? throw new Exception("Errore durante l'esecuzione della stored procedure InsertWorkersFields in WorkerRepository.");
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

        public IEnumerable<VwApiWorkersfield> GetWorkersFieldsById(WorkerIdAndValueRequestFilter filter)
        {
            var query = _context.VwApiWorkersfields
                .Where(w => w.WorkerId == filter.WorkerId)
                .AsNoTracking()
                .ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(WorkerRepository), nameof(GetWorkersFieldsById));
            return query;
        }

        public VwApiWorkersfield? GetLastWorkerFieldLine(WorkerIdAndValueRequestFilter filter)
        {
            // SELECT TOP 1 * FROM RM_WorkersFields WHERE WorkerID = {0} ORDER BY Line DESC
            return _context.VwApiWorkersfields
            .Where(w => w.WorkerId == filter.WorkerId)
            .OrderByDescending(w => w.Line)
            .AsNoTracking()
            .FirstOrDefault()
                ?? throw new ArgumentNullException("Nessun risultato per GetLastWorkerFieldLine in WorkerRepository");
        }

        public VwApiWorker GetWorkerByIdAndPassword(WorkerIdAndPasswordFilter filter)
        {
            return _context.VwApiWorkers
            .Where(w => w.WorkerId == filter.WorkerId && w.Password == filter.Password)
            .AsNoTracking()
            .FirstOrDefault()
                ?? throw new ArgumentNullException("Nessun risultato per GetWorkerByIdAndPassword in WorkerRepository");
        }
    }
}