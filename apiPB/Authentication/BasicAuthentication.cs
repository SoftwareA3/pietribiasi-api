using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using apiPB.Services;
using Microsoft.VisualBasic;

namespace apiPB.Authentication
{
    public class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string Realm = "Gestione Commesse";
        private readonly LogService _logService;

        public BasicAuthentication(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder, LogService logService)
            : base(options, logger, encoder)
        {
            // Inizializza il servizio di log per scrive i messaggi di log sul file API.log
            _logService = logService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Controlla l'esistenza dell'header Authorization
            if (!Request.Headers.TryGetValue("Authorization", out var headerValues))
            {
                // Logger.LogWarning("Authorization header is missing.");
                // return Task.FromResult(AuthenticateResult.Fail("Authorization header is missing."));
                
                var miss = Task.FromResult(AuthenticateResult.Fail("Authorization header is missing."));
                
                _logService.AppendMessageToLog("Authorization header is missing.", 401, "Unauthorized");
                
                return miss;
            }

            string authHeader = headerValues.FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(authHeader) || 
                !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                // Logger.LogWarning("Authorization header is missing or invalid.");
                var miss = Task.FromResult(AuthenticateResult.Fail("Missing or invalid Authorization header."));

                _logService.AppendMessageToLog("Authorization header is missing or invalid.", 401, "Unauthorized");

                return miss;
            }

            // Extract and decode credentials
            string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            if (string.IsNullOrWhiteSpace(encodedCredentials))
            {
                // Logger.LogWarning("Encoded credentials are empty.");
                var empty = Task.FromResult(AuthenticateResult.Fail("Empty credentials."));

                _logService.AppendMessageToLog("Encoded credentials are empty.", 401, "Unauthorized");

                return empty;
            }

            string credentials;
            try
            {
                var credentialBytes = Convert.FromBase64String(encodedCredentials);
                credentials = Encoding.UTF8.GetString(credentialBytes); // Use UTF-8 encoding
            }
            catch (FormatException)
            {
                // Logger.LogWarning("Base64 decoding failed for the Authorization header.");
                var invalid = Task.FromResult(AuthenticateResult.Fail("Invalid Base64 string."));

                _logService.AppendMessageToLog("Base64 decoding failed for the Authorization header.", 401, "Unauthorized");

                return invalid;
            }

            // Divide le credenziali trovando la posizione del separatore ':'
            int separatorIndex = credentials.IndexOf(':');
            // Se il separatore è minore di 0 o è in una posizione non valida ritorna errore
            if (separatorIndex <= 0)
            {
                // Logger.LogWarning("Invalid credentials format: ':' separator missing or in an invalid position.");
                var invalid = Task.FromResult(AuthenticateResult.Fail("Invalid credentials format."));

                _logService.AppendMessageToLog("Invalid credentials format: ':' separator missing or in an invalid position.", 401, "Unauthorized");
                return invalid;
            }

            // Crea due variabili per distinguere le credenziali, prendendo le sottostringhe 
            // dalla posizione 0 alla posizione del separatore per username
            // dalla posizione del separatore + 1 (il separatore è ':') fino alla fine della stringa 
            var username = credentials.Substring(0, separatorIndex);
            var password = credentials.Substring(separatorIndex + 1);

            // Controlla se username o password sono vuoti
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                // Logger.LogWarning("Empty username or password provided.");
                var empty = Task.FromResult(AuthenticateResult.Fail("Empty username or password."));

                _logService.AppendMessageToLog("Empty username or password provided.", 401, "Unauthorized");

                return empty;
            }

            // Controlla le credenziali chiamando la funzione ValidateCredentials
            // La funzione e questa condizione sono sostituibili in caso di cambio di meccanismo per la validazione
            if (!ValidateCredentials(username, password))
            {
                // Logger.LogWarning("Invalid username or password for user: {Username}", username);
                
                var invalid = Task.FromResult(AuthenticateResult.Fail("Invalid username or password."));

                _logService.AppendMessageToLog($"Invalid username or password for user: {username}", 401, "Unauthorized");
                
                return invalid;
            }

            // Crea un'identità e un principale con le credenziali trovate
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            var success = Task.FromResult(AuthenticateResult.Success(ticket));

            _logService.AppendMessageToLog($"User {username} authenticated successfully.", 200, "OK");

            return success;
        }

        // Semplice controllo di corrispondenza tra le credenziali fornite e quelle attese
        // Fixme: si potrebbero inserire i valori in appsettings.json in modo che siano nascoste
        private bool ValidateCredentials(string username, string password)
        {
            return username == "admin" && password == "password"; 
        }

        // Metodo per la gestione della risposta di autenticazione
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
            return base.HandleChallengeAsync(properties);
        }
    }
}
