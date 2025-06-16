using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Services.Implementation;
using Microsoft.AspNetCore.Mvc;

namespace apiPB.Utils.Abstraction
{
    public interface IResponseHandler
    {
        /// <summary>
        /// Se il file di log è attivo, scrive la risposta del BadRequest sul file di Log
        /// </summary>
        /// <param name="httpContext">HttpContext della richiesta</param>
        /// <param name="isLogActive">Determina se il file di log è attivo</param>
        /// <returns>BadRequestObjectResult: scrive sul file di log e ritorna BadRequest</returns>
        BadRequestObjectResult HandleBadRequest(HttpContext httpContext, bool isLogActive, string message = "Bad Request");

        /// <summary>
        /// Se il file di log è attivo, scrive la risposta del NotFound sul file di Log
        /// </summary>
        /// <param name="httpContext">HttpContext della richiesta</param>
        /// <param name="isLogActive">Determina se il file di log è attivo</param>
        /// <returns>NotFoundObjectResult: scrive sul file di log e ritorna NotFound</returns>
        NotFoundObjectResult HandleNotFound(HttpContext httpContext, bool isLogActive, string message = "Not Found");

        OkObjectResult HandleOk(HttpContext httpContext, bool isLogActive, string message = "Ok");

        OkObjectResult HandleOkAndItem<T>(HttpContext httpContext, T item, bool isLogActive, string message = "Ok");

        OkObjectResult HandleOkAndList<T>(HttpContext httpContext, List<T> itemList, bool isLogActive, string message = "Ok");

        CreatedAtActionResult HandleCreated<T>(HttpContext httpContext, List<T> itemList, bool isLogActive, string message = "Created");

        NoContentResult HandleNoContent(HttpContext httpContext, bool isLogActive, string message = "No Content");
    }
}