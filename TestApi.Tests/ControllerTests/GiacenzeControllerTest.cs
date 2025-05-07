using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using apiPB.Controllers; 
using apiPB.Dto.Models; 
using apiPB.Services.Request.Abstraction; 
using apiPB.Services; 
using Microsoft.IdentityModel.Tokens; 
using apiPB.Services.Utils.Abstraction;


namespace TestApi.Tests.ControllerTests
{
    public class GiacenzeControllerTests
    {
        private readonly Mock<IGiacenzeRequestService> _giacenzeRequestServiceMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly GiacenzeController _controller;

        // DTO utilizzati per i test
        private readonly GiacenzeDto _sampleGiacenzaDto = new GiacenzeDto
        {
            Item = "ITEM001",
            Description = "Sample Item Description",
            BarCode = "1234567890123",
            FiscalYear = 2024,
            Storage = "MAG01",
            BookInv = 100.0
        };

        public GiacenzeControllerTests()
        {
            _giacenzeRequestServiceMock = new Mock<IGiacenzeRequestService>();
            _logServiceMock = new Mock<ILogService>();

            _controller = new GiacenzeController(_logServiceMock.Object, _giacenzeRequestServiceMock.Object);

            // Mock HttpContext per requestPath e User
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            
            // Impostazioni predefinite per Metodo, Path e tipo di richiesta
            httpRequestMock.Setup(r => r.Method).Returns("GET"); 
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/giacenze/get_all"));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            // Mock User per [Authorize] se necessario (esempio con utente generico autenticato)
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContextMock.Setup(c => c.User).Returns(claimsPrincipal);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            // Setup generico per i metodi di log (_logIsActive è false)
            _logServiceMock.Setup(x => x.AppendMessageToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                false)); 

            _logServiceMock.Setup(x => x.AppendMessageAndListToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<List<GiacenzeDto>>(),
                false)); 
        }

        private void MockRequestPath(string method, string path)
        {
            var httpRequestMock = Mock.Get(_controller.HttpContext.Request);
            httpRequestMock.Setup(r => r.Method).Returns(method);
            httpRequestMock.Setup(r => r.Path).Returns(new PathString(path));
        }

        [Fact]
        public void GetGiacenze_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var expectedPath = "GET api/giacenze/get_all";
            MockRequestPath("GET", "/api/giacenze/get_all");

            var mockDataList = new List<GiacenzeDto> { _sampleGiacenzaDto, new GiacenzeDto { Item = "ITEM002" } };
            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze()).Returns(mockDataList);

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<GiacenzeDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
            Assert.Equal(_sampleGiacenzaDto.Item, returnValue.First().Item);

            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(
                expectedPath, 
                200, 
                "OK", 
                It.Is<List<GiacenzeDto>>(list => list.Count == 2 && list.Contains(_sampleGiacenzaDto)), 
                false), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var expectedPath = "GET api/giacenze/get_all";
            MockRequestPath("GET", "/api/giacenze/get_all");

            var emptyList = new List<GiacenzeDto>();
            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze()).Returns(emptyList);

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            _logServiceMock.Verify(log => log.AppendMessageToLog(
                expectedPath, 
                404, 
                "Not Found", 
                false), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            var expectedPath = "GET api/giacenze/get_all";
            MockRequestPath("GET", "/api/giacenze/get_all");

            var emptyList = new List<GiacenzeDto>(); 
            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze()).Returns(emptyList);


            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            _logServiceMock.Verify(log => log.AppendMessageToLog(
                expectedPath,
                404,
                "Not Found",
                false), Times.Once);
        }
    }
}