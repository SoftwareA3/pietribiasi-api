using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;
using apiPB.Dto.Models;
using apiPB.Utils.Implementation;
using apiPB.Dto.Request;

namespace apiPB.Controllers
{
    [Route("api/giacenze")]
    [Authorize]
    [ApiController]
    public class GiacenzeController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;

        private readonly IGiacenzeRequestService _giacenzeRequestService;


        public GiacenzeController(IResponseHandler responseHandler, IGiacenzeRequestService giacenzeRequestService)
        {
            _responseHandler = responseHandler;
            _giacenzeRequestService = giacenzeRequestService;
        }

        [HttpGet("get_all")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_giacenze
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_giacenze</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetGiacenze()
        {
            try
            {
                var giacenzeDto = _giacenzeRequestService.GetGiacenze().ToList();

                return _responseHandler.HandleOkAndList<GiacenzeDto>(HttpContext, giacenzeDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in GiacenzeController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna una lista vuota in GiacenzeController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in GiacenzeController: " + ex.Message);
            }
        }

        [HttpPost("get_by_item")]
        /// <summary>
        /// Ritorna le informazioni della vista vw_api_giacenze filtrate per un componente
        /// </summary>
        /// <param name="request">Il componente da filtrare</param>
        /// <response code="200">Ritorna le informazioni della vista vw_api_giacenze filtrate per un componente</response>
        /// <response code="400">Richiesta non valida</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetGiacenzeByItem([FromBody] ComponentRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Component))
                {
                    return _responseHandler.HandleBadRequest(HttpContext, "Richiesta non valida: il componente non può essere nullo o vuoto.");
                }

                var giacenzaDto = _giacenzeRequestService.GetGiacenzeByItem(request);

                return _responseHandler.HandleOkAndItem<GiacenzeDto>(HttpContext, giacenzaDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in GiacenzeController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in GiacenzeController: " + ex.Message);
            }
        }
    }
}