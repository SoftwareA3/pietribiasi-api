using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;


namespace apiPB.Repository.Implementation
{
    public class VwApiMocomponentRepository : IVwApiMocomponentRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiMocomponentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiMocomponent> GetVwApiMocomponent(string job)
        {
            return _context.VwApiMocomponents
            .Where(m => m.Job == job)
            .AsNoTracking();
        }
    }
}