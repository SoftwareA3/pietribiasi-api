using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Repository.Abstraction;
using apiPB.Models;
using Microsoft.EntityFrameworkCore;

namespace apiPB.Repository.Implementation
{
    public class RmWorkersFieldRepository : IRmWorkersFieldRepository
    {
        private readonly ApplicationDbContext _context;

        public RmWorkersFieldRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // Restituisce tutti i campi del lavoratore
        public IEnumerable<RmWorkersField> GetRmWorkersFieldsById(int workerId)
        {
            return _context.RmWorkersFields
            .Where(w => w.WorkerId == workerId)
            .AsNoTracking()
            .ToList();
        }

        // Restituisce la MAX(Line) del lavoratore con id workerId
        public RmWorkersField? GetLastWorkerFeldLine(int workerId)
        {
            return _context.RmWorkersFields
            .FromSqlRaw(@"SELECT TOP 1 * FROM RM_WorkersFields WHERE WorkerID = {0} ORDER BY Line DESC", workerId)
            .AsNoTracking()
            .FirstOrDefault();
        }
    }
}