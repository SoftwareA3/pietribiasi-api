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
        private readonly IMagoRequestService _magoRequestService;
        private readonly IResponseHandler _responseHandler;
        private readonly bool _isLogActive = false;

        public MagoController(
            IMagoRequestService magoRequestService,
            IResponseHandler responseHandler)
        {
            _magoRequestService = magoRequestService;
            _responseHandler = responseHandler;
        }

        /// <summary>
        /// Effettua il login a Mago
        /// </summary>
        /// <param name="magoLoginRequestDto">Credenziali per il login</param>
        /// <response code="200">Login effettuato con successo</response>
        /// <response code="400">Richiesta di login non valida</response>
        /// <response code="404">Login non riuscito</response>
        /// <returns>Token di accesso e informazioni di risposta</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] MagoLoginRequestDto? magoLoginRequestDto)
        {
            if (magoLoginRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Richiesta di login non valida");

            var magoIdDto = await _magoRequestService.LoginAsync(magoLoginRequestDto);

            if (magoIdDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Login non riuscito");

            return _responseHandler.HandleOkAndItem(HttpContext, magoIdDto, _isLogActive, "Login effettuato con successo");
        }

        /// <summary>
        /// Effettua il logoff da Mago
        /// </summary>
        /// <param name="tokenRequestDto">Token di accesso</param>
        /// <response code="200">Logoff effettuato con successo</response>
        /// <response code="400">Richiesta di logoff non valida</response>
        /// <response code="404">Logoff non riuscito</response>
        /// <returns>Informazioni di risposta</returns>
        [HttpPost("logoff")]
        public async Task<IActionResult> LogOff([FromBody] MagoTokenRequestDto tokenRequestDto)
        {
            if (tokenRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Token non valido");

            try
            {
                await _magoRequestService.LogoffAsync(tokenRequestDto);
                return _responseHandler.HandleOk(HttpContext, _isLogActive, "Logoff effettuato con successo");
            }
            catch (Exception)
            {

                return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Logoff non riuscito");
            }
        }

        /// <summary>
        /// Sincronizza le informazioni salvate con Mago
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione. Contiene l'ID dell'utente che ha effettuato la richiesta</param>
        /// <response code="200">Sincronizzazione effettuata con successo</response>
        /// <response code="400">Richiesta di sincronizzazione non valida</response>
        /// <response code="404">Sincronizzazione non riuscita</response>
        /// <returns>Informazioni di risposta: ritorna le informazioni che sono state sincronizzate</returns>
        [HttpPost("synchronize")]
        [Authorize]
        public async Task<IActionResult> Syncronize([FromBody] WorkerIdSyncRequestDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Dati della sincronizzazione non validi");

            try
            {
                var response = await _magoRequestService.SyncronizeAsync(request);
                return _responseHandler.HandleOkAndItem(HttpContext, response, _isLogActive, "Sincronizzazione effettuata con successo");
            }
            catch (Exception)
            {
                return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Sincronizzazione non riuscita");
            }
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
            var settingsDto = _magoRequestService.GetSettings();
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

            var settingsDto = _magoRequestService.EditSettings(request);
            if (settingsDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Modifica delle impostazioni non riuscita");

            return _responseHandler.HandleOkAndItem(HttpContext, settingsDto, _isLogActive, "Impostazioni modificate con successo");
        }

        [HttpGet("get_sync_global_active")]
        [Authorize]
        public IActionResult GetSyncGlobalActive()
        {
            var syncGlobalActiveDto = _magoRequestService.GetSyncGlobalActive();
            if (syncGlobalActiveDto == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);
            return _responseHandler.HandleOkAndItem(HttpContext, syncGlobalActiveDto, _isLogActive);
        }
    }
}