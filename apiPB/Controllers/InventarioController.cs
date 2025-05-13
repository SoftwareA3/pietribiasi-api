using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using apiPB.Dto.Models;
using apiPB.Filters;
using apiPB.Services.Abstraction;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;
using apiPB.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/inventario")]
    [Authorize]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IInventarioRequestService _inventarioRequestService;
        private readonly bool _isLogActive;
        public InventarioController(IResponseHandler responseHandler, IInventarioRequestService inventarioRequestService)
        {
            _responseHandler = responseHandler;
            _inventarioRequestService = inventarioRequestService;
            _isLogActive = false;
        }

        [HttpGet("get_all")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_inventario
        /// /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_inventario</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetInventario()
        {
            var inventarioDto = _inventarioRequestService.GetInventario().ToList();

            if (inventarioDto == null || !inventarioDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, inventarioDto, _isLogActive);
        }

        [HttpPost("post_inventario")]
        /// <summary>
        /// Invia la lista di Dto al database
        /// </summary>
        /// <param name="IEnumerable<a3AppInventarioRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="201">Crea delle entry nel database. Se le entry esistono, aggiorna alcuni campi</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PostInventarioList([FromBody] IEnumerable<InventarioRequestDto>? inventarioRequestDto)
        {
            if (inventarioRequestDto == null || !inventarioRequestDto.Any()) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var inventarioDto = _inventarioRequestService.PostInventarioList(inventarioRequestDto).ToList();

            if (inventarioDto == null || !inventarioDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, inventarioDto, _isLogActive);
        }

        [HttpPost("get_view_inventario")]
        /// <summary>
        /// Ritorna la lista di A3AppInventario in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: FromDate, ToDate, Item, BarCode</param>
        /// <response code="200">Ritorna la lista di A3AppInventario in base al filtro passato</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetViewInventario([FromBody] ViewInventarioRequestDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var inventarioDto = _inventarioRequestService.GetViewInventario(request).ToList();

            if (inventarioDto == null || !inventarioDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, inventarioDto, _isLogActive);
        }

        [HttpPut("view_inventario/edit_book_inv")]
        /// <summary>
        /// Aggiorna il record di A3AppInventario in base al filtro passato
        /// </summary>
        /// <param name="ViewInventarioPutRequestDto">Oggetto che contiene i parametri di ricerca</param>
        /// <response code="200">Ritorna l'elemento modificato</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PutViewInventario([FromBody] ViewInventarioPutRequestDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var inventarioDto = _inventarioRequestService.PutViewInventario(request);

            if (inventarioDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndItem(HttpContext, inventarioDto, _isLogActive);
        }
    }
}