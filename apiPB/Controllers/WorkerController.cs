using Microsoft.AspNetCore.Mvc;
using apiPB.Mappers.Dto;
using apiPB.Dto.Request;
using apiPB.Services;
using Microsoft.IdentityModel.Tokens;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Services.Abstraction;
using Microsoft.AspNetCore.Authorization;
using apiPB.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/worker")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IWorkersRequestService _workerRequestService;
        private readonly bool _isLogActive;
        
        public WorkerController(IResponseHandler responseHandler, IWorkersRequestService workersRequestService
        )
        {
            _responseHandler = responseHandler;
            _workerRequestService = workersRequestService;
            _isLogActive = false;
        }

        [HttpGet]
        /// <summary>
        /// Ritorna tutti i VwWorkers presenti nella vista del database
        /// </summary>
        /// <response code="200">Ritorna tutti i VwWorkers</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetAllWorkers()
        {
            var workersDto = _workerRequestService.GetWorkers().ToList();

            if (workersDto == null || workersDto.Count == 0) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, workersDto, _isLogActive);
        }

        /// <summary>
        /// Verifica le credenziali inviate tramite Basic Authentication e recupera le informazioni del lavoratore autenticato.
        /// </summary>
        /// <returns>WorkerDto; 200 OK se le credenziali sono corrette, altrimenti 404 Not Found.</returns>
        [HttpPost("login")]
        public IActionResult LoginWithPassword([FromBody] PasswordWorkersRequestDto? passwordWorkersRequestDto)
        {
            if (passwordWorkersRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            try
            {
                var workerDto = _workerRequestService.LoginWithPassword(passwordWorkersRequestDto);

                return _responseHandler.HandleOkAndItem(HttpContext, workerDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, $"Il servizio ritorna null in WorkerController: {ex.Message}");
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, $"Errore durante l'esecuzione del Service in WorkerController: {ex.Message}");
            }
        }
    }
}