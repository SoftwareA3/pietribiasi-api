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

        public BadRequestObjectResult HandleBadRequest(HttpContext httpContext, string message = "Bad Request")
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageToLog(requestPath, StatusCodes.Status400BadRequest, message);
            _logService.AppendErrorToLog(message);
            return new BadRequestObjectResult(message);
        }

        public NotFoundObjectResult HandleNotFound(HttpContext httpContext, string message = "Not Found")
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageToLog(requestPath, StatusCodes.Status404NotFound, message);
            _logService.AppendErrorToLog(message);
            return new NotFoundObjectResult(message);
        }

        public OkObjectResult HandleOk(HttpContext httpContext, string message = "Ok")
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageToLog(requestPath, StatusCodes.Status200OK, message);
            return new OkObjectResult(message);
        }

        public OkObjectResult HandleOkAndItem<T>(HttpContext httpContext, T item, string message = "Ok")
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageAndItemToLog(requestPath, StatusCodes.Status200OK, message, item);
            return new OkObjectResult(item);
        }

        public OkObjectResult HandleOkAndList<T>(HttpContext httpContext, List<T> itemList, string message = "Ok")
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageAndListToLog(requestPath, StatusCodes.Status200OK, message, itemList);
            return new OkObjectResult(itemList);
        }

        public CreatedAtActionResult HandleCreated<T>(HttpContext httpContext, List<T> itemList, string message = "Created")
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageAndListToLog(requestPath, StatusCodes.Status201Created, message, itemList);
            return new CreatedAtActionResult(nameof(itemList), null, null, itemList);
        }

        public NoContentResult HandleNoContent(HttpContext httpContext, string message = "No Content")
        {
            string requestPath = BuildHttpContextString(httpContext);
            _logService.AppendMessageToLog(requestPath, StatusCodes.Status204NoContent, message);
            _logService.AppendWarningToLog(message);
            return new NoContentResult();
        }
    }
}