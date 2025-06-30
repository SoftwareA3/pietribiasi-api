using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;
using apiPB.Utils.Implementation;

namespace apiPB.Controllers
{
    [Route("api/action_message")]
    [Authorize]
    [ApiController]
    public class ActionMessageController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IActionMessageRequestService _actionMessageRequestService;
        private readonly bool _isLogActive;

        public ActionMessageController(IResponseHandler responseHandler, IActionMessageRequestService actionMessageRequestService)
        {
            _responseHandler = responseHandler;
            _actionMessageRequestService = actionMessageRequestService;
            _isLogActive = false;
        }

        [HttpPost("get_action_messages_filtered")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_om_action_messages filtrate in base ai parametri passati
        /// </summary>
        /// <param name="ImportedLogMessageDto">DTO per la richiesta di importazione dei messaggi di log</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_om_action_messages</response>
        /// <response code="204">Nessun messaggio di azione trovato con i filtri specificati</response>
        /// <response code="400">Richiesta non valida</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetActionMessagesByFilter([FromBody] ImportedLogMessageDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            try
            {
                var actionMessagesDto = _actionMessageRequestService.GetActionMessagesByFilter(request); 

                if (actionMessagesDto == null)
                {
                    return _responseHandler.HandleNoContent(HttpContext, _isLogActive, "Nessun messaggio di azione trovato con i filtri specificati.");
                }

                return _responseHandler.HandleOkAndItem(HttpContext, actionMessagesDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in ActionMessageController" + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in ActionMessageController" + ex.Message);
            }
        }
    }
}