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
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;
using apiPB.Utils.Implementation;

namespace TestApi.Tests.ControllerTests
{
    public class MoStepControllerTests
    {
        private readonly Mock<IMoStepRequestService> _moStepRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
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

        public MoStepControllerTests()
        {
            _moStepRequestServiceMock = new Mock<IMoStepRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new MoStepController(_responseHandlerMock.Object, _moStepRequestServiceMock.Object);

            var httpContextMock = new Mock<HttpContext>();

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
        }

        // --- Tests for GetVwApiMostepWithJob ---

        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Job, returnValue.First().Job);
            Assert.Equal(_sampleMostepDto.Operation, returnValue.First().Operation);
            _responseHandlerMock.Verify(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepDto>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetVwApiMostepWithJob(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Throws(new EmptyListException("MoStepRequestService", "GetMostepWithJob", "Lista vuota"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnNotFound_WhenServiceThrowsEseption()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Throws(new Exception("Errore generico"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Throws(new ArgumentNullException("MoStepRequestService", "Argomento nullo"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Tests for GetMostepWithMono ---

        [Fact]
        public void GetMostepWithMono_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepWithMono(_sampleMonoRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Mono, returnValue.First().Mono);
            Assert.Equal(_sampleMostepDto.Job, returnValue.First().Job);
            _responseHandlerMock.Verify(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepDto>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Throws(new ArgumentNullException("MoStepRequestService", "Argomento nullo"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepWithMono(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Throws(new EmptyListException("MoStepRequestService", "GetMostepWithMono", "Lista vuota"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));
            // Act
            var result = _controller.GetMostepWithMono(_sampleMonoRequestDto);
            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Throws(new Exception("Errore generico"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Tests for GetMostepWithOperation ---
        [Fact]
        public void GetMostepWithOperation_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepWithOperation(_sampleOperationRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Operation, returnValue.First().Operation);
            Assert.Equal(_sampleMostepDto.Mono, returnValue.First().Mono);
            _responseHandlerMock.Verify(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepDto>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Throws(new ArgumentNullException("MoStepRequestService", "Argomento nullo"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepWithOperation(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Throws(new EmptyListException("MoStepRequestService", "GetMostepWithOperation", "Lista vuota"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Throws(new Exception("Errore generico"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }
    }
}