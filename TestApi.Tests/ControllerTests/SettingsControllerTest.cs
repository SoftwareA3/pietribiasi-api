using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
    public class SettingsControllerTest
    {
        private readonly Mock<ISettingsRequestService> _settingsServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
        private readonly SettingsController _settingsController;

        private readonly SettingsDto _testSettingsDto = new SettingsDto
        {
            MagoUrl = "http://example.com",
            Username = "testUser",
            Password = "testPassword",
            Company = "TestCompany",
            SpecificatorType = 1,
            Closed = false,
            RectificationReasonPositive = "Positive Reason",
            RectificationReasonNegative = "Negative Reason",
            Storage = "TestStorage",
            SyncGlobalActive = true
        };

        private readonly SyncGobalActiveRequestDto _syncGlobalActiveDto = new SyncGobalActiveRequestDto
        {
            SyncGlobalActive = true
        };

        public SettingsControllerTest()
        {
            _settingsServiceMock = new Mock<ISettingsRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();
            _settingsController = new SettingsController(_settingsServiceMock.Object, _responseHandlerMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            _responseHandlerMock.Setup(x => x.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<SettingsDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(_testSettingsDto));
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<SettingsDto>>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(new List<SettingsDto> { _testSettingsDto }));
            _responseHandlerMock.Setup(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));
        }

        [Fact]
        public void GetSettings_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.GetSettings()).Returns(_testSettingsDto);

            // Act
            var result = _settingsController.GetSettings();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<SettingsDto>(okResult.Value);
            Assert.Equal(_testSettingsDto, okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetSettings_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.GetSettings()).Throws(new ArgumentNullException("Settings not found"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _settingsController.GetSettings();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not Found", notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetSettings_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.GetSettings()).Throws(new Exception("Unexpected error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _settingsController.GetSettings();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not Found", notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // -- Test per EditSettings --
        [Fact]
        public void EditSettings_ShouldReturnOkResult_WhenDataExist()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.EditSettings(It.IsAny<SettingsDto>())).Returns(_testSettingsDto);

            // Act
            var result = _settingsController.EditSettings(_testSettingsDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<SettingsDto>(okResult.Value);
            Assert.Equal(_testSettingsDto, returnValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void EditSettings_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _settingsController.EditSettings(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Richiesta di modifica delle impostazioni non valida"), Times.Once);
        }

        [Fact]
        public void EditSettings_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.EditSettings(It.IsAny<SettingsDto>())).Throws(new ArgumentNullException());

            // Act
            var result = _settingsController.EditSettings(_testSettingsDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void EditSettings_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.EditSettings(It.IsAny<SettingsDto>())).Throws(new Exception("Unexpected error"));

            // Act
            var result = _settingsController.EditSettings(_testSettingsDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // - Test per GetSyncGlobalActive -
        [Fact]
        public void GetSyncGlobalActive_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.GetSyncGlobalActive()).Returns(_syncGlobalActiveDto);
            _responseHandlerMock.Setup(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<SyncGobalActiveRequestDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(_syncGlobalActiveDto));

            // Act
            var result = _settingsController.GetSyncGlobalActive();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<SyncGobalActiveRequestDto>(okResult.Value);
            Assert.Equal(_syncGlobalActiveDto, returnValue);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetSyncGlobalActive_ShouldReturnNotFound_WhenServiceThrowsArgumentNullException()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.GetSyncGlobalActive()).Throws(new ArgumentNullException("SyncGlobalActive not found"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _settingsController.GetSyncGlobalActive();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not Found", notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetSyncGlobalActive_ShouldReturnNotFound_WhenServiceThrowsException()
        {
            // Arrange
            _settingsServiceMock.Setup(s => s.GetSyncGlobalActive()).Throws(new Exception("Unexpected error"));
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _settingsController.GetSyncGlobalActive();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not Found", notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}