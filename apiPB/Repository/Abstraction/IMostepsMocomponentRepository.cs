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
    }
}