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
        private readonly Mock<ILogService> _logServiceMock;
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
            _logServiceMock = new Mock<ILogService>();

            _controller = new WorkerController(_logServiceMock.Object, _workerRequestServiceMock.Object);

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

            _logServiceMock.Setup(x => x.AppendMessageToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndListToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<List<WorkerDto>>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndItemToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<WorkerDto>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndItemToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<WorkersFieldDto>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndItemToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<PasswordWorkersRequestDto>(),
                It.IsAny<bool>()));
        }

        // --- Test per GetAllWorkers ---
        [Fact]
        public void GetAllWorkers_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<WorkerDto> { _sampleWorkerDto };
            _workerRequestServiceMock.Setup(service => service.GetWorkers()).Returns(mockData);

            // Act
            var result = _controller.GetAllWorkers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<List<WorkerDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleWorkerDto.WorkerId, returnValue.First().WorkerId);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(
                It.IsAny<string>(), 
                200, 
                "OK", 
                It.Is<List<WorkerDto>>(list => list.Count == 1), 
                false), 
                Times.Once);
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
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        // I test commentati sono deprecati e non necessari

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
            MockRequestPath("POST", "/api/worker/login");

            // Act
            var result = _controller.LoginWithPassword(_samplePasswordRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<WorkerDto>(okResult.Value);
            Assert.Equal(_sampleWorkerDto.WorkerId, returnValue.WorkerId);
            _logServiceMock.Verify(log => log.AppendMessageAndItemToLog(It.IsAny<string>(), 200, "OK", It.IsAny<WorkerDto>(), false), Times.Once);
        }

        [Fact]
        public void LoginWithPassword_ShouldReturnNotFound_WhenCredentialsAreInvalid()
        {
            // Arrange
            _workerRequestServiceMock.Setup(service => service.LoginWithPassword(It.IsAny<PasswordWorkersRequestDto>()))
                .Returns((WorkerDto)null);
            MockRequestPath("POST", "/api/worker/login");

            // Act
            var result = _controller.LoginWithPassword(_samplePasswordRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageAndItemToLog(It.IsAny<string>(), 404, "Not Found", It.IsAny<PasswordWorkersRequestDto>(), false), Times.Once);
        }

        // Metodo helper per mockare HttpContext.Request.Method e Path per ogni test
        private void MockRequestPath(string method, string path)
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Method).Returns(method);
            httpRequestMock.Setup(r => r.Path).Returns(new PathString(path));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
        }
    }
}