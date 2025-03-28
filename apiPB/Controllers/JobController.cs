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
        // Ritorna tutte le informazioni della vista vw_api_jobs
        public IActionResult GetVwApiJobs()
        {
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
        // Ritorna tutte le informazioni della vista vw_api_mo
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
        // Ritorna tutte le informazioni della vista vw_api_mostep
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
        // Ritorna tutte le informazioni della vista vw_api_mocomponent
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
        // Ritorna tutte le informazioni della vista vw_api_mo_steps_components
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