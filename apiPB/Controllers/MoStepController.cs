using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;

namespace apiPB.Controllers
{
    [Route("api/mostep")]
    [Authorize]
    [ApiController]
    public class MoStepController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IMoStepRequestService _moStepRequestService;
        private readonly bool _isLogActive;
        public MoStepController(IResponseHandler responseHandler, IMoStepRequestService moStepRequestService)
        {
            _responseHandler = responseHandler;
            _moStepRequestService = moStepRequestService;
            _isLogActive = false;
        }

        [HttpPost("job")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetVwApiMostepWithJob([FromBody] JobRequestDto? mostepRequestDto)
        {
            if(mostepRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var mostepDto = _moStepRequestService.GetMostepWithJob(mostepRequestDto).ToList();

            if(mostepDto == null || !mostepDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, mostepDto, _isLogActive);
        }

        [HttpPost("odp")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// </summary>
        /// <param name="mostepMonoRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepWithMono([FromBody] MonoRequestDto? mostepMonoRequestDto)
        {
            if(mostepMonoRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var mostepDto = _moStepRequestService.GetMostepWithMono(mostepMonoRequestDto).ToList();

            if(mostepDto == null || !mostepDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, mostepDto, _isLogActive);
        }

        [HttpPost("lavorazioni")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mostep
        /// /// </summary>
        /// <param name="mostepLavorazioniRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mostep</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepWithOperation([FromBody] OperationRequestDto? mostepOperationRequestDto)
        {
            if(mostepOperationRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext, _isLogActive);

            var mostepDto = _moStepRequestService.GetMostepWithOperation(mostepOperationRequestDto).ToList();

            if(mostepDto == null || !mostepDto.Any()) return _responseHandler.HandleNotFound(HttpContext, _isLogActive);

            return _responseHandler.HandleOkAndList(HttpContext, mostepDto, _isLogActive);
        }
    }
}