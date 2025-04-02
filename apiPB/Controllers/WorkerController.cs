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

        [HttpGet]
        /// <summary>
        /// Ritorna tutti i VwWorkers presenti nella vista del database
        /// </summary>
        /// <response code="200">Ritorna tutti i VwWorkers</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetAllWorkers()
        {
            // Stringa necessaria per il log: inserisce il nome del metodo e il percorso della richiesta
            // Esempio: GET api/worker
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
        /// <summary>
        /// Ritorna tutte le informazioni della tabella RmWorkersField
        /// </summary>
        /// <param name="id">Id del lavoratore</param>
        /// <response code="200">Ritorna tutte le informazioni della tabella RmWorkersField</response>
        /// <response code="404">Non trovato</response>
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

        [HttpPost("lastlogin")]
        /// <summary>
        /// Ritorna Alcune informazioni della tabella RmWorkersField con Last Login aggiornato o inserito con un nuovo record
        /// /// </summary>
        /// <param name="passwordWorkersRequestDto">Oggetto PasswordWorkersRequestDto: richiede la password dell'utente</param>
        /// <response code="201">Creato</response>
        /// <response code="404">Non trovato</response>
        public async Task<IActionResult> UpdateOrCreateLastLogin([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";
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

        /// <summary>
        /// Verifica le credenziali inviate tramite Basic Authentication.
        /// </summary>
        /// <returns>200 OK se le credenziali sono corrette, altrimenti 401 Unauthorized.</returns>
        [HttpPost("login")]
        public IActionResult LoginWithPassword([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var workerIdAndPasswordDto = _workerRequestService.LoginWithPassword(passwordWorkersRequestDto);
            if(workerIdAndPasswordDto == null)
            {
                // Se il lavoratore non esiste, restituisce 401 Unauthorized
                _logService.AppendMessageToLog("Invalid credentials", 401, "Unauthorized");
                return Unauthorized(new { message = "Invalid credentials" });
            }
            
            var ok = Ok(workerIdAndPasswordDto);

            _logService.AppendMessageAndItemToLog(requestPath, ok.StatusCode, "OK", workerIdAndPasswordDto);

            return ok;   
        }
    }
}