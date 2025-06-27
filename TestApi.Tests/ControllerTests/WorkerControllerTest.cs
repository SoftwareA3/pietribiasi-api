using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using apiPB.Controllers;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;
using apiPB.Utils.Implementation;

namespace TestApi.Tests.ControllerTests
{
    public class WorkerControllerTest
    {
        private readonly Mock<IWorkersRequestService> _workerRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
        private readonly WorkerController _controller;

        // DTO utilizzati per i test
        private readonly WorkerDto _sampleWorkerDto = new WorkerDto
        {
            WorkerId = 43,
            Name = "Mario",
            LastName = "Rossi",
            Pin = "1234",
            Password = "password123",
            TipoUtente = "Admin",
            StorageVersamenti = "Storage1",
            Storage = "MainStorage",
            LastLogin = "2023-10-01 12:00:00"
        };

        private readonly PasswordWorkersRequestDto _samplePasswordRequestDto = new PasswordWorkersRequestDto
        {
            Password = "password123"
        };

        private readonly WorkerIdAndPasswordRequestDto _sampleWorkerIdAndPasswordRequestDto = new WorkerIdAndPasswordRequestDto
        {
            WorkerId = 43,
            Password = "password123"
        };

        private readonly WorkersFieldDto _sampleWorkersFieldDto = new WorkersFieldDto
        {
            WorkerId = 43,
            Line = 1,
            FieldName = "LastLogin",
            FieldValue = "2023-10-01 12:00:00",
            Notes = "Test login",
            HideOnLayout = "N",
            Tbcreated = new DateTime(2023, 10, 1, 12, 0, 0),
            Tbmodified = new DateTime(2023, 10, 1, 12, 0, 0),
            TbcreatedId = 1,
            TbmodifiedId = 1
        };

        public WorkerControllerTest()
        {
            _workerRequestServiceMock = new Mock<IWorkersRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new WorkerController(_responseHandlerMock.Object, _workerRequestServiceMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            // Impostazioni predefinite per Metodo, Path e tipo di richiesta
            httpRequestMock.Setup(r => r.Method).Returns("GET");
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/worker"));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            _responseHandlerMock.Setup(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));
            _responseHandlerMock.Setup(x => x.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<WorkerDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(_sampleWorkerDto));
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<WorkerDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(new List<WorkerDto> { _sampleWorkerDto }));
        }

        // --- Test per GetAllWorkers ---
        [Fact]
        public void GetAllWorkers_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<WorkerDto> { _sampleWorkerDto };
            _workerRequestServiceMock.Setup(service => service.GetWorkers()).Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<WorkerDto>>(), false, "Ok"))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetAllWorkers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<List<WorkerDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleWorkerDto.WorkerId, returnValue.First().WorkerId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<WorkerDto>>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetAllWorkers_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.GetWorkers()).Throws(new EmptyListException("WorkerController", "GetAllWorkers", "Nessun risultato trovato."));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetAllWorkers();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAllWorkers_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.GetWorkers()).Throws(new ArgumentNullException("WorkerController", "Il servizio ritorna null."));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetAllWorkers();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAllWorkers_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.GetWorkers()).Throws(new Exception("Errore generico."));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetAllWorkers();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Test per LoginWithPassword ---
        [Fact]
        public void LoginWithPassword_ShouldReturnOk_WhenDataExists()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.LoginWithPassword(It.IsAny<PasswordWorkersRequestDto>()))
                .Returns(_sampleWorkerDto);
            _responseHandlerMock.Setup(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<WorkerDto>(), false, "Ok"))
                .Returns(new OkObjectResult(_sampleWorkerDto));

            // Act
            var result = _controller.LoginWithPassword(_samplePasswordRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<WorkerDto>(okResult.Value);
            Assert.Equal(_sampleWorkerDto.WorkerId, returnValue.WorkerId);
            _responseHandlerMock.Verify(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<WorkerDto>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void LoginWithPassword_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.LoginWithPassword(It.IsAny<PasswordWorkersRequestDto>()))
                .Throws(new ArgumentNullException("WorkerController", "Il servizio ritorna null."));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.LoginWithPassword(_samplePasswordRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void LoginWithPassword_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.LoginWithPassword(It.IsAny<PasswordWorkersRequestDto>()))
                .Throws(new Exception("Errore generico."));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.LoginWithPassword(_samplePasswordRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void LoginWithPassword_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            PasswordWorkersRequestDto? nullDto = null;

            // Act
            var result = _controller.LoginWithPassword(nullDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void LoginWithPassword_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.LoginWithPassword(It.IsAny<PasswordWorkersRequestDto>()))
                .Throws(new EmptyListException("WorkerController", "LoginWithPassword", "Nessun risultato trovato."));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.LoginWithPassword(_samplePasswordRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }
    }
}