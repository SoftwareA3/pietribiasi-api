using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;

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
        /// <param name="ImportedLogMessageDto">Collezione contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista A3_app_inventario</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetActionMessagesByFilter([FromBody] ImportedLogMessageDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var actionMessagesDto = _actionMessageRequestService.GetActionMessagesByFilter(request);

            if (actionMessagesDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndItem(HttpContext, actionMessagesDto, _isLogActive);
        }
    }
}