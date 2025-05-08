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
            return _context.A3AppInventarios.ToList();
        }

        public IEnumerable<A3AppInventario> PostInventarioList(IEnumerable<InventarioFilter> filterList)
        {
            var inventarioList = new List<A3AppInventario>();
            
            // Lista di elementi che sono stati modificati
            var editedList = new List<A3AppInventario>();

            foreach (var filter in filterList)
            {
                // Verifica che non esiste già un elemento con lo stesso Item e/o BarCode
                var existingItem = _context.A3AppInventarios.FirstOrDefault(m => 
                    m.Item == filter.Item && 
                    (string.IsNullOrEmpty(filter.BarCode) || m.BarCode == filter.BarCode));
                if (existingItem != null)
                {
                    existingItem.WorkerId = filter.WorkerId;
                    existingItem.SavedDate = DateTime.Now;
                    existingItem.BookInv = filter.BookInv; 
                    _context.A3AppInventarios.Update(existingItem);
                    editedList.Add(existingItem);
                    continue;
                }
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

            if (inventarioList.Count > 0)
            {
                _context.A3AppInventarios.AddRange(inventarioList);
            }
            _context.SaveChanges();

            if(editedList.Count > 0)
            {
                // Se sono stati modificati elementi, li aggiunge alla lista finale
                inventarioList.AddRange(editedList);
            }

            return inventarioList;
        }

        public IEnumerable<A3AppInventario> GetViewInventario(ViewInventarioRequestFilter filter)
        {
            return _context.A3AppInventarios
            .Where(i => (filter.WorkerId == null || i.WorkerId == filter.WorkerId)
                        && (filter.FromDateTime == null || i.SavedDate >= filter.FromDateTime)
                        && (filter.ToDateTime == null || i.SavedDate <= filter.ToDateTime)
                        && (string.IsNullOrEmpty(filter.Item) || i.Item == filter.Item)
                        && (string.IsNullOrEmpty(filter.BarCode) || i.BarCode == filter.BarCode))
            .ToList();
        }

        public A3AppInventario PutViewInventario(ViewInventarioPutFilter filter)
        {
            var inventario = _context.A3AppInventarios.FirstOrDefault(m => m.InvId == filter.InvId);

            if (inventario == null)
            {
                return null;
            }

            inventario.SavedDate = DateTime.Now;
            inventario.BookInv = filter.BookInv;
            _context.SaveChanges();

            return inventario;
        }
    }

    
}