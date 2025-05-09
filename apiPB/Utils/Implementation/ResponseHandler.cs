using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Utils.Abstraction;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace apiPB.Utils.Implementation
{
    public class ResponseHandler : IResponseHandler
    {
        private readonly ILogService _logService;

        public ResponseHandler(ILogService logService)
        {
            _logService = logService;
        }

        // Stringa necessaria per il log: inserisce il nome del metodo e il percorso della richiesta
        // Esempio: GET api/job
        private string BuildHttpContextString(HttpContext httpContext)
        {
            return $"{httpContext.Request.Method ?? string.Empty} {httpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";
        }

        public BadRequestObjectResult HandleBadRequest(HttpContext httpContext, bool isLogActive)
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageToLog(requestPath, StatusCodes.Status400BadRequest, "La richiesta non può essere vuota", isLogActive);
            return new BadRequestObjectResult("La richiesta non può essere vuota.");
        }

        public NotFoundObjectResult HandleNotFound(HttpContext httpContext, bool isLogActive)
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageToLog(requestPath, StatusCodes.Status404NotFound, "Nessun Risultato trovato", isLogActive);
            return new NotFoundObjectResult("Non risultato trovato.");
        }

        public OkObjectResult HandleOkAndItem<T>(HttpContext httpContext, T item, bool isLogActive)
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageAndItemToLog(requestPath, StatusCodes.Status200OK, "Ok", item, isLogActive);
            return new OkObjectResult(item);
        }

        public OkObjectResult HandleOkAndList<T>(HttpContext httpContext, List<T> itemList, bool isLogActive)
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageAndListToLog(requestPath, StatusCodes.Status200OK, "Ok", itemList, isLogActive);
            return new OkObjectResult(itemList);
        }

        public CreatedAtActionResult HandleCreated<T>(HttpContext httpContext, List<T> itemList, bool isLogActive)
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageAndListToLog(requestPath, StatusCodes.Status201Created, "Created", itemList, isLogActive);
            return new CreatedAtActionResult(nameof(itemList), null, null, itemList);
        }
    }
}