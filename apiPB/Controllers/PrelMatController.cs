using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using apiPB.Dto.Models;
using apiPB.Filters;
using apiPB.Services.Request.Abstraction;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;

namespace apiPB.Controllers
{
    [Route("api/prel_mat")]
    [Authorize]
    [ApiController]
    public class PrelMatController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IPrelMatRequestService _prelMatRequestService;
        public PrelMatController(LogService logService, IPrelMatRequestService prelMatRequestService)
        {
            _logService = logService;
            _prelMatRequestService = prelMatRequestService;
        }

        [HttpGet("get_all")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_prel_mat
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_prel_mat</response>
        /// <response code="404">Non trovato</response>
        public IActionResult getAllPrelMat()
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppPrelMatDto = _prelMatRequestService.GetAppPrelMat().ToList();

            if (a3AppPrelMatDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", a3AppPrelMatDto);

            return Ok(a3AppPrelMatDto);
        }

        [HttpPost("post_prel_mat")]
        /// <summary>
        /// Invia la lista di A3AppPrelMat al database
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="201">Crea delle entry nel database</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PostPrelMatList([FromBody] IEnumerable<PrelMatRequestDto> a3AppPrelMatRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppPrelMatDto = _prelMatRequestService.PostPrelMatList(a3AppPrelMatRequestDto).ToList();

            if (a3AppPrelMatDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", a3AppPrelMatDto);

            return Ok(a3AppPrelMatDto);
        }

        [HttpPost("get_view_prel_mat")]
        /// <summary>
        /// Ritorna le informazioni della vista A3_app_prel_mat filtrate in base ai parametri opzionali inseriti
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna le informazioni della vista A3_app_prel_mat filtrate in base ai parametri opzionali inseriti</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetViewPrelMat([FromBody] ViewPrelMatRequestDto a3AppPrelMatRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppPrelMatDto = _prelMatRequestService.GetViewPrelMatList(a3AppPrelMatRequestDto).ToList();

            if (a3AppPrelMatDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", a3AppPrelMatDto);

            return Ok(a3AppPrelMatDto);
        }

        [HttpPut("view_prel_mat/edit_prel_qty")]
        /// <summary>
        /// Aggiorna la riga della tabella A3_app_prel_mat in base al filtro passato
        /// /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna le informazioni della vista A3_app_prel_mat filtrate in base ai parametri opzionali inseriti</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PutViewPrelMat([FromBody] ViewPrelMatPutRequestDto a3AppPrelMatRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppPrelMatDto = _prelMatRequestService.PutViewPrelMat(a3AppPrelMatRequestDto);

            if (a3AppPrelMatDto == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndItemToLog(requestPath, Ok().StatusCode, "OK", a3AppPrelMatDto);

            return Ok(a3AppPrelMatDto);
        }

        [HttpDelete("view_prel_mat/delete_prel_mat_id")]
        /// <summary>
        /// Elimina la riga della tabella A3_app_prel_mat in base al filtro passato
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna l'elemento eliminato dalla tabella</response>
        /// <response code="404">Non trovato</response>
        public IActionResult DeletePrelMatId([FromBody] ViewPrelMatDeleteRequestDto a3AppPrelMatRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var a3AppPrelMatDto = _prelMatRequestService.DeletePrelMatId(a3AppPrelMatRequestDto);

            if (a3AppPrelMatDto == null)
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndItemToLog(requestPath, Ok().StatusCode, "OK", a3AppPrelMatDto);

            return Ok(a3AppPrelMatDto);
        }
    }
}