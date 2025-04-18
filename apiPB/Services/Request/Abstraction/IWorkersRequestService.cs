using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Services.Request.Abstraction
{
    public interface IWorkersRequestService
    {
        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_workers
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di WorkersFieldDto: restituisce una collezione generica di Dto WorkersFieldDto
        /// </returns>
        IEnumerable<WorkersFieldDto> GetWorkersFieldsById(WorkersFieldRequestDto request);

        /// <summary>
        /// Ritorna l'ultima linea di RmWorkersField, dato il WorkerId
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// WorkersFieldDto: restituisce l'ultima linea di RmWorkersField
        /// </returns>
        WorkersFieldDto? GetLastWorkerFieldLine(WorkersRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_workers
        /// </summary>
        /// <returns>
        /// IEnumerable di WorkerDto: restituisce una collezione generica di Dto WorkerDto
        /// </returns>
        IEnumerable<WorkerDto> GetWorkers();

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_workers, dato PasswordWorkersRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// WorkerDto: restituisce un Dto WorkerDto con le informazioni del lavoratore
        /// </returns>
        WorkerDto? GetWorkerByPassword(WorkersRequestDto request);

        /// <summary>
        /// Invoca la store procedure dbo.InsertWorkersFields passando il Dto di richiesta
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// Task: restituisce un task che rappresenta l'operazione asincrona. Aggiorna il campo LastLogin della tabella RmWorkersFields o inserisce un nuovo record se non esiste
        /// </returns>
        Task CallStoredProcedure(WorkersFieldRequestDto request);

        /// <summary>
        /// Aggiorna il campo LastLogin della tabella RmWorkersFields o inserisce un nuovo record se non esiste
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// WorkersFieldDto: restituisce un Dto WorkersFieldDto con le informazioni aggiornate
        /// </returns>
        Task<WorkersFieldDto?> UpdateOrCreateLastLogin(WorkersRequestDto request);
        
        /// <summary>
        /// Data la password del lavoratore, recupera il WorkerId rispettivo. 
        /// Autenticazione tramite WorkerId e Password, cercando corrispondenza nel database.
        /// Aggiornamento del campo LastLogin della tabella RmWorkersField.
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// WorkerIdAndPasswordRequestDto: restituisce un Dto WorkerIdAndPasswordRequestDto con le informazioni del lavoratore loggato
        /// </returns>
        WorkerDto? LoginWithPassword(WorkersRequestDto request);

        /// <summary>
        /// Ritorna il lavoratore in base all'ID e alla password forniti.
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>WorkerDto: restituisce un WorkerDto con tutte le informazioni sul lavoratore loggato</returns>
        WorkerDto? GetWorkerByIdAndPassword(WorkersRequestDto request);

    }
}