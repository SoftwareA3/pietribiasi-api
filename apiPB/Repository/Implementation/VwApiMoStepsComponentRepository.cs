using apiPB.Data;
using Microsoft.EntityFrameworkCore;
using apiPB.Repository.Abstraction;
using apiPB.Models;
using apiPB.Filters;

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
        // Parametri di ricerca: Job, RtgStep, Alternate, AltRtgStep
        // Parametri di ricerca opzionali: Position, Component
        public IEnumerable<VwApiMoStepsComponent> GetVwApiMoStepsComponent(VwApiMoStepsComponentRequestFilter filter)
        {
            var query = _context.VwApiMoStepsComponents
            .AsNoTracking()
            .Where(m => m.Job == filter.Job && m.RtgStep == filter.RtgStep && m.Alternate == filter.Alternate && m.AltRtgStep == filter.AltRtgStep);

            if (filter.Position != null)
            {
                query = query.Where(m => m.Position == filter.Position);
            }

            if (filter.Component != null)
            {
                query = query.Where(m => m.Component == filter.Component);
            }

            var list = query.ToList();

            Console.WriteLine(query.ToQueryString());

            return list;
        }
    }
}