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
using System;

namespace TestApi.Tests.ControllerTests
{
    public class GiacenzeControllerTests
    {
        private readonly Mock<IGiacenzeRequestService> _giacenzeRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
        private readonly GiacenzeController _controller;

        // Costanti per i test
        private const string CONTROLLER_NAME = nameof(GiacenzeController);
        private const string GET_GIACENZE_METHOD = nameof(GiacenzeController.GetGiacenze);
        private const string TEST_USER = "testuser";
        private const string API_PATH = "/api/giacenze";

        // Messaggi di errore standardizzati
        private static class ErrorMessages
        {
            public const string EMPTY_REQUEST = "La richiesta non può essere vuota.";
            public const string NO_RESULTS = "Nessun risultato trovato.";
            public const string SERVICE_NULL = "Il servizio non ha restituito dati.";
            public const string GENERIC_ERROR = "Errore generico nel servizio.";
            
            public static string ServiceReturnsNull(string controllerName, string message) 
                => $"Il servizio ritorna null in {controllerName}: {message}";
            
            public static string ServiceReturnsEmptyList(string controllerName, string message) 
                => $"Il servizio ritorna una lista vuota in {controllerName}: {message}";
            
            public static string ServiceExecutionError(string controllerName, string message) 
                => $"Errore durante l'esecuzione del Service in {controllerName}: {message}";
        }

        // DTO utilizzati per i test
        private readonly GiacenzeDto _sampleGiacenzaDto = new GiacenzeDto
        {
            Item = "ITEM001",
            Description = "Sample Item Description",
            BarCode = "1234567890123",
            FiscalYear = 2024,
            Storage = "MAG01",
            BookInv = 100.0
        };

        public GiacenzeControllerTests()
        {
            _giacenzeRequestServiceMock = new Mock<IGiacenzeRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new GiacenzeController(_responseHandlerMock.Object, _giacenzeRequestServiceMock.Object);

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
        public void GetGiacenze_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockDataList = new List<GiacenzeDto> 
            { 
                _sampleGiacenzaDto, 
                new GiacenzeDto { Item = "ITEM002", Description = "Another Item", BarCode = "9876543210987", FiscalYear = 2024, Storage = "MAG02", BookInv = 200.0 } 
            };

            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze())
                .Returns(mockDataList);

            // Il controller chiama HandleOkAndList con solo 3 parametri (non include il messaggio)
            _responseHandlerMock.Setup(handler => handler.HandleOkAndList(
                It.IsAny<HttpContext>(),
                It.IsAny<List<GiacenzeDto>>(),
                It.IsAny<bool>(), "Ok"))
                .Returns(new OkObjectResult(mockDataList));

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            
            var returnValue = Assert.IsAssignableFrom<IEnumerable<GiacenzeDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
            Assert.Contains(_sampleGiacenzaDto, returnValue);
            Assert.Equal("ITEM001", returnValue.First(g => g.Item == "ITEM001").Item);

            // Verify service was called
            _giacenzeRequestServiceMock.Verify(service => service.GetGiacenze(), Times.Once);
            
            // Verify response handler was called with correct parameters
            _responseHandlerMock.Verify(handler => handler.HandleOkAndList(
                It.IsAny<HttpContext>(),
                It.Is<List<GiacenzeDto>>(list => list.Count == 2),
                false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            var exceptionMessage = ErrorMessages.NO_RESULTS;
            var expectedException = new EmptyListException(CONTROLLER_NAME, GET_GIACENZE_METHOD, exceptionMessage);
            var expectedResponseMessage = ErrorMessages.ServiceReturnsEmptyList(CONTROLLER_NAME, exceptionMessage);

            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze())
                .Throws(expectedException);

            _responseHandlerMock.Setup(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(),
                It.IsAny<bool>(),
                expectedResponseMessage))
                .Returns(new NotFoundObjectResult(expectedResponseMessage));

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(expectedResponseMessage, notFoundResult.Value);

            // Verify interactions
            _giacenzeRequestServiceMock.Verify(service => service.GetGiacenze(), Times.Once);
            _responseHandlerMock.Verify(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                false, 
                expectedResponseMessage), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            var parameterName = "collection";
            var exceptionMessage = ErrorMessages.SERVICE_NULL;
            var expectedException = new ArgumentNullException(parameterName, exceptionMessage);
            
            // Il controller costruisce il messaggio usando ex.Message (che include il parameter name)
            var expectedControllerMessage = ErrorMessages.ServiceReturnsNull(CONTROLLER_NAME, expectedException.Message);

            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze())
                .Throws(expectedException);

            _responseHandlerMock.Setup(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(),
                It.IsAny<bool>(),
                expectedControllerMessage))
                .Returns(new NotFoundObjectResult(expectedControllerMessage));

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(expectedControllerMessage, notFoundResult.Value);

            // Verify interactions
            _giacenzeRequestServiceMock.Verify(service => service.GetGiacenze(), Times.Once);
            _responseHandlerMock.Verify(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                false, 
                expectedControllerMessage), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnNotFound_WhenServiceThrowsGenericException()
        {
            // Arrange
            var exceptionMessage = ErrorMessages.GENERIC_ERROR;
            var expectedException = new Exception(exceptionMessage);
            var expectedControllerMessage = ErrorMessages.ServiceExecutionError(CONTROLLER_NAME, exceptionMessage);

            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze())
                .Throws(expectedException);

            _responseHandlerMock.Setup(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                expectedControllerMessage))
                .Returns(new NotFoundObjectResult(expectedControllerMessage));

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(expectedControllerMessage, notFoundResult.Value);

            // Verify interactions
            _giacenzeRequestServiceMock.Verify(service => service.GetGiacenze(), Times.Once);
            _responseHandlerMock.Verify(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                false, 
                expectedControllerMessage), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnNotFound_WhenServiceThrowsInvalidOperationException()
        {
            // Arrange
            var exceptionMessage = "Operazione non valida";
            var expectedException = new InvalidOperationException(exceptionMessage);
            // Il controller gestisce InvalidOperationException nel catch generico di Exception
            var expectedControllerMessage = ErrorMessages.ServiceExecutionError(CONTROLLER_NAME, exceptionMessage);

            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze())
                .Throws(expectedException);

            _responseHandlerMock.Setup(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), 
                expectedControllerMessage))
                .Returns(new NotFoundObjectResult(expectedControllerMessage));

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal(expectedControllerMessage, notFoundResult.Value);

            // Verify interactions
            _giacenzeRequestServiceMock.Verify(service => service.GetGiacenze(), Times.Once);
            _responseHandlerMock.Verify(handler => handler.HandleNotFound(
                It.IsAny<HttpContext>(), 
                false, 
                expectedControllerMessage), Times.Once);
        }
    }
}