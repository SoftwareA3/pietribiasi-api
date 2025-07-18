using apiPB.Data;
using apiPB.Repository.Abstraction;
using apiPB.Models;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;
using apiPB.Utils.Implementation;

namespace apiPB.Repository.Implementation
{
    public class PrelMatRepository : IPrelMatRepository
    {
        private readonly ApplicationDbContext _context;

        public PrelMatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<A3AppPrelMat> GetAppPrelMat()
        {
            var query = _context.A3AppPrelMats.AsNoTracking().OrderByDescending(i => i.SavedDate).ToList();
            ApplicationExceptionHandler.ValidateEmptyList(query, nameof(PrelMatRepository), nameof(GetAppPrelMat));
            return query;
        }

        public IEnumerable<A3AppPrelMat> PostPrelMatList(IEnumerable<PrelMatFilter> filterList)
        {
            var list = new List<A3AppPrelMat>();

            foreach (var filter in filterList)
            {
                // Creazione dell'oggetto da inserire nel database
                var prelMat = new A3AppPrelMat
                {
                    WorkerId = filter.WorkerId,
                    SavedDate = DateTime.Now,
                    Job = filter.Job,
                    RtgStep = filter.RtgStep,
                    Alternate = filter.Alternate,
                    AltRtgStep = filter.AltRtgStep,
                    Operation = filter.Operation,
                    OperDesc = filter.OperDesc,
                    Position = filter.Position,
                    Component = filter.Component,
                    Bom = filter.Bom,
                    Variant = filter.Variant,
                    ItemDesc = filter.ItemDesc,
                    Moid = filter.Moid,
                    Mono = filter.Mono,
                    CreationDate = filter.CreationDate,
                    UoM = filter.UoM,
                    ProductionQty = filter.ProductionQty,
                    ProducedQty = filter.ProducedQty,
                    ResQty = filter.ResQty,
                    Storage = filter.Storage,
                    BarCode = filter.BarCode,
                    Wc = filter.Wc,
                    PrelQty = filter.PrelQty,
                    Deleted = filter.Deleted,
                    NeededQty = filter.NeededQty,
                };

                list.Add(prelMat);
            }

            _context.A3AppPrelMats.AddRange(list);
            _context.SaveChanges();

            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(list, nameof(PrelMatRepository), nameof(PostPrelMatList));

            return list;
        }

        public IEnumerable<A3AppPrelMat> GetViewPrelMat(ViewPrelMatRequestFilter filter)
        {
            var query = _context.A3AppPrelMats.Where(i =>
                (filter.WorkerId == null || i.WorkerId == filter.WorkerId)
                && (filter.FromDateTime == null || i.SavedDate >= filter.FromDateTime)
                && (filter.ToDateTime == null || i.SavedDate <= filter.ToDateTime)
                && (filter.DataImp == null || i.DataImp >= filter.DataImp.Value.Date)
                && (string.IsNullOrEmpty(filter.Job) || i.Job == filter.Job)
                && (string.IsNullOrEmpty(filter.Operation) || i.Operation == filter.Operation)
                && (string.IsNullOrEmpty(filter.Mono) || i.Mono == filter.Mono)
                && (string.IsNullOrEmpty(filter.Component) || i.Component == filter.Component)
                && (string.IsNullOrEmpty(filter.BarCode) || i.BarCode == filter.BarCode)
            // && (filter.Imported == null || i.Imported == filter.Imported.Value)
            );

            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(PrelMatRepository), nameof(GetViewPrelMat));

            // Ordinamento ascendente se il campo DataImp è specificato e Imported è true
            if (filter.Imported.HasValue && filter.Imported.Value == true && filter.DataImp.HasValue)
            {
                return query.OrderBy(i => i.DataImp).ToList();
            }

            // Ordinamento discendente se il campo DataImp non è specificato e Imported è tue
            if (filter.Imported.HasValue && filter.Imported.Value == true)
            {
                return query.OrderByDescending(i => i.DataImp).ToList();
            }

            // Altrimenti, ordinamento discendente per SavedDate
            else
            {
                return query.OrderByDescending(i => i.SavedDate).ToList();
            }
        }

        public A3AppPrelMat PutViewPrelMat(ViewPrelMatPutFilter filter)
        {
            var editPrelMat = _context.A3AppPrelMats.FirstOrDefault(m => m.PrelMatId == filter.PrelMatId)
                ?? throw new ArgumentNullException($"Prelievo non trovato per l'aggiornamento con l'ID specificato {filter.PrelMatId} per PutViewPrelMat in in PrelMatRepository.");

            editPrelMat.SavedDate = DateTime.Now;
            editPrelMat.PrelQty = filter.PrelQty;
            _context.SaveChanges();

            return editPrelMat;
        }
        public A3AppPrelMat DeletePrelMatId(ViewPrelMatDeleteFilter filter)
        {
            var deletePrelMat = _context.A3AppPrelMats.Find(filter.PrelMatId)
                ?? throw new ArgumentNullException($"Il prelievo non è stato trovato con l'ID {filter.PrelMatId} per DeletePrelMatId in PrelMatRepository.");

            _context.A3AppPrelMats.Remove(deletePrelMat);
            _context.SaveChanges();
            return deletePrelMat;
        }

        public IEnumerable<A3AppPrelMat> UpdatePrelMatImported(WorkerIdSyncFilter filter, bool updateDeletedItems)
        {
            if (filter == null || filter.WorkerId == null)
            {
                throw new ArgumentNullException(nameof(filter), "Il filtro non può essere nullo");
            }
            var notImported = _context.A3AppPrelMats
                .Where(x => x.Imported == false && (updateDeletedItems ? x.Deleted == true : x.Deleted == false))
                .ToList();

            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(notImported, nameof(PrelMatRepository), nameof(UpdatePrelMatImported));

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

                _context.A3AppPrelMats.Update(item);
                _context.SaveChanges();
            }

            // Ritorna la lista aggiornata
            return GetAppPrelMat();
        }

        public IEnumerable<A3AppPrelMat> GetPrelMatWithComponent(ComponentFilter filter)
        {
            var query = _context.A3AppPrelMats.Where(i => i.Component == filter.Component && i.Imported == false).ToList();
            ApplicationExceptionHandler.ValidateEmptyList(query, nameof(PrelMatRepository), nameof(GetPrelMatWithComponent));
            return query;
        }

        public IEnumerable<A3AppPrelMat> GetNotImportedPrelMat()
        {
            var query = _context.A3AppPrelMats
                .Where(x => x.Imported == false)
                .OrderByDescending(i => i.SavedDate)
                .ToList();
            ApplicationExceptionHandler.ValidateEmptyList(query, nameof(PrelMatRepository), nameof(GetNotImportedPrelMat));
            return query;
        }

        public IEnumerable<A3AppPrelMat> GetNotImportedAppPrelMatByFilter(ViewPrelMatRequestFilter filter)
        {
            var query = GetViewPrelMat(filter)
                .Where(i => i.Imported == false)
                .OrderByDescending(i => i.SavedDate)
                .ToList();
            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(query, nameof(PrelMatRepository), nameof(GetNotImportedAppPrelMatByFilter));
            return query;
        }

        public IEnumerable<A3AppPrelMat> UpdateImportedById(UpdateImportedIdFilter filter, bool updateDeletedItems)
        {
            if (filter == null || filter.WorkerId == null)
            {
                throw new ArgumentNullException(nameof(filter), "Il filtro non può essere nullo");
            }
            var notImported = _context.A3AppPrelMats
                .Where(x => x.Imported == false && (updateDeletedItems ? x.Deleted == true : x.Deleted == false) && x.PrelMatId == filter.Id)
                .ToList();

            ApplicationExceptionHandler.ValidateNotNullOrEmptyList(notImported, nameof(PrelMatRepository), nameof(UpdateImportedById));

            // if (notImported.Count == 0)
            // {
            //     return notImported;
            // }

            foreach (var item in notImported)
            {
                item.Imported = true;
                item.UserImp = filter.WorkerId.ToString();
                item.DataImp = DateTime.Now;

                _context.A3AppPrelMats.Update(item);
                _context.SaveChanges();
            }

            // Ritorna la lista aggiornata
            return GetAppPrelMat();
        }
    }
}