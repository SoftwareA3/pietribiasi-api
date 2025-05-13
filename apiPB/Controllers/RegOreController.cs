using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/reg_ore")]
    [Authorize]
    [ApiController]
    public class RegOreController : Controller
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IRegOreRequestService _regOreRequestService;
        private readonly bool _isLogActive;
        public RegOreController(IResponseHandler responseHandler, IRegOreRequestService regOreRequestService)
        {
            _responseHandler = responseHandler;
            _regOreRequestService = regOreRequestService;
            _isLogActive = false;
        }

        [HttpGet("get_all")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_reg_ore
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetAllRegOre()
        {   
            var a3AppRegOreDto = _regOreRequestService.GetAppRegOre().ToList();

            if (a3AppRegOreDto == null || !a3AppRegOreDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, a3AppRegOreDto, _isLogActive);
        }

        [HttpPost("post_reg_ore")]
        /// <summary>
        /// Invia la lista di A3AppRegOre al database
        /// </summary>
        /// <param name="IEnumerable<a3AppRegOreRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="201">Crea delle entry nel database</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PostRegOreList([FromBody] IEnumerable<RegOreRequestDto>? a3AppRegOreRequestDto)
        {
            if (a3AppRegOreRequestDto == null || !a3AppRegOreRequestDto.Any()) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var a3AppRegOreDto = _regOreRequestService.PostAppRegOre(a3AppRegOreRequestDto).ToList();

            if (a3AppRegOreDto == null || !a3AppRegOreDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleCreated(HttpContext, a3AppRegOreDto, _isLogActive);
        }

        [HttpPost("view_ore")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_reg_ore filtrate in base ai parametri del Dto di richiesta
        /// </summary>
        /// <param name="A3AppViewOreRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_reg_ore filtrate</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetA3AppRegOre([FromBody] ViewOreRequestDto? a3AppViewOreRequestDto)
        {
            if(a3AppViewOreRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var a3AppRegOreDto = _regOreRequestService.GetAppViewOre(a3AppViewOreRequestDto).ToList();

            if (a3AppRegOreDto == null || !a3AppRegOreDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, a3AppRegOreDto, _isLogActive);
        }

        [HttpPut("view_ore/edit_working_time")]
        /// <summary>
        /// Aggiorna il tempo di lavoro della riga della tabella A3_app_reg_ore in base al filtro passato
        /// </summary>
        /// <param name="A3AppViewOrePutRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna il record modificato della tabella A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PutA3AppRegOre([FromBody] ViewOrePutRequestDto? a3AppViewOrePutRequestDto)
        {
            if(a3AppViewOrePutRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var a3AppRegOreDto = _regOreRequestService.PutAppViewOre(a3AppViewOrePutRequestDto);

            if (a3AppRegOreDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndItem(HttpContext, a3AppRegOreDto, _isLogActive);
        }

        [HttpDelete("view_ore/delete_reg_ore_id")]
        /// <summary>
        /// Elimina la riga della tabella A3_app_reg_ore in base al filtro passato
        /// </summary>
        /// <param name="A3AppDeleteRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna il record eliminato della tabella A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult DeleteRegOreId([FromBody] ViewOreDeleteRequestDto? a3AppDeleteRequestDto)
        {
            if(a3AppDeleteRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var a3AppRegOreDto = _regOreRequestService.DeleteRegOreId(a3AppDeleteRequestDto);

            if (a3AppRegOreDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndItem(HttpContext, a3AppRegOreDto, _isLogActive);
        }
    }
}