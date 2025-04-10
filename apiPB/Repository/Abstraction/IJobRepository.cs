using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IJobRepository
    {
        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_jobs
        /// </summary>
        /// <returns>
        /// IEnumerable di VwApiJob: restituisce una collezione generica di modelli VwApiJob
        /// </returns>
        IEnumerable<VwApiJob> GetJobs();

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mosteps
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job</param>
        /// <returns>
        /// IEnumerable di VwApiMostep: restituisce una collezione generica di modelli VwApiMostep
        /// </returns>
        IEnumerable<VwApiMostep> GetMostep(MostepRequestFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mo_steps_component
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, RtgStep, Alternate, AltRtgStep.Il filtro contiene parametri opzionali: Position, Component</param>
        /// <returns>
        /// IEnumerable di VwApiMostepsMocomponent: restituisce una collezione generica di modelli VwApiMostepsMocomponent
        /// </returns>
        IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponent(MostepsMocomponentRequestFilter filter); 

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentRequestFilter
        /// Viene applicato il filtro Distinct per evitare duplicati
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentDistinct(MostepsMocomponentRequestFilter filter);

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