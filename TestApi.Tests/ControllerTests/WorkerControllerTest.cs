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
using apiPB.Services.Request.Abstraction;
using apiPB.Services.Utils.Abstraction;

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

            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>()))
                .Returns(new BadRequestObjectResult("La richiesta non può essere vuota."));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>()))
                .Returns(new NotFoundObjectResult("Non risultato trovato."));
        }

        // --- Test per GetAllWorkers ---
        [Fact]
        public void GetAllWorkers_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<WorkerDto> { _sampleWorkerDto };
            _workerRequestServiceMock.Setup(service => service.GetWorkers()).Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<WorkerDto>>(), false))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetAllWorkers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<List<WorkerDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleWorkerDto.WorkerId, returnValue.First().WorkerId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<WorkerDto>>(), false), Times.Once);
        }

        [Fact]
        public void GetAllWorkers_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<WorkerDto>();
            _workerRequestServiceMock.Setup(service => service.GetWorkers()).Returns(emptyList);

            // Act
            var result = _controller.GetAllWorkers();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false), Times.Once);
        }

        // --- I test commentati sono deprecati e non necessari --- //

        // // --- Test per GetWorkersFieldsById ---
        // [Fact]
        // public void GetWorkersFieldsById_ShouldReturnOkResult_WhenDataExists()
        // {
        //     // Arrange
        //     int id = 43;
        //     var mockData = new List<WorkersFieldDto> { _sampleWorkersFieldDto };
        //     _workerRequestServiceMock.Setup(service => service.GetWorkersFieldsById(It.Is<WorkersFieldRequestDto>(dto => dto.WorkerId == id)))
        //         .Returns(mockData);
        //     MockRequestPath("GET", $"/api/worker/workersfield/{id}");

        //     // Act
        //     var result = _controller.GetWorkersFieldsById(id);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     Assert.Equal(200, okResult.StatusCode);
        //     var returnValue = Assert.IsType<List<WorkersFieldDto>>(okResult.Value);
        //     Assert.Single(returnValue);
        //     Assert.Equal(_sampleWorkersFieldDto.WorkerId, returnValue.First().WorkerId);
        //     _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", It.IsAny<List<WorkersFieldDto>>(), false), Times.Once);
        // }

        // [Fact]
        // public void GetWorkersFieldsById_ShouldReturnNotFound_WhenNoDataExists()
        // {
        //     // Arrange
        //     _workerRequestServiceMock.Setup(service => service.GetWorkersFieldsById(It.IsAny<WorkersFieldRequestDto>()))
        //         .Returns((IEnumerable<WorkersFieldDto>)null);
        //     MockRequestPath("GET", "/api/worker/workersfield/43");

        //     // Act
        //     var result = _controller.GetWorkersFieldsById(43);

        //     // Assert
        //     var notFoundResult = Assert.IsType<NotFoundResult>(result);
        //     Assert.Equal(404, notFoundResult.StatusCode);
        //     _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        // }
        
        // // --- Test per UpdateOrCreateLastLogin ---
        // [Fact]
        // public async Task UpdateOrCreateLastLogin_ShouldReturnCreatedResult_WhenDataExists()
        // {
        //     // Arrange
        //     _workerRequestServiceMock.Setup(service => service.UpdateOrCreateLastLogin(It.IsAny<PasswordWorkersRequestDto>()))
        //         .ReturnsAsync(_sampleWorkersFieldDto);
        //     MockRequestPath("POST", "/api/worker/lastlogin");

        //     // Act
        //     var result = await _controller.UpdateOrCreateLastLogin(_samplePasswordRequestDto);

        //     // Assert
        //     var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        //     Assert.Equal(201, createdResult.StatusCode);
        //     Assert.Equal("GetWorkersFieldsById", createdResult.ActionName);
        //     Assert.Equal(43, createdResult.RouteValues["id"]);
        //     var returnValue = Assert.IsType<WorkersFieldDto>(createdResult.Value);
        //     Assert.Equal(_sampleWorkersFieldDto.WorkerId, returnValue.WorkerId);
        //     _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 201, "Created", false), Times.Once);
        // }

        // [Fact]
        // public async Task UpdateOrCreateLastLogin_ShouldReturnNotFound_WhenNoDataExists()
        // {
        //     // Arrange
        //     _workerRequestServiceMock.Setup(service => service.UpdateOrCreateLastLogin(It.IsAny<PasswordWorkersRequestDto>()))
        //         .ReturnsAsync((WorkersFieldDto)null);
        //     MockRequestPath("POST", "/api/worker/lastlogin");

        //     // Act
        //     var result = await _controller.UpdateOrCreateLastLogin(_samplePasswordRequestDto);

        //     // Assert
        //     var notFoundResult = Assert.IsType<NotFoundResult>(result);
        //     Assert.Equal(404, notFoundResult.StatusCode);
        //     _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        // }

        // --- Test per LoginWithPassword ---
        [Fact]
        public void LoginWithPassword_ShouldReturnOkResult_WhenCredentialsAreValid()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.LoginWithPassword(It.IsAny<PasswordWorkersRequestDto>()))
                .Returns(_sampleWorkerDto);
            _responseHandlerMock.Setup(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<WorkerDto>(), false))
                .Returns(new OkObjectResult(_sampleWorkerDto));

            // Act
            var result = _controller.LoginWithPassword(_samplePasswordRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<WorkerDto>(okResult.Value);
            Assert.Equal(_sampleWorkerDto.WorkerId, returnValue.WorkerId);
            _responseHandlerMock.Verify(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<WorkerDto>(), false), Times.Once);
        }

        [Fact]
        public void LoginWithPassword_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            PasswordWorkersRequestDto nullRequest = null;

            // Act
            var result = _controller.LoginWithPassword(nullRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false), Times.Once);
        }

        [Fact]
        public void LoginWithPassword_ShouldReturnNotFound_WhenCredentialsAreInvalid()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.LoginWithPassword(It.IsAny<PasswordWorkersRequestDto>()))
                .Returns((WorkerDto)null);

            // Act
            var result = _controller.LoginWithPassword(_samplePasswordRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false), Times.Once);
        }
    }
}