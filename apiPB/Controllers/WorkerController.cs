using Microsoft.AspNetCore.Mvc;
using apiPB.Mappers.Dto;
using apiPB.Dto.Request;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/worker")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IWorkersRequestService _workerRequestService;
        
        public WorkerController(LogService logService,
        IWorkersRequestService workersRequestService
        )
        {
            _logService = logService;
            _workerRequestService = workersRequestService;
        }

        // Ritorna tutti i VwWorkers presenti nella vista del database
        [HttpGet]
        public IActionResult GetAllWorkers()
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var workersDto = _workerRequestService.GetWorkers();

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

            var workersFieldDto = _workerRequestService.GetWorkersFieldsById(new WorkersFieldRequestDto { WorkerId = id });

            if(workersFieldDto == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndItemToLog(requestPath, Ok().StatusCode, "OK", workersFieldDto);
            
            return Ok(workersFieldDto);
        }

        // Ritorna Alcune informazioni della tabella RmWorkersField con Last Login aggiornato o inserito con un nuovo record
        [HttpPost("lastlogin")]
        public async Task<IActionResult> UpdateOrCreateLastLogin([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            // Chiama un service che prende un PasswordWorkersRequestDto e restituisce un VwApiWorkerDto
            // Questo Dto viene passato per la chiamata alla stored procedure che accetta un VwApiWorkerDto
            // e restituisce un Task. L'idea è far ritornare un VwApiWorkerDto con l'ultimo login aggiornato
            // o inserito con un nuovo record
            // Removed 'async' keyword as no asynchronous operations are present in this method.
            //var worker = _passwordWorkersRequestService.GetWorkerByPassword(passwordWorkersRequestDto);

            // if (worker == null)
            // {
            //     _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

            //     return NotFound();
            // }
            
            // await _passwordWorkersRequestService.CallStoredProcedure(worker);

            // var lastWorkerField = _passwordWorkersRequestService.GetLastWorkerFieldLine(worker);

            // if (lastWorkerField == null)
            // {
            //     _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

            //     return NotFound();
            // }

            var lastWorkerField = await _workerRequestService.UpdateOrCreateLastLogin(passwordWorkersRequestDto);

            if(lastWorkerField == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }
            
            var created = CreatedAtAction(nameof(GetWorkersFieldsById), new { id = lastWorkerField.WorkerId }, lastWorkerField);

            _logService.AppendMessageToLog(requestPath, created.StatusCode, "Created");

            
            return created;
        }
    }
}