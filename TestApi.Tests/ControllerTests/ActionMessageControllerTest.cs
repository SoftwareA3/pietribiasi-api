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
    public class ActionMessageControllerTest
    {
        private readonly Mock<IActionMessageRequestService> _actionMessageServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
        private readonly ActionMessageController _actionMessageController;

        private readonly ImportedLogMessageDto _testImportedLogMessageDto = new ImportedLogMessageDto
        {
            Moid = 123,
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 2,
            Mono = "Mono1",
            Bom = "Bom1",
            Variant = "Variant1",
            Wc = "WC1",
            Operation = "Operation1",
            WorkerId = 456,
            ActionType = 1
        };

        private readonly ActionMessageListDto _testActionMessageListDto = new ActionMessageListDto
        {
            ActionType = "TestAction",
            Moid = 123,
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 2,
            Mono = "Mono1",
            Bom = "Bom1",
            Variant = "Variant1",
            Wc = "WC1",
            Operation = "Operation1",
            WorkerId = 456,
            ActionMessageDetails = new List<ActionMessageDetailsDto>(),
            OmMessageDetails = new List<OmMessageDetailsDto>()
        };

        public ActionMessageControllerTest()
        {
            _actionMessageServiceMock = new Mock<IActionMessageRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();
            _actionMessageController = new ActionMessageController(_responseHandlerMock.Object, _actionMessageServiceMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            _responseHandlerMock.Setup(x => x.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<ActionMessageListDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(_testActionMessageListDto));
            _responseHandlerMock.Setup(x => x.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));
        }

        [Fact]
        public void GetActionMessagesByFilter_ShouldReturnOkResult_WhenDataExist()
        {
            // Arrange
            _actionMessageServiceMock.Setup(s => s.GetActionMessagesByFilter(It.IsAny<ImportedLogMessageDto>()))
                .Returns(_testActionMessageListDto);
            _responseHandlerMock.Setup(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<ActionMessageListDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(_testActionMessageListDto));

            // Act
            var result = _actionMessageController.GetActionMessagesByFilter(_testImportedLogMessageDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedData = Assert.IsAssignableFrom<ActionMessageListDto>(okResult.Value);
            Assert.Equal(_testActionMessageListDto, returnedData);
        }

        [Fact]
        public void GetActionMessageByFilter_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _actionMessageServiceMock.Setup(s => s.GetActionMessagesByFilter(It.IsAny<ImportedLogMessageDto>()))
                .Throws(new ArgumentNullException("Not Found"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _actionMessageController.GetActionMessagesByFilter(_testImportedLogMessageDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not Found", notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetActionMessagesByFilter_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _actionMessageController.GetActionMessagesByFilter(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void GetActionMessagesByFilter_ShouldReturnNoContent_WhenNoDataFound()
        {
            // Arrange
            _actionMessageServiceMock.Setup(s => s.GetActionMessagesByFilter(It.IsAny<ImportedLogMessageDto>()))
                .Returns((ActionMessageListDto)null);
            _responseHandlerMock.Setup(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());

            // Act
            var result = _actionMessageController.GetActionMessagesByFilter(_testImportedLogMessageDto);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public void GetActionMessagesByFilter_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _actionMessageServiceMock.Setup(s => s.GetActionMessagesByFilter(It.IsAny<ImportedLogMessageDto>()))
                .Throws(new Exception("Service error"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Errore durante l'esecuzione del Service in ActionMessageControllerService error"));

            // Act
            var result = _actionMessageController.GetActionMessagesByFilter(_testImportedLogMessageDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Errore durante l'esecuzione del Service in ActionMessageControllerService error", notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}