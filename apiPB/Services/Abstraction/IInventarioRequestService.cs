using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface IInventarioRequestService
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella vw_api_inventario
        /// </summary>
        /// <returns>
        /// IEnumerable di InventarioDto: restituisce una collezione generica di InventarioDto
        /// </returns>
        IEnumerable<InventarioDto> GetInventario();

        /// <summary>
        /// Inserisce la lista di InventarioDto nel database
        /// </summary>
        /// <param name="inventarioList">Lista di InventarioDto da inserire</param>
        /// <returns>
        /// IEnumerable di InventarioDto: restituisce una collezione generica di InventarioDto
        /// </returns>
        IEnumerable<InventarioDto> PostInventarioList(IEnumerable<InventarioRequestDto> inventarioList);

        /// <summary>
        /// Ritorna la lista di InventarioDto in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: FromDate, ToDate, Item, BarCode</param>
        /// <returns>
        /// IEnumerable di InventarioDto: restituisce una collezione generica di InventarioDto
        /// </returns>
        IEnumerable<InventarioDto> GetViewInventario(ViewInventarioRequestDto request);

        /// <summary>
        /// Aggiorna il record di InventarioDto in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: InvId, BookInv</param>
        /// <returns>
        /// InventarioDto: restituisce l'elemento modificato
        /// </returns>
        InventarioDto PutViewInventario(ViewInventarioPutRequestDto request);

        // IEnumerable<PrelMatDto> GetNotImportedPrelMat();

        // /// <summary>
        // /// Aggiorna tutte le informazioni sincronizzate dalla tabella A3_app_prel_mat, impostando Imported a true
        // /// </summary>
        // /// <param name="request">Filtro per l'esecuzione della query. Richiede le proprietà: WorkerId</param>
        // /// <returns>
        // /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat, cioè i record modificati
        // /// </returns>
        // IEnumerable<PrelMatDto> UpdatePrelMatImported(WorkerIdSyncRequestDto request);

        IEnumerable<InventarioDto> GetNotImportedInventario();

        IEnumerable<InventarioDto> UpdateInventarioImported(WorkerIdSyncRequestDto? request);

        IEnumerable<InventarioDto> GetNotImportedAppInventarioByFilter(ViewInventarioRequestDto request);

        InventarioDto UpdateImportedById(UpdateImportedIdRequestDto request);
    }
}