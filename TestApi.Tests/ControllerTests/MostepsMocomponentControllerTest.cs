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
using apiPB.Utils.Abstraction;
using apiPB.Services.Abstraction;
using apiPB.Utils.Implementation;

namespace TestApi.Tests.ControllerTests
{
    public class MostepsMocomponentControllerTests
    {
        private readonly Mock<IMostepsMocomponentRequestService> _requestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
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

        public MostepsMocomponentControllerTests()
        {
            _requestServiceMock = new Mock<IMostepsMocomponentRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new MostepsMocomponentController(_responseHandlerMock.Object, _requestServiceMock.Object);

            var httpContextMock = new Mock<HttpContext>();

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
        }

        // --- Tests for GetMostepsMocomponentWithJob ---

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(_sampleJobRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Job, returnValue.First().Job);
            _responseHandlerMock.Verify(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Throws(new ArgumentNullException("MostepsMocomponentRequestService", "Argomento nullo"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Throws(new EmptyListException("MostepsMocomponentRequestService", "GetMostepsMocomponentJobDistinct", "Lista vuota"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Throws(new Exception("Errore generico"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithMono ---

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(_sampleMonoRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Mono, returnValue.First().Mono);
            _responseHandlerMock.Verify(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Throws(new ArgumentNullException("MostepsMocomponentRequestService", "Argomento nullo"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Throws(new EmptyListException("MostepsMocomponentRequestService", "GetMostepsMocomponentMonoDistinct", "Lista vuota"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Throws(new Exception("Errore generico"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithOperation ---

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(_sampleOperationRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Operation, returnValue.First().Operation);
            _responseHandlerMock.Verify(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Throws(new ArgumentNullException("MostepsMocomponentRequestService", "Argomento nullo"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Throws(new EmptyListException("MostepsMocomponentRequestService", "GetMostepsMocomponentOperationDistinct", "Lista vuota"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Throws(new Exception("Errore generico"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithBarCode ---

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(_sampleBarCodeRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.BarCode, returnValue.First().BarCode);
            _responseHandlerMock.Verify(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Throws(new ArgumentNullException("MostepsMocomponentRequestService", "Argomento nullo"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(_sampleBarCodeRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Throws(new EmptyListException("MostepsMocomponentRequestService", "GetMostepsMocomponentBarCodeDistinct", "Lista vuota"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(_sampleBarCodeRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Throws(new Exception("Errore generico"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(_sampleBarCodeRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }
    }
}