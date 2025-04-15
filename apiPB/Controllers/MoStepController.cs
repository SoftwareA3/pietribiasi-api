using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/mostep")]
    [Authorize]
    [ApiController]
    public class MoStepController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IMoStepRequestService _moStepRequestService;
        public MoStepController(LogService logService, IMoStepRequestService moStepRequestService)
        {
            _logService = logService;
            _moStepRequestService = moStepRequestService;
        }

        [HttpPost("post_mostep")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMostepFromBody([FromBody] MostepRequestDto mostepRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _moStepRequestService.GetMostepByMoId(mostepRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto);

            return Ok(mostepDto);
        }

        [HttpPost("mostep/odp")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepOdpRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepWithOdp([FromBody] MostepOdpRequestDto mostepOdpRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _moStepRequestService.GetMostepWithOdp(mostepOdpRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto);

            return Ok(mostepDto);
        }

        [HttpPost("mostep/lavorazioni")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// /// </summary>
        /// <param name="mostepLavorazioniRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepWithLavorazione([FromBody] MostepLavorazioniRequestDto mostepLavorazioniRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _moStepRequestService.GetMostepWithLavorazione(mostepLavorazioniRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto);

            return Ok(mostepDto);
        }
    }
}