using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Services.Request.Abstraction
{
    public interface IMostepsMocomponentRequestService
    {
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentJobRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>IEnumerable di MostepsMocomponentDto: restituisce una collezione generica di Dto MostepsMocomponentDto</returns>
        IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentJobDistinct(JobRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentMonoRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>IEnumerable di MostepsMocomponentDto: restituisce una collezione generica di Dto MostepsMocomponentDto</returns>
        IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentMonoDistinct(MonoRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentOperationRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>IEnumerable di MostepsMocomponentDto: restituisce una collezione generica di Dto MostepsMocomponentDto</returns>
        IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentOperationDistinct(OperationRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentBarCodeRequestDto
        /// /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>IEnumerable di MostepsMocomponentDto: restituisce una collezione generica di Dto MostepsMocomponentDto</returns>
        IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentBarCodeDistinct(BarCodeRequestDto request);
    }
}