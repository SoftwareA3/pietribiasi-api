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

        [Authorize]
        [HttpGet]
        // Ritorna tutte le informazioni della vista vw_api_jobs
        public IActionResult GetVwApiJobs()
        {
            string requestPath = "GET " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            var jobs = _context.VwApiJobs.ToList()
            .Select(j => j.ToVwApiJobDto());

            if (jobs == null)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog(requestPath, nf.StatusCode, "Not Found");

                return nf;
            }

            var ok = Ok(jobs);

            _logService.AppendMessageAndListToLog(requestPath, ok.StatusCode, "OK", jobs.ToList());

            return ok;
        }

        [Authorize]
        [HttpPost("mo")]
        // Ritorna tutte le informazioni della vista vw_api_mo
        public IActionResult PostVWApiMo([FromBody] VwApiMoRequestDto moRequestDto)
        {
            string requestPath = "POST " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            var jobMo = _context.VwApiMos
            .Where(j => j.Job == moRequestDto.Job && j.RtgStep == moRequestDto.RtgStep && j.Alternate == moRequestDto.Alternate && j.AltRtgStep == moRequestDto.AltRtgStep)
            .ToList();

            if(jobMo.Count == 0)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog(requestPath, nf.StatusCode, "Not Found");

                return nf;
            }

            var ok = Ok(jobMo);

            _logService.AppendMessageAndListToLog(requestPath, ok.StatusCode, "OK", jobMo.ToList());

            return ok;
        }

        [Authorize]
        [HttpPost("mostep")]
        // Ritorna tutte le informazioni della vista vw_api_mostep
        public IActionResult PostVwApiMostep([FromBody] VwApiMostepRequestDto mostepRequestDto)
        {
            string requestPath = "POST " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            var mostep = _context.VwApiMosteps
            .Where(m => m.Job == mostepRequestDto.Job)
            .ToList();

            if(mostep.Count == 0)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog(requestPath, nf.StatusCode, "Not Found");

                return nf;
            }

            var ok = Ok(mostep);

            _logService.AppendMessageAndListToLog(requestPath, ok.StatusCode, "OK", mostep.ToList());

            return ok;
        }

    }

    
}