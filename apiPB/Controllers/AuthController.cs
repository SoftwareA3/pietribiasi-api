using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using apiPB.Dto.Models;
using apiPB.Services.Request.Abstraction;
using apiPB.Services;
using apiPB.Dto.Request;

namespace apiPB.Controllers
{
    [ApiController]
    [Route("api/auth")]
    // Controller per la verifica delle credenziali.
    public class AuthController : ControllerBase
    {
        private readonly IWorkersRequestService _workerRequestService;
        private readonly LogService _logService;

        public AuthController(IWorkersRequestService workerRequestService, LogService logService)
        {
            _workerRequestService = workerRequestService;
            _logService = logService;
        }

        /// <summary>
        /// Verifica le credenziali inviate tramite Basic Authentication.
        /// </summary>
        /// <returns>200 OK se le credenziali sono corrette, altrimenti 401 Unauthorized.</returns>
        [HttpPost("login")]
        public IActionResult LoginWithPassword([FromBody] PasswordWorkersRequestDto passwordWorkersRequestDto)
        {
            string requestPath = $"{HttpContext.Request.Method} {HttpContext.Request.Path.Value?.TrimStart('/') ?? string.Empty}";

            var workerIdAndPasswordDto = _workerRequestService.LoginWithPassword(passwordWorkersRequestDto);
            if(workerIdAndPasswordDto == null)
            {
                // Se il lavoratore non esiste, restituisce 401 Unauthorized
                _logService.AppendMessageToLog("Invalid credentials", 401, "Unauthorized");
                return Unauthorized(new { message = "Invalid credentials" });
            }
            
            var ok = Ok(workerIdAndPasswordDto);

            _logService.AppendMessageAndItemToLog(requestPath, ok.StatusCode, "OK", workerIdAndPasswordDto);

            return ok;   
        }

        [Authorize]
        [HttpGet("validate")]
        public IActionResult Login()
        {
            // Se il codice raggiunge questo punto, l'utente è già stato autenticato
            // dal middleware BasicAuthentication, quindi possiamo restituire un risultato positivo
            return Ok(new { 
                message = "Login successful", 
                username = User.Identity?.Name 
            });
        }
    }
}