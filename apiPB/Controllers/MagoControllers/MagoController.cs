using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using apiPB.Services;
using apiPB.Dto.Request;
using apiPB.Services.Abstraction;
using System.Threading.Tasks;
using apiPB.Utils.Abstraction;
using apiPB.Dto.Models;

namespace apiPB.MagoApi.Controllers
{
    [Route("api/mago_api")]
    [ApiController]
    public class MagoController : ControllerBase
    {
        private readonly IMagoAccessService _magoAccessService;
        private readonly IMagoRequestService _magoRequestService;   
        private readonly IResponseHandler _responseHandler;

        public MagoController( 
            IMagoAccessService magoAccessService,
            IMagoRequestService magoRequestService,
            IResponseHandler responseHandler)
        {
            _magoAccessService = magoAccessService;
            _magoRequestService = magoRequestService;
            _responseHandler = responseHandler;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] MagoLoginRequestDto? magoLoginRequestDto)
        {
            if (magoLoginRequestDto == null) return BadRequest("Invalid request");

            var magoIdDto = await _magoAccessService.LoginAsync(magoLoginRequestDto);

            if (magoIdDto == null) return NotFound("MagoId not found");

            return Ok(magoIdDto);
        }

        [HttpPost("logoff")]
        public async Task<IActionResult> LogOff([FromBody] MagoTokenRequestDto tokenRequestDto)
        {
            if(tokenRequestDto == null) return BadRequest("Invalid request");

            try
            {
                await _magoAccessService.LogoffAsync(tokenRequestDto);
                return Ok("Logoff successful");
            }
            catch (Exception ex)
            {
                return BadRequest($"Logoff failed: {ex.Message}");
            }
        }

        [HttpPost("synchronize")]
        [Authorize]
        public async Task<IActionResult> Syncronize([FromBody] WorkerIdSyncRequestDto? request)
        {
            if (request == null) return BadRequest("Invalid request");

            try
            {
                await _magoRequestService.SyncronizeAsync(request);
                return Ok("Synchronization successful");
            }
            catch (Exception ex)
            {
                return BadRequest($"Synchronization failed: {ex.Message}");
            }
        }

        [HttpGet("get_settings")]
        [Authorize]
        public IActionResult GetSettings()
        {
            var settingsDto = _magoRequestService.GetSettings();
            if(settingsDto == null) return _responseHandler.HandleNotFound(HttpContext, false);
            return _responseHandler.HandleOkAndItem(HttpContext, settingsDto, false);
        }

        [HttpPost("edit_settings")]
        [Authorize]
        public IActionResult EditSettings([FromBody] SettingsDto? request)
        {
            if (request == null) return BadRequest("Invalid request");

            var settingsDto = _magoRequestService.EditSettings(request);
            if (settingsDto == null) return _responseHandler.HandleNotFound(HttpContext, false);

            return _responseHandler.HandleOkAndItem(HttpContext, settingsDto, false);
        }
    }
}