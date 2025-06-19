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
using apiPB.Utils.Implementation;
using Microsoft.AspNetCore.Http.HttpResults;
using apiPB.Filters;

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
            Imported = false,
            UserImp = null,
            DataImp = null
        };

        private readonly ComponentRequestDto _sampleComponentRequest = new ComponentRequestDto
        {
            Component = "COMP456"
        };

        private readonly ViewPrelMatRequestDto _sampleViewRequest = new ViewPrelMatRequestDto
        {
            WorkerId = 43,
            FromDateTime = new DateTime(2023, 1, 1),
            ToDateTime = new DateTime(2023, 12, 31)
        };

        private readonly PrelMatRequestDto _samplePostRequest = new PrelMatRequestDto
        {
            WorkerId = 43,
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
        };

        private readonly UpdateImportedIdRequestDto _sampleUpdateRequest = new UpdateImportedIdRequestDto
        {
            Id = 1
        };

        private readonly ViewPrelMatPutRequestDto _samplePutRequest = new ViewPrelMatPutRequestDto
        {
            PrelMatId = 1,
            PrelQty = 10.0
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

            // Setup per HttpContext (necessario per HandleOkAndList/HandleOkAndItem, ecc.)
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser"),
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Setup di default per i metodi del response handler che restituiscono IActionResult
            _responseHandlerMock.Setup(rh => rh.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns((HttpContext ctx, IEnumerable<PrelMatDto> list, bool logActive, string message) => new OkObjectResult(list));
            _responseHandlerMock.Setup(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<PrelMatDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns((HttpContext ctx, PrelMatDto item, bool logActive, string message) => new OkObjectResult(item));
            _responseHandlerMock.Setup(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));
        }

        [Fact]
        public void GetAllPrelMat_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var expectedList = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.GetAppPrelMat())
                .Returns(expectedList);

            // Act
            var result = _controller.GetAllPrelMat();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PrelMatDto>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Single(returnValue);
            _responseHandlerMock.Verify(rh => rh.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }


        [Fact]
        public void GetAllPrelMat_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetAppPrelMat())
                .Throws(new ArgumentNullException("service"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetAllPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAllPrelMat_ShouldReturnNoContent_WhenServiceThrowsExpectedEmptyListException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetAppPrelMat())
                .Throws(new ExpectedEmptyListException("PrelMatController", "GetAllPrelMat", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());

            // Act
            var result = _controller.GetAllPrelMat();

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAllPrelMat_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetAppPrelMat())
                .Throws(new EmptyListException("PrelMatController", "GetAllPrelMat", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found")); // Mock specifico per questo caso

            // Act
            var result = _controller.GetAllPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetAllPrelMat_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetAppPrelMat())
                .Throws(new Exception("Test exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found")); // Mock specifico per questo caso

            // Act
            var result = _controller.GetAllPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMatWithFilter_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var expectedList = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedAppPrelMatByFilter(It.IsAny<ViewPrelMatRequestDto>()))
                .Returns(expectedList);

            // Act
            var result = _controller.GetNotImportedPrelMatWithFilter(_sampleViewRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PrelMatDto>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Single(returnValue);
            _responseHandlerMock.Verify(rh => rh.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMatWithFilter_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.GetNotImportedPrelMatWithFilter(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMatWithFilter_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedAppPrelMatByFilter(It.IsAny<ViewPrelMatRequestDto>()))
                .Throws(new EmptyListException("PrelMatController", "GetNotImportedPrelMatWithFilter", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedPrelMatWithFilter(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMatWithFilter_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedAppPrelMatByFilter(It.IsAny<ViewPrelMatRequestDto>()))
                .Throws(new ArgumentNullException("service"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedPrelMatWithFilter(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMatWithFilter_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedAppPrelMatByFilter(It.IsAny<ViewPrelMatRequestDto>()))
                .Throws(new Exception("Generic exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedPrelMatWithFilter(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public void PostPrelMatList_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var requestList = new List<PrelMatRequestDto> { _samplePostRequest };
            var expectedResult = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.PostPrelMatList(It.IsAny<IEnumerable<PrelMatRequestDto>>()))
                .Returns(expectedResult);

            // Act
            var result = _controller.PostPrelMatList(requestList);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PrelMatDto>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Single(returnValue);
            _responseHandlerMock.Verify(rh => rh.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            var requestList = new List<PrelMatRequestDto> { _samplePostRequest };
            _prelMatRequestServiceMock.Setup(service => service.PostPrelMatList(It.IsAny<IEnumerable<PrelMatRequestDto>>()))
                .Throws(new EmptyListException("PrelMatController", "PostPrelMatList", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PostPrelMatList(requestList);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ShouldReturnBadRequest_WhenRequestListIsNull()
        {
            // Act
            var result = _controller.PostPrelMatList(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ShouldReturnBadRequest_WhenRequestListIsEmpty()
        {
            // Act
            var result = _controller.PostPrelMatList(new List<PrelMatRequestDto>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public void PostPrelMatList_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            var requestList = new List<PrelMatRequestDto> { _samplePostRequest };
            _prelMatRequestServiceMock.Setup(service => service.PostPrelMatList(It.IsAny<IEnumerable<PrelMatRequestDto>>()))
                .Throws(new ArgumentNullException("service"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PostPrelMatList(requestList);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            var requestList = new List<PrelMatRequestDto> { _samplePostRequest };
            _prelMatRequestServiceMock.Setup(service => service.PostPrelMatList(It.IsAny<IEnumerable<PrelMatRequestDto>>()))
                .Throws(new Exception("Generic exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PostPrelMatList(requestList);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // -- Test per GetViewPrelMat --
        [Fact]
        public void GetViewPrelMat_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var expectedList = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.GetViewPrelMatList(It.IsAny<ViewPrelMatRequestDto>()))
                .Returns(expectedList);

            // Act
            var result = _controller.GetViewPrelMat(_sampleViewRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PrelMatDto>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Single(returnValue);
            _responseHandlerMock.Verify(rh => rh.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void GetViewPrelMat_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.GetViewPrelMat(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetViewPrelMat_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetViewPrelMatList(It.IsAny<ViewPrelMatRequestDto>()))
                .Throws(new EmptyListException("PrelMatController", "GetViewPrelMat", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetViewPrelMat(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetViewPrelMat_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetViewPrelMatList(It.IsAny<ViewPrelMatRequestDto>()))
                .Throws(new ArgumentNullException("service"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetViewPrelMat(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetViewPrelMat_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetViewPrelMatList(It.IsAny<ViewPrelMatRequestDto>()))
                .Throws(new Exception("Generic exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetViewPrelMat(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // -- Test per PutViewPrelMat --

        [Fact]
        public void PutViewPrelMat_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var expectedList = _samplePrelMatDto;
            _prelMatRequestServiceMock.Setup(service => service.PutViewPrelMat(It.IsAny<ViewPrelMatPutRequestDto>()))
                .Returns(expectedList);

            // Act
            var result = _controller.PutViewPrelMat(_samplePutRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PrelMatDto>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<PrelMatDto>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void PutViewPrelMat_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.PutViewPrelMat(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PutViewPrelMat_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.PutViewPrelMat(It.IsAny<ViewPrelMatPutRequestDto>()))
                .Throws(new EmptyListException("PrelMatController", "PutViewPrelMat", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PutViewPrelMat(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PutViewPrelMat_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.PutViewPrelMat(It.IsAny<ViewPrelMatPutRequestDto>()))
                .Throws(new ArgumentNullException("service"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PutViewPrelMat(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void PutViewPrelMat_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.PutViewPrelMat(It.IsAny<ViewPrelMatPutRequestDto>()))
                .Throws(new Exception("Generic exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PutViewPrelMat(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // -- Test per GetNotImportedPrelMat --

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var expectedList = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedPrelMat())
                .Returns(expectedList);

            // Act
            var result = _controller.GetNotImportedPrelMat();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PrelMatDto>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Single(returnValue);
            _responseHandlerMock.Verify(rh => rh.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedPrelMat())
                .Throws(new EmptyListException("PrelMatController", "GetNotImportedPrelMat", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnNoContent_WhenServiceThrowsExpectedEmptyListException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedPrelMat())
                .Throws(new ExpectedEmptyListException("PrelMatController", "GetNotImportedPrelMat", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());

            // Act
            var result = _controller.GetNotImportedPrelMat();

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedPrelMat())
                .Throws(new Exception("Test exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedPrelMat())
                .Throws(new ArgumentNullException("service"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedPrelMat())
                .Throws(new EmptyListException("PrelMatController", "GetNotImportedPrelMat", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnNotFound_WhenServiceThrowsExeption()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetNotImportedPrelMat())
                .Throws(new Exception("Test exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetNotImportedPrelMat();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // -- Test per DeletePrelMatId --

        [Fact]
        public void DeletePrelMatId_ShouldReturnOk_WhenServiceDeletesSuccessfully()
        {
            // Arrange
            var deletedDto = _samplePrelMatDto;
            _prelMatRequestServiceMock.Setup(service => service.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteRequestDto>()))
                .Returns(deletedDto);

            // Act
            var result = _controller.DeletePrelMatId(_sampleDeleteRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PrelMatDto>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(deletedDto.PrelMatId, returnValue.PrelMatId);
            _responseHandlerMock.Verify(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<PrelMatDto>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void DeletePrelMatId_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteRequestDto>()))
                .Throws(new ArgumentNullException("service"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.DeletePrelMatId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DeletePrelMatId_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.DeletePrelMatId(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DeletePrelMatId_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteRequestDto>()))
                .Throws(new NullReferenceException("No PrelMat found with the given ID"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.DeletePrelMatId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DeletePrelMatId_ShouldReturnNotFound_WhenServiceThrowsEmptyListException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteRequestDto>()))
                .Throws(new EmptyListException("PrelMatController", "DeletePrelMatId", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.DeletePrelMatId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void DeletePrelMatId_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteRequestDto>()))
                .Throws(new Exception("Test exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.DeletePrelMatId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        // -- Test per GetPrelMatWithComponent --
        [Fact]
        public void GetPrelMatWithComponent_ShouldReturnOk_WhenServiceReturnsData()
        {
            // Arrange
            var expectedList = new List<PrelMatDto> { _samplePrelMatDto };
            _prelMatRequestServiceMock.Setup(service => service.GetPrelMatWithComponent(It.IsAny<ComponentRequestDto>()))
                .Returns(expectedList);

            // Act
            var result = _controller.GetPrelMatWithComponent(_sampleComponentRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PrelMatDto>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Single(returnValue);
            _responseHandlerMock.Verify(rh => rh.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<PrelMatDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void GetPrelMatWithComponent_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.GetPrelMatWithComponent(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetPrelMatWithComponent_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetPrelMatWithComponent(It.IsAny<ComponentRequestDto>()))
                .Throws(new EmptyListException("PrelMatController", "GetPrelMatWithComponent", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetPrelMatWithComponent(_sampleComponentRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetPrelMatWithComponent_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetPrelMatWithComponent(It.IsAny<ComponentRequestDto>()))
                .Throws(new ArgumentNullException("service"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetPrelMatWithComponent(_sampleComponentRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetPrelMatWithComponent_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetPrelMatWithComponent(It.IsAny<ComponentRequestDto>()))
                .Throws(new Exception("Generic exception"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetPrelMatWithComponent(_sampleComponentRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetPrelMatWithComponent_ShouldReturnNoContent_WhenServiceThrowsExpectedEmptyListException()
        {
            // Arrange
            _prelMatRequestServiceMock.Setup(service => service.GetPrelMatWithComponent(It.IsAny<ComponentRequestDto>()))
                .Throws(new ExpectedEmptyListException("PrelMatController", "GetPrelMatWithComponent", "Lista vuota"));
            _responseHandlerMock.Setup(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());

            // Act
            var result = _controller.GetPrelMatWithComponent(_sampleComponentRequest);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
        }
    }
}