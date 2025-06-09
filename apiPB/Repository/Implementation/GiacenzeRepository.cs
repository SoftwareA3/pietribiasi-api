using apiPB.Repository.Abstraction;
using apiPB.Models;
using apiPB.Data;
using apiPB.Utils.Implementation;

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
    }
}