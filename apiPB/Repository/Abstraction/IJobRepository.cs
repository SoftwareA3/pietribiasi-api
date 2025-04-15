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
    }
}