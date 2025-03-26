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
            var workers = _context.VwApiWorkers.ToList()
            .Select(w => w.ToWorkerDto());

            if (workers.IsNullOrEmpty())
            {
                var nf = NotFound();

                _logService.AppendMessageToLog(requestPath, nf.StatusCode, "Not Found");

                return nf;
            }

            var ok = Ok(workers);

            _logService.AppendMessageAndListToLog(requestPath, ok.StatusCode, "OK", workers.ToList());
            // _logService.AppendWorkerListToLog(workers.ToList());

            return ok;
        }

        [HttpGet("workersfield/{id}")]
        public IActionResult GetWorkersFieldsById([FromRoute] int id)
        {
            string requestPath = "GET " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            // Recupera tutti i RmWorkersFields tramite WorkerId
            var workersField = _context.RmWorkersFields.ToList()
            .FindAll(w => w.WorkerId == id);

            if(workersField.IsNullOrEmpty())
            {
                var nf = NotFound();

                _logService.AppendMessageToLog(requestPath, nf.StatusCode, "Not Found");

                return nf;
            }

            var workersFieldDtos = workersField.Select(s => s.ToWorkersFieldDto()).ToList();

            var ok = Ok(workersFieldDtos);

            _logService.AppendMessageAndListToLog(requestPath, ok.StatusCode, "OK", workersFieldDtos);
            // _logService.AppendWorkersFieldListToLog(workersFieldDtos);
            
            return ok;
        }

        // Ritorna Alcune informazioni della tabella RmWorkersField con Last Login aggiornato o inserito con un nuovo record
        [HttpPost("lastlogin")]
        public async Task<IActionResult> PostLastLoginLineOrUpdate([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        {
            string requestPath = "POST " + HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty;

            // Trova worker tramite la password 
            var worker = _context.VwApiWorkers.ToList()
            .FirstOrDefault(w => w.Password == passwordWorkersRequestDto.Password);

            if (worker == null)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog(requestPath, nf.StatusCode, "Not Found");

                return nf;
            }
            
            // Invoca la stored procedure passando il WorkerId trovato con la query e la data corrente
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.InsertWorkersFields @WorkerId = {0}, @FieldValue = {1}", 
            worker.WorkerId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            // Salva il contesto del database
            await _context.SaveChangesAsync();

            // Recupera l'ultimo record inserito nella tabella RmWorkersFields per ritornare alcune informazioni
            // Se non si vogliono tornare informazioni, basta rimuovere questo pezzo e modificare CreatedAtAction
            var workersField = _context.RmWorkersFields
            .FromSqlRaw(@"SELECT TOP 1 * FROM RM_WorkersFields WHERE WorkerID = {0} ORDER BY Line DESC", worker.WorkerId)
            .AsNoTracking()
            .FirstOrDefault();  

            if(workersField == null)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog(requestPath, nf.StatusCode, "Not Found");

                return nf;
            }
            
            var created = CreatedAtAction(nameof(GetWorkersFieldsById), new { id = worker.WorkerId }, workersField.ToWorkersFieldRequestDto());

            _logService.AppendMessageToLog(requestPath, created.StatusCode, "Created");

            
            return created;
        }
    }
}