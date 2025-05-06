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
    }
}