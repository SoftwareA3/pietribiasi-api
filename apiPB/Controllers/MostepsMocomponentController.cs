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

        [HttpPost("post_mostepsmocomponent")]
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

        [HttpPost("job")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components filtrate per Job
        /// </summary>
        /// <param name="mostepsMocomponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithJob([FromBody] MostepsMocomponentRequestDto mostepsMocomponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentJobDistinct(mostepsMocomponentRequestDto).ToList();

            if(mostepComponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepComponentDto);

            return Ok(mostepComponentDto);
        }

        [HttpPost("mono")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components filtrate per Mono
        /// </summary>
        /// <param name="mostepsMocomponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithMono([FromBody] MostepsMocomponentRequestDto mostepsMocomponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentMonoDistinct(mostepsMocomponentRequestDto).ToList();

            if(mostepComponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepComponentDto);

            return Ok(mostepComponentDto);
        }

        [HttpPost("operation")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components filtrate per Operation
        /// /// </summary>
        /// <param name="mostepsMocomponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithOperation([FromBody] MostepsMocomponentRequestDto mostepsMocomponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentOperationDistinct(mostepsMocomponentRequestDto).ToList();

            if(mostepComponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepComponentDto);

            return Ok(mostepComponentDto);
        }

        [HttpPost("barcode")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components filtrate per BarCode
        /// </summary>
        /// <param name="mostepsMocomponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithBarCode([FromBody] MostepsMocomponentRequestDto mostepsMocomponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentBarCodeDistinct(mostepsMocomponentRequestDto).ToList();

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