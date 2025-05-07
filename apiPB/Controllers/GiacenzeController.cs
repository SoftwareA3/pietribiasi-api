using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Request.Abstraction;
using apiPB.Services.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/giacenze")]
    [Authorize]
    [ApiController]
    public class GiacenzeController : ControllerBase
    {
        private readonly ILogService _logService;

        private readonly IGiacenzeRequestService _giacenzeRequestService;

        private readonly bool _logIsActive;

        public GiacenzeController(ILogService logService, IGiacenzeRequestService giacenzeRequestService)
        {
            _logService = logService;
            _giacenzeRequestService = giacenzeRequestService;
            _logIsActive = false;
        }

        [HttpGet("get_all")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_giacenze
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_giacenze</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetGiacenze()
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var giacenzeDto = _giacenzeRequestService.GetGiacenze().ToList();

            if (giacenzeDto.IsNullOrEmpty())
            {
                _logService.AppendMessageToLog(requestPath, NotFound().StatusCode, "Not Found", _logIsActive);

                return NotFound();
            }

            _logService.AppendMessageAndListToLog(requestPath, Ok().StatusCode, "OK", giacenzeDto, _logIsActive);

            return Ok(giacenzeDto);
        }
    }
}