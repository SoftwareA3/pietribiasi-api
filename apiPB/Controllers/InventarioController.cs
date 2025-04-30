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
    [Route("api/inventario")]
    [Authorize]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly LogService _logService;
        private readonly IInventarioRequestService _inventarioRequestService;
        public InventarioController(LogService logService, IInventarioRequestService inventarioRequestService)
        {
            _logService = logService;
            _inventarioRequestService = inventarioRequestService;
        }

        [HttpGet("get_all")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_inventario
        /// /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_inventario</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetInventario()
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var inventarioDto = _inventarioRequestService.GetInventario().ToList();

            if (inventarioDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", inventarioDto);

            return Ok(inventarioDto);
        }

        [HttpPost("post_inventario")]
        /// <summary>
        /// Invia la lista di A3AppInventario al database
        /// </summary>
        /// <param name="IEnumerable<a3AppInventarioRequestDto>">Oggetto contenente i parametri di ricerca</param>
        /// <response code="201">Crea delle entry nel database</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PostInventarioList([FromBody] IEnumerable<InventarioRequestDto> inventarioRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var inventarioDto = _inventarioRequestService.PostInventarioList(inventarioRequestDto).ToList();

            if (inventarioDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found");

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", inventarioDto);

            return Ok(inventarioDto);
        }
    }
}