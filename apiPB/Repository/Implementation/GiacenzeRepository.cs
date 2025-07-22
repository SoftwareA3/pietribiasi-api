using apiPB.Repository.Abstraction;
using apiPB.Models;
using apiPB.Data;
using apiPB.Utils.Implementation;
using apiPB.Filters;
using apiPB.Utils.Abstraction;

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
            var query = _context.VwApiGiacenzes;
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(GiacenzeRepository), nameof(GetGiacenze));
            return query.ToList();
        }

        public VwApiGiacenze GetGiacenzaByItem(ComponentFilter request)
        {
            if (request == null || string.IsNullOrEmpty(request.Component))
            {
                throw new ArgumentException("Invalid request: Item cannot be null or empty.", nameof(request));
            }

            var query = _context.VwApiGiacenzes
                .Where(g => g.Item == request.Component)
                .FirstOrDefault();
            return query ?? throw new ArgumentNullException("Item not found in giacenze.", nameof(request.Component));
        }
    }
}