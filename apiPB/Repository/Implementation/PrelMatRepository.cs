using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
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
            var query = _context.A3AppPrelMats.AsQueryable();

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

            if(filter.Component != null)
            {
                query = query.Where(m => m.Component == filter.Component);
            }

            if (filter.BarCode != null)
            {
                query = query.Where(m => m.BarCode == filter.BarCode);
            }

            if(filter.WorkerId != null)
            {
                query = query.Where(m => m.WorkerId == filter.WorkerId);
            }

            var list = query.ToList();
            return list;
        }

        public A3AppPrelMat PutViewPrelMat(ViewPrelMatPutFilter filter)
        {
            var editPrelMat = _context.A3AppPrelMats.FirstOrDefault(m => m.PrelMatId == filter.PrelMatId);
            
            if (editPrelMat != null)
            {
                editPrelMat.PrelQty = filter.PrelQty;
                _context.SaveChanges();
            }
            return editPrelMat ?? throw new InvalidOperationException("Record not found.");
        }
        public A3AppPrelMat DeletePrelMatId(ViewPrelMatDeleteFilter filter)
        {
            var deletePrelMat = _context.A3AppPrelMats.Find(filter.PrelMatId);
            if (deletePrelMat != null)
            {
                _context.A3AppPrelMats.Remove(deletePrelMat);
                _context.SaveChanges();
            }
            return deletePrelMat ?? throw new InvalidOperationException("Record not found.");
        }
    }
}