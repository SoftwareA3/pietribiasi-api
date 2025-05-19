using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/ommessage")]
    [Authorize]
    [ApiController]
    public class OmmessageController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IOmmessageRequestService _ommessageRequestService;
        private readonly bool _isLogActive;

        public OmmessageController(IResponseHandler responseHandler, IOmmessageRequestService ommessageRequestService)
        {
            _responseHandler = responseHandler;
            _ommessageRequestService = ommessageRequestService;
            _isLogActive = false;
        }

        [HttpPost("get_log_from_moid")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mo_steps_components filtrate per MoId
        /// </summary>
        /// <param name="ommessageInfoRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mo_steps_components</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetOmmessageGroupedByMoId([FromBody] MoIdRequestDto moIdRequestDto)
        {
            if(moIdRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var ommessageInfoRequestDto = _ommessageRequestService.GetOmmessageGroupedByMoid(moIdRequestDto).ToList();

            if (ommessageInfoRequestDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, ommessageInfoRequestDto, _isLogActive);
        }
    }
}