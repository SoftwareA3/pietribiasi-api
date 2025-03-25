using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace apiPB.Authentication
{
    public class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string Realm = "Gestione Commesse";

        public BasicAuthentication(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check if the Authorization header exists
            if (!Request.Headers.TryGetValue("Authorization", out var headerValues))
            {
                Logger.LogWarning("Authorization header is missing.");
                return Task.FromResult(AuthenticateResult.Fail("Authorization header is missing."));
            }

            string authHeader = headerValues.FirstOrDefault() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(authHeader) || 
                !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                Logger.LogWarning("Authorization header is missing or invalid.");
                return Task.FromResult(AuthenticateResult.Fail("Missing or invalid Authorization header."));
            }

            // Extract and decode credentials
            string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            if (string.IsNullOrWhiteSpace(encodedCredentials))
            {
                Logger.LogWarning("Encoded credentials are empty.");
                return Task.FromResult(AuthenticateResult.Fail("Empty credentials."));
            }

            string credentials;
            try
            {
                var credentialBytes = Convert.FromBase64String(encodedCredentials);
                credentials = Encoding.UTF8.GetString(credentialBytes); // Use UTF-8 encoding
            }
            catch (FormatException)
            {
                Logger.LogWarning("Base64 decoding failed for the Authorization header.");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Base64 string."));
            }

            // Split credentials into username and password
            int separatorIndex = credentials.IndexOf(':');
            if (separatorIndex <= 0)
            {
                Logger.LogWarning("Invalid credentials format: ':' separator missing or in an invalid position.");
                return Task.FromResult(AuthenticateResult.Fail("Invalid credentials format."));
            }

            var username = credentials.Substring(0, separatorIndex);
            var password = credentials.Substring(separatorIndex + 1);

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Logger.LogWarning("Empty username or password provided.");
                return Task.FromResult(AuthenticateResult.Fail("Empty username or password."));
            }

            // Validate credentials (replace with secure logic in production)
            if (!ValidateCredentials(username, password))
            {
                Logger.LogWarning("Invalid username or password for user: {Username}", username);
                return Task.FromResult(AuthenticateResult.Fail("Invalid username or password."));
            }

            // Create claims and authentication ticket
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private bool ValidateCredentials(string username, string password)
        {
            // Replace this with a secure validation mechanism (e.g., hashed passwords in a database)
            return username == "admin1" && password == "password1"; // Placeholder logic
        }

        // Add the WWW-Authenticate header in case of 401 Unauthorized
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
            return base.HandleChallengeAsync(properties);
        }
    }
}
