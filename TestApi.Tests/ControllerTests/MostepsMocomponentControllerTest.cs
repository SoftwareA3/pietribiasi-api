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

namespace TestApi.Tests.ControllerTests
{
    public class MostepsMocomponentControllerTest
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

        public MostepsMocomponentControllerTest()
        {
            _requestServiceMock = new Mock<IMostepsMocomponentRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new MostepsMocomponentController(_responseHandlerMock.Object, _requestServiceMock.Object);

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
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "La richiesta non può essere vuota."))
                .Returns(new BadRequestObjectResult("La richiesta non può essere vuota."));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Non risultato trovato."))
                .Returns(new NotFoundObjectResult("Non risultato trovato."));
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<MostepsMocomponentDto>>(), It.IsAny<bool>(), "Ok"))
                .Returns(new OkObjectResult(new List<MostepsMocomponentDto>()));
            _responseHandlerMock.Setup(x => x.HandleOk(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Ok"))
                .Returns(new OkObjectResult("Ok"));
        }

        // --- Tests for GetMostepsMocomponentWithJob ---

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepsMocomponentDto>>(), 
                false, "Ok")).Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(_sampleJobRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Job, returnValue.First().Job);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), 
                It.Is<List<MostepsMocomponentDto>>(list => list.Count == 1), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnBadRequest_WhenJobIsNull()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Throws(new ArgumentNullException("JobRequestDto cannot be null"));
            _responseHandlerMock.Setup(log => log.HandleBadRequest(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), "Bad Request")).Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithJob_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepsMocomponentDto>();
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentJobDistinct(It.IsAny<JobRequestDto>()))
                .Returns(emptyList);
            _responseHandlerMock.Setup(log => log.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), "Not Found")).Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithJob(_sampleJobRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithMono ---

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepsMocomponentDto>>(), 
                false, "Ok")).Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(_sampleMonoRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Mono, returnValue.First().Mono);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), 
                It.Is<List<MostepsMocomponentDto>>(list => list.Count == 1), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Throws(new ArgumentNullException("MonoRequestDto cannot be null"));
            _responseHandlerMock.Setup(log => log.HandleBadRequest(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), "Bad Request")).Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithMono_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepsMocomponentDto>();
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentMonoDistinct(It.IsAny<MonoRequestDto>()))
                .Returns(emptyList);
            _responseHandlerMock.Setup(log => log.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), "Not Found")).Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithMono(_sampleMonoRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithOperation ---

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepsMocomponentDto>>(), 
                false, "Ok")).Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(_sampleOperationRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.Operation, returnValue.First().Operation);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), 
                It.Is<List<MostepsMocomponentDto>>(list => list.Count == 1), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Throws(new ArgumentNullException("OperationRequestDto cannot be null"));
            _responseHandlerMock.Setup(log => log.HandleBadRequest(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), "Bad Request")).Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithOperation_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepsMocomponentDto>();
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentOperationDistinct(It.IsAny<OperationRequestDto>()))
                .Returns(emptyList);
            _responseHandlerMock.Setup(log => log.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), "Not Found")).Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithOperation(_sampleOperationRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }

        // --- Tests for GetMostepsMocomponentWithBarCode ---

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MostepsMocomponentDto> { _sampleMostepsMocomponentDto };
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<MostepsMocomponentDto>>(), 
                false, "Ok")).Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(_sampleBarCodeRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MostepsMocomponentDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleMostepsMocomponentDto.BarCode, returnValue.First().BarCode);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), 
                It.Is<List<MostepsMocomponentDto>>(list => list.Count == 1), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Throws(new ArgumentNullException("BarCodeRequestDto cannot be null"));
            _responseHandlerMock.Setup(log => log.HandleBadRequest(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), "Bad Request")).Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }

        [Fact]
        public void GetMostepsMocomponentWithBarCode_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<MostepsMocomponentDto>();
            _requestServiceMock.Setup(service => service.GetMostepsMocomponentBarCodeDistinct(It.IsAny<BarCodeRequestDto>()))
                .Returns(emptyList);
            _responseHandlerMock.Setup(log => log.HandleNotFound(
                It.IsAny<HttpContext>(), 
                It.IsAny<bool>(), "Not Found")).Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetMostepsMocomponentWithBarCode(_sampleBarCodeRequestDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }
    }
}