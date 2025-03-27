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
    public class VwApiMoRepository : IVwApiMoRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiMoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Restituisce tutte le informazioni della vista vw_api_mo
        // Parametri di ricerca: job, rtgStep, alternate, altRtgStep
        public IEnumerable<VwApiMo> GetVwApiMo(string job, short rtgStep, string alternate, short altRtgStep)
        {
            return _context.VwApiMos
            .Where(j => j.Job == job 
                && j.RtgStep == rtgStep 
                && j.Alternate == alternate 
                && j.AltRtgStep == altRtgStep)
            .AsNoTracking()
            .ToList();
        }
    }
}