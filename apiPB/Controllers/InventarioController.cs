using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using apiPB.Dto.Models;
using apiPB.Filters;
using apiPB.Services.Abstraction;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;
using apiPB.Utils.Abstraction;
using apiPB.Utils.Implementation;

namespace apiPB.Controllers
{
    [Route("api/inventario")]
    [Authorize]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IInventarioRequestService _inventarioRequestService;
        public InventarioController(IResponseHandler responseHandler, IInventarioRequestService inventarioRequestService)
        {
            _responseHandler = responseHandler;
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
            try
            {
                var inventarioDto = _inventarioRequestService.GetInventario().ToList();

                return _responseHandler.HandleOkAndList(HttpContext, inventarioDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in InventarioController: " + ex.Message);
            }
            catch (ExpectedEmptyListException ex)
            {
                return _responseHandler.HandleNoContent(HttpContext, "Il servizio ritorna una lista vuota in InventarioController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna una lista vuota in InventarioController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in InventarioController: " + ex.Message);
            }
        }

        [HttpPost("post_inventario")]
        /// <summary>
        /// Invia la lista di Dto al database per la creazione o l'aggiornamento dei record di A3AppInventario
        /// </summary>
        /// <param name="IEnumerable<a3AppInventarioRequestDto>">Collezione contenente i parametri dei record da creare o modificare</param>
        /// <response code="201">Crea delle entry nel database. Se le entry esistono, aggiorna alcuni campi</response>
        /// <response code="404">Non trovato/non creato/non modificato</response>
        public IActionResult PostInventarioList([FromBody] IEnumerable<InventarioRequestDto>? inventarioRequestDto)
        {
            if (inventarioRequestDto == null || !inventarioRequestDto.Any()) return _responseHandler.HandleBadRequest(HttpContext);

            try
            {
                var inventarioDto = _inventarioRequestService.PostInventarioList(inventarioRequestDto).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, inventarioDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in InventarioController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna una lista vuota in InventarioController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in InventarioController: " + ex.Message);
            }
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
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext);

            try
            {
                var inventarioDto = _inventarioRequestService.GetViewInventario(request).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, inventarioDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in InventarioController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna una lista vuota in InventarioController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in InventarioController: " + ex.Message);
            }
        }

        [HttpPut("view_inventario/edit_book_inv")]
        /// <summary>
        /// Aggiorna il record di A3AppInventario in base al filtro passato
        /// </summary>
        /// <param name="ViewInventarioPutRequestDto">Oggetto che contiene i parametri di ricerca per trovare e modificare il record</param>
        /// <response code="200">Ritorna l'elemento modificato</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PutViewInventario([FromBody] ViewInventarioPutRequestDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext);

            try
            {
                var inventarioDto = _inventarioRequestService.PutViewInventario(request);

                return _responseHandler.HandleOkAndItem(HttpContext, inventarioDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in InventarioController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in InventarioController: " + ex.Message);
            }
        }

        [HttpGet("get_inventario_not_imported")]
        /// <summary>
        /// Ritorna la lista di A3AppInventario non importati
        /// </summary>
        /// <response code="200">Ritorna la lista di A3AppInventario non importati</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetInventarioNotImported()
        {
            try
            {
                var inventarioDto = _inventarioRequestService.GetNotImportedInventario().ToList();

                return _responseHandler.HandleOkAndList(HttpContext, inventarioDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in InventarioController: " + ex.Message);
            }
            catch (ExpectedEmptyListException ex)
            {
                return _responseHandler.HandleNoContent(HttpContext, "Il servizio ritorna una lista vuota in InventarioController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in InventarioController: " + ex.Message);
            }
        }

        [HttpPost("view_inventario/not_imported/filtered")]
        /// <summary>
        /// Ritorna la lista di A3AppInventario non importati in base al filtro passato
        /// /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: FromDate, ToDate, Item, BarCode</param>
        /// <response code="200">Ritorna la lista di A3AppInventario non importati in base al filtro passato</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetNotImportedAppInventarioByFilter([FromBody] ViewInventarioRequestDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext);

            try
            {
                var inventarioDto = _inventarioRequestService.GetNotImportedAppInventarioByFilter(request).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, inventarioDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in InventarioController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna una lista vuota in InventarioController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in InventarioController: " + ex.Message);
            }
        }
    }
}