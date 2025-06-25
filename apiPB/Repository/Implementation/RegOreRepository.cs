using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using apiPB.Utils.Implementation;


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
            var query = _context.A3AppRegOres.AsNoTracking().OrderByDescending(i => i.SavedDate).ToList();
            ApplicationExceptionHandler.ValidateEmptyList(query, nameof(RegOreRepository), nameof(GetAppRegOre));
            return query;
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

            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(list, nameof(RegOreRepository), nameof(PostRegOreList));

            return list;
        }

        public IEnumerable<A3AppRegOre> GetAppViewOre(ViewOreRequestFilter filter)
        {
            var query = _context.A3AppRegOres.Where(i =>
                (filter.WorkerId == null || i.WorkerId == filter.WorkerId)
                && (filter.FromDateTime == null || i.SavedDate >= filter.FromDateTime)
                && (filter.ToDateTime == null || i.SavedDate <= filter.ToDateTime)
                && (filter.DataImp == null || i.DataImp >= filter.DataImp.Value.Date)
                && (string.IsNullOrEmpty(filter.Job) || i.Job == filter.Job)
                && (string.IsNullOrEmpty(filter.Operation) || i.Operation == filter.Operation)
                && (string.IsNullOrEmpty(filter.Mono) || i.Mono == filter.Mono)
            // && (filter.Imported == null || i.Imported == filter.Imported.Value)
            );

            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(RegOreRepository), nameof(GetAppViewOre));

            if (filter.Imported.HasValue && filter.Imported.Value == true)
            {
                return query.OrderByDescending(i => i.DataImp).ToList();
            }
            else
            {
                return query.OrderByDescending(i => i.SavedDate).ToList();
            }
        }

        public A3AppRegOre PutAppViewOre(ViewOrePutFilter filter)
        {
            var editRegOre = _context.A3AppRegOres.FirstOrDefault(m => m.RegOreId == filter.RegOreId)
                ?? throw new ArgumentNullException($"Registrazione ore non trovata per l'aggiornamento con l'ID specificato {filter.RegOreId} per PutAppViewOre in in RegOreRepository.");

            editRegOre.SavedDate = DateTime.Now;
            editRegOre.WorkingTime = filter.WorkingTime;
            _context.SaveChanges();

            return editRegOre;
        }

        public A3AppRegOre DeleteRegOreId(ViewOreDeleteRequestFilter filter)
        {
            var deleteRegOre = _context.A3AppRegOres.FirstOrDefault(m => m.RegOreId == filter.RegOreId)
                ?? throw new ArgumentNullException($"Registrazione ore non trovata per l'eliminazione con l'ID specificato {filter.RegOreId} per DeleteRegOreId in RegOreRepository.");

            _context.A3AppRegOres.Remove(deleteRegOre);
            _context.SaveChanges();

            return deleteRegOre;
        }

        public IEnumerable<A3AppRegOre> GetNotImportedRegOre()
        {
            var query = _context.A3AppRegOres
                .Where(x => x.Imported == false)
                .OrderByDescending(i => i.SavedDate)
                .ToList();
            ApplicationExceptionHandler.ValidateEmptyList(query, nameof(RegOreRepository), nameof(GetNotImportedRegOre));
            return query;
        }

        public IEnumerable<A3AppRegOre> UpdateRegOreImported(WorkerIdSyncFilter? filter)
        {
            if (filter == null || filter.WorkerId == null)
            {
                throw new ArgumentNullException(nameof(filter), "Il filtro non può essere nullo");
            }
            var notImported = _context.A3AppRegOres
                .Where(x => x.Imported == false)
                .ToList();
            
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(notImported, nameof(RegOreRepository), nameof(UpdateRegOreImported));

            // Deprecato: gestito da un'eccezione che viene lanciata se non ci sono elementi da importare
            // if (notImported.Count == 0)
            // {
            //     return notImported;
            // }

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
            var query =  GetAppViewOre(filter)
                .Where(i => i.Imported == false)
                .OrderByDescending(i => i.SavedDate)
                .ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(RegOreRepository), nameof(GetNotImportedAppRegOreByFilter));
            return query;
        }

        public IEnumerable<A3AppRegOre> UpdateImportedById(UpdateImportedIdFilter filter)
        {
            if (filter == null || filter.WorkerId == null)
            {
                throw new ArgumentNullException(nameof(filter), "Il filtro non può essere nullo o l'ID deve essere maggiore di zero e il WorkerId non può essere nullo.");
            }
            var notImported = _context.A3AppRegOres
                .Where(x => x.Imported == false && x.RegOreId == filter.Id)
                .ToList();
            
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(notImported, nameof(RegOreRepository), nameof(UpdateImportedById));

            // if (notImported.Count == 0)
            // {
            //     return notImported;
            // }

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