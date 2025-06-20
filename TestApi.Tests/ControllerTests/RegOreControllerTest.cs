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
using Microsoft.AspNetCore.Http.HttpResults;
using apiPB.Utils.Implementation;

namespace TestApi.Tests.ControllerTests
{
    public class RegOreControllerTests
    {
        private readonly Mock<IRegOreRequestService> _regOreRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
        private readonly RegOreController _controller;

        // DTO utilizzati per i test
        private readonly RegOreDto _sampleRegOreDto = new RegOreDto
        {
            RegOreId = 1,
            WorkerId = 43,
            SavedDate = new DateTime(2023, 10, 1, 12, 0, 0),
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
            Uom = "HR",
            ProductionQty = 100,
            ProducedQty = 75,
            ResQty = 25,
            Storage = "WH01",
            Wc = "WC01",
            WorkingTime = 480,
            Imported = false,
            UserImp = "User123",
            DataImp = new DateTime(2023, 10, 1, 12, 0, 0)
        };

        private readonly RegOreRequestDto _sampleRegOreRequestDto = new RegOreRequestDto
        {
            WorkerId = 43,
            Job = "JOB123",
            RtgStep = 1,
            Alternate = "ALT1",
            AltRtgStep = 2,
            Operation = "OP123",
            Bom = "BOM123",
            Variant = "VAR1",
            Moid = 100,
            Mono = "MO123",
            UserImp = "User123",
            DataImp = new DateTime(2023, 10, 1, 12, 0, 0),
            Uom = "HR",
            WorkingTime = 480
        };

        private readonly ViewOreRequestDto _sampleViewRequest = new ViewOreRequestDto
        {
            WorkerId = 43,
            FromDateTime = new DateTime(2023, 10, 1, 12, 0, 0).AddDays(-1),
            ToDateTime = new DateTime(2023, 10, 1, 12, 0, 0).AddDays(1),
            Job = "JOB123",
            Operation = "OP123",
            Mono = "MO123"
        };

        private readonly ViewOrePutRequestDto _samplePutRequest = new ViewOrePutRequestDto
        {
            RegOreId = 1,
            WorkingTime = 540
        };

        private readonly ViewOreDeleteRequestDto _sampleDeleteRequest = new ViewOreDeleteRequestDto
        {
            RegOreId = 1
        };

        public RegOreControllerTests()
        {
            _regOreRequestServiceMock = new Mock<IRegOreRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new RegOreController(_responseHandlerMock.Object, _regOreRequestServiceMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            // Impostazioni predefinite per Metodo, Path e tipo di richiesta
            httpRequestMock.Setup(r => r.Method).Returns("GET");
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/reg_ore/test"));
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
            _responseHandlerMock.Setup(x => x.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<RegOreDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(_sampleRegOreDto));
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(new List<RegOreDto> { _sampleRegOreDto }));
        }

        // --- Test per getAllRegOre ---
        [Fact]
        public void GetAllRegOre_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<RegOreDto> { _sampleRegOreDto };
            _regOreRequestServiceMock.Setup(service => service.GetAppRegOre()).Returns(mockData);

            // Act
            var result = _controller.GetAllRegOre();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<RegOreDto>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Single(returnValue);
            Assert.Equal(_sampleRegOreDto.RegOreId, returnValue.First().RegOreId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetAllRegOre_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetAppRegOre())
                .Throws(new EmptyListException("RegOreController", "GetAllRegOre", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetAllRegOre();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAllRegOre_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetAppRegOre()).Throws(new ArgumentNullException("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetAllRegOre();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAllRegOre_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetAppRegOre()).Throws(new Exception("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetAllRegOre();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAllRegOre_ShouldReturnNoContent_WhenServiceThrowsExpectedEmptyListException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetAppRegOre()).Throws(new ExpectedEmptyListException("RegOreController", "GetAllRegOre", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());

            // Act
            var result = _controller.GetAllRegOre();

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Test per PostRegOreList ---
        [Fact]
        public void PostRegOreList_ShouldReturnCreatedResult_WhenDataIsProcessed()
        {
            // Arrange
            var requestDtoList = new List<RegOreRequestDto> { _sampleRegOreRequestDto };
            var responseDtoList = new List<RegOreDto> { _sampleRegOreDto };
            _regOreRequestServiceMock.Setup(service => service.PostAppRegOre(It.IsAny<IEnumerable<RegOreRequestDto>>()))
                .Returns(responseDtoList);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(responseDtoList));

            // Act
            var result = _controller.PostRegOreList(requestDtoList);

            // Assert
            var createdResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, createdResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<RegOreDto>>(createdResult.Value);
            Assert.Equal(responseDtoList.Count, returnValue.Count());
            Assert.Equal(responseDtoList.First().RegOreId, returnValue.First().RegOreId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PostRegOreList_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.PostRegOreList(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PostRegOreList_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.PostAppRegOre(It.IsAny<IEnumerable<RegOreRequestDto>>()))
                .Throws(new EmptyListException("RegOreController", "PostRegOreList", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PostRegOreList(new List<RegOreRequestDto> { _sampleRegOreRequestDto });

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PostRegOreList_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.PostAppRegOre(It.IsAny<IEnumerable<RegOreRequestDto>>()))
                .Throws(new Exception("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PostRegOreList(new List<RegOreRequestDto> { _sampleRegOreRequestDto });

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PostRegOreList_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.PostAppRegOre(It.IsAny<IEnumerable<RegOreRequestDto>>()))
                .Throws(new ArgumentNullException("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PostRegOreList(new List<RegOreRequestDto> { _sampleRegOreRequestDto });

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Test per GetA3AppRegOre ---
        [Fact]
        public void GetA3AppRegOre_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<RegOreDto> { _sampleRegOreDto };
            _regOreRequestServiceMock.Setup(service => service.GetAppViewOre(It.IsAny<ViewOreRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), false, "Ok"))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetA3AppRegOre(_sampleViewRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<RegOreDto>>(okResult.Value);
            Assert.Equal(mockData.Count, returnValue.Count());
            Assert.Equal(mockData.First().RegOreId, returnValue.First().RegOreId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetA3AppRegOre_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetAppViewOre(It.IsAny<ViewOreRequestDto>()))
                .Throws(new EmptyListException("RegOreController", "GetA3AppRegOre", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetA3AppRegOre(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetA3AppRegOre_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetAppViewOre(It.IsAny<ViewOreRequestDto>()))
                .Throws(new ArgumentNullException("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetA3AppRegOre(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetA3AppRegOre_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetA3AppRegOre(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetA3AppRegOre_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetAppViewOre(It.IsAny<ViewOreRequestDto>()))
                .Throws(new Exception("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetA3AppRegOre(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Test per PutA3AppRegOre ---
        [Fact]
        public void PutA3AppRegOre_ShouldReturnOkResult_WhenDataIsUpdated()
        {
            // Arrange
            var updatedDto = new RegOreDto
            {
                RegOreId = _samplePutRequest.RegOreId,
                WorkerId = _sampleRegOreDto.WorkerId,
                SavedDate = DateTime.Now,
                Job = _sampleRegOreDto.Job,
                Operation = _sampleRegOreDto.Operation,
                Bom = _sampleRegOreDto.Bom,
                Variant = _sampleRegOreDto.Variant,
                WorkingTime = _samplePutRequest.WorkingTime
            };
            _regOreRequestServiceMock.Setup(service => service.PutAppViewOre(It.IsAny<ViewOrePutRequestDto>()))
                .Returns(updatedDto);
            _responseHandlerMock.Setup(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<RegOreDto>(), false, "Ok"))
                .Returns(new OkObjectResult(updatedDto));

            // Act
            var result = _controller.PutA3AppRegOre(_samplePutRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<RegOreDto>(okResult.Value);
            Assert.Equal(updatedDto.RegOreId, returnValue.RegOreId);
            Assert.Equal(updatedDto.WorkingTime, returnValue.WorkingTime);
            _responseHandlerMock.Verify(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<RegOreDto>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void PutA3AppRegOre_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.PutA3AppRegOre(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), false, "Bad Request"), Times.Once);
        }

        [Fact]
        public void PutA3AppRegOre_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.PutAppViewOre(It.IsAny<ViewOrePutRequestDto>()))
                .Throws(new EmptyListException("RegOreController", "PutA3AppRegOre", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PutA3AppRegOre(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PutA3AppRegOre_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.PutAppViewOre(It.IsAny<ViewOrePutRequestDto>()))
                .Throws(new Exception("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PutA3AppRegOre(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PutA3AppRegOre_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.PutAppViewOre(It.IsAny<ViewOrePutRequestDto>()))
                .Throws(new ArgumentNullException("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PutA3AppRegOre(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Test per DeleteRegOreId ---
        [Fact]
        public void DeleteRegOreId_ShouldReturnOkResult_WhenDataIsDeleted()
        {
            // Arrange
            var deletedDto = _sampleRegOreDto;
            _regOreRequestServiceMock.Setup(service => service.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestDto>()))
                .Returns(deletedDto);
            _responseHandlerMock.Setup(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<RegOreDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(deletedDto));

            // Act
            var result = _controller.DeleteRegOreId(_sampleDeleteRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<RegOreDto>(okResult.Value);
            Assert.Equal(deletedDto.RegOreId, returnValue.RegOreId);
            _responseHandlerMock.Verify(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<RegOreDto>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DeleteRegOreId_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.DeleteRegOreId(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DeleteRegOreId_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestDto>()))
                .Throws(new EmptyListException("RegOreController", "DeleteRegOreId", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.DeleteRegOreId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DeleteRegOreId_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestDto>()))
                .Throws(new ArgumentNullException("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.DeleteRegOreId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DeleteRegOreId_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestDto>()))
                .Throws(new Exception("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.DeleteRegOreId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // --- Test per GetRegOreNotImported ---
        [Fact]
        public void GetRegOreNotImported_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<RegOreDto> { _sampleRegOreDto };
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOre())
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), false, "Ok"))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetRegOreNotImported();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<RegOreDto>>(okResult.Value);
            Assert.Equal(mockData.Count, returnValue.Count());
            Assert.Equal(mockData.First().RegOreId, returnValue.First().RegOreId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetRegOreNotImported_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOre())
                .Throws(new EmptyListException("RegOreController", "GetRegOreNotImported", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetRegOreNotImported();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetRegOreNotImported_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOre())
                .Throws(new ArgumentNullException("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetRegOreNotImported();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetRegOreNotImported_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOre())
                .Throws(new Exception("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetRegOreNotImported();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetRegOreNotImported_ShouldReturnNoContent_WhenServiceThrowsExpectedEmptyListException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOre())
                .Throws(new ExpectedEmptyListException("RegOreController", "GetRegOreNotImported", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());

            // Act
            var result = _controller.GetRegOreNotImported();

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }


        // -- Test per GetNotImportedAppRegOre ---

        [Fact]
        public void GetNotImportedAppRegOre_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<RegOreDto> { _sampleRegOreDto };
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOre())
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), false, "Ok"))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetNotImportedAppRegOre(_sampleViewRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<RegOreDto>>(okResult.Value);
            Assert.Equal(mockData.Count, returnValue.Count());
            Assert.Equal(mockData.First().RegOreId, returnValue.First().RegOreId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<RegOreDto>>(), false, "Ok"), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppRegOre_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOreByFilter(_sampleViewRequest))
                .Throws(new EmptyListException("RegOreController", "GetNotImportedAppRegOre", "Lista vuota"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedAppRegOre(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppRegOre_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOreByFilter(_sampleViewRequest))
                .Throws(new ArgumentNullException("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedAppRegOre(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppRegOre_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetNotImportedAppRegOre(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppRegOre_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetNotImportedAppRegOreByFilter(_sampleViewRequest))
                .Throws(new Exception("Service error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedAppRegOre(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        } 
    }
}