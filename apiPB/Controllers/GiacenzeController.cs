using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;
using apiPB.Dto.Models;
using apiPB.Utils.Implementation;

namespace apiPB.Controllers
{
    [Route("api/giacenze")]
    [Authorize]
    [ApiController]
    public class GiacenzeController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;

        private readonly IGiacenzeRequestService _giacenzeRequestService;

        private readonly bool _isLogActive;

        public GiacenzeController(IResponseHandler responseHandler, IGiacenzeRequestService giacenzeRequestService)
        {
            _responseHandler = responseHandler;
            _giacenzeRequestService = giacenzeRequestService;
            _isLogActive = false;
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

                return _responseHandler.HandleOkAndList<GiacenzeDto>(HttpContext, giacenzeDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in GiacenzeController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna una lista vuota in GiacenzeController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in GiacenzeController: " + ex.Message);
            }
        }
    }
}