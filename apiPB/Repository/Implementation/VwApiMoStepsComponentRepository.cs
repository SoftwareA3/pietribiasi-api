using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using Microsoft.EntityFrameworkCore;
using apiPB.Repository.Abstraction;
using apiPB.Models;


namespace apiPB.Repository.Implementation
{
    public class VwApiMoStepsComponentRepository : IVwApiMoStepsComponentRepository
    {
        private readonly ApplicationDbContext _context;

        public VwApiMoStepsComponentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Restituisce tutte le informazioni della vista vw_api_mo_steps_component
        // Parametri di ricerca: job, rtgStep, alternate, altRtgStep
        // Parametri di ricerca opzionali: position, component
        public IEnumerable<VwApiMoStepsComponent> GetVwApiMoStepsComponent(string job, short rtgStep, string alternate, short altRtgStep, short? position, string? component)
        {
            var query = _context.VwApiMoStepsComponents
            .AsNoTracking()
            .Where(m => m.Job == job && m.RtgStep == rtgStep && m.Alternate == alternate && m.AltRtgStep == altRtgStep);

            if (position != null)
            {
                query = query.Where(m => m.Position == position);
            }

            if (component != null)
            {
                query = query.Where(m => m.Component == component);
            }

            var list = query.ToList();

            Console.WriteLine(query.ToQueryString());

            return list;
        }
    }
}