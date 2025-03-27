using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;
using apiPB.Data;
using apiPB.Repository.Abstraction;
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

        public IEnumerable<VwApiWorker> GetVwApiWorkers()
        {
            return _context.VwApiWorkers.AsNoTracking().ToList();
        }

        public VwApiWorker? GetVwApiWorkerByPassword(string password)
        {
            return _context.VwApiWorkers.AsNoTracking().FirstOrDefault(w => w.Password == password);
        }

        public Task CallStoredProcedure(int workerId)
        {
            return _context.Database.ExecuteSqlRawAsync("EXEC dbo.InsertWorkersFields @WorkerId = {0}, @FieldValue = {1}", 
            workerId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}