using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IInventarioRepository
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella vw_api_inventario
        /// </summary>
        /// <returns>IEnumerable<VwApiInventario> Ritorna una collezione generica di Modelli ApiInventario</returns>
        IEnumerable<A3AppInventario> GetInventario();

        /// <summary>
        /// Inserisce la lista di A3AppInventario nel database
        /// </summary>
        /// <param name="IEnumerable<a3AppInventarioList>">Lista di A3AppInventario da inserire</param>
        /// <returns>IEnumerable<A3AppInventario> Ritorna una collezione generica di Modelli ApiInventario</returns>
        IEnumerable<A3AppInventario> PostInventarioList(IEnumerable<InventarioFilter> filterList);

        /// <summary>
        /// Ritorna la lista di A3AppInventario in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: FronDate, ToDate, Item, BarCode</param>
        /// <returns>IEnumerable<A3AppInventario> Ritorna una collezione generica di Modelli ApiInventario</returns>
        IEnumerable<A3AppInventario> GetViewInventario(ViewInventarioRequestFilter filter);

        /// <summary>
        /// Aggiorna il record di A3AppInventario in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: InvId, BookInv</param>
        /// <returns>A3AppInventario Ritorna l'elemento modificato</returns>
        A3AppInventario PutViewInventario(ViewInventarioPutFilter filter);
    }
}