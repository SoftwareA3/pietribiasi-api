using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/job")]
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IJobRequestService _jobRequestService;

        public JobController(LogService logService, 
            IJobRequestService jobRequestService)
        {
            _logService = logService;
            _jobRequestService = jobRequestService;
        }

        [HttpGet]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_jobs
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_jobs</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiJobs()
        {
            // Stringa necessaria per il log: inserisce il nome del metodo e il percorso della richiesta
            // Esempio: GET api/job
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var jobsDto = _jobRequestService.GetJobs()
            .ToList();

            if (jobsDto.IsNullOrEmpty())
            {   
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", jobsDto);

            return Ok(jobsDto);    
        }

        [HttpPost("mo")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo
        /// </summary>
        /// <param name="moRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVWApiMoFromBody([FromBody] MoRequestDto moRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";
            var jobMoDto = _jobRequestService.GetMo(moRequestDto)
            .ToList();

            if(jobMoDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", jobMoDto);

            return Ok(jobMoDto);
        }

        [HttpPost("mostep")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMostepFromBody([FromBody] MostepRequestDto mostepRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _jobRequestService.GetMostepByMoId(mostepRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto);

            return Ok(mostepDto);
        }

        [HttpPost("mocomponent")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mocomponent
        /// </summary>
        /// <param name="moComponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mocomponent</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMocomponent([FromBody] MocomponentRequestDto moComponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mocomponentDto = _jobRequestService.GetMocomponent(moComponentRequestDto).ToList();

            if(mocomponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mocomponentDto);

            return Ok(mocomponentDto);
        }

        [HttpPost("mostepcomponent")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components
        /// </summary>
        /// <param name="moStepsComponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMoStepsComponent([FromBody] MoStepsComponentRequestDto moStepsComponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _jobRequestService.GetMoStepsComponentByMoId(moStepsComponentRequestDto).ToList();

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