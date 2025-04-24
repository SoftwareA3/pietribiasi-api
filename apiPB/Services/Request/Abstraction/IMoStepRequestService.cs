using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Services.Request.Abstraction
{
    public interface IMoStepRequestService
    {
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mosteps, dato MostepRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di MostepDto: restituisce una collezione generica di Dto MostepDto
        /// </returns>
        IEnumerable<MostepDto> GetMostepWithJob(JobRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepOdpRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Enumerable di MostepDto: restituisce una collezione generica di Dto MostepDto</returns>
        IEnumerable<MostepDto> GetMostepWithMono(MonoRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepLavorazioniRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Enumerable di MostepDto: restituisce una collezione generica di Dto MostepDto</returns>
        IEnumerable<MostepDto> GetMostepWithOperation(OperationRequestDto request);
    }
}