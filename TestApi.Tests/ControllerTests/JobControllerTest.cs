using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using apiPB.Controllers; 
using apiPB.Dto.Models;  
using apiPB.Services.Abstraction; 
using apiPB.Services; 
using Microsoft.IdentityModel.Tokens;
using apiPB.Utils.Abstraction;
using apiPB.Utils.Implementation;

namespace TestApi.Tests.ControllerTests 
{
    public class JobControllerTest
    {
        private readonly Mock<IJobRequestService> _jobRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
        private readonly JobController _controller;

        // Costanti per i test
        private const string CONTROLLER_NAME = nameof(JobController);
        private const string GET_JOBS_METHOD = nameof(JobController.GetVwApiJobs);
        private const string TEST_USER = "testuser";
        private const string API_PATH = "/api/job";

        // Messaggi di errore standardizzati
        private static class ErrorMessages
        {
            public const string EMPTY_REQUEST = "La richiesta non può essere vuota.";
            public const string NO_RESULTS = "Nessun risultato trovato.";
            public const string SERVICE_NULL = "Il servizio non ha restituito dati.";
            public const string GENERIC_ERROR = "Errore generico nel servizio.";
            
            public static string ServiceReturnsNull(string controllerName, string message) 
                => $"Il servizio ritorna null in {controllerName}: {message}";
            
            public static string ServiceReturnsEmptyList(string controllerName) 
                => $"Il servizio ritorna una lista vuota in {controllerName}: Lista vuota";
            
            public static string ServiceExecutionError(string controllerName, string message) 
                => $"Errore durante l'esecuzione del Service in {controllerName}: {message}";
        }

        // DTO utilizzati per i test
        private readonly JobDto _sampleJobDto = new JobDto
        {
            Job = "JOB001",
            Description = "Sample Job Description"
        };

        public JobControllerTest()
        {
            _jobRequestServiceMock = new Mock<IJobRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new JobController(_responseHandlerMock.Object, _jobRequestServiceMock.Object);

            SetupHttpContext();
            SetupDefaultResponseHandlerMocks();
        }

        private void SetupHttpContext()
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            httpRequestMock.Setup(r => r.Method).Returns("GET");
            httpRequestMock.Setup(r => r.Path).Returns(new PathString(API_PATH));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            // Mock User per [Authorize]
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, TEST_USER) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContextMock.Setup(c => c.User).Returns(claimsPrincipal);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
        }

        private void SetupDefaultResponseHandlerMocks()
        {
            _responseHandlerMock.Setup(x => x.HandleBadRequest(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                ErrorMessages.EMPTY_REQUEST))
                .Returns(new BadRequestObjectResult(ErrorMessages.EMPTY_REQUEST));

            _responseHandlerMock.Setup(x => x.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                ErrorMessages.NO_RESULTS))
                .Returns(new NotFoundObjectResult(ErrorMessages.NO_RESULTS));
        }

        [Fact]
        public void GetJobs_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockDataList = new List<JobDto> 
            { 
                _sampleJobDto, 
                new JobDto { Job = "JOB002", Description = "Another Job" } 
            };

            _jobRequestServiceMock.Setup(service => service.GetJobs())
                .Returns(mockDataList);

            _responseHandlerMock.Setup(handler => handler.HandleOkAndList(
                It.IsAny<HttpContext>(),
                It.IsAny<List<JobDto>>(),
                false, 
                "Ok"))
                .Returns(new OkObjectResult(mockDataList));

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            
            var returnValue = Assert.IsAssignableFrom<IEnumerable<JobDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
            Assert.Contains(_sampleJobDto, returnValue);
            Assert.Equal("JOB001", returnValue.First(j => j.Job == "JOB001").Job);

            // Verify service was called
            _jobRequestServiceMock.Verify(service => service.GetJobs(), Times.Once);
            
            // Verify response handler was called with correct parameters
            _responseHandlerMock.Verify(handler => handler.HandleOkAndList(
                It.IsAny<HttpContext>(),
                It.Is<List<JobDto>>(list => list.Count == 2),
                false, 
                "Ok"), Times.Once);
        }

        [Fact]
        public void GetJobs_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            var exceptionMessage = ErrorMessages.NO_RESULTS;
            var expectedException = new EmptyListException(CONTROLLER_NAME, GET_JOBS_METHOD, exceptionMessage);
            var expectedResponseMessage = ErrorMessages.ServiceExecutionError(CONTROLLER_NAME, exceptionMessage);
            var expectedHandlerResponseMessage = ErrorMessages.ServiceReturnsEmptyList(CONTROLLER_NAME);

            _jobRequestServiceMock.Setup(service => service.GetJobs())
                .Throws(expectedException);

            _responseHandlerMock.Setup(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(),
                It.IsAny<bool>(),
                expectedResponseMessage))
                .Returns(new NotFoundObjectResult(expectedHandlerResponseMessage));

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(expectedHandlerResponseMessage, notFoundResult.Value);

            // Verify interactions
            _jobRequestServiceMock.Verify(service => service.GetJobs(), Times.Once);
            _responseHandlerMock.Verify(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                expectedResponseMessage), Times.Once);
        }

        [Fact]
        public void GetJobs_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            var parameterName = CONTROLLER_NAME;
            var exceptionMessage = ErrorMessages.SERVICE_NULL;
            var expectedException = new ArgumentNullException(parameterName, exceptionMessage);
            
            // Costruzione dinamica del messaggio di errore come farebbe il controller
            var expectedControllerMessage = ErrorMessages.ServiceReturnsNull(CONTROLLER_NAME, expectedException.Message);
            var expectedHandlerResponse = "Il servizio ritorna null in JobController: Test null exception";

            _jobRequestServiceMock.Setup(service => service.GetJobs())
                .Throws(expectedException);

            _responseHandlerMock.Setup(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(),
                It.IsAny<bool>(),
                expectedControllerMessage))
                .Returns(new NotFoundObjectResult(expectedHandlerResponse));

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(expectedHandlerResponse, notFoundResult.Value);

            // Verify interactions
            _jobRequestServiceMock.Verify(service => service.GetJobs(), Times.Once);
            _responseHandlerMock.Verify(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                expectedControllerMessage), Times.Once);
        }

        [Fact]
        public void GetJobs_ShouldReturnNotFound_WhenServiceThrowsGenericException()
        {
            // Arrange
            var exceptionMessage = ErrorMessages.GENERIC_ERROR;
            var expectedException = new Exception(exceptionMessage);
            var expectedControllerMessage = ErrorMessages.ServiceExecutionError(CONTROLLER_NAME, exceptionMessage);

            _jobRequestServiceMock.Setup(service => service.GetJobs())
                .Throws(expectedException);

            _responseHandlerMock.Setup(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                expectedControllerMessage))
                .Returns(new NotFoundObjectResult(expectedControllerMessage));

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(expectedControllerMessage, notFoundResult.Value);

            // Verify interactions
            _jobRequestServiceMock.Verify(service => service.GetJobs(), Times.Once);
            _responseHandlerMock.Verify(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                expectedControllerMessage), Times.Once);
        }

        [Fact]
        public void GetJobs_ShouldReturnNotFound_WhenServiceThrowsInvalidOperationException()
        {
            // Arrange
            var exceptionMessage = "Operazione non valida";
            var expectedException = new InvalidOperationException(exceptionMessage);
            var expectedControllerMessage = $"Il servizio non ha trovato dati in {CONTROLLER_NAME}: {exceptionMessage}";

            _jobRequestServiceMock.Setup(service => service.GetJobs())
                .Throws(expectedException);

            _responseHandlerMock.Setup(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                expectedControllerMessage))
                .Returns(new NotFoundObjectResult(expectedControllerMessage));

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(expectedControllerMessage, notFoundResult.Value);

            // Verify interactions
            _jobRequestServiceMock.Verify(service => service.GetJobs(), Times.Once);
            _responseHandlerMock.Verify(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                expectedControllerMessage), Times.Once);
        }
    }
}