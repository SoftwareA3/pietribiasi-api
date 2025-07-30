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
    [Authorize]
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
        public IActionResult GetSettings()
        {
            try
            {
                var settingsDto = _settingsService.GetSettings();
                return _responseHandler.HandleOkAndItem(HttpContext, settingsDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in SettingsController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in SettingsController: " + ex.Message);
            }
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
        public IActionResult EditSettings([FromBody] SettingsDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Richiesta di modifica delle impostazioni non valida");

            try
            {
                var settingsDto = _settingsService.EditSettings(request);

                return _responseHandler.HandleOkAndItem(HttpContext, settingsDto, _isLogActive, "Impostazioni modificate con successo");
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in SettingsController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in SettingsController: " + ex.Message);
            }
        }

        /// <summary>
        /// Ritorna le informazioni sulla sincronizzazione globale attiva
        /// Questa API è utilizzata per verificare se un utente non amministratore 
        /// ha la possibilità di visualizzare la pagina di sincronizzazione dalla home.
        /// </summary>
        /// <response code="200">Ritorna le informazioni della tabella A3AppSyncGlobalActive</response>
        /// <response code="404">Non trovato</response>
        [HttpGet("get_sync_global_active")]
        public IActionResult GetSyncGlobalActive()
        {
            try
            {
                var syncGlobalActiveDto = _settingsService.GetSyncGlobalActive();
                return _responseHandler.HandleOkAndItem(HttpContext, syncGlobalActiveDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in SettingsController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in SettingsController: " + ex.Message);
            }
        }

        /// <summary>
        /// Ritorna le informazioni sulla terminazione delle lavorazioni utente
        /// Questa API è utilizzata per verificare se un utente non amministratore
        /// ha la possibilità di terminare le lavorazioni utente.
        /// </summary>
        /// <response code="200">Ritorna le informazioni della colonna TerminaLavorazioniUtente nella tabella Settings</response>
        /// <response code="404">Non trovato</response>
        /// <returns>Informazioni sulla terminazione delle lavorazioni utente</returns>
        [HttpGet("get_termina_lavorazioni_utente")]
        public IActionResult GetTerminaLavorazioniUtente()
        {
            try
            {
                var terminaLavorazioniUtenteDto = _settingsService.GetTerminaLavorazioniUtente();
                return _responseHandler.HandleOkAndItem(HttpContext, terminaLavorazioniUtenteDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in SettingsController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in SettingsController: " + ex.Message);
            }
        }
    }
}