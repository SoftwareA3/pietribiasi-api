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
using apiPB.Services.Utils.Abstraction;
using apiPB.Services.Request.Abstraction;

namespace TestApi.Tests.ControllerTests
{
    public class MostepsMocomponentControllerTest
    {
        private readonly Mock<IMostepsMocomponentRequestService> _requestServiceMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly MostepsMocomponentController _controller;

        // Sample DTOs for testing
        private readonly MostepsMocomponentDto _sampleMostepsMocomponentDto = new MostepsMocomponentDto
        {
            Job = "JOB123",
            RtgStep = 1,
            Alternate = "ALT1",
            AltRtgStep = 1,
            Operation = "OP123",
            OperDesc = "Operation Description",
            Position = 1,
            Component = "COMP123",
            Bom = "BOM123",
            Variant = "VAR1",
            ItemDesc = "Item Description",
            Moid = 100,
            Mono = "MONO123",
            CreationDate = new DateTime(2023, 10, 1, 12, 0, 0),
            UoM = "PCS",
            ProductionQty = 10.0,
            ProducedQty = 5.0,
            ResQty = 5.0,
            Storage = "MAIN",
            BarCode = "BC123456",
            Wc = "WC1"
        };

        private readonly JobRequestDto _sampleJobRequestDto = new JobRequestDto
        {
            Job = "JOB123"
        };

        private readonly MonoRequestDto _sampleMonoRequestDto = new MonoRequestDto
        {
            Job = "JOB123",
            Mono = "MONO123",
            CreationDate = new DateTime(2023, 10, 1, 12, 0, 0)
        };

        private readonly OperationRequestDto _sampleOperationRequestDto = new OperationRequestDto
        {
            Job = "JOB123",
            Mono = "MONO123",
            CreationDate = new DateTime(2023, 10, 1, 12, 0, 0),
            Operation = "OP123"
        };

        private readonly BarCodeRequestDto _sampleBarCodeRequestDto = new BarCodeRequestDto
        {
            Job = "JOB123",
            Mono = "MONO123",
            CreationDate = new DateTime(2023, 10, 1, 12, 0, 0),
            Operation = "OP123",
            BarCode = "BC123456"
        };

        public MostepsMocomponentControllerTest()
        {
            _requestServiceMock = new Mock<IMostepsMocomponentRequestService>();
            _logServiceMock = new Mock<ILogService>();

            _controller = new MostepsMocomponentController(_logServiceMock.Object, _requestServiceMock.Object);

            // Configure controller context with mocked HttpContext
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            
            // Default settings for HttpContext
            httpRequestMock.Setup(r => r.Method).Returns("GET");
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/mostepsmocomponent/test"));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            // Set up logging service mocks
            _logServiceMock.Setup(x => x.AppendMessageToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndListToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<List<MostepsMocomponentDto>>(),
                It.IsAny<bool>()));
        }

        // --- Tests for GetMostepsMocomponentWithJob ---

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/mostepsmocomponent/job");

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(_sampleJobRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Job, returnValue.First().Job);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", 
                It.Is<List<MostepsMocomponentDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepsMocomponentDto>();
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Returns(emptyList);
            MockRequestPath("POST", "/api/mostepsmocomponent/job");

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithMono ---

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/mostepsmocomponent/mono");

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(_sampleMonoRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Mono, returnValue.First().Mono);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", 
                It.Is<List<MostepsMocomponentDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepsMocomponentDto>();
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Returns(emptyList);
            MockRequestPath("POST", "/api/mostepsmocomponent/mono");

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithOperation ---

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/mostepsmocomponent/operation");

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(_sampleOperationRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Operation, returnValue.First().Operation);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", 
                It.Is<List<MostepsMocomponentDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepsMocomponentDto>();
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Returns(emptyList);
            MockRequestPath("POST", "/api/mostepsmocomponent/operation");

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithBarCode ---

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/mostepsmocomponent/barcode");

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(_sampleBarCodeRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.BarCode, returnValue.First().BarCode);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", 
                It.Is<List<MostepsMocomponentDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepsMocomponentDto>();
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Returns(emptyList);
            MockRequestPath("POST", "/api/mostepsmocomponent/barcode");

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(_sampleBarCodeRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        // Helper method to mock HttpContext.Request.Method and Path for tests
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