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
        public IActionResult GetVwApiJobs()
        {
            var jobs = _context.VwApiJobs.ToList()
            .Select(j => j.ToVwApiJobDto());

            if (jobs == null)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog("GET api/job", nf.StatusCode, "Not Found");

                return nf;
            }

            var ok = Ok(jobs);

            _logService.AppendMessageToLog("GET api/job", ok.StatusCode, "OK");
            _logService.AppendJobListToLog(jobs.ToList());

            return ok;
        }

        [Authorize]
        [HttpPost("mo")]
        public IActionResult GetVWApiMo([FromBody] VwApiMoRequestDto moRequestDto)
        {
            var jobMo = _context.VwApiMos
            .Where(j => j.Job == moRequestDto.Job && j.RtgStep == moRequestDto.RtgStep && j.Alternate == moRequestDto.Alternate && j.AltRtgStep == moRequestDto.AltRtgStep)
            .ToList();

            if(jobMo.Count == 0)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog("GET api/job", nf.StatusCode, "Not Found");

                return nf;
            }

            var ok = Ok(jobMo);

            _logService.AppendMessageToLog("GET api/job", ok.StatusCode, "OK");
            _logService.AppendJobMoListToLog(jobMo.ToList());

            return ok;
        }

        [Authorize]
        [HttpPost("mostep")]
        public IActionResult GetVwApiMostep([FromBody] VwApiMostepRequestDto mostepRequestDto)
        {
            var mostep = _context.VwApiMosteps
            .Where(m => m.Job == mostepRequestDto.Job)
            .ToList();

            if(mostep.Count == 0)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog("GET api/job", nf.StatusCode, "Not Found");

                return nf;
            }

            var ok = Ok(mostep);

            _logService.AppendMessageToLog("GET api/job", ok.StatusCode, "OK");
            _logService.AppendMostepListToLog(mostep.ToList());

            return ok;
        }

    }

    
}