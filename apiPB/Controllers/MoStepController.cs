using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Request.Abstraction;
using apiPB.Services.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/mostep")]
    [Authorize]
    [ApiController]
    public class MoStepController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IMoStepRequestService _moStepRequestService;
        private readonly bool _logIsActive;
        public MoStepController(ILogService logService, IMoStepRequestService moStepRequestService)
        {
            _logService = logService;
            _moStepRequestService = moStepRequestService;
            _logIsActive = false;
        }

        [HttpPost("job")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMostepWithJob([FromBody] JobRequestDto mostepRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _moStepRequestService.GetMostepWithJob(mostepRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found", _logIsActive);

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto, _logIsActive);

            return Ok(mostepDto);
        }

        [HttpPost("odp")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepMonoRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepWithMono([FromBody] MonoRequestDto mostepMonoRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _moStepRequestService.GetMostepWithMono(mostepMonoRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found", _logIsActive);

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto, _logIsActive);

            return Ok(mostepDto);
        }

        [HttpPost("lavorazioni")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// /// </summary>
        /// <param name="mostepLavorazioniRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepWithOperation([FromBody] OperationRequestDto mostepOperationRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _moStepRequestService.GetMostepWithOperation(mostepOperationRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found", _logIsActive);

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto, _logIsActive);

            return Ok(mostepDto);
        }
    }
}