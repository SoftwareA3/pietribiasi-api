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
    public class VwApiJobRepository : IVwApiJobRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiJobRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiJob> GetVwApiJobs()
        {
            return _context.VwApiJobs.AsNoTracking().ToList();
        }
    }
}