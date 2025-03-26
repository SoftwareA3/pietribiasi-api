using Microsoft.AspNetCore.Mvc;
using apiPB.Models;
using apiPB.Data;
using apiPB.Mappers;
using apiPB.Dto;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace apiPB.Controllers
{
    [Route("api/worker")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly ApplicationDbContext _context;
        
        public WorkerController(LogService logService, ApplicationDbContext context) 
        {
            _logService = logService;

            _context = context;
        }

        // Ritorna tutti i VwWorkers presenti nella vista del database
        [HttpGet]
        public IActionResult GetAllWorkers()
        {
            string requestPath = "GET " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            // Lista di WorkerQueryResults recuperata tramite query al database
            var workersDto = _context.VwApiWorkers.ToList()
            .Select(w => w.ToWorkerDto());

            if (workersDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            } 

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", workersDto.ToList());
            // _logService.AppendWorkerListToLog(workers.ToList());

            return Ok(workersDto.ToList());
        }

        [HttpGet("workersfield/{id}")]
        public IActionResult GetWorkersFieldsById([FromRoute] int id)
        {
            string requestPath = "GET " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            // Recupera tutti i RmWorkersFields tramite WorkerId
            var workersFieldDto = _context.RmWorkersFields.ToList()
            .Select(w => w.ToWorkersFieldDto())
            .ToList()
            .Where(w => w.WorkerId == id);

            if(workersFieldDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", workersFieldDto.ToList());
            
            return Ok(workersFieldDto.ToList());
        }

        // Ritorna Alcune informazioni della tabella RmWorkersField con Last Login aggiornato o inserito con un nuovo record
        [HttpPost("lastlogin")]
        public async Task<IActionResult> PostLastLoginLineOrUpdate([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        {
            string requestPath = "POST " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            // Trova worker tramite la password 
            var workerDto = _context.VwApiWorkers.ToList()
            .FirstOrDefault(w => w.Password == passwordWorkersRequestDto.Password);

            if (workerDto == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }
            
            // Invoca la stored procedure passando il WorkerId trovato con la query e la data corrente
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.InsertWorkersFields @WorkerId = {0}, @FieldValue = {1}", 
            workerDto.WorkerId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            // Salva il contesto del database
            await _context.SaveChangesAsync();

            // Recupera l'ultimo record inserito nella tabella RmWorkersFields per ritornare alcune informazioni
            // Se non si vogliono tornare informazioni, basta rimuovere questo pezzo e modificare CreatedAtAction
            var workersField = _context.RmWorkersFields
            .FromSqlRaw(@"SELECT TOP 1 * FROM RM_WorkersFields WHERE WorkerID = {0} ORDER BY Line DESC", workerDto.WorkerId)
            .AsNoTracking()
            .ToList()
            .Select(w => w.ToWorkersFieldRequestDto())
            .FirstOrDefault();  

            if(workersField == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }
            
            var created = CreatedAtAction(nameof(GetWorkersFieldsById), new { id = workerDto.WorkerId }, workersField);

            _logService.AppendMessageToLog(requestPath, created.StatusCode, "Created");

            
            return created;
        }
    }
}