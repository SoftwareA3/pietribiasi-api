using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Repository.Abstraction;
using apiPB.Models;
using apiPB.Filters;
using apiPB.Data;

namespace apiPB.Repository.Implementation
{
    public class InventarioRepository : IInventarioRepository
    {
        private readonly ApplicationDbContext _context;

        public InventarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<A3AppInventario> GetInventario()
        {
            return _context.A3AppInventarios.AsQueryable().ToList();
        }

        public IEnumerable<A3AppInventario> PostInventarioList(IEnumerable<InventarioFilter> filterList)
        {
            var inventarioList = new List<A3AppInventario>();

            foreach (var filter in filterList)
            {
                var inventario = new A3AppInventario
                {
                    WorkerId = filter.WorkerId,
                    SavedDate = DateTime.Now,
                    Item = filter.Item,
                    Description = filter.Description,
                    BarCode = filter.BarCode,
                    FiscalYear = filter.FiscalYear,
                    Storage = filter.Storage,
                    BookInv = filter.BookInv,
                };

                inventarioList.Add(inventario);
            }

            _context.A3AppInventarios.AddRange(inventarioList);
            _context.SaveChanges();

            return inventarioList;
        }
    }

    
}