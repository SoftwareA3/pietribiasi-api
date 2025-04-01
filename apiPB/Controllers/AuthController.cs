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

namespace apiPB.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Verifica le credenziali inviate tramite Basic Authentication.
        /// </summary>
        /// <returns>200 OK se le credenziali sono corrette, altrimenti 401 Unauthorized.</returns>
        [HttpGet("login")]
        public IActionResult Login()
        {
            // Controlla l'esistenza dell'header Authorization
            if (!Request.Headers.TryGetValue("Authorization", out var headerValues))
            {
                return Unauthorized(new { message = "Authorization header is missing." });
            }

            string authHeader = headerValues.FirstOrDefault() ?? string.Empty;

            // Verifica che l'header inizi con "Basic "
            if (string.IsNullOrWhiteSpace(authHeader) ||
                !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new { message = "Authorization header is missing or invalid." });
            }

            // Estrae e decodifica le credenziali
            string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            if (string.IsNullOrWhiteSpace(encodedCredentials))
            {
                return Unauthorized(new { message = "Encoded credentials are empty." });
            }

            string credentials;
            try
            {
                byte[] credentialBytes = Convert.FromBase64String(encodedCredentials);
                credentials = Encoding.UTF8.GetString(credentialBytes);
            }
            catch (FormatException)
            {
                return Unauthorized(new { message = "Invalid Base64 string." });
            }

            // Divide le credenziali in username e password
            int separatorIndex = credentials.IndexOf(':');
            if (separatorIndex <= 0)
            {
                return Unauthorized(new { message = "Invalid credentials format." });
            }

            string username = credentials.Substring(0, separatorIndex);
            string password = credentials.Substring(separatorIndex + 1);

            // Recupera le credenziali attese dal file di configurazione (appsettings.json)
            string expectedUsername = _configuration.GetValue<string>("Authentication:Username") ?? string.Empty;
            string expectedPassword = _configuration.GetValue<string>("Authentication:Password") ?? string.Empty;

            if (username == expectedUsername && password == expectedPassword)
            {
                return Ok(new { message = "Login successful." });
            }
            else
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }
        }
    }
}