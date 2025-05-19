using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using System.Linq.Expressions;

namespace apiPB.Repository.Implementation
{
    public class OmmessageRepository : IOmmessageRepository
    {
        private readonly ApplicationDbContext _context;

        public OmmessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<VwOmmessage> GetOmmessagesFilteredByMoId(MoIdFilter moIdFilter)
        {
            if (moIdFilter == null)
            {
                return new List<VwOmmessage>();
            }
            return _context.VwOmmessages
                .Where(m => m.Moid == moIdFilter.Moid)
                .ToList();
        }
    }
}