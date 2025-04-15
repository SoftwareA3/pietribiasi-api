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
    }    
}