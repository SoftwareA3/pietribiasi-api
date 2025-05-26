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
            return _context.A3AppRegOres.Where(i =>
                (filter.WorkerId == null || i.WorkerId == filter.WorkerId)
                && (filter.FromDateTime == null || i.SavedDate >= filter.FromDateTime)
                && (filter.ToDateTime == null || i.SavedDate <= filter.ToDateTime)
                && (filter.DataImp == null || i.DataImp >= filter.DataImp.Value.Date)
                && (string.IsNullOrEmpty(filter.Job) || i.Job == filter.Job)
                && (string.IsNullOrEmpty(filter.Operation) || i.Operation == filter.Operation)
                && (string.IsNullOrEmpty(filter.Mono) || i.Mono == filter.Mono)
            ).ToList();
        }

        public A3AppRegOre PutAppViewOre(ViewOrePutFilter filter)
        {
            var editRegOre = _context.A3AppRegOres.FirstOrDefault(m => m.RegOreId == filter.RegOreId);

            if (editRegOre == null)
            {
                return null;
            }
            editRegOre.SavedDate = DateTime.Now;
            editRegOre.WorkingTime = filter.WorkingTime;
            _context.SaveChanges();

            return editRegOre;
        }

        public A3AppRegOre DeleteRegOreId(ViewOreDeleteRequestFilter filter)
        {
            var deleteRegOre = _context.A3AppRegOres.FirstOrDefault(m => m.RegOreId == filter.RegOreId);

            if (deleteRegOre == null)
            {
                return null;
            }

            _context.A3AppRegOres.Remove(deleteRegOre);
            _context.SaveChanges();

            return deleteRegOre;
        }

        public IEnumerable<A3AppRegOre> GetNotImportedRegOre()
        {
            return _context.A3AppRegOres
                .Where(x => x.Imported == false)
                .ToList();
        }

        public IEnumerable<A3AppRegOre> UpdateRegOreImported(WorkerIdSyncFilter? filter)
        {
            var notImported = _context.A3AppRegOres
                .Where(x => x.Imported == false)
                .ToList();

            if (notImported.Count == 0)
            {
                return notImported;
            }

            foreach (var item in notImported)
            {
                item.Imported = true;
                item.UserImp = filter.WorkerId.ToString();
                item.DataImp = DateTime.Now;

                var dbItem = _context.A3AppRegOres.FirstOrDefault(x => x.RegOreId == item.RegOreId);
                if (dbItem != null)
                {
                    dbItem.Imported = item.Imported;
                    dbItem.UserImp = item.UserImp;
                    dbItem.DataImp = item.DataImp;
                }
            }
            _context.SaveChanges();

            // Ritorna la lista aggiornata
            return GetAppRegOre();
        }

        public IEnumerable<A3AppRegOre> GetNotImportedAppRegOreByFilter(ViewOreRequestFilter filter)
        {
            return GetAppViewOre(filter)
                .Where(i => i.Imported == false)
                .ToList();
        }

        public IEnumerable<A3AppRegOre> UpdateImportedById(UpdateImportedIdFilter filter)
        {
            var notImported = _context.A3AppRegOres
                .Where(x => x.Imported == false && x.RegOreId == filter.Id)
                .ToList();

            if (notImported.Count == 0)
            {
                return notImported;
            }

            foreach (var item in notImported)
            {
                item.Imported = true;
                item.UserImp = filter.WorkerId.ToString();
                item.DataImp = DateTime.Now;

                var dbItem = _context.A3AppRegOres.FirstOrDefault(x => x.RegOreId == item.RegOreId);
                if (dbItem != null)
                {
                    dbItem.Imported = item.Imported;
                    dbItem.UserImp = item.UserImp;
                    dbItem.DataImp = item.DataImp;

                    _context.A3AppRegOres.Update(dbItem);
                    _context.SaveChanges();
                }
            }
            // Ritorna la lista aggiornata
            return GetAppRegOre();
        }
    }
}