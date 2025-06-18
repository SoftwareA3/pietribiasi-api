using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IWorkerRepository
    {
        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_workers
        /// </summary>
        /// <returns>
        /// IEnumerable di VwApiWorker: restituisce una collezione generica di modelli VwApiWorker
        /// </returns>
        IEnumerable<VwApiWorker> GetWorkers();

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_workers 
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Password</param>
        /// <returns>
        /// IEnumerable di VwApiWorker: restituisce un oggetto del modello VwApiWorker
        /// </returns>
        VwApiWorker? GetWorkerByPassword(PasswordWorkersRequestFilter filter);

        /// <summary>
        /// Invoca la stored procedure dbo.InsertWorkersFields passando workerId e la dataora corrente
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: WorkerId, FieldValue</param>
        /// <returns></returns>
        Task CallStoredProcedure(WorkerIdAndValueRequestFilter filter);

        /// <summary>
        /// Recupera il lavoratore tramite la password. Tramite il lavoratore recupera il workerId e lo passa alla stored procedure dbo.InsertWorkersFields
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Password</param>
        /// <returns>
        /// Task: restituisce un task che rappresenta l'operazione asincrona. Aggiorna il campo LastLogin della tabella RmWorkersFields o inserisce un nuovo record se non esiste
        /// </returns>
        Task CreateOrUpdateLastLogin (PasswordWorkersRequestFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della tabella RmWorkersField, filtrate per WorkerId
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: WorkerId</param>
        /// <returns>
        /// IEnumerable di RmWorkersField: restituisce una collezione generica di modelli RmWorkersField
        /// </returns>
        IEnumerable<VwApiWorkersfield> GetWorkersFieldsById(WorkerIdAndValueRequestFilter filter);

        /// <summary>
        /// Esegue la query per ottenere il record con linea massima, dato il WorkerId
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: WorkerId</param>
        /// <returns>
        /// RmWorkersField: restituisce il record con linea massima della tabella RmWorkersField
        /// </returns>
        VwApiWorkersfield? GetLastWorkerFieldLine(WorkerIdAndValueRequestFilter filter);

        /// <summary>
        /// /// Esegue una query su id e password per trovare un lavoratore
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: WorkerId, Password</param>
        /// <returns>
        /// VwApiWorker: restituisce un oggetto del modello VwApiWorker
        /// </returns>
        VwApiWorker GetWorkerByIdAndPassword(WorkerIdAndPasswordFilter filter);
    }
}