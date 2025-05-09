using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using apiPB.Services.Request.Abstraction;
using apiPB.Services.Utils.Abstraction;
using apiPB.Dto.Request;

namespace apiPB.Authentication
{
    public class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string Realm = "Gestione Commesse";
        private readonly ILogService _logService;
        private readonly IWorkersRequestService _workerRequestService;
        private readonly bool _isLogActive;

        public BasicAuthentication(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder, ILogService logService, IWorkersRequestService workerRequestService)
            : base(options, logger, encoder)
        {
            // Inizializza il servizio di log per scrive i messaggi di log sul file API.log
            _logService = logService;

            // Servizio per interrogare il database e verificare le credenziali
            _workerRequestService = workerRequestService;

            _isLogActive = false;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Controlla l'esistenza dell'header Authorization
            if (!Request.Headers.TryGetValue("Authorization", out var headerValues))
            {
                _logService.AppendMessageToLog("Authorization header is missing.", 401, "Unauthorized", _isLogActive);
                
                var miss = Task.FromResult(AuthenticateResult.Fail("Authorization header is missing."));
                
                return miss;
            }

            string authHeader = headerValues.FirstOrDefault() ?? string.Empty;

            // Controlla se l'header è vuoto, ha spazi vuoti o non inizia con "Basic "
            if (string.IsNullOrWhiteSpace(authHeader) || 
                !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                _logService.AppendMessageToLog("Authorization header is missing or invalid.", 401, "Unauthorized", _isLogActive);

                var miss = Task.FromResult(AuthenticateResult.Fail("Missing or invalid Authorization header."));

                return miss;
            }

            // Extract and decode credentials
            string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            if (string.IsNullOrWhiteSpace(encodedCredentials))
            {
                _logService.AppendMessageToLog("Encoded credentials are empty.", 401, "Unauthorized", _isLogActive);

                var empty = Task.FromResult(AuthenticateResult.Fail("Empty credentials."));

                return empty;
            }

            string credentials;
            try
            {
                var credentialBytes = Convert.FromBase64String(encodedCredentials);
                credentials = Encoding.UTF8.GetString(credentialBytes);
            }
            catch (FormatException)
            {
                _logService.AppendMessageToLog("Base64 decoding failed for the Authorization header.", 401, "Unauthorized", _isLogActive);

                var invalid = Task.FromResult(AuthenticateResult.Fail("Invalid Base64 string."));

                return invalid;
            }

            // Divide le credenziali trovando la posizione del separatore ':'
            int separatorIndex = credentials.IndexOf(':');
            // Se il separatore è minore di 0 o è in una posizione non valida ritorna errore
            if (separatorIndex <= 0)
            {
                _logService.AppendMessageToLog("Invalid credentials format: ':' separator missing or in an invalid position.", 401, "Unauthorized", _isLogActive);

                var invalid = Task.FromResult(AuthenticateResult.Fail("Invalid credentials format."));

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
                _logService.AppendMessageToLog("Empty username or password provided.", 401, "Unauthorized", _isLogActive);

                var empty = Task.FromResult(AuthenticateResult.Fail("Empty username or password."));

                return empty;
            }

            // Controlla le credenziali chiamando la funzione ValidateCredentials
            // La funzione e questa condizione sono sostituibili in caso di cambio di meccanismo per la validazione
            if (!ValidateCredentials(username, password))
            {
                _logService.AppendMessageToLog($"Invalid username or password for user: {username}", 401, "Unauthorized", _isLogActive);

                var invalid = Task.FromResult(AuthenticateResult.Fail("Invalid username or password."));
                
                return invalid;
            }

            // Crea un'identità e un principale con le credenziali trovate
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            _logService.AppendMessageToLog($"User {username} authenticated successfully.", 200, "OK", _isLogActive);

            var success = Task.FromResult(AuthenticateResult.Success(ticket));

            return success;
        }

        // Semplice controllo di corrispondenza tra le credenziali fornite e quelle attese
        // Fixme: si potrebbero inserire i valori in appsettings.json in modo che siano nascoste
        private bool ValidateCredentials(string username, string password)
        {
            // Usando la password, il servizio recupera Password e WorkerId e le usa come credenziali
            var credentials = _workerRequestService.GetWorkerByPassword(new PasswordWorkersRequestDto { Password = password });
            string expectedUsername = credentials?.WorkerId.ToString() ?? string.Empty;
            string expectedPassword = credentials?.Password ?? string.Empty;
            return username == expectedUsername && password == expectedPassword; 
        }

        // Metodo per la gestione della risposta di autenticazione
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
            return base.HandleChallengeAsync(properties);
        }
    }
}
