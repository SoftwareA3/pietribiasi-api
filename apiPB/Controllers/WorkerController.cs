using Microsoft.AspNetCore.Mvc;
using apiPB.Models;
using apiPB.Data;
using apiPB.Mappers.Dto;
using apiPB.Dto.Request;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using apiPB.Repository.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/worker")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IRmWorkersFieldRepository _rmWorkersFieldRepository;
        private readonly IVwApiWorkerRepository _vwApiWorkerRepository;
        
        public WorkerController(LogService logService,
        IRmWorkersFieldRepository rmWorkersFieldRepository, 
        IVwApiWorkerRepository vwApiWorkerRepository)
        {
            _logService = logService;
            _rmWorkersFieldRepository = rmWorkersFieldRepository;
            _vwApiWorkerRepository = vwApiWorkerRepository;
        }

        // Ritorna tutti i VwWorkers presenti nella vista del database
        [HttpGet]
        public IActionResult GetAllWorkers()
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var workersDto = _vwApiWorkerRepository.GetVwApiWorkers()
            .Select(w => w.ToWorkerDto());

            if (workersDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            } 

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", workersDto.ToList());

            return Ok(workersDto.ToList());
        }

        [HttpGet("workersfield/{id}")]
        public IActionResult GetWorkersFieldsById([FromRoute] int id)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var workersFieldDto = _rmWorkersFieldRepository.GetRmWorkersFieldsById(id)
            .Select(w => w.ToWorkersFieldRequestDto())
            .ToList();

            if(workersFieldDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", workersFieldDto);
            
            return Ok(workersFieldDto);
        }

        // Ritorna Alcune informazioni della tabella RmWorkersField con Last Login aggiornato o inserito con un nuovo record
        [HttpPost("lastlogin")]
        public async Task<IActionResult> UpdateOrCreateLastLogin([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var worker = _vwApiWorkerRepository.GetVwApiWorkerByPassword(passwordWorkersRequestDto.Password);

            if (worker == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }
            
            await _vwApiWorkerRepository.CallStoredProcedure(worker.WorkerId);

            var lastWorkerField = _rmWorkersFieldRepository.GetLastWorkerFeldLine(worker.WorkerId);

            if (lastWorkerField == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            var workersFieldDto = lastWorkerField.ToWorkersFieldRequestDto();
            
            var created = CreatedAtAction(nameof(GetWorkersFieldsById), new { id = workersFieldDto.WorkerId }, workersFieldDto);

            _logService.AppendMessageToLog(requestPath, created.StatusCode, "Created");

            
            return created;
        }
    }
}