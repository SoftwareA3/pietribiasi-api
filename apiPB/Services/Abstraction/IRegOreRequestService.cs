using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Services.Abstraction
{
    public interface IRegOreRequestService
    {
        /// <summary>
        /// Ritorna tutte le informazioni salvate nella tabella A3_app_reg_ore
        /// </summary>
        /// <returns>Enumerable di A3AppRegOreDto: restituisce una collezione generica di Dto A3AppRegOreDto</returns>
        IEnumerable<RegOreDto> GetAppRegOre();

        /// <summary>
        /// Ritorna tutte le informazioni salvate nella tabella A3_app_reg_ore, dato A3AppRegOreRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Enumerable di A3AppRegOreDto: restituisce una collezione generica di Dto A3AppRegOreDto</returns>
        IEnumerable<RegOreDto> PostAppRegOre(IEnumerable<RegOreRequestDto> requestList);

        /// <summary>
        /// Ritorna tutte le informazioni salvate nella tabella A3_app_reg_ore, dato A3AppViewOreRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Enumerable di A3AppRegOreDto: restituisce una collezione generica di Dto A3AppViewOreDto</returns>
        IEnumerable<RegOreDto> GetAppViewOre(ViewOreRequestDto request);

        /// <summary>
        /// Ritorna il record modificato della tabella A3_app_reg_ore, dato A3AppViewOrePutRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>A3AppRegOreDto: restituisce il record modificato della tabella A3_app_reg_ore</returns>
        RegOreDto? PutAppViewOre(ViewOrePutRequestDto request);

        /// <summary>
        /// Elimina il record della tabella A3_app_reg_ore, dato A3AppDeleteRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>Ritorna il Dto dell'elemento eliminato</returns>
        RegOreDto? DeleteRegOreId(ViewOreDeleteRequestDto request);
    }
}