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
        [HttpPost("synchronize_all")]
        [Authorize]
        public async Task<IActionResult> Syncronize([FromBody] WorkerIdSyncRequestDto? request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Dati della sincronizzazione non validi");

            var settingsAndResponseTuple = await _magoRequestService.LoginWithWorkerIdAsync(request);
            if (settingsAndResponseTuple.LoginResponse == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Login non riuscito durante la sincronizzazione");
            if (settingsAndResponseTuple.Settings == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Impostazioni non trovate durante la sincronizzazione");

            try
            {
                var response = await _magoRequestService.SyncronizeAsync(settingsAndResponseTuple.LoginResponse, settingsAndResponseTuple.Settings, request);
                return _responseHandler.HandleOkAndItem(HttpContext, response, _isLogActive, "Sincronizzazione effettuata con successo");
            }
            catch (Exception e)
            {
                return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Sincronizzazione non riuscita" + e.Message);
            }
            finally
            {
                var loginResult = settingsAndResponseTuple.LoginResponse.Token;
                if (loginResult != null)
                {
                    await _magoRequestService.LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = loginResult
                    });
                }
            }
        }

        /// <summary>
        /// Sincronizza le ore registrate salvate con Mago, filtrate
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione delle ore registrate filtrate</param>
        /// <param name="workerId">ID dell'utente che ha effettuato la richiesta</param>
        /// <response code="200">Sincronizzazione delle ore registrate filtrate effettuata con successo</response>
        /// <response code="400">Richiesta di sincronizzazione delle ore registrate filtrate non valida</response>
        /// <response code="404">Sincronizzazione delle ore registrate filtrate non riuscita</response>
        /// <returns>Informazioni di risposta: ritorna le ore registrate filtrate sincronizzate</returns>
        [HttpPost("sync_reg_ore_filtered")]
        [Authorize]
        public async Task<IActionResult> SyncRegOreFiltered([FromBody] SyncRegOreFilteredDto request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Richiesta di sincronizzazione delle ore registrate filtrate non valida");

            var settingsAndResponseTuple = await _magoRequestService.LoginWithWorkerIdAsync(request.WorkerIdSyncRequestDto);
            if (settingsAndResponseTuple.LoginResponse == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Login non riuscito durante la sincronizzazione delle ore registrate filtrate");
            if (settingsAndResponseTuple.Settings == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Impostazioni non trovate durante la sincronizzazione");

            try
            {
                var response = await _magoRequestService.SyncRegOreFiltered(settingsAndResponseTuple.LoginResponse, settingsAndResponseTuple.Settings, request);
                return _responseHandler.HandleOkAndItem(HttpContext, response, _isLogActive, "Sincronizzazione delle ore registrate filtrate effettuata con successo");
            }
            catch (Exception e)
            {
                return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Sincronizzazione delle ore registrate filtrate non riuscita: " + e.Message);
            }
            finally
            {
                var loginResult = settingsAndResponseTuple.LoginResponse.Token;
                if (loginResult != null)
                {
                    await _magoRequestService.LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = loginResult
                    });
                }
            }
        }

        /// <summary>
        /// Sincronizza i materiali prelevati salvati con Mago, filtrati
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione dei materiali prelevati filtrati</param>
        /// <param name="workerId">ID dell'utente che ha effettuato la richiesta</param>
        /// <response code="200">Sincronizzazione dei materiali prelevati filtrati effettuata con successo</response>
        /// <response code="400">Richiesta di sincronizzazione dei materiali prelevati filtrati non valida</response>
        /// <response code="404">Sincronizzazione dei materiali prelevati filtrati non riuscita</response>
        /// <returns>Informazioni di risposta: ritorna i materiali prelevati filtrati sincronizzati</returns>
        [HttpPost("sync_prel_mat_filtered")]
        [Authorize]
        public async Task<IActionResult> SyncPrelMatFiltered([FromBody] SyncPrelMatFilteredDto request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Richiesta di sincronizzazione dei materiali prelevati filtrati non valida");

            var settingsAndResponseTuple = await _magoRequestService.LoginWithWorkerIdAsync(request.WorkerIdSyncRequestDto);
            if (settingsAndResponseTuple.LoginResponse == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Login non riuscito durante la sincronizzazione dei materiali prelevati filtrati");
            if (settingsAndResponseTuple.Settings == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Impostazioni non trovate durante la sincronizzazione");

            try
            {
                var response = await _magoRequestService.SyncPrelMatFiltered(settingsAndResponseTuple.LoginResponse, settingsAndResponseTuple.Settings, request);
                return _responseHandler.HandleOkAndItem(HttpContext, response, _isLogActive, "Sincronizzazione dei materiali prelevati filtrati effettuata con successo");
            }
            catch (Exception e)
            {
                return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Sincronizzazione dei materiali prelevati filtrati non riuscita: " + e.Message);
            }
            finally
            {
                var loginResult = settingsAndResponseTuple.LoginResponse.Token;
                if (loginResult != null)
                {
                    await _magoRequestService.LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = loginResult
                    });
                }
            }
        }

        /// <summary>
        /// Sincronizza le movimentazioni di inventario salvate con Mago, filtrate
        /// </summary>
        /// <param name="request">Richiesta di sincronizzazione delle movimentazioni di inventario filtrate</param>
        /// <param name="workerId">ID dell'utente che ha effettuato la richiesta</param>
        /// <response code="200">Sincronizzazione delle movimentazioni di inventario filtrate effettuata con successo</response>
        /// <response code="400">Richiesta di sincronizzazione delle movimentazioni di inventario filtrate non valida</response>
        /// <response code="404">Sincronizzazione delle movimentazioni di inventario filtrate non riuscita</response>
        /// <returns>Informazioni di risposta: ritorna le movimentazioni di inventario filtrate sincronizzate</returns>
        [HttpPost("sync_inventario_filtered")]
        [Authorize]
        public async Task<IActionResult> SyncInventarioFiltered([FromBody] SyncInventarioFilteredDto request)
        {
            if (request == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Richiesta di sincronizzazione delle movimentazioni di inventario filtrate non valida");
            if (request.WorkerIdSyncRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "WorkerIdSyncRequestDto non valido");

            var settingsAndResponseTuple = await _magoRequestService.LoginWithWorkerIdAsync(request.WorkerIdSyncRequestDto);
            if (settingsAndResponseTuple.LoginResponse == null) return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Login non riuscito durante la sincronizzazione delle movimentazioni di inventario filtrate");
            if (settingsAndResponseTuple.Settings == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Impostazioni non trovate durante la sincronizzazione");

            try
            {
                var response = await _magoRequestService.SyncInventarioFiltered(settingsAndResponseTuple.LoginResponse, settingsAndResponseTuple.Settings, request);
                return _responseHandler.HandleOkAndItem(HttpContext, response, _isLogActive, "Sincronizzazione delle movimentazioni di inventario filtrate effettuata con successo");
            }
            catch (Exception e)
            {
                return _responseHandler.HandleBadRequest(HttpContext, _isLogActive, "Sincronizzazione delle movimentazioni di inventario filtrate non riuscita: " + e.Message);
            }
            finally
            {
                var loginResult = settingsAndResponseTuple.LoginResponse.Token;
                if (loginResult != null)
                {
                    await _magoRequestService.LogoffAsync(new MagoTokenRequestDto
                    {
                        Token = loginResult
                    });
                }
            }
        }
    }
}