using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using apiPB.Controllers;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Services;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;

namespace TestApi.Tests.ControllerTests
{
    public class PrelMatControllerTests
    {
        private readonly Mock<IPrelMatRequestService> _prelMatRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
        private readonly PrelMatController _controller;

        // DTO utilizzati per i test
        private readonly PrelMatDto _samplePrelMatDto = new PrelMatDto
        {
            PrelMatId = 1,
            WorkerId = 43,
            SavedDate = new DateTime(2023, 10, 1, 12, 0, 0),
            Job = "JOB123",
            RtgStep = 1,
            Alternate = "ALT1",
            AltRtgStep = 2,
            Operation = "OP123",
            OperDesc = "Operation Description",
            Position = 1,
            Component = "COMP456",
            Bom = "BOM123",
            Variant = "VAR1",
            ItemDesc = "Item Description",
            Moid = 100,
            Mono = "MO123",
            CreationDate = new DateTime(2023, 9, 1),
            UoM = "PCS",
            ProductionQty = 100,
            ProducedQty = 75,
            ResQty = 25,
            Storage = "WH01",
            BarCode = "BC123456789",
            Wc = "WC01",
            PrelQty = 10,
            Imported = false,
            UserImp = "User123",
            DataImp = new DateTime(2023, 10, 1, 12, 0, 0)
        };

        private readonly PrelMatRequestDto _samplePrelMatRequestDto = new PrelMatRequestDto
        {
            WorkerId = 43,
            Job = "JOB123",
            RtgStep = 1,
            Alternate = "ALT1",
            AltRtgStep = 2,
            Operation = "OP123",
            OperDesc = "Operation Description",
            Bom = "BOM123",
            Variant = "VAR1",
            ItemDesc = "Item Description",
            Moid = 100,
            Mono = "MO123",
            CreationDate = new DateTime(2023, 9, 1),
            ProductionQty = 100,
            ProducedQty = 75,
            ResQty = 25,
            Storage = "WH01",
            Wc = "WC01",
            Imported = false,
            PrelQty = 10,
            UoM = "PCS",
            BarCode = "BC123456789",
            Position = 1,
            Component = "COMP456"
        };

        private readonly ViewPrelMatRequestDto _sampleViewRequest = new ViewPrelMatRequestDto
        {
            WorkerId = 43,
            FromDateTime = new DateTime(2023, 10, 1, 12, 0, 0).AddDays(-1),
            ToDateTime = new DateTime(2023, 10, 1, 12, 0, 0).AddDays(1),
            Job = "JOB123",
            Operation = "OP123",
            Mono = "MO123",
            Component = "COMP456",
            BarCode = "BC123456789"
        };

        private readonly ViewPrelMatPutRequestDto _samplePutRequest = new ViewPrelMatPutRequestDto
        {
            PrelMatId = 1,
            PrelQty = 15
        };

        private readonly ViewPrelMatDeleteRequestDto _sampleDeleteRequest = new ViewPrelMatDeleteRequestDto
        {
            PrelMatId = 1
        };

        public PrelMatControllerTests()
        {
            _prelMatRequestServiceMock = new Mock<IPrelMatRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new PrelMatController(_responseHandlerMock.Object, _prelMatRequestServiceMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            // Impostazioni predefinite per Metodo, Path e tipo di richiesta
            httpRequestMock.Setup(r => r.Method).Returns("GET");
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/prel_mat/test"));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "La richiesta non può essere vuota."))
                .Returns(new BadRequestObjectResult("La richiesta non può essere vuota."));
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Bad Request"))
                .Returns(new BadRequestObjectResult("Bad Request"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"))
                .Returns(new NotFoundObjectResult("Not Found"));
        }

        // --- Test per getAllPrelMat ---
        [Fact]
        public void GetAllPrelMat_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.GetAppPrelMat()).Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), false, "Ok"))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetAllPrelMat();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PrelMatDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_samplePrelMatDto.PrelMatId, returnValue.First().PrelMatId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetAllPrelMat_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<PrelMatDto>();
            _prelMatRequestServiceMock.Setup(service => service.GetAppPrelMat()).Returns(emptyList);

            // Act
            var result = _controller.GetAllPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }

        // --- Test per PostPrelMatList ---
        [Fact]
        public void PostPrelMatList_ShouldReturnOkResult_WhenDataIsProcessed()
        {
            // Arrange
            var requestDtoList = new List<PrelMatRequestDto> { _samplePrelMatRequestDto };
            var responseDtoList = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.PostPrelMatList(It.IsAny<IEnumerable<PrelMatRequestDto>>()))
                .Returns(responseDtoList);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), false, "Ok"))
                .Returns(new OkObjectResult(responseDtoList));

            // Act
            var result = _controller.PostPrelMatList(requestDtoList);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PrelMatDto>>(okResult.Value);
            Assert.Equal(responseDtoList.Count, returnValue.Count());
            Assert.Equal(responseDtoList.First().PrelMatId, returnValue.First().PrelMatId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.PostPrelMatList(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var requestDtoList = new List<PrelMatRequestDto> { _samplePrelMatRequestDto };
            _prelMatRequestServiceMock.Setup(service => service.PostPrelMatList(It.IsAny<IEnumerable<PrelMatRequestDto>>()))
                .Returns(new List<PrelMatDto>());

            // Act
            var result = _controller.PostPrelMatList(requestDtoList);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ShouldReturnBadRequest_WhenRequestIsEmptyList()
        {
            // Act
            var result = _controller.PostPrelMatList(new List<PrelMatRequestDto>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }

        // --- Test per GetViewPrelMat ---
        [Fact]
        public void GetViewPrelMat_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.GetViewPrelMatList(It.IsAny<ViewPrelMatRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), false, "Ok"))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetViewPrelMat(_sampleViewRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PrelMatDto>>(okResult.Value);
            Assert.Equal(mockData.Count, returnValue.Count());
            Assert.Equal(mockData.First().PrelMatId, returnValue.First().PrelMatId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetViewPrelMat_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetViewPrelMatList(It.IsAny<ViewPrelMatRequestDto>()))
                .Returns(new List<PrelMatDto>());

            // Act
            var result = _controller.GetViewPrelMat(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }

        [Fact]
        public void GetViewPrelMat_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.GetViewPrelMat(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }

        // --- Test per PutViewPrelMat ---
        [Fact]
        public void PutViewPrelMat_ShouldReturnOkResult_WhenDataIsUpdated()
        {
            // Arrange
            var updatedDto = new PrelMatDto
            {
                PrelMatId = _samplePutRequest.PrelMatId,
                PrelQty = _samplePutRequest.PrelQty,
                WorkerId = _samplePrelMatDto.WorkerId,
                SavedDate = DateTime.Now,
                Job = _samplePrelMatDto.Job,
                Operation = _samplePrelMatDto.Operation,
                Bom = _samplePrelMatDto.Bom,
                Variant = _samplePrelMatDto.Variant,
                BarCode = _samplePrelMatDto.BarCode
            };
            _prelMatRequestServiceMock.Setup(service => service.PutViewPrelMat(It.IsAny<ViewPrelMatPutRequestDto>()))
                .Returns(updatedDto);
            _responseHandlerMock.Setup(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<PrelMatDto>(), false, "Ok"))
                .Returns(new OkObjectResult(updatedDto));

            // Act
            var result = _controller.PutViewPrelMat(_samplePutRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<PrelMatDto>(okResult.Value);
            Assert.Equal(updatedDto.PrelMatId, returnValue.PrelMatId);
            Assert.Equal(updatedDto.PrelQty, returnValue.PrelQty);
            _responseHandlerMock.Verify(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<PrelMatDto>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void PutViewPrelMat_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.PutViewPrelMat(It.IsAny<ViewPrelMatPutRequestDto>()))
                .Returns((PrelMatDto)null);

            // Act
            var result = _controller.PutViewPrelMat(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }

        // --- Test per DeletePrelMatId ---
        [Fact]
        public void DeletePrelMatId_ShouldReturnOkResult_WhenDataIsDeleted()
        {
            // Arrange
            var deletedDto = _samplePrelMatDto;
            _prelMatRequestServiceMock.Setup(service => service.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteRequestDto>()))
                .Returns(deletedDto);
            _responseHandlerMock.Setup(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<PrelMatDto>(), false, "Ok"))
                .Returns(new OkObjectResult(deletedDto));

            // Act
            var result = _controller.DeletePrelMatId(_sampleDeleteRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<PrelMatDto>(okResult.Value);
            Assert.Equal(deletedDto.PrelMatId, returnValue.PrelMatId);
            _responseHandlerMock.Verify(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<PrelMatDto>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void DeletePrelMatId_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteRequestDto>()))
                .Returns((PrelMatDto)null);

            // Act
            var result = _controller.DeletePrelMatId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), false, "Not Found"), Times.Once);
        }

        [Fact]
        public void DeletePrelMatId_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.DeletePrelMatId(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }
    }
}