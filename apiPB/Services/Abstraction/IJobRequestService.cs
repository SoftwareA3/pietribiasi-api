using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Services.Abstraction
{
    public interface IJobRequestService
    {
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_jobs
        /// </summary>
        /// <returns>
        /// IEnumerable di JobDto: restituisce una collezione generica di Dto JobDto
        /// </returns>
        IEnumerable<JobDto> GetJobs();
    }
}