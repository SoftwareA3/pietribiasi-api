using Microsoft.AspNetCore.Mvc;
using apiPB.Mappers.Dto;
using apiPB.Dto.Request;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using apiPB.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/worker")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IWorkersRequestService _workerRequestService;
        private readonly bool _isLogActive;
        
        public WorkerController(IResponseHandler responseHandler, IWorkersRequestService workersRequestService
        )
        {
            _responseHandler = responseHandler;
            _workerRequestService = workersRequestService;
            _isLogActive = false;
        }

        [HttpGet]
        /// <summary>
        /// Ritorna tutti i VwWorkers presenti nella vista del database
        /// </summary>
        /// <response code="200">Ritorna tutti i VwWorkers</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetAllWorkers()
        {
            var workersDto = _workerRequestService.GetWorkers().ToList();

            if (workersDto == null || workersDto.Count == 0) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, workersDto, _isLogActive);
        }

        // --- Metodi deprecati --- //

        // [HttpGet("workersfield/{id}")]
        // /// <summary>
        // /// Ritorna tutte le informazioni della tabella RmWorkersField
        // /// </summary>
        // /// <param name="id">Id del lavoratore</param>
        // /// <response code="200">Ritorna tutte le informazioni della tabella RmWorkersField</response>
        // /// <response code="404">Non trovato</response>
        // public IActionResult GetWorkersFieldsById([FromRoute] int id)
        // {
        //     string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

        //     var workersFieldDto = _workerRequestService.GetWorkersFieldsById(new WorkersFieldRequestDto { WorkerId = id }).ToList();

        //     if(workersFieldDto == null)
        //     {
        //         _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found", _isLogActive);

        //         return NotFound();
        //     }

        //     _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", workersFieldDto, _isLogActive);
            
        //     return Ok(workersFieldDto);
        // }

        // [HttpPost("lastlogin")]
        // /// <summary>
        // /// Ritorna Alcune informazioni della tabella RmWorkersField con Last Login aggiornato o inserito con un nuovo record
        // /// /// </summary>
        // /// <param name="passwordWorkersRequestDto">Oggetto PasswordWorkersRequestDto: richiede la password dell'utente</param>
        // /// <response code="201">Creato</response>
        // /// <response code="404">Non trovato</response>
        // public async Task<IActionResult> UpdateOrCreateLastLogin([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        // {
        //     string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";
        //     var lastWorkerField = await _workerRequestService.UpdateOrCreateLastLogin(passwordWorkersRequestDto);

        //     if(lastWorkerField == null)
        //     {
        //         _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found", _isLogActive);

        //         return NotFound();
        //     }
            
        //     var created = CreatedAtAction(nameof(GetWorkersFieldsById), new { id = lastWorkerField.WorkerId }, lastWorkerField);

        //     _logService.AppendMessageToLog(requestPath, created.StatusCode, "Created", _isLogActive);

            
        //     return created;
        // }

        /// <summary>
        /// Verifica le credenziali inviate tramite Basic Authentication e recupera le informazioni del lavoratore autenticato.
        /// </summary>
        /// <returns>WorkerDto; 200 OK se le credenziali sono corrette, altrimenti 404 Not Found.</returns>
        [HttpPost("login")]
        public IActionResult LoginWithPassword([FromBody] PasswordWorkersRequestDto? passwordWorkersRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";
            
            if(passwordWorkersRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);
            
            var workerDto = _workerRequestService.LoginWithPassword(passwordWorkersRequestDto);
            
            if(workerDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);
            
            return _responseHandler.HandleOkAndItem(HttpContext, workerDto, _isLogActive);
        }
    }
}