using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("api/prel_mat")]
    [Authorize]
    [ApiController]
    public class PrelMatController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IPrelMatRequestService _prelMatRequestService;
        private readonly bool _isLogActive;
        public PrelMatController(IResponseHandler responseHandler, IPrelMatRequestService prelMatRequestService)
        {
            _responseHandler = responseHandler;
            _prelMatRequestService = prelMatRequestService;
            _isLogActive = false;
        }

        [HttpGet("get_all")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista A3_app_prel_mat
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_prel_mat</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetAllPrelMat()
        {
            try
            {
                var a3AppPrelMatDto = _prelMatRequestService.GetAppPrelMat().ToList();

                return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in PrelMatController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in PrelMatController: " + ex.Message);
            }
        }

        [HttpPost("post_prel_mat")]
        /// <summary>
        /// Invia la lista di A3AppPrelMat al database
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Collezione contenente i record da creare in A3AppPrelMat</param>
        /// <response code="201">Crea delle entry nel database</response>
        /// <response code="404">Non trovato</response>
        /// <response code="400">Bad Request</response>
        public IActionResult PostPrelMatList([FromBody] IEnumerable<PrelMatRequestDto>? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null || !a3AppPrelMatRequestDto.Any()) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            try
            {
                var a3AppPrelMatDto = _prelMatRequestService.PostPrelMatList(a3AppPrelMatRequestDto).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in PrelMatController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in PrelMatController: " + ex.Message);
            }
        }

        [HttpPost("get_view_prel_mat")]
        /// <summary>
        /// Ritorna le informazioni della vista A3_app_prel_mat filtrate in base ai parametri opzionali inseriti
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna le informazioni della vista A3_app_prel_mat filtrate in base ai parametri opzionali inseriti</response>
        /// <response code="404">Non trovato</response>
        /// <response code="400">Bad Request</response>
        public IActionResult GetViewPrelMat([FromBody] ViewPrelMatRequestDto? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            try
            {
                var a3AppPrelMatDto = _prelMatRequestService.GetViewPrelMatList(a3AppPrelMatRequestDto).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in PrelMatController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in PrelMatController: " + ex.Message);
            }
        }

        [HttpPut("view_prel_mat/edit_prel_qty")]
        /// <summary>
        /// Aggiorna la riga della tabella A3_app_prel_mat in base al filtro passato
        /// /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna le informazioni della vista A3_app_prel_mat filtrate in base ai parametri opzionali inseriti</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PutViewPrelMat([FromBody] ViewPrelMatPutRequestDto? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            try
            {
                var a3AppPrelMatDto = _prelMatRequestService.PutViewPrelMat(a3AppPrelMatRequestDto);

                return _responseHandler.HandleOkAndItem(HttpContext, a3AppPrelMatDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in PrelMatController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in PrelMatController: " + ex.Message);
            }
        }

        [HttpDelete("view_prel_mat/delete_prel_mat_id")]
        /// <summary>
        /// Elimina la riga della tabella A3_app_prel_mat in base al filtro passato
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna l'elemento eliminato dalla tabella</response>
        /// <response code="404">Non trovato</response>
        /// <response code="400">Bad Request</response>
        public IActionResult DeletePrelMatId([FromBody] ViewPrelMatDeleteRequestDto? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);
            try
            {
                var a3AppPrelMatDto = _prelMatRequestService.DeletePrelMatId(a3AppPrelMatRequestDto);

                return _responseHandler.HandleOkAndItem(HttpContext, a3AppPrelMatDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in PrelMatController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in PrelMatController: " + ex.Message);
            }
        }

        /// <summary>
        /// Ritorna la lista di A3AppPrelMat filtrati per componente
        /// Questa API è utilizzata per recuperare le quantità del componente specificato che sono già state salvate.
        /// Con queste viene poi effettuata una somma nel Frontend per specificare all'utente la quantità totale salvata fin'ora.
        /// </summary>
        /// <param name="a3AppPrelMatRequestDto">Identificativo del componente da cercare</param>
        /// <returns>Ritorna la lista delle richieste filtrate per componente</returns>
        [HttpPost("get_prel_mat_with_component")]
        public IActionResult GetPrelMatWithComponent([FromBody] ComponentRequestDto? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            try
            {
                var a3AppPrelMatDto = _prelMatRequestService.GetPrelMatWithComponent(a3AppPrelMatRequestDto).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in PrelMatController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in PrelMatController: " + ex.Message);
            }
        }

        [HttpGet("view_prel_mat/not_imported")]
        /// <summary>
        /// Ritorna la lista di A3AppPrelMat non importati
        /// /// </summary>
        /// <response code="200">Ritorna la lista di A3AppPrelMat non importati</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetNotImportedPrelMat()
        {
            try
            {
                var a3AppPrelMatDto = _prelMatRequestService.GetNotImportedPrelMat().ToList();

                return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in PrelMatController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in PrelMatController: " + ex.Message);
            }
        }

        [HttpPost("view_prel_mat/not_imported/filtered")]
        /// <summary>
        /// Ritorna la lista di A3AppPrelMat non importati in base al filtro passato
        /// /// </summary>
        /// <response code="200">Ritorna la lista di A3AppPrelMat non importati in base al filtro passato</response>
        /// <response code="404">Non trovato</response>
        /// <response code="400">Bad Request</response>
        public IActionResult GetNotImportedPrelMatWithFilter(ViewPrelMatRequestDto request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            try
            {
                var a3AppPrelMatDto = _prelMatRequestService.GetNotImportedAppPrelMatByFilter(request).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in PrelMatController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in PrelMatController: " + ex.Message);
            }
        }
    }
}