using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Services.Request.Abstraction
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

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mocomponents, dato MocomponentRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di MocomponentDto: restituisce una collezione generica di Dto MocomponentDto
        /// </returns>
        IEnumerable<MocomponentDto> GetMocomponent(MocomponentRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mosteps, dato MostepRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di MostepDto: restituisce una collezione generica di Dto MostepDto
        /// </returns>
        IEnumerable<MoDto> GetMo(MoRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mosteps, dato MostepRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di MostepDto: restituisce una collezione generica di Dto MostepDto
        /// </returns>
        IEnumerable<MostepDto> GetMostepByMoId(MostepRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MoStepsComponentRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di MoStepsComponentDto: restituisce una collezione generica di Dto MoStepsComponentDto
        /// </returns>
        IEnumerable<MoStepsComponentDto> GetMoStepsComponentByMoId(MoStepsComponentRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>IEnumerable di MoStepsComponentDto: restituisce una collezione generica di Dto MoStepsComponentDto</returns>
        IEnumerable<MoStepsComponentDto> GetMoStepsComponentForRegOre(MostepRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MoStepsComponentRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>IEnumerable di MoStepsComponentDto: restituisce una collezione generica di Dto MoStepsComponentDto</returns>
        IEnumerable<MoStepsComponentDto> GetMoStepsComponentDistinct(MoStepsComponentRequestDto request);
    }
}