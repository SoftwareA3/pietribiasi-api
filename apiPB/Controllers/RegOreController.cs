using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/reg_ore")]
    [Authorize]
    [ApiController]
    public class RegOreController : Controller
    {
        private readonly LogService _logService;
        private readonly IRegOreRequestService _regOreRequestService;
        public RegOreController(LogService logService, IRegOreRequestService regOreRequestService)
        {
            _logService = logService;
            _regOreRequestService = regOreRequestService;
        }

        [HttpGet("get_all")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_reg_ore
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult getAllRegOre()
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";
                
            var a3AppRegOreDto = _regOreRequestService.GetAppRegOre().ToList();

            if (a3AppRegOreDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", a3AppRegOreDto);

            return Ok(a3AppRegOreDto);
        }

        [HttpPost("post_reg_ore")]
        /// <summary>
        /// Invia la lista di A3AppRegOre al database
        /// </summary>
        /// <param name="IEnumerable<a3AppRegOreRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="201">Crea delle entry nel database</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PostRegOreList([FromBody] IEnumerable<RegOreRequestDto> a3AppRegOreRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppRegOreDto = _regOreRequestService.PostAppRegOre(a3AppRegOreRequestDto).ToList();

            if (a3AppRegOreDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            var createdResult = CreatedAtAction(nameof(PostRegOreList), a3AppRegOreDto);
            _logService.AppendMessageAndListToLog(requestPath, createdResult.StatusCode, "Created", a3AppRegOreDto);

            return CreatedAtAction(nameof(PostRegOreList), a3AppRegOreDto);
        }

        [HttpPost("view_ore")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_reg_ore filtrate in base ai parametri del Dto di richiesta
        /// </summary>
        /// <param name="A3AppViewOreRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_reg_ore filtrate</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetA3AppRegOre([FromBody] ViewOreRequestDto a3AppViewOreRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppRegOreDto = _regOreRequestService.GetAppViewOre(a3AppViewOreRequestDto).ToList();

            if (a3AppRegOreDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", a3AppRegOreDto);

            return Ok(a3AppRegOreDto);
        }

        [HttpPut("view_ore/edit_working_time")]
        /// <summary>
        /// Aggiorna il tempo di lavoro della riga della tabella A3_app_reg_ore in base al filtro passato
        /// </summary>
        /// <param name="A3AppViewOrePutRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna il record modificato della tabella A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PutA3AppRegOre([FromBody] ViewOrePutRequestDto a3AppViewOrePutRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppRegOreDto = _regOreRequestService.PutAppViewOre(a3AppViewOrePutRequestDto);

            if (a3AppRegOreDto == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndItemToLog(requestPath, Ok().StatusCode, "OK", a3AppRegOreDto);

            return Ok(a3AppRegOreDto);
        }

        [HttpDelete("view_ore/delete_reg_ore_id")]
        /// <summary>
        /// Elimina la riga della tabella A3_app_reg_ore in base al filtro passato
        /// </summary>
        /// <param name="A3AppDeleteRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna il record eliminato della tabella A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult DeleteRegOreId([FromBody] ViewOreDeleteRequestDto a3AppDeleteRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppRegOreDto = _regOreRequestService.DeleteRegOreId(a3AppDeleteRequestDto);

            if (a3AppRegOreDto == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndItemToLog(requestPath, Ok().StatusCode, "Deleted", a3AppRegOreDto);

            return Ok(a3AppRegOreDto);
        }
    }
}