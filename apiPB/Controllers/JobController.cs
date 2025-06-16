using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/job")]
    [Authorize]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IJobRequestService _jobRequestService;
        private readonly bool _isLogActive;

        public JobController(IResponseHandler responseHandler, IJobRequestService jobRequestService)
        {
            _responseHandler = responseHandler;
            _jobRequestService = jobRequestService;
            _isLogActive = false;
        }

        [HttpGet]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_jobs
        /// </summary>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_jobs</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiJobs()
        {
            try
            {
                var jobsDto = _jobRequestService.GetJobs().ToList();

                return _responseHandler.HandleOkAndList(HttpContext, jobsDto, _isLogActive);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio ritorna null in JobController: " + ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Il servizio non ha trovato dati in JobController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, _isLogActive, "Errore durante l'esecuzione del Service in JobController: " + ex.Message);
            }  
        }
    }    
}