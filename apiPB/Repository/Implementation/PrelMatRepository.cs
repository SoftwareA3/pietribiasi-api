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
            return _context.A3AppPrelMats.AsNoTracking().ToList();
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

            return list;
        }
        
        public IEnumerable<A3AppPrelMat> GetViewPrelMat(ViewPrelMatRequestFilter filter)
        {
            return _context.A3AppPrelMats.Where(i =>
                (filter.WorkerId == null || i.WorkerId == filter.WorkerId)
                && (filter.FromDateTime == null || i.SavedDate >= filter.FromDateTime)
                && (filter.ToDateTime == null || i.SavedDate <= filter.ToDateTime)
                && (string.IsNullOrEmpty(filter.Job) || i.Job == filter.Job)
                && (string.IsNullOrEmpty(filter.Operation) || i.Operation == filter.Operation)
                && (string.IsNullOrEmpty(filter.Mono) || i.Mono == filter.Mono)
                && (string.IsNullOrEmpty(filter.Component) || i.Component == filter.Component)
                && (string.IsNullOrEmpty(filter.BarCode) || i.BarCode == filter.BarCode)
            ).ToList();
        }

        public A3AppPrelMat PutViewPrelMat(ViewPrelMatPutFilter filter)
        {
            var editPrelMat = _context.A3AppPrelMats.FirstOrDefault(m => m.PrelMatId == filter.PrelMatId);
            
            if (editPrelMat == null)
            {
                return null;
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
                return null;
            }
            _context.A3AppPrelMats.Remove(deletePrelMat);
            _context.SaveChanges();
            return deletePrelMat;
        }

        public IEnumerable<A3AppPrelMat> UpdatePrelMatImported(int? workerId)
        {
            var notImported = _context.A3AppPrelMats
                .Where(x => x.Imported == false)
                .ToList();
            
            if (notImported.Count == 0)
            {
                return notImported;
            }

            foreach (var item in notImported)
            {
                item.Imported = true;
                item.UserImp = workerId.ToString();
                item.DataImp = DateTime.Now;

                _context.A3AppPrelMats.Update(item);
                _context.SaveChanges();
            }

            // Ritorna la lista aggiornata
            return GetAppPrelMat();
        }
    }
}