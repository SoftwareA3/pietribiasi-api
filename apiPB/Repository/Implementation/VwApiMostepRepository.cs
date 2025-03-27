using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace apiPB.Repository.Implementation
{
    public class VwApiMostepRepository : IVwApiMostepRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiMostepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Restituisce tutte le informazioni della vista vw_api_mosteps filtrate per job
        public IEnumerable<VwApiMostep> GetVwApiMostep(string job)
        {
            return _context.VwApiMosteps
            .Where(m => m.Job == job)
            .AsNoTracking()
            .ToList();
        }
    }
}