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
        /// Ritorna tutte le informazioni della vista vw_api_mosteps, dato MostepRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di MostepDto: restituisce una collezione generica di Dto MostepDto
        /// </returns>
        IEnumerable<MostepDto> GetMostepByMoId(MostepRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di MostepsMocomponentDto: restituisce una collezione generica di Dto MostepsMocomponentDto
        /// </returns>
        IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentByMoId(MostepsMocomponentRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>IEnumerable di MostepsMocomponentDto: restituisce una collezione generica di Dto MostepsMocomponentDto</returns>
        IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentForRegOre(MostepRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepsMocomponentRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>IEnumerable di MostepsMocomponentDto: restituisce una collezione generica di Dto MostepsMocomponentDto</returns>
        IEnumerable<MostepsMocomponentDto> GetMostepsMocomponentDistinct(MostepsMocomponentRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepOdpRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Enumerable di MostepDto: restituisce una collezione generica di Dto MostepDto</returns>
        IEnumerable<MostepDto> GetMostepWithOdp(MostepOdpRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_component, dato MostepLavorazioniRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Enumerable di MostepDto: restituisce una collezione generica di Dto MostepDto</returns>
        IEnumerable<MostepDto> GetMostepWithLavorazione(MostepLavorazioniRequestDto request);

        /// <summary>
        /// Ritorna tutte le informazioni salvate nella tabella A3_app_reg_ore
        /// </summary>
        /// <returns>Enumerable di A3AppRegOreDto: restituisce una collezione generica di Dto A3AppRegOreDto</returns>
        IEnumerable<A3AppRegOreDto> GetAppRegOre();

        /// <summary>
        /// Ritorna tutte le informazioni salvate nella tabella A3_app_reg_ore, dato A3AppRegOreRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Enumerable di A3AppRegOreDto: restituisce una collezione generica di Dto A3AppRegOreDto</returns>
        IEnumerable<A3AppRegOreDto> PostAppRegOre(IEnumerable<A3AppRegOreRequestDto> requestList);

        /// <summary>
        /// Ritorna tutte le informazioni salvate nella tabella A3_app_reg_ore, dato A3AppViewOreRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Enumerable di A3AppRegOreDto: restituisce una collezione generica di Dto A3AppViewOreDto</returns>
        IEnumerable<A3AppRegOreDto> GetAppViewOre(A3AppViewOreRequestDto request);

        /// <summary>
        /// Ritorna il record modificato della tabella A3_app_reg_ore, dato A3AppViewOrePutRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>A3AppRegOreDto: restituisce il record modificato della tabella A3_app_reg_ore</returns>
        A3AppRegOreDto? PutAppViewOre(A3AppViewOrePutRequestDto request);

        /// <summary>
        /// Elimina il record della tabella A3_app_reg_ore, dato A3AppDeleteRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Ritorna il Dto dell'elemento eliminato</returns>
        A3AppRegOreDto DeleteRegOreId(A3AppDeleteRequestDto request);
    }
}