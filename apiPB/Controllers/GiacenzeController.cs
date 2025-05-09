using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Request.Abstraction;
using apiPB.Services.Utils.Abstraction;
using apiPB.Dto.Models;

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
            var giacenzeDto = _giacenzeRequestService.GetGiacenze().ToList();

            if (giacenzeDto.IsNullOrEmpty()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList<GiacenzeDto>(HttpContext, giacenzeDto, _isLogActive);
        }
    }
}