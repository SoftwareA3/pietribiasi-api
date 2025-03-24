using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using apiPB.Models;
using apiPB.Data;
using apiPB.Dto;

namespace apiPB.Services
{
    public class WorkerService
    {
        private readonly ApplicationDbContext _context;

        public WorkerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RmWorkersField?> GetWorkerFieldsLastLineFromWorkerId(int workerId)
        {
            // Dalla tabella WorkersFields, viene preso il numero massimo di Line del workerId
            var worker = await _context.RmWorkersFields
            .FromSqlRaw(@"SELECT TOP 1 * FROM RM_WorkersFields WHERE WorkerID = {0} ORDER BY Line DESC", workerId)
            .AsNoTracking()
            .FirstOrDefaultAsync();  
            
            return worker;
        }
    }
}