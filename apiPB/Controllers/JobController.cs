using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using apiPB.Services;
using apiPB.Data;
using Microsoft.AspNetCore.Authorization;
using apiPB.Mappers;
using apiPB.Dto;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using apiPB.Models;
using apiPB.Repository.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/job")]
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IVwApiJobRepository _vwApiJobRepository;
        private readonly IVwApiMoRepository _vwApiMoRepository;
        private readonly IVwApiMostepRepository _vwApiMostepRepository;
        private readonly IVwApiMocomponentRepository _vwApiMocomponentRepository;

        public JobController(LogService logService, 
            IVwApiJobRepository vwApiJobRepository,
            IVwApiMoRepository vwApiMoRepository,
            IVwApiMostepRepository vwApiMostepRepository,
            IVwApiMocomponentRepository vwApiMocomponentRepository)
        {
            _logService = logService;
            _vwApiJobRepository = vwApiJobRepository;
            _vwApiMoRepository = vwApiMoRepository;
            _vwApiMostepRepository = vwApiMostepRepository;
            _vwApiMocomponentRepository = vwApiMocomponentRepository;
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
            var jobMoDto = _vwApiMoRepository.GetVwApiMo(moRequestDto.Job, moRequestDto.RtgStep, moRequestDto.Alternate, moRequestDto.AltRtgStep)
            .Select(j => j.ToVwApiMoDto())
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

            var mostepDto = _vwApiMostepRepository.GetVwApiMostep(mostepRequestDto.Job)
            .Select(m => m.ToVwApiMoStepDto())
            .ToList();

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
        public IActionResult GetVwApiMocomponent([FromBody] VwApiMocomponentDto moComponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mocomponentDto = _vwApiMocomponentRepository.GetVwApiMocomponent(moComponentRequestDto.Job)
            .Select(m => m.ToVwApiMocomponentDto())
            .ToList();

            if(mocomponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            return Ok(mocomponentDto);
        }

        [HttpPost("mostepcomponent")]
        // Ritorna tutte le informazioni della vista vw_api_mostep e vw_api_mo_steps_components
        public IActionResult GetVwApiMoStepsComponent([FromBody] VwApiMoStepsComponentRequestDto moStepsComponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _vwApiMostepRepository.GetVwApiMostep(moStepsComponentRequestDto.Job)
            .Select(m => m.ToVwApiMoStepDto())
            .ToList();

            return Ok(mostepComponentDto);
        }
    }

    
}