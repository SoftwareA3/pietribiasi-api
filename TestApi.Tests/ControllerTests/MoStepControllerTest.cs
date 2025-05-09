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

        public MoStepControllerTest()
        {
            _moStepRequestServiceMock = new Mock<IMoStepRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new MoStepController(_responseHandlerMock.Object, _moStepRequestServiceMock.Object);

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

            // Setup ResponseHandler service
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>()))
                .Returns(new BadRequestObjectResult("La richiesta non può essere vuota."));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>()))
                .Returns(new NotFoundObjectResult("Non risultato trovato."));
        }

        // --- Tests for GetVwApiMostepWithJob ---
        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepDto>>(), 
                false)).Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Job, returnValue.First().Job);
            Assert.Equal(_sampleMostepDto.Operation, returnValue.First().Operation);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepDto>>(), 
                false), Times.Once);
        }

        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Returns(new List<MostepDto>());

            // Act
            var result = _controller.GetVwApiMostepWithJob(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(
                It.IsAny<HttpContext>(), 
                false), Times.Once);
        }

        [Fact]
        public void GetVwApiMostepWithJob_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<MostepDto>();
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithJob(It.IsAny<JobRequestDto>()))
                .Returns(emptyList);

            // Act
            var result = _controller.GetVwApiMostepWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(
                It.IsAny<HttpContext>(), 
                false), Times.Once);
        }

        // --- Tests for GetMostepWithMono ---
        [Fact]
        public void GetMostepWithMono_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepDto>>(), 
                false)).Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepWithMono(_sampleMonoRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Mono, returnValue.First().Mono);
            Assert.Equal(_sampleMostepDto.Job, returnValue.First().Job);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepDto>>(), 
                false), Times.Once);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Returns(new List<MostepDto>());

            // Act
            var result = _controller.GetMostepWithMono(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(
                It.IsAny<HttpContext>(), 
                false), Times.Once);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepDto>();
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithMono(It.IsAny<MonoRequestDto>()))
                .Returns(emptyList);

            // Act
            var result = _controller.GetMostepWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(
                It.IsAny<HttpContext>(), 
                false), Times.Once);
        }

        // --- Tests for GetMostepWithOperation ---
        [Fact]
        public void GetMostepWithOperation_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepDto> { _sampleMostepDto };
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepDto>>(), 
                false)).Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepWithOperation(_sampleOperationRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepDto.Operation, returnValue.First().Operation);
            Assert.Equal(_sampleMostepDto.Mono, returnValue.First().Mono);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepDto>>(), 
                false), Times.Once);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Returns(new List<MostepDto>());

            // Act
            var result = _controller.GetMostepWithOperation(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(
                It.IsAny<HttpContext>(), 
                false), Times.Once);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepDto>();
            _moStepRequestServiceMock.Setup(service => service.GetMostepWithOperation(It.IsAny<OperationRequestDto>()))
                .Returns(emptyList);

            // Act
            var result = _controller.GetMostepWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(
                It.IsAny<HttpContext>(), 
                false), Times.Once);
        }
    }
}