using Xunit;
using Moq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Text.Encodings.Web;
using apiPB.Authentication; 
using apiPB.Services.Abstraction; 
using apiPB.Utils.Abstraction;   
using apiPB.Dto.Models;
using apiPB.Dto.Request;                 
using System.Collections.Generic;

namespace TestApi.Tests.AuthenticationTests 
{
    public class BasicAuthenticationTest 
    {
        private readonly Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _optionsMonitorMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<ILogger> _loggerMock; 
        private readonly Mock<UrlEncoder> _urlEncoderMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly Mock<IWorkersRequestService> _workerRequestServiceMock;
        
        public BasicAuthenticationTest()
        {
            _optionsMonitorMock = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger>(); 
            _urlEncoderMock = new Mock<UrlEncoder>();
            _logServiceMock = new Mock<ILogService>();
            _workerRequestServiceMock = new Mock<IWorkersRequestService>();

            // Setup comune per i mock
            _optionsMonitorMock.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());
            _loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
            
            var defaultUrlEncoder = UrlEncoder.Default; 
            _urlEncoderMock.Setup(e => e.Encode(It.IsAny<string>())).Returns((string s) => defaultUrlEncoder.Encode(s));


            _logServiceMock.Setup(x => x.AppendMessageToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                false)); // _isLogActive è false in BasicAuthentication
        }

        private BasicAuthentication CreateAuthHandler(HttpContext httpContext)
        {
            var handler = new BasicAuthentication(
                _optionsMonitorMock.Object,
                _loggerFactoryMock.Object,
                _urlEncoderMock.Object, 
                _logServiceMock.Object,
                _workerRequestServiceMock.Object);

            var scheme = new AuthenticationScheme("Basic", "Basic Authentication", typeof(BasicAuthentication));
            handler.InitializeAsync(scheme, httpContext).Wait(); 
            return handler;
        }

        private HttpContext CreateMockHttpContext(string authorizationHeaderValue = null)
        {
            var requestMock = new Mock<HttpRequest>();
            var headerDictionary = new HeaderDictionary();
            
            if (authorizationHeaderValue != null)
            {
                headerDictionary.Add("Authorization", authorizationHeaderValue);
            }
            requestMock.Setup(r => r.Headers).Returns(headerDictionary);

            var responseMock = new Mock<HttpResponse>();
            var responseHeaderDictionary = new HeaderDictionary(); 
            responseMock.Setup(r => r.Headers).Returns(responseHeaderDictionary);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Request).Returns(requestMock.Object);
            httpContextMock.Setup(c => c.Response).Returns(responseMock.Object);
            
            // Necessario per InitializeAsync
            var items = new Dictionary<object, object>();
            httpContextMock.Setup(c => c.Items).Returns(items);

            return httpContextMock.Object;
        }

        // --- Test Esistenti (Mantenuti per completezza, potrebbero necessitare di aggiustamenti) ---

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenAuthorizationHeaderMissing()
        {
            // Arrange
            var httpContext = CreateMockHttpContext(authorizationHeaderValue: null); 
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Authorization header is missing.", result.Failure.Message); 
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Authorization header is missing.", 401, "Unauthorized", false), Times.Once);
        }


        // --- Test per la Porzione 1: Controllo authHeader ---
        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenAuthHeaderIsEmpty()
        {
            // Arrange
            var httpContext = CreateMockHttpContext(string.Empty); 
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Missing or invalid Authorization header.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Authorization header is missing or invalid.", 401, "Unauthorized", false), Times.Once);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenAuthHeaderIsWhitespace()
        {
            // Arrange
            var httpContext = CreateMockHttpContext("   "); 
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Missing or invalid Authorization header.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Authorization header is missing or invalid.", 401, "Unauthorized", false), Times.Once);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenAuthHeaderIsNotBasicScheme()
        {
            // Arrange
            var httpContext = CreateMockHttpContext("Bearer sometoken"); 
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Missing or invalid Authorization header.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Authorization header is missing or invalid.", 401, "Unauthorized", false), Times.Once);
        }

        // --- Test per la Porzione 2: Controllo encodedCredentials ---
        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenEncodedCredentialsAreEmpty()
        {
            // Arrange
            // Header "Basic " seguito da null
            var httpContext = CreateMockHttpContext("Basic "); 
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Empty credentials.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Encoded credentials are empty.", 401, "Unauthorized", false), Times.Once);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenEncodedCredentialsAreWhitespace()
        {
            // Arrange
            // Header "Basic " seguito da spazi
            var httpContext = CreateMockHttpContext("Basic    "); 
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Empty credentials.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Encoded credentials are empty.", 401, "Unauthorized", false), Times.Once);
        }


        // --- Altri Test Esistenti (Adattati o Mantenuti) ---
        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenAuthorizationHeaderIsMalformed_InvalidBase64()
        {
            // Arrange
            // "Basic " seguito da una stringa non Base64 valida
            var httpContext = CreateMockHttpContext("Basic dX%%%Jlcl9uYW1lOnBhc3N3b3Jk"); 
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Invalid Base64 string.", result.Failure.Message); 
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Base64 decoding failed for the Authorization header.", 401, "Unauthorized", false), Times.Once);
        }
        
        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenCredentialsFormatIsInvalid_NoSeparator()
        {
            // Arrange
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("usernamepassword")); // Nessun ':'
            var httpContext = CreateMockHttpContext($"Basic {credentials}");
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Invalid credentials format.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Invalid credentials format: ':' separator missing or in an invalid position.", 401, "Unauthorized", false), Times.Once);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenUsernameIsEmptyInCredentials()
        {
            // Arrange
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(":password")); 
            var httpContext = CreateMockHttpContext($"Basic {credentials}");
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Invalid credentials format.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Invalid credentials format: ':' separator missing or in an invalid position.", 401, "Unauthorized", false), Times.Once);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenPasswordIsEmptyInCredentials()
        {
            // Arrange
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("username:")); 
            var httpContext = CreateMockHttpContext($"Basic {credentials}");
            var handler = CreateAuthHandler(httpContext);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Empty username or password.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                "Empty username or password provided.", 401, "Unauthorized", false), Times.Once);
        }


        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenCredentialsAreInvalid_ServiceReturnsNull()
        {
            // Arrange
            var username = "43";
            var password = "invalidpassword";
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            var httpContext = CreateMockHttpContext($"Basic {credentials}");
            var handler = CreateAuthHandler(httpContext);

            _workerRequestServiceMock.Setup(service => service.GetWorkerByPassword(It.Is<PasswordWorkersRequestDto>(dto => dto.Password == password)))
                .Returns((WorkerDto)null); 

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Invalid username or password.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                $"Invalid username or password for user: {username}", 401, "Unauthorized", false), Times.Once);
        }

        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenCredentialsAreValidButUsernameMismatch()
        {
            // Arrange
            var providedUsernameInHeader = "99"; 
            var actualWorkerIdFromService = 43;    
            var password = "validpassword";        
            
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{providedUsernameInHeader}:{password}"));
            var httpContext = CreateMockHttpContext($"Basic {credentials}");
            var handler = CreateAuthHandler(httpContext);

            var workerFromService = new WorkerDto { WorkerId = actualWorkerIdFromService, Password = password };
            _workerRequestServiceMock.Setup(service => service.GetWorkerByPassword(It.Is<PasswordWorkersRequestDto>(dto => dto.Password == password)))
                .Returns(workerFromService);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Invalid username or password.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                $"Invalid username or password for user: {providedUsernameInHeader}", 401, "Unauthorized", false), Times.Once);
        }
        
        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnFail_WhenCredentialsAreValidButPasswordMismatchAfterServiceCall()
        {
            // Arrange
            var username = "43"; 
            var passwordInHeader = "passwordInHeader";      
            var passwordInDbForWorker = "differentPasswordInDb"; 

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{passwordInHeader}"));
            var httpContext = CreateMockHttpContext($"Basic {credentials}");
            var handler = CreateAuthHandler(httpContext);

            var workerFromService = new WorkerDto { WorkerId = int.Parse(username), Password = passwordInDbForWorker };
            _workerRequestServiceMock.Setup(service => service.GetWorkerByPassword(It.Is<PasswordWorkersRequestDto>(dto => dto.Password == passwordInHeader)))
                .Returns(workerFromService);

            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failure);
            Assert.Equal("Invalid username or password.", result.Failure.Message);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                $"Invalid username or password for user: {username}", 401, "Unauthorized", false), Times.Once);
        }


        [Fact]
        public async Task HandleAuthenticateAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var username = "43"; 
            var password = "validpassword"; 
            var credentialsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            var httpContext = CreateMockHttpContext($"Basic {credentialsBase64}");
            var handler = CreateAuthHandler(httpContext);

            var validWorker = new WorkerDto 
            { 
                WorkerId = int.Parse(username), 
                Password = password             
            };
            _workerRequestServiceMock.Setup(service => service.GetWorkerByPassword(It.Is<PasswordWorkersRequestDto>(dto => dto.Password == password)))
                .Returns(validWorker); 
            
            // Act
            var result = await handler.AuthenticateAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Principal);
            Assert.True(result.Principal.Identity.IsAuthenticated);
            Assert.Equal(username, result.Principal.Identity.Name); 
            Assert.Equal("Basic", result.Principal.Identity.AuthenticationType); 

            _logServiceMock.Verify(log => log.AppendMessageToLog(
                $"User {username} authenticated successfully.", 200, "OK", false), Times.Once);
        }

        [Fact]
        public async Task HandleChallengeAsync_ShouldSetWWWAuthenticateHeader()
        {
            // Arrange
            var httpContext = CreateMockHttpContext(); 
            var handler = CreateAuthHandler(httpContext);
            var authProperties = new AuthenticationProperties(); 

            // Act
            // HandleChallengeAsync è protetto, ma viene chiamato internamente da ChallengeAsync.
            await handler.ChallengeAsync(authProperties); 

            // Assert
            var responseHeaders = httpContext.Response.Headers;
            Assert.True(responseHeaders.ContainsKey("WWW-Authenticate"));
            Assert.Equal("Basic realm=\"Gestione Commesse\"", responseHeaders["WWW-Authenticate"].ToString());
        }
    }
}
