using apiPB.Models;
using apiPB.Data;
using apiPB.Repository.Abstraction;
using apiPB.Filters;
using Microsoft.EntityFrameworkCore;

namespace apiPB.Repository.Implementation
{
    public class VwApiWorkerRepository : IVwApiWorkerRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiWorkerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Restituisce tutte le informazioni della vista vw_api_workers
        public IEnumerable<VwApiWorker> GetVwApiWorkers()
        {
            return _context.VwApiWorkers.AsNoTracking().ToList();
        }

        // Restituisce tutte le informazioni della vista vw_api_workers 
        // Parametri: Password
        public VwApiWorker? GetVwApiWorkerByPassword(PasswordWorkersRequestFilter filter)
        {
            return _context.VwApiWorkers.AsNoTracking().FirstOrDefault(w => w.Password == filter.Password);
        }

        // Invoca la stored procedure dbo.InsertWorkersFields passando workerId e la dataora corrente
        public Task CallStoredProcedure(int workerId)
        {
            return _context.Database.ExecuteSqlRawAsync("EXEC dbo.InsertWorkersFields @WorkerId = {0}, @FieldValue = {1}", 
            workerId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}