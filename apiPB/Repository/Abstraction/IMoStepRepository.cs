using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IMoStepRepository
    {
        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mosteps
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job</param>
        /// <returns>
        /// IEnumerable di VwApiMostep: restituisce una collezione generica di modelli VwApiMostep
        /// </returns>
        IEnumerable<VwApiMostep> GetMostepWithJob(MostepJobFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mostep filtrate per MostepOdpRequestFilter
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, Mono, CreationDate</param>
        /// <returns>
        /// IEnumerable di VwApiMostep: restituisce una collezione generica di modelli VwApiMostep
        /// </returns>
        IEnumerable<VwApiMostep> GetMostepWithMono(MostepMonoFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mostep filtrate per MostepLavorazioniRequestFilter
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, Mono, CreationDate, Operation</param>
        /// <returns>
        /// IEnumerable di VwApiMostep: restituisce una collezione generica di modelli VwApiMostep
        /// </returns>
        IEnumerable<VwApiMostep> GetMostepWithOperation(MostepOperationFilter filter);
    }
}