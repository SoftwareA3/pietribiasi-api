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
        /// Restituisce tutte le informazioni della vista vw_api_mocomponents
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job</param>
        /// <returns> 
        /// IEnumerable di VwApiMocomponent: restituisce una collezione generica di modelli VwApiMocomponent
        /// </returns>
        IEnumerable<VwApiMocomponent> GetMocomponent(MocomponentRequestFilter filter);

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
        /// IEnumerable di VwApiMoStepsComponent: restituisce una collezione generica di modelli VwApiMoStepsComponent
        /// </returns>
        IEnumerable<VwApiMoStepsComponent> GetMoStepsComponent(MoStepsComponentRequestFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mo
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, RtgStep, Alternate, AltRtgStep</param>
        /// <returns>
        /// IEnumerable di VwApiMo: restituisce una collezione generica di modelli VwApiMo
        /// </returns>
        IEnumerable<VwApiMo> GetMo(MoRequestFilter filter);    

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mo_steps_component, dato MoStepsComponentRequestFilter
        /// Viene applicato il filtro Distinct per evitare duplicati
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<VwApiMoStepsComponent> GetMoStepsComponentDistinct(MoStepsComponentRequestFilter filter);
    }
}