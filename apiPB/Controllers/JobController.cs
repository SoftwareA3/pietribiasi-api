using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Request.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/job")]
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IJobRequestService _jobRequestService;

        public JobController(LogService logService, 
            IJobRequestService jobRequestService)
        {
            _logService = logService;
            _jobRequestService = jobRequestService;
        }

        [HttpGet]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_jobs
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_jobs</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiJobs()
        {
            // Stringa necessaria per il log: inserisce il nome del metodo e il percorso della richiesta
            // Esempio: GET api/job
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var jobsDto = _jobRequestService.GetJobs()
            .ToList();

            if (jobsDto.IsNullOrEmpty())
            {   
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", jobsDto);

            return Ok(jobsDto);    
        }

        [HttpPost("mostep")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMostepFromBody([FromBody] MostepRequestDto mostepRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _jobRequestService.GetMostepByMoId(mostepRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto);

            return Ok(mostepDto);
        }

        [HttpPost("mostepmocomponent")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components
        /// </summary>
        /// <param name="MostepsMocomponentRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMostepsMocomponent([FromBody] MostepsMocomponentRequestDto mostepsMocomponentRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _jobRequestService.GetMostepsMocomponentByMoId(mostepsMocomponentRequestDto).ToList();

            if(mostepComponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepComponentDto);

            return Ok(mostepComponentDto);
        }

        [HttpPost("mostepcomponent/regore")]
        /// <summary>  
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components
        /// </summary>
        /// <param name="mostepRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentForRegOreDistinct([FromBody] MostepRequestDto mostepRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepComponentDto = _jobRequestService.GetMostepsMocomponentForRegOre(mostepRequestDto).ToList();

            if(mostepComponentDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepComponentDto);

            return Ok(mostepComponentDto);
        }

        [HttpPost("mostep/odp")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepOdpRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepWithOdp([FromBody] MostepOdpRequestDto mostepOdpRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _jobRequestService.GetMostepWithOdp(mostepOdpRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto);

            return Ok(mostepDto);
        }

        [HttpPost("mostep/lavorazioni")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// /// </summary>
        /// <param name="mostepLavorazioniRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepWithLavorazione([FromBody] MostepLavorazioniRequestDto mostepLavorazioniRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var mostepDto = _jobRequestService.GetMostepWithLavorazione(mostepLavorazioniRequestDto).ToList();

            if(mostepDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", mostepDto);

            return Ok(mostepDto);
        }

        [HttpGet("get_all_reg_ore")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_reg_ore
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult getAllRegOre()
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";
                
            var a3AppRegOreDto = _jobRequestService.GetAppRegOre().ToList();

            if (a3AppRegOreDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", a3AppRegOreDto);

            return Ok(a3AppRegOreDto);
        }

        [HttpPost("reg_ore")]
        /// <summary>
        /// Invia la lista di A3AppRegOre al database
        /// </summary>
        /// <param name="IEnumerable<a3AppRegOreRequestDto>">Oggetto contenente i parametri di ricerca</param>
        /// <response code="201">Crea delle entry nel database</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PostRegOreList([FromBody] IEnumerable<A3AppRegOreRequestDto> a3AppRegOreRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppRegOreDto = _jobRequestService.PostAppRegOre(a3AppRegOreRequestDto).ToList();

            if (a3AppRegOreDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            var createdResult = CreatedAtAction(nameof(PostRegOreList), a3AppRegOreDto);
            _logService.AppendMessageAndListToLog(requestPath, createdResult.StatusCode, "Created", a3AppRegOreDto);

            return CreatedAtAction(nameof(PostRegOreList), a3AppRegOreDto);
        }

        [HttpPost("view_ore")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_reg_ore filtrate in base ai parametri del Dto di richiesta
        /// </summary>
        /// <param name="A3AppViewOreRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_reg_ore filtrate</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetA3AppRegOre([FromBody] A3AppViewOreRequestDto a3AppViewOreRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppRegOreDto = _jobRequestService.GetAppViewOre(a3AppViewOreRequestDto).ToList();

            if (a3AppRegOreDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", a3AppRegOreDto);

            return Ok(a3AppRegOreDto);
        }

        [HttpPut("view_ore/edit_working_time")]
        /// <summary>
        /// Aggiorna il tempo di lavoro della riga della tabella A3_app_reg_ore in base al filtro passato
        /// </summary>
        /// <param name="A3AppViewOrePutRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna il record modificato della tabella A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PutA3AppRegOre([FromBody] A3AppViewOrePutRequestDto a3AppViewOrePutRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppRegOreDto = _jobRequestService.PutAppViewOre(a3AppViewOrePutRequestDto);

            if (a3AppRegOreDto == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndItemToLog(requestPath, Ok().StatusCode, "OK", a3AppRegOreDto);

            return Ok(a3AppRegOreDto);
        }

        [HttpDelete("view_ore/delete_reg_ore_id")]
        /// <summary>
        /// Elimina la riga della tabella A3_app_reg_ore in base al filtro passato
        /// </summary>
        /// <param name="A3AppDeleteRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna il record eliminato della tabella A3_app_reg_ore</response>
        /// <response code="404">Non trovato</response>
        public IActionResult DeleteRegOreId([FromBody] A3AppDeleteRequestDto a3AppDeleteRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppRegOreDto = _jobRequestService.DeleteRegOreId(a3AppDeleteRequestDto);

            if (a3AppRegOreDto == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndItemToLog(requestPath, Ok().StatusCode, "Deleted", a3AppRegOreDto);

            return Ok(a3AppRegOreDto);
        }
    }    
}