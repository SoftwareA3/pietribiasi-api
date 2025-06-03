using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using apiPB.Controllers; 
using apiPB.Dto.Models; 
using apiPB.Services.Abstraction; 
using apiPB.Services; 
using Microsoft.IdentityModel.Tokens; 
using apiPB.Utils.Abstraction;


namespace TestApi.Tests.ControllerTests
{
    public class GiacenzeControllerTests
    {
        private readonly Mock<IGiacenzeRequestService> _giacenzeRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
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

        private readonly List<GiacenzeDto> _sampleGiacenzaDtoList = new List<GiacenzeDto>
        {
            new GiacenzeDto
            {
                Item = "ITEM001",
                Description = "Sample Item Description",
                BarCode = "1234567890123",
                FiscalYear = 2024,
                Storage = "MAG01",
                BookInv = 100.0
            },
            new GiacenzeDto
            {
                Item = "ITEM002",
                Description = "Another Item Description",
                BarCode = "9876543210987",
                FiscalYear = 2024,
                Storage = "MAG02",
                BookInv = 200.0
            }
        };

        public GiacenzeControllerTests()
        {
            _giacenzeRequestServiceMock = new Mock<IGiacenzeRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new GiacenzeController(_responseHandlerMock.Object, _giacenzeRequestServiceMock.Object);

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

            // Setup generico per i metodi di log (_isLogActive è false)
            _responseHandlerMock.Setup(x => x.HandleBadRequest(_controller.HttpContext, false, "La richiesta non può essere vuota."))
                .Returns(new BadRequestObjectResult("La richiesta non può essere vuota."));
            _responseHandlerMock.Setup(x => x.HandleNotFound(_controller.HttpContext, false, "Non risultato trovato."))
                .Returns(new NotFoundObjectResult("Non risultato trovato."));
        }

        [Fact]
        public void GetGiacenze_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockDataList = _sampleGiacenzaDtoList;
            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze()).Returns(mockDataList);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                _controller.HttpContext,
                It.IsAny<List<GiacenzeDto>>(),
                It.IsAny<bool>(), "Ok")).Returns(new OkObjectResult(mockDataList));

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<GiacenzeDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
            Assert.Equal(_sampleGiacenzaDto.Item, returnValue.First().Item);

            _responseHandlerMock.Verify(log => log.HandleOkAndList(
                _controller.HttpContext,
                It.IsAny<List<GiacenzeDto>>(),
                It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<GiacenzeDto>();
            _giacenzeRequestServiceMock.Setup(service => service.GetGiacenze()).Returns(emptyList);
            _responseHandlerMock.Setup(log => log.HandleNotFound(
            _controller.HttpContext,
            It.IsAny<bool>(), "Not Found")).Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetGiacenze();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            _responseHandlerMock.Verify(log => log.HandleNotFound(
            _controller.HttpContext, 
            It.IsAny<bool>(), "Not Found"), Times.Once);
        }
    }
}