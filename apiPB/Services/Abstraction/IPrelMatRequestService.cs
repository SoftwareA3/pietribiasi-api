using System.ComponentModel;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface IPrelMatRequestService
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella A3_app_prel_mat
        /// </summary>
        /// <returns>
        /// IEnumerable di PrelMatDto: restituisce una collezione generica di PrelMatDto
        /// </returns>
        IEnumerable<PrelMatDto> GetAppPrelMat();

        /// <summary>
        /// Inserisce la lista di A3AppPrelMat nel database
        /// </summary>
        /// <param name="requestList">Lista di A3AppPrelMat da inserire</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat
        /// </returns>
        IEnumerable<PrelMatDto> PostPrelMatList(IEnumerable<PrelMatRequestDto> requestList);

        /// <summary>
        /// Aggiorna la lista di A3AppPrelMat nel database
        /// </summary>
        /// <param name="request"> Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat:  restituisce una collezione generica di Dto A3AppPrelMatDto
        /// </returns>
        IEnumerable<PrelMatDto> GetViewPrelMatList(ViewPrelMatRequestDto request);

        /// <summary>
        /// Aggiorna la riga della tabella A3_app_prel_mat dato A3AppViewMatPutRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// A3AppPrelMatDto: restituisce l'elemento modificato
        /// </returns>
        PrelMatDto? PutViewPrelMat(ViewPrelMatPutRequestDto request);

        /// <summary>
        /// Elimina la riga della tabella A3_app_prel_mat dato A3AppDeleteRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// A3AppPrelMatDto: restituisce l'elemento eliminato
        /// </returns>
        PrelMatDto? DeletePrelMatId(ViewPrelMatDeleteRequestDto request);

        //IEnumerable<PrelMatDto> UpdatePrelMatImported(int? workerId);

        /// <summary>
        /// Ritorna le informazioni della tabella A3_app_prel_mat filtrate per Component
        /// </summary>
        /// <param name="request">Filtro per l'esecuzione della query. Richiede le proprietà: Component</param>
        IEnumerable<PrelMatDto> GetPrelMatWithComponent(ComponentRequestDto? request);

        /// <summary>
        /// Ritorna tutte le informazioni della tabella A3_app_prel_mat che hanno Imported = false
        /// </summary>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat, ossia i record che hanno Imported = false
        IEnumerable<PrelMatDto> GetNotImportedPrelMat();

        /// <summary>
        /// Aggiorna tutte le informazioni sincronizzate dalla tabella A3_app_prel_mat, impostando Imported a true
        /// </summary>
        /// <param name="request">Filtro per l'esecuzione della query. Richiede le proprietà: WorkerId</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat, cioè i record modificati
        /// </returns>
        IEnumerable<PrelMatDto> UpdatePrelMatImported(WorkerIdSyncRequestDto request);

        IEnumerable<PrelMatDto> GetNotImportedAppPrelMatByFilter(ViewPrelMatRequestDto request);

        IEnumerable<PrelMatDto> UpdateImportedById(UpdateImportedIdRequestDto request);
    }
}