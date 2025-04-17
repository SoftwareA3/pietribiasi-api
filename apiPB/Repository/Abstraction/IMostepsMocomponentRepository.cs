using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IMostepsMocomponentRepository
    {
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
        /// Restituisce tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentJobFilter
        /// Viene applicato il filtro Distinct per evitare duplicati    
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job</param>
        /// <returns>
        /// IEnumerable di VwApiMostepsMocomponent: restituisce una collezione generica di modelli VwApiMostepsMocomponent
        /// </returns>
        IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentJobDistinct(MostepsMocomponentJobFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentMonoFilter
        /// Viene applicato il filtro Distinct per evitare duplicati
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, Mono, CreationDate</param>
        /// <returns>
        /// IEnumerable di VwApiMostepsMocomponent: restituisce una collezione generica di modelli VwApiMostepsMocomponent
        /// </returns>
        IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentMonoDistinct(MostepsMocomponentMonoFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentOperationFilter
        /// Viene applicato il filtro Distinct per evitare duplicati
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, Mono, CreationDate, Operation</param>
        /// <returns>
        /// IEnumerable di VwApiMostepsMocomponent: restituisce una collezione generica di modelli VwApiMostepsMocomponent
        /// </returns>
        IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentOperationDistinct(MostepsMocomponentOperationFilter filter);

        /// <summary>
        /// Restituisce tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentBarCodeFilter
        /// Viene applicato il filtro Distinct per evitare duplicati
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: Job, Mono, CreationDate, Operation, BarCode</param>
        /// <returns>
        /// IEnumerable di VwApiMostepsMocomponent: restituisce una collezione generica di modelli VwApiMostepsMocomponent
        /// </returns>
        IEnumerable<VwApiMostepsMocomponent> GetMostepsMocomponentBarCodeDistinct(MostepsMocomponentBarCodeFilter filter);
    }
}