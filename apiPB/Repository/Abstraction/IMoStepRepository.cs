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
        IEnumerable<VwApiMostep> GetMostep(MostepRequestFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mostep filtrate per MostepOdpRequestFilter
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, Mono, CreationDate</param>
        /// <returns>
        /// IEnumerable di VwApiMostep: restituisce una collezione generica di modelli VwApiMostep
        /// </returns>
        IEnumerable<VwApiMostep> GetMostepWithOdp(MostepOdpRequestFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mostep filtrate per MostepLavorazioniRequestFilter
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, Mono, CreationDate, Operation</param>
        /// <returns>
        /// IEnumerable di VwApiMostep: restituisce una collezione generica di modelli VwApiMostep
        /// </returns>
        IEnumerable<VwApiMostep> GetMostepWithLavorazione(MostepLavorazioniRequestFilter filter);
    }
}