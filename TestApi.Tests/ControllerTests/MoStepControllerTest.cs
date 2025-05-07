using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using apiPB.Controllers;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Services;
using apiPB.Services.Request.Abstraction;
using Microsoft.IdentityModel.Tokens;
using apiPB.Services.Utils.Abstraction;

namespace TestApi.Tests.ControllerTests
{
    public class MoStepControllerTest
    {
        private readonly Mock<IMoStepRequestService> _moStepRequestServiceMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly MoStepController _controller;

        // Sample DTOs for testing
        private readonly MostepDto _sampleMostepDto = new MostepDto
        {
            Job = "JOB123",
            Mono = "MONO123",
            Operation = "OP123",
            OperDesc = "Sample Operation Description",
            Moid = 1,
            RtgStep = 10,
            Alternate = "ALT1",
            AltRtgStep = 20,
            Bom = "BOM123",
            Variant = "VAR1",
            ItemDesc = "Sample Item Description",
            CreationDate = new DateTime(2023, 10, 1),
            Uom = "PCS",
            ProductionQty = 100.0,
            ProducedQty = 50.0,
            ResQty = 50.0,
            Storage = "MAIN",
            Wc = "WC01"
        };

        private readonly JobRequestDto _sampleJobRequestDto = new JobRequestDto
        {
            Job = "JOB123"
        };

        private readonly MonoRequestDto _sampleMonoRequestDto = new MonoRequestDto
        {
            Mono = "MONO123",
            Job = "JOB123",
            CreationDate = new DateTime(2023, 10, 1)
        };

        private readonly OperationRequestDto _sampleOperationRequestDto = new OperationRequestDto
        {
            Operation = "OP123",
            Mono = "MONO123",
            Job = "JOB123",
            CreationDate = new DateTime(2023, 10, 1)
        };

        public MoStepControllerTest()
        {
            _moStepRequestServiceMock = new Mock<IMoStepRequestService>();
            _logServiceMock = new Mock<ILogService>();

            _controller = new MoStepController(_logServiceMock.Object, _moStepRequestServiceMock.Object);

            // Setup default HttpContext
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            httpRequestMock.Setup(r => r.Method).Returns("POST");
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/mostep/test"));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            // Setup logging service
            _logServiceMock.Setup(x => x.AppendMessageToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndListToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<List<MostepDto>>(),
                It.IsAny<bool>()));
        }

        // --- Tests for GetVwApiMostepWithJob ---
        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/mostep/job");

            // Act
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Job, returnValue.First().Job);
            Assert.Equal(_sampleMostepDto.Operation, returnValue.First().Operation);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(
                It.IsAny<string>(), 200, "OK", It.Is<List<MostepDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepDto>();
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Returns(emptyList);
            MockRequestPath("POST", "/api/mostep/job");

            // Act
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        [Fact]
        public void GetVwApiMostepWithJob_ShouldHandleNullRequest()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Returns(new List<MostepDto>());
            MockRequestPath("POST", "/api/mostep/job");

            // Act
            var result = _controller.GetVwApiMostepWithJob(null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // --- Tests for GetMostepWithMono ---
        [Fact]
        public void GetMostepWithMono_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/mostep/odp");

            // Act
            var result = _controller.GetMostepWithMono(_sampleMonoRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Mono, returnValue.First().Mono);
            Assert.Equal(_sampleMostepDto.Job, returnValue.First().Job);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(
                It.IsAny<string>(), 200, "OK", It.Is<List<MostepDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepDto>();
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Returns(emptyList);
            MockRequestPath("POST", "/api/mostep/odp");

            // Act
            var result = _controller.GetMostepWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        [Fact]
        public void GetMostepWithMono_ShouldHandleNullRequest()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Returns(new List<MostepDto>());
            MockRequestPath("POST", "/api/mostep/odp");

            // Act
            var result = _controller.GetMostepWithMono(null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // --- Tests for GetMostepWithOperation ---
        [Fact]
        public void GetMostepWithOperation_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/mostep/lavorazioni");

            // Act
            var result = _controller.GetMostepWithOperation(_sampleOperationRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Operation, returnValue.First().Operation);
            Assert.Equal(_sampleMostepDto.Mono, returnValue.First().Mono);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(
                It.IsAny<string>(), 200, "OK", It.Is<List<MostepDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepDto>();
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Returns(emptyList);
            MockRequestPath("POST", "/api/mostep/lavorazioni");

            // Act
            var result = _controller.GetMostepWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(
                It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldHandleNullRequest()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Returns(new List<MostepDto>());
            MockRequestPath("POST", "/api/mostep/lavorazioni");

            // Act
            var result = _controller.GetMostepWithOperation(null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // --- Test for handling empty lists in IsNullOrEmpty extension method ---
        [Fact]
        public void Controller_ShouldHandleEmptyListWithIsNullOrEmpty()
        {
            // Arrange
            var emptyList = new List<MostepDto>();
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Returns(emptyList);
            MockRequestPath("POST", "/api/mostep/job");

            // Act - The controller internally calls IsNullOrEmpty on the result
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert - Verify we get NotFound for empty list
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // Helper method to mock HttpContext for each test
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