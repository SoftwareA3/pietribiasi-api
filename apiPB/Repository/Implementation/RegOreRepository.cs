using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;


namespace apiPB.Repository.Implementation
{
    public class RegOreRepository : IRegOreRepository
    {
        private readonly ApplicationDbContext _context;

        public RegOreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<A3AppRegOre> GetAppRegOre()
        {
            return _context.A3AppRegOres.AsNoTracking().ToList();
        }

        public IEnumerable<A3AppRegOre> PostRegOreList(IEnumerable<RegOreFilter> filterList)
        {
            var list = new List<A3AppRegOre>();

            foreach (var filter in filterList)
            {
                // Creazione dell'oggetto da inserire nel database
                var regOre = new A3AppRegOre
                {
                    WorkerId = filter.WorkerId,
                    SavedDate = DateTime.Now,
                    Job = filter.Job,
                    RtgStep = filter.RtgStep,
                    Alternate = filter.Alternate,
                    AltRtgStep = filter.AltRtgStep,
                    Operation = filter.Operation,
                    OperDesc = filter.OperDesc,
                    Bom = filter.Bom,
                    Variant = filter.Variant,
                    ItemDesc = filter.ItemDesc,
                    Moid = filter.Moid,
                    Mono = filter.Mono,
                    Uom = filter.Uom,
                    CreationDate = filter.CreationDate,
                    ProductionQty = filter.ProductionQty,
                    ProducedQty = filter.ProducedQty,
                    ResQty = filter.ResQty,
                    Storage = filter.Storage,
                    Wc = filter.Wc,
                    WorkingTime = filter.WorkingTime
                };

                list.Add(regOre);
            }

            _context.A3AppRegOres.AddRange(list);
            _context.SaveChanges();

            return list;
        }

        public IEnumerable<A3AppRegOre> GetAppViewOre(ViewOreRequestFilter filter)
        {
            var query = _context.A3AppRegOres.AsNoTracking();

            if(filter.FromDateTime != null && filter.ToDateTime != null)
            {
                query = query.Where(m => m.SavedDate >= filter.FromDateTime && m.SavedDate <= filter.ToDateTime);
            }
            else if(filter.FromDateTime != null && filter.ToDateTime == null)
            {
                query = query.Where(m => m.SavedDate >= filter.FromDateTime);
            }
            else if(filter.FromDateTime == null && filter.ToDateTime != null)
            {
                query = query.Where(m => m.SavedDate <= filter.ToDateTime);
            }

            if (filter.Job != null)
            {
                query = query.Where(m => m.Job == filter.Job);
            }

            if (filter.Operation != null)
            {
                query = query.Where(m => m.Operation == filter.Operation);
            }

            if (filter.Mono != null)
            {
                query = query.Where(m => m.Mono == filter.Mono);
            }

            if(filter.WorkerId != null)
            {
                query = query.Where(m => m.WorkerId == filter.WorkerId);
            }

            var list = query.ToList();
            return list;
        }

        public A3AppRegOre PutAppViewOre(ViewOrePutFilter filter)
        {
            var editRegOre = _context.A3AppRegOres.FirstOrDefault(m => m.RegOreId == filter.RegOreId);

            if (editRegOre != null)
            {
                editRegOre.WorkingTime = filter.WorkingTime;
                _context.SaveChanges();
            }

            return editRegOre ?? throw new InvalidOperationException("Record not found.");
        }

        public A3AppRegOre DeleteRegOreId(ViewOreDeleteRequestFilter filter)
        {
            var deleteRegOre = _context.A3AppRegOres.FirstOrDefault(m => m.RegOreId == filter.RegOreId);

            if (deleteRegOre != null)
            {
                _context.A3AppRegOres.Remove(deleteRegOre);
                _context.SaveChanges();
            }

            return deleteRegOre ?? throw new InvalidOperationException("Record not found.");
        }
    }
}