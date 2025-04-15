using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/mostepsmocomponent")]
    [Authorize]
    [ApiController]
    public class MostepsMocomponentController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IMostepsMocomponentRequestService _mostepsMocomponentRequestService;

        public MostepsMocomponentController(LogService logService, IMostepsMocomponentRequestService mostepsMocomponentRequestService)
        {
            _logService = logService;
            _mostepsMocomponentRequestService = mostepsMocomponentRequestService;
        }

        [HttpPost("post_mostepmocomponent")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components
        /// </summary>
        /// <param name="MostepsMocomponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMostepsMocomponent([FromBody] MostepsMocomponentRequestDto mostepsMocomponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentByMoId(mostepsMocomponentRequestDto).ToList();

            if(mostepComponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepComponentDto);

            return Ok(mostepComponentDto);
        }

        [HttpPost("regore")]
        /// <summary>  
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components
        /// </summary>
        /// <param name="mostepRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentForRegOreDistinct([FromBody] MostepRequestDto mostepRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentForRegOre(mostepRequestDto).ToList();

            if(mostepComponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepComponentDto);

            return Ok(mostepComponentDto);
        }
    }
}