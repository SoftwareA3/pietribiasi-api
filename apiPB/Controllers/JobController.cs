using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Mappers.Dto;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Repository.Abstraction;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/job")]
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IVwApiJobRepository _vwApiJobRepository;
        private readonly IMoRequestService _moRequestService;
        private readonly IMostepRequestService _mostepRequestService;
        private readonly IMocomponentRequestService _mocomponentRequestService;
        private readonly IMoStepsComponentRequestService _moStepsComponentRequestService;

        public JobController(LogService logService, 
            IVwApiJobRepository vwApiJobRepository,
            IMoRequestService moRequestService,
            IMostepRequestService mostepRequestService,
            IMocomponentRequestService mocomponentRequestService,
            IMoStepsComponentRequestService moStepsComponentRequestService)
        {
            _logService = logService;
            _vwApiJobRepository = vwApiJobRepository;
            _moRequestService = moRequestService;
            _mostepRequestService = mostepRequestService;
            _mocomponentRequestService = mocomponentRequestService;
            _moStepsComponentRequestService = moStepsComponentRequestService;
        }

        [HttpGet]
        // Ritorna tutte le informazioni della vista vw_api_jobs
        public IActionResult GetVwApiJobs()
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var jobsDto = _vwApiJobRepository.GetVwApiJobs()
            .Select(j => j.ToVwApiJobDto())
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
        public IActionResult GetVWApiMoFromBody([FromBody] VwApiMoRequestDto moRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";
            var jobMoDto = _moRequestService.GetVwApiMo(moRequestDto)
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
        public IActionResult GetVwApiMostepFromBody([FromBody] VwApiMostepRequestDto mostepRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _mostepRequestService.GetMostepByMoId(mostepRequestDto).ToList();

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
        public IActionResult GetVwApiMocomponent([FromBody] VwApiMocomponentRequestDto moComponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mocomponentDto = _mocomponentRequestService.GetVwApiMocomponent(moComponentRequestDto).ToList();

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
        public IActionResult GetVwApiMoStepsComponent([FromBody] VwApiMoStepsComponentRequestDto moStepsComponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _moStepsComponentRequestService.GetMoStepsComponentByMoId(moStepsComponentRequestDto).ToList();

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