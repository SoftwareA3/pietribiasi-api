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
using apiPB.Models;

namespace apiPB.Controllers
{
    [Route("api/job")]
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly ApplicationDbContext _context;

        public JobController(LogService logService, ApplicationDbContext context) 
        {

            _logService = logService;

            _context = context;
        }

        [HttpGet]
        // Ritorna tutte le informazioni della vista vw_api_jobs
        public IActionResult GetVwApiJobs()
        {
            string requestPath = "GET " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            var jobsDto = _context.VwApiJobs.ToList()
            .Select(j => j.ToVwApiJobDto());

            if (jobsDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", jobsDto.ToList());

            return Ok(jobsDto.ToList());
        }

        [HttpPost("mo")]
        // Ritorna tutte le informazioni della vista vw_api_mo
        public IActionResult PostVWApiMo([FromBody] VwApiMoRequestDto moRequestDto)
        {
            string requestPath = "POST " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            var jobMoDto = _context.VwApiMos
            .Where(j => j.Job == moRequestDto.Job && j.RtgStep == moRequestDto.RtgStep && j.Alternate == moRequestDto.Alternate && j.AltRtgStep == moRequestDto.AltRtgStep)
            .ToList()
            .Select(j => j.ToVwApiMoDto());

            if(jobMoDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", jobMoDto.ToList());

            return Ok(jobMoDto.ToList());
        }

        [HttpPost("mostep")]
        // Ritorna tutte le informazioni della vista vw_api_mostep
        public IActionResult PostVwApiMostep([FromBody] VwApiMostepRequestDto mostepRequestDto)
        {
            string requestPath = "POST " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            var mostepDto = _context.VwApiMosteps
            .Where(m => m.Job == mostepRequestDto.Job)
            .ToList()
            .Select(m => m.ToVwApiMoStepDto());

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto.ToList());

            return Ok(mostepDto.ToList());
        }

    }

    
}