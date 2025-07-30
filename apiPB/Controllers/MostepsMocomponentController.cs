using Microsoft.AspNetCore.Mvc;
using apiPB.Services;
using Microsoft.AspNetCore.Authorization;
using apiPB.Dto.Request;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;
using apiPB.Utils.Implementation;

namespace apiPB.Controllers
{
    [Route("api/mostepsmocomponent")]
    [Authorize]
    [ApiController]
    public class MostepsMocomponentController : ControllerBase
    {
        private readonly IResponseHandler _responseHandler;
        private readonly IMostepsMocomponentRequestService _mostepsMocomponentRequestService;

        public MostepsMocomponentController(IResponseHandler responseHandler, IMostepsMocomponentRequestService mostepsMocomponentRequestService)
        {
            _responseHandler = responseHandler;
            _mostepsMocomponentRequestService = mostepsMocomponentRequestService;
        }

        [HttpPost("job")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mosteps_mocomponents
        /// </summary>
        /// <param name="mostepsMocomponentJobRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mosteps_mocomponents</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithJob([FromBody] JobRequestDto? mostepsMocomponentJobRequestDto)
        {
            if (mostepsMocomponentJobRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext);

            try
            {
                var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentJobDistinct(mostepsMocomponentJobRequestDto).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, mostepComponentDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in MostepsMocomponentController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio non ha trovato dati in MostepsMocomponentController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in MostepsMocomponentController: " + ex.Message);
            }
        }

        [HttpPost("mono")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mosteps_mocomponents
        /// </summary>
        /// <param name="mostepsMocomponentMonoRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mosteps_mocomponents</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithMono([FromBody] MonoRequestDto? mostepsMocomponentMonoRequestDto)
        {
            if (mostepsMocomponentMonoRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext);

            try
            {
                var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentMonoDistinct(mostepsMocomponentMonoRequestDto).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, mostepComponentDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in MostepsMocomponentController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio non ha trovato dati in MostepsMocomponentController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in MostepsMocomponentController: " + ex.Message);
            }
        }


        [HttpPost("operation")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mosteps_mocomponents 
        /// /// </summary>
        /// <param name="mostepsMocomponentOperationRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mosteps_mocomponents</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithOperation([FromBody] OperationRequestDto? mostepsMocomponentOperationRequestDto)
        {
            if (mostepsMocomponentOperationRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext);

            try
            {
                var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentOperationDistinct(mostepsMocomponentOperationRequestDto).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, mostepComponentDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in MostepsMocomponentController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio non ha trovato dati in MostepsMocomponentController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in MostepsMocomponentController: " + ex.Message);
            }
        }

        [HttpPost("component_barcode")]
        /// <summary>
        /// Ritorna tutte le informazioni della vista vw_api_mosteps_mocomponents
        /// </summary>
        /// <param name="mostepsMocomponentBarCodeRequestDto">Oggetto contenente i parametri di ricerca</param>
        /// <response code="200">Ritorna tutte le informazioni della vista vw_api_mosteps_mocomponents</response>
        /// <response code="404">Non trovato</response>
        public IActionResult GetMostepsMocomponentWithBarCode([FromBody] BarCodeRequestDto? mostepsMocomponentBarCodeRequestDto)
        {
            if (mostepsMocomponentBarCodeRequestDto == null) return _responseHandler.HandleBadRequest(HttpContext);

            try
            {
                var mostepComponentDto = _mostepsMocomponentRequestService.GetMostepsMocomponentBarCodeDistinct(mostepsMocomponentBarCodeRequestDto).ToList();

                return _responseHandler.HandleOkAndList(HttpContext, mostepComponentDto);
            }
            catch (ArgumentNullException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio ritorna null in MostepsMocomponentController: " + ex.Message);
            }
            catch (EmptyListException ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Il servizio non ha trovato dati in MostepsMocomponentController: " + ex.Message);
            }
            catch (Exception ex)
            {
                return _responseHandler.HandleNotFound(HttpContext, "Errore durante l'esecuzione del Service in MostepsMocomponentController: " + ex.Message);
            }
        }
    }
}