using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Repository.Abstraction;
using apiPB.Models;
using apiPB.Filters;
using apiPB.Data;

namespace apiPB.Repository.Implementation
{
    public class GiacenzeRepository : IGiacenzeRepository
    {
        private readonly ApplicationDbContext _context;
        public GiacenzeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<VwApiGiacenze> GetGiacenze()
        {
            return _context.VwApiGiacenzes.ToList();
        }
    }
}