using apiPB.Data;
using apiPB.Repository.Abstraction;
using apiPB.Models;
using Microsoft.EntityFrameworkCore;
using apiPB.Filters;

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
            return _context.A3AppPrelMats.AsNoTracking().OrderByDescending(i => i.SavedDate).ToList()
                ?? throw new Exception("Nessun risultato per GetAppPrelMat in PrelMatRepository");
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
                    PrelQty = filter.PrelQty
                };

                list.Add(prelMat);
            }

            _context.A3AppPrelMats.AddRange(list);
            _context.SaveChanges();

            return list ?? throw new Exception("Nessun risultato per PostPrelMat in PrelMatRepository");
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
                && (filter.Imported == null || i.Imported == filter.Imported.Value)
            );

            if (filter.Imported.HasValue && filter.Imported.Value == true)
            {
                return query.OrderByDescending(i => i.DataImp).ToList();
            }
            else
            {
                return query.OrderByDescending(i => i.SavedDate).ToList();
            }
        }

        public A3AppPrelMat PutViewPrelMat(ViewPrelMatPutFilter filter)
        {
            var editPrelMat = _context.A3AppPrelMats.FirstOrDefault(m => m.PrelMatId == filter.PrelMatId);

            if (editPrelMat == null)
            {
                throw new ArgumentNullException(nameof(editPrelMat), "L'oggetto A3AppPrelMat non è stato trovato.");
            }
            editPrelMat.SavedDate = DateTime.Now;
            editPrelMat.PrelQty = filter.PrelQty;
            _context.SaveChanges();

            return editPrelMat;
        }
        public A3AppPrelMat DeletePrelMatId(ViewPrelMatDeleteFilter filter)
        {
            var deletePrelMat = _context.A3AppPrelMats.Find(filter.PrelMatId);
            if (deletePrelMat == null)
            {
                throw new Exception($"L'oggetto A3AppPrelMat non è stato trovato con l'ID {filter.PrelMatId} per DeletePrelMatId in PrelMatRepository.");
            }
            _context.A3AppPrelMats.Remove(deletePrelMat);
            _context.SaveChanges();
            return deletePrelMat;
        }

        public IEnumerable<A3AppPrelMat> UpdatePrelMatImported(WorkerIdSyncFilter filter)
        {
            if (filter == null || filter.WorkerId == null)
            {
                throw new ArgumentNullException(nameof(filter), "Il filtro non può essere nullo");
            }
            var notImported = _context.A3AppPrelMats
                .Where(x => x.Imported == false)
                .ToList() ?? throw new Exception("Nessun risultato per UpdatePrelMatImported in PrelMatRepository");

            if (notImported.Count == 0)
            {
                return notImported;
            }

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
            return _context.A3AppPrelMats.Where(i => i.Component == filter.Component && i.Imported == false).ToList()
                ?? throw new Exception("Nessun risultato per GetPrelMatWithComponent in PrelMatRepository");
        }

        public IEnumerable<A3AppPrelMat> GetNotImportedPrelMat()
        {
            return _context.A3AppPrelMats
                .Where(x => x.Imported == false)
                .OrderByDescending(i => i.SavedDate)
                .ToList()
                ?? throw new Exception("Nessun risultato per GetNotImportedPrelMat in PrelMatRepository");
        }
        public IEnumerable<A3AppPrelMat> GetNotImportedAppPrelMatByFilter(ViewPrelMatRequestFilter filter)
        {
            return GetViewPrelMat(filter)
                .Where(i => i.Imported == false)
                .OrderByDescending(i => i.SavedDate)
                .ToList()
                ?? throw new Exception("Nessun risultato per GetNotImportedAppPrelMatByFilter in PrelMatRepository");
        }

        public IEnumerable<A3AppPrelMat> UpdateImportedById(UpdateImportedIdFilter filter)
        {
            if (filter == null || filter.WorkerId == null)
            {
                throw new ArgumentNullException(nameof(filter), "Il filtro non può essere nullo");
            }
            var notImported = _context.A3AppPrelMats
                .Where(x => x.Imported == false && x.PrelMatId == filter.Id)
                .ToList()
                ?? throw new Exception("Nessun risultato per UpdateImportedById in PrelMatRepository");

            if (notImported.Count == 0)
            {
                return notImported;
            }

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