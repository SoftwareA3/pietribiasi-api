using apiPB.Filters;
using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IPrelMatRepository
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella A3_app_prel_mat
        /// </summary>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat
        /// </returns>
        IEnumerable<A3AppPrelMat> GetAppPrelMat();

        /// <summary>
        /// Inserisce la lista di A3AppPrelMat nel database
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatList>">Lista di A3AppPrelMat da inserire</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat
        /// </returns>
        IEnumerable<A3AppPrelMat> PostPrelMatList(IEnumerable<PrelMatFilter> filterList);

        /// <summary>
        /// Ritorna la lista di A3AppPrelMat in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: FromDate, ToDate, Job, MoNo, Operation, BarCode</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat
        /// </returns>
        IEnumerable<A3AppPrelMat> GetViewPrelMat(ViewPrelMatRequestFilter filter);

        /// <summary>
        /// Aggiorna la riga della tabella A3_app_prel_mat in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: PrelMatId, PrelQty</param>
        /// <returns>
        /// A3AppPrelMat: restituisce l'elemento modificato
        /// </returns>
        A3AppPrelMat PutViewPrelMat(ViewPrelMatPutFilter filter);

        /// <summary>
        /// Elimina la riga della tabella A3_app_prel_mat in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: PrelMatId</param>
        /// <returns>
        /// A3AppPrelMat: restituisce l'elemento eliminato
        /// </returns>
        A3AppPrelMat DeletePrelMatId(ViewPrelMatDeleteFilter filter);

        /// <summary>
        /// Aggiorna tutte le informazioni sincronizzate dalla tabella A3_app_prel_mat, impostando Imported a true
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: WorkerId</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat, cioè i record modificati
        /// </returns>
        IEnumerable<A3AppPrelMat> UpdatePrelMatImported(WorkerIdSyncFilter filter);

        /// <summary>
        /// Ritorna la lista di A3AppPrelMat in base al filtro passato per recuperare le informazioni sulla quantità salvata
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Component</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat
        /// </returns>
        IEnumerable<A3AppPrelMat>? GetPrelMatWithComponent(ComponentFilter? filter);

        /// <summary>   
        /// Ritorna la lista di A3AppPrelMat non importati
        /// </summary>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat
        /// </returns>
        IEnumerable<A3AppPrelMat> GetNotImportedPrelMat();

        IEnumerable<A3AppPrelMat> GetNotImportedAppPrelMatByFilter(ViewPrelMatRequestFilter filter);
        
        IEnumerable<A3AppPrelMat> UpdateImportedById(UpdateImportedIdFilter filter);
    }
}