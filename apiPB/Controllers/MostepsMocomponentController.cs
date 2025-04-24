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

        // [HttpPost("post_mostepsmocomponent")]
        // /// <summary>
        // /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components
        // /// </summary>
        // /// <param name="MostepsMocomponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        // /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        // /// <response code="404">Non trovato</response>
        // public IActionResult GetVwApiMostepsMocomponent([FromBody] MostepsMocomponentRequestDto mostepsMocomponentRequestDto)
        // {
        //     string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

        //     var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentByMoId(mostepsMocomponentRequestDto).ToList();

        //     if(mostepComponentDto.IsNullOrEmpty())
        //     {
        //         _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

        //         return NotFound();
        //     }

        //     _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepComponentDto);

        //     return Ok(mostepComponentDto);
        // }

        [HttpPost("job")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components filtrate per Job
        /// </summary>
        /// <param name="mostepsMocomponentJobRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithJob([FromBody] JobRequestDto mostepsMocomponentJobRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentJobDistinct(mostepsMocomponentJobRequestDto).ToList();

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
        /// <param name="mostepsMocomponentMonoRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithMono([FromBody] MonoRequestDto mostepsMocomponentMonoRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentMonoDistinct(mostepsMocomponentMonoRequestDto).ToList();

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
        /// <param name="mostepsMocomponentOperationRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithOperation([FromBody] OperationRequestDto mostepsMocomponentOperationRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentOperationDistinct(mostepsMocomponentOperationRequestDto).ToList();

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
        /// <param name="mostepsMocomponentBarCodeRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithBarCode([FromBody] BarCodeRequestDto mostepsMocomponentBarCodeRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentBarCodeDistinct(mostepsMocomponentBarCodeRequestDto).ToList();

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