using apiPB.Repository.Abstraction;
using apiPB.Models;
using apiPB.Filters;
using apiPB.Data;
using System.Security.Cryptography.X509Certificates;

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
                // Effettua anche un controllo solo sugli elementi non importati
                var existingItem = _context.A3AppInventarios.Where(m =>
                    m.Item == filter.Item &&
                    (string.IsNullOrEmpty(filter.BarCode) || m.BarCode == filter.BarCode) &&
                    m.Imported == false).FirstOrDefault();
                if (existingItem != null)
                {
                    Console.WriteLine("Match in ExistingItem");
                    existingItem.WorkerId = filter.WorkerId;
                    existingItem.SavedDate = DateTime.Now;
                    existingItem.BookInv = filter.BookInv;
                    existingItem.PrevBookInv = filter.PrevBookInv;
                    existingItem.UoM = filter.UoM;
                    CalculateBookInvDiff(existingItem);
                    
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
                    PrevBookInv = filter.PrevBookInv,
                    UoM = filter.UoM,
                };
                CalculateBookInvDiff(inventario);

                inventarioList.Add(inventario);
            }

            if (inventarioList.Count > 0)
            {
                // Rimuove eventuali duplicati per Item e BarCode nella lista prima di aggiungerli al database
                inventarioList = inventarioList
                    .GroupBy(x => new { x.Item, BarCode = string.IsNullOrEmpty(x.BarCode) ? null : x.BarCode })
                    .Select(g => g.Key.BarCode == null
                        ? g.Last() 
                        : g.Last())
                    .ToList();
                _context.A3AppInventarios.AddRange(inventarioList);
            }
            _context.SaveChanges();

            if (editedList.Count > 0)
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
                        && (filter.DataImp == null || i.DataImp >= filter.DataImp.Value.Date)
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
            CalculateBookInvDiff(inventario);
            _context.SaveChanges();

            return inventario;
        }

        private void CalculateBookInvDiff(A3AppInventario inventario)
        {
            if (inventario.BookInv != null && inventario.BookInv - inventario.PrevBookInv > 0)
            {
                inventario.InvRsn = true;
            }
            else
            {
                inventario.InvRsn = false;
            }
            if (inventario.BookInv.HasValue && inventario.PrevBookInv.HasValue)
            {
                inventario.BookInvDiff = Math.Abs(inventario.BookInv.Value - inventario.PrevBookInv.Value);
            }
            else
            {
                inventario.BookInvDiff = null;
            };
        }

        public IEnumerable<A3AppInventario> GetNotImportedInventario()
        {
            return _context.A3AppInventarios
                .Where(x => x.Imported == false)
                .ToList();
        }

        public IEnumerable<A3AppInventario> UpdateInventarioImported(WorkerIdSyncFilter? filter)
        {
            var notImported = _context.A3AppInventarios
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

                var dbItem = _context.A3AppInventarios.FirstOrDefault(x => x.InvId == item.InvId);
                if (dbItem != null)
                {
                    dbItem.Imported = item.Imported;
                    dbItem.UserImp = item.UserImp;
                    dbItem.DataImp = item.DataImp;

                    _context.A3AppInventarios.Update(dbItem);
                    _context.SaveChanges();
                }
            }
            // Ritorna la lista aggiornata
            return GetInventario();
        }
    }
}