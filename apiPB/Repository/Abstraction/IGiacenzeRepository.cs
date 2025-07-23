using apiPB.Filters;
using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IGiacenzeRepository
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella vw_api_giacenze
        /// </summary>
        /// <returns>IEnumerable<VwApiGiacenze> Ritorna una collezione generica di Modelli ApiGiacenze</returns>
        /// <remarks>
        IEnumerable<VwApiGiacenze> GetGiacenze();

        /// <summary>
        /// Restituisce le informazioni dalla tabella vw_api_giacenze in base al filtro passato
        /// </summary>
        /// <param name="request">Componente da usare come filtro per la richiesta</param>
        /// <returns></returns>
        VwApiGiacenze GetGiacenzaByItem(ComponentFilter request);
    }
}