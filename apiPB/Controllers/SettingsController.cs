using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using apiPB.Services;
using apiPB.Dto.Request;
using apiPB.Services.Abstraction;
using System.Threading.Tasks;
using apiPB.Utils.Abstraction;
using apiPB.Dto.Models;

namespace apiPB.Controllers
{
    [Route("api/settings")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRequestService _settingsService;
        private readonly IResponseHandler _responseHandler;
        private readonly bool _isLogActive = false;

        public SettingsController(ISettingsRequestService settingsService, IResponseHandler responseHandler)
        {
            _settingsService = settingsService;
            _responseHandler = responseHandler;
        }

        /// <summary>
        /// Ritorna le informazioni della tabella A3AppSettings
        /// </summary>
        /// <response code="200">Ritorna le informazioni della tabella A3AppSettings</response>
        /// <response code="404">Non trovato</response>
        [HttpGet("get_settings")]
        [Authorize]
        public IActionResult GetSettings()
        {
            var settingsDto = _settingsService.GetSettings();
            if (settingsDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);
            return _responseHandler.HandleOkAndItem(HttpContext, settingsDto, _isLogActive);
        }

        /// <summary>
        /// Modifica le informazioni della tabella A3AppSettings
        /// </summary>
        /// <param name="request">Richiesta di modifica delle impostazioni</param>
        /// <response code="200">Modifica delle impostazioni effettuata con successo</response>
        /// <response code="400">Richiesta di modifica delle impostazioni non valida</response>
        /// <response code="404">Modifica delle impostazioni non riuscita</response>
        /// <returns>Informazioni di risposta: ritorna le informazioni che sono state modificate</returns>
        [HttpPost("edit_settings")]
        [Authorize]
        public IActionResult EditSettings([FromBody] SettingsDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Richiesta di modifica delle impostazioni non valida");

            var settingsDto = _settingsService.EditSettings(request);
            if (settingsDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Modifica delle impostazioni non riuscita");

            return _responseHandler.HandleOkAndItem(HttpContext, settingsDto, _isLogActive, "Impostazioni modificate con successo");
        }

        [HttpGet("get_sync_global_active")]
        [Authorize]
        public IActionResult GetSyncGlobalActive()
        {
            var syncGlobalActiveDto = _settingsService.GetSyncGlobalActive();
            if (syncGlobalActiveDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);
            return _responseHandler.HandleOkAndItem(HttpContext, syncGlobalActiveDto, _isLogActive);
        }
    }
}