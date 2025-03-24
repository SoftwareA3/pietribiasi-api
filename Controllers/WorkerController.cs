using Microsoft.AspNetCore.Mvc;
using apiPB.Models;
using apiPB.Data;
using apiPB.Mappers;
using apiPB.Dto;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;

namespace apiPB.Controllers
{
    [Route("api/worker")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        // variabile utilizzata per il recupero delle informazioni dal file appsettings.json
        private readonly IConfiguration _configuration;
        private readonly WorkerService _workerService;
        private readonly LogService _logService;
        private readonly ApplicationDbContext _context;
        
        public WorkerController(WorkerService workerService, LogService logService, ApplicationDbContext context, IConfiguration configuration) 
        {
            _configuration = configuration;

            _workerService = workerService;

            _logService = logService;

            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Lista di WorkerQueryResults recuperata tramite query al database
            var workers = _context.VwWorkers.ToList()
            .Select(w => w.ToWorkerDto());

            if (workers.IsNullOrEmpty())
            {
                var nf = NotFound();

                _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker; StatusCode: {nf.StatusCode}; Message: Not Found;");

                return nf;
            }

            var ok = Ok(workers);

            _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker; StatusCode: {ok.StatusCode}; Message: OK;");
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

                _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker/{password}; StatusCode: {nf.StatusCode}; Message: Not Found;");

                return nf;
            }

            var workerDtos = workers.Select(w => w.ToWorkerDto()).ToList();

            var ok = Ok(workerDtos);

            _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker/{password}; StatusCode: {ok.StatusCode}; Message: OK;");
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

                _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker/{id}; StatusCode: {nf.StatusCode}; Message: Not Found;");

                return nf;
            }

            var workersFieldDtos = workersField.Select(s => s.ToWorkersFieldDto()).ToList();

            var ok = Ok(workersFieldDtos);

            _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker/{id}; StatusCode: {ok.StatusCode}; Message: OK;");
            _logService.AppendWorkersFieldListToLog(workersFieldDtos);
            
            return ok;
        }

        [HttpGet("/workersfield/createOrUpdate/{password}")]
        public IActionResult PostLastLoginLineOrUpdate([FromRoute] string password)
        {
            // Ritorna le informazioni della tabella della query con Last Login aggiornato o creato
            var worker = _context.VwWorkers.ToList()
            .FirstOrDefault(w => w.Password == password);

            if (worker == null)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker/{password}; StatusCode: {nf.StatusCode}; Message: Not Found;");

                return nf;
            }

            var workersFieldId = worker.WorkerId;
            
            // Chiama la funzione in Services per cercare quale entry aggiornare
            // Ritorna il record con MAX(Line)
            var workersField = _workerService.GetWorkerFieldsLastLineFromWorkerId(workersFieldId);
            
            if(workersField == null)
            {
                var nf = NotFound();

                _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker/{password}; StatusCode: {nf.StatusCode}; Message: Not Found;");

                return nf;
            }
            
            // Esiste una entry con Last Login come valore del campo FieldValue
            if(workersField.FieldName == "Last Login")
            {
                // Aggiorna FieldValue con data e ora attuali
                workersField.FieldValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                _context.SaveChanges();

                var ok = Ok(workersField.ToWorkersFieldDto());

                _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker/{password}; StatusCode: {ok.StatusCode}; Message: OK;");
                
                // Crea una lista per passarla al metodo AppendWorkersFieldListToLog
                _logService.AppendWorkersFieldListToLog([workersField.ToWorkersFieldDto()]);

                return ok;
            }
            // Viene creato un nuovo record con Line = 4; FieldName "Last Login";
            // FieldValue, Tbcreated, Tbmodified con DateTime attuali e 
            // TbcreatedId, TbmodifiedId con il campo "UserId" nel file appsettings.json
            else
            {
                var newRecord = new RmWorkersField
                {
                    WorkerId = workersField.WorkerId,
                    Line = (short) (workersField.Line + 1),
                    FieldName = "Last Login",
                    FieldValue = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"),
                    Notes = workersField.Notes,
                    HideOnLayout = workersField.HideOnLayout,
                    Tbcreated = DateTime.Now,
                    Tbmodified = DateTime.Now,
                    TbcreatedId = _configuration.GetValue<int>("UserId"),
                    TbmodifiedId = _configuration.GetValue<int>("UserId")
                };

                _context.RmWorkersFields.Add(newRecord);
                _context.SaveChanges();
                
                var created = CreatedAtAction(nameof(GetWorkersFieldsById), new { id = newRecord.WorkerId }, newRecord.ToWorkersFieldDto());

                _logService.AppendMessageToLog($"Time: {DateTime.Now}; GET api/worker/{password}; StatusCode: {created.StatusCode}; Message: Created;");

                
                return created;
            }
        }
    }
}