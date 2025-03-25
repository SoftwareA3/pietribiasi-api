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
        // variabile utilizzata per il recupero delle informazioni dal file appsettings.json
        private readonly IConfiguration _configuration;
        private readonly LogService _logService;
        private readonly ApplicationDbContext _context;
        
        public WorkerController(LogService logService, ApplicationDbContext context, IConfiguration configuration) 
        {
            _configuration = configuration;

            _logService = logService;

            _context = context;
        }

        // Ritorna tutti i VwWorkers presenti nella vista del database
        [HttpGet]
        public IActionResult GetAll()
        {
            // Lista di WorkerQueryResults recuperata tramite query al database
            var workers = _context.VwWorkers.ToList()
            .Select(w => w.ToWorkerDto());

            if (workers.IsNullOrEmpty())
            {
                var nf = NotFound();

                _logService.AppendMessageToLog("GET api/worker", nf.StatusCode, "Not Found");

                return nf;
            }

            var ok = Ok(workers);

            _logService.AppendMessageToLog("GET api/worker", ok.StatusCode, "OK");
            _logService.AppendWorkerListToLog(workers.ToList());

            return ok;
        }

        [HttpGet("{password}")]
        public IActionResult GetWorkerByPassword([FromRoute] string password)
        {
            // Chiama il metodo che restituisce tutti i WorkerQueryResults
            // Dalla query, il campo Password dovrebbe essere univoco
            var workers = _context.VwWorkers.ToList()
            .FindAll(w => w.Password == password);

            if (workers.IsNullOrEmpty())
            {
                var nf = NotFound();

                _logService.AppendMessageToLog($"GET api/worker/{password}", nf.StatusCode, "Not Found");

                return nf;
            }

            var workerDtos = workers.Select(w => w.ToWorkerDto()).ToList();

            var ok = Ok(workerDtos);

            _logService.AppendMessageToLog($"GET api/worker/{password}", ok.StatusCode, "OK");
            _logService.AppendWorkerListToLog(workerDtos);

            return ok;
        }

        [HttpGet("/workersfield/{id}")]
        public IActionResult GetWorkersFieldsById([FromRoute] int id)
        {
            // Recupera tutti i RmWorkersFields tramite WorkerId
            var workersField = _context.RmWorkersFields.ToList()
            .FindAll(w => w.WorkerId == id);

            if(workersField.IsNullOrEmpty())
            {
                var nf = NotFound();

                _logService.AppendMessageToLog($"GET api/worker/{id}", nf.StatusCode, "Not Found");

                return nf;
            }

            var workersFieldDtos = workersField.Select(s => s.ToWorkersFieldDto()).ToList();

            var ok = Ok(workersFieldDtos);

            _logService.AppendMessageToLog($"GET api/worker{id}", ok.StatusCode, "OK");
            _logService.AppendWorkersFieldListToLog(workersFieldDtos);
            
            return ok;
        }

        // Ritorna Alcune informazioni della tabella RmWorkersField con Last Login aggiornato o inserito con un nuovo record
        [HttpPost]
        public async Task<IActionResult> PostLastLoginLineOrUpdate([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        {
            // Trova worker tramite la password 
            var worker = _context.VwWorkers.ToList()
            .FirstOrDefault(w => w.Password == passwordWorkersRequestDto.Password);

            if (worker == null)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog("POST api/worker", nf.StatusCode, "Not Found");

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

                _logService.AppendMessageToLog("POST api/worker", nf.StatusCode, "Not Found");

                return nf;
            }
            
            var created = CreatedAtAction(nameof(GetWorkersFieldsById), new { id = worker.WorkerId }, workersField.ToWorkersFieldRequestDto());

            _logService.AppendMessageToLog("POST api/worker", created.StatusCode, "Created");

            
            return created;
        }
    }
}