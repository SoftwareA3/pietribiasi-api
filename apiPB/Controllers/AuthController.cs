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
using apiPB.Services.Abstraction;
using apiPB.Services;
using apiPB.Dto.Request;

namespace apiPB.Controllers
{
    [ApiController]
    [Route("api/auth")]
    // Controller per la verifica delle credenziali.
    public class AuthController : ControllerBase
    {
        // Classe usata unicamente per la verifica delle credenziali.
        public AuthController()
        {
        }

        [Authorize]
        [HttpGet("validate")]
        public IActionResult Login()
        {
            // Se il codice raggiunge questo punto, la richiesta è stata autenticata
            // dal middleware BasicAuthentication, quindi viene restituito un messaggio di successo.
            return Ok(new { 
                message = "Login successful", 
                username = User.Identity?.Name 
            });
        }
    }
}