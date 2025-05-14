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
            var a3AppPrelMatDto = _prelMatRequestService.GetAppPrelMat().ToList();

            if (a3AppPrelMatDto == null || !a3AppPrelMatDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
        }

        [HttpPost("post_prel_mat")]
        /// <summary>
        /// Invia la lista di A3AppPrelMat al database
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="201">Crea delle entry nel database</response>
        /// <response code="404">Non trovato</response>
        public IActionResult PostPrelMatList([FromBody] IEnumerable<PrelMatRequestDto>? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null || !a3AppPrelMatRequestDto.Any()) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var a3AppPrelMatDto = _prelMatRequestService.PostPrelMatList(a3AppPrelMatRequestDto).ToList();

            if (a3AppPrelMatDto == null || !a3AppPrelMatDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
        }

        [HttpPost("get_view_prel_mat")]
        /// <summary>
        /// Ritorna le informazioni della vista A3_app_prel_mat filtrate in base ai parametri opzionali inseriti
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Collezione contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna le informazioni della vista A3_app_prel_mat filtrate in base ai parametri opzionali inseriti</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetViewPrelMat([FromBody] ViewPrelMatRequestDto? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);
            
            var a3AppPrelMatDto = _prelMatRequestService.GetViewPrelMatList(a3AppPrelMatRequestDto).ToList();

            if (a3AppPrelMatDto == null || !a3AppPrelMatDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
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
            
            var a3AppPrelMatDto = _prelMatRequestService.PutViewPrelMat(a3AppPrelMatRequestDto);

            if (a3AppPrelMatDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndItem(HttpContext, a3AppPrelMatDto, _isLogActive);
        }

        [HttpDelete("view_prel_mat/delete_prel_mat_id")]
        /// <summary>
        /// Elimina la riga della tabella A3_app_prel_mat in base al filtro passato
        /// </summary>
        /// <param name="IEnumerable<a3AppPrelMatRequestDto>">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna l'elemento eliminato dalla tabella</response>
        /// <response code="404">Non trovato</response>
        public IActionResult DeletePrelMatId([FromBody] ViewPrelMatDeleteRequestDto? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var a3AppPrelMatDto = _prelMatRequestService.DeletePrelMatId(a3AppPrelMatRequestDto);

            if (a3AppPrelMatDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndItem(HttpContext, a3AppPrelMatDto, _isLogActive);
        }

        [HttpPost("get_prel_mat_with_moid")]
        public IActionResult GetPrelMatWithMoId([FromBody] MoidRequestDto? a3AppPrelMatRequestDto)
        {
            if (a3AppPrelMatRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var a3AppPrelMatDto = _prelMatRequestService.GetPrelMatWithMoId(a3AppPrelMatRequestDto).ToList();

            if (a3AppPrelMatDto == null || !a3AppPrelMatDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, a3AppPrelMatDto, _isLogActive);
        }
    }
}