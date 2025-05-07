using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Necessario per HttpContext
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using apiPB.Controllers;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Services;
using apiPB.Services.Request.Abstraction;
using apiPB.Services.Utils.Abstraction;


namespace TestApi.Tests.ControllerTests
{
    public class InventarioControllerTests
    {
        private readonly Mock<IInventarioRequestService> _inventarioRequestServiceMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly InventarioController _controller;

        // DTO utilizzati per i test
        private readonly InventarioDto _sampleInventarioDto = new InventarioDto
        {
            InvId = 1,
            WorkerId = 43,
            SavedDate = new DateTime(2023, 10, 1, 12, 0, 0),
            Item = "123ABC123Item",
            Description = "123ABC123Description",
            BarCode = "123ABC123Barcode",
            FiscalYear = 2025,
            Storage = "SEDE",
            BookInv = 1,
            Imported = false,
            UserImp = "UserTest",
            DataImp = new DateTime(2023, 10, 1, 12, 0, 0)
        };

        private readonly InventarioRequestDto _sampleInventarioRequestDto = new InventarioRequestDto
        {
            InvId = 1, 
            WorkerId = 43,
            SavedDate = new DateTime(2023, 10, 1, 12, 0, 0), 
            Item = "123ABC123Item",
            Description = "123ABC123Description",
            BarCode = "123ABC123Barcode",
            FiscalYear = 2025,
            Storage = "SEDE",
            BookInv = 1,
            Imported = false,
            UserImp = "UserTest",
            DataImp = new DateTime(2023, 10, 1, 12, 0, 0)
        };

        private readonly ViewInventarioRequestDto _sampleViewRequest = new ViewInventarioRequestDto
        {
            WorkerId = 43,
            FromDateTime = new DateTime(2023, 10, 1, 12, 0, 0).AddDays(-1),
            ToDateTime = new DateTime(2023, 10, 1, 12, 0, 0).AddDays(1),
            Item = "123ABC123Item",
            BarCode = "123ABC123Barcode"
        };

        private readonly ViewInventarioPutRequestDto _samplePutRequest = new ViewInventarioPutRequestDto
        {
            InvId = 1,
            BookInv = 2 
        };

        public InventarioControllerTests()
        {
            _inventarioRequestServiceMock = new Mock<IInventarioRequestService>();
            _logServiceMock = new Mock<ILogService>();

            _controller = new InventarioController(_logServiceMock.Object, _inventarioRequestServiceMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            // Impostazioni predefinite per Metodo, Path e tipo di richiesta
            // Mock HttpContext per requestPath e User
            httpRequestMock.Setup(r => r.Method).Returns("GET"); 
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/inventario/test")); 
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            _logServiceMock.Setup(x => x.AppendMessageToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndListToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<List<InventarioDto>>(),
                It.IsAny<bool>()));
                
            _logServiceMock.Setup(x => x.AppendMessageAndItemToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<InventarioDto>(),
                It.IsAny<bool>()));
        }

        // --- Test per GetInventario ---
        [Fact]
        public void GetInventario_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<InventarioDto> { _sampleInventarioDto };
            _inventarioRequestServiceMock.Setup(service => service.GetInventario()).Returns(mockData);

            // Act
            var result = _controller.GetInventario();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InventarioDto>>(okResult.Value);
            Assert.Single(returnValue); 
            Assert.Equal(_sampleInventarioDto.InvId, returnValue.First().InvId);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", It.Is<List<InventarioDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetInventario_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<InventarioDto>();
            _inventarioRequestServiceMock.Setup(service => service.GetInventario()).Returns(emptyList);

            // Act
            var result = _controller.GetInventario();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        // --- Test per PostInventarioList ---
        [Fact]
        public void PostInventarioList_ShouldReturnOkResult_WhenDataIsProcessed()
        {
            // Arrange
            var requestDtoList = new List<InventarioRequestDto> { _sampleInventarioRequestDto };
            var responseDtoList = new List<InventarioDto> { _sampleInventarioDto };
            _inventarioRequestServiceMock.Setup(service => service.PostInventarioList(It.IsAny<IEnumerable<InventarioRequestDto>>()))
                .Returns(responseDtoList);
            MockRequestPath("POST", "/api/inventario/post_inventario");


            // Act
            var result = _controller.PostInventarioList(requestDtoList);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode); 
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InventarioDto>>(okResult.Value);
            Assert.Equal(responseDtoList.Count, returnValue.Count());
            Assert.Equal(responseDtoList.First().InvId, returnValue.First().InvId);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", It.Is<List<InventarioDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void PostInventarioList_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var requestDtoList = new List<InventarioRequestDto> { _sampleInventarioRequestDto };
            _inventarioRequestServiceMock.Setup(service => service.PostInventarioList(It.IsAny<IEnumerable<InventarioRequestDto>>()))
                .Returns(new List<InventarioDto>()); 
            MockRequestPath("POST", "/api/inventario/post_inventario");


            // Act
            var result = _controller.PostInventarioList(requestDtoList);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        [Fact]
        public void PostInventarioList_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            MockRequestPath("POST", "/api/inventario/post_inventario");

            // Act
            var result = _controller.PostInventarioList(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Request body is null or empty", badRequestResult.Value);
        }

        [Fact]
        public void PostInventarioList_ShouldReturnBadRequest_WhenRequestIsEmptyList()
        {
            // Arrange
            MockRequestPath("POST", "/api/inventario/post_inventario");

            // Act
            var result = _controller.PostInventarioList(new List<InventarioRequestDto>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Request body is null or empty", badRequestResult.Value);
        }


        // --- Test per GetViewInventario ---
        [Fact]
        public void GetViewInventario_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<InventarioDto> { _sampleInventarioDto };
            _inventarioRequestServiceMock.Setup(service => service.GetViewInventario(It.IsAny<ViewInventarioRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/inventario/get_view_inventario");

            // Act
            var result = _controller.GetViewInventario(_sampleViewRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InventarioDto>>(okResult.Value);
            Assert.Equal(mockData.Count, returnValue.Count());
            Assert.Equal(mockData.First().InvId, returnValue.First().InvId);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", It.Is<List<InventarioDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetViewInventario_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            _inventarioRequestServiceMock.Setup(service => service.GetViewInventario(It.IsAny<ViewInventarioRequestDto>()))
                .Returns(new List<InventarioDto>());
            MockRequestPath("POST", "/api/inventario/get_view_inventario");

            // Act
            var result = _controller.GetViewInventario(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        [Fact]
        public void GetViewInventario_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            MockRequestPath("POST", "/api/inventario/get_view_inventario");

            // Act
            var result = _controller.GetViewInventario(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Request body is null or empty", badRequestResult.Value);
        }

        // --- Test per PutViewInventario ---
        [Fact]
        public void PutViewInventario_ShouldReturnOkResult_WhenDataIsUpdated()
        {
            // Arrange
            var updatedDto = new InventarioDto 
            {
                InvId = _samplePutRequest.InvId,
                BookInv = _samplePutRequest.BookInv, 
                WorkerId = _sampleInventarioDto.WorkerId, 
                SavedDate = DateTime.Now, 
                Item = _sampleInventarioDto.Item,
                Description = _sampleInventarioDto.Description,
                BarCode = _sampleInventarioDto.BarCode,
                FiscalYear = _sampleInventarioDto.FiscalYear,
                Storage = _sampleInventarioDto.Storage,
                Imported = _sampleInventarioDto.Imported
            };
            _inventarioRequestServiceMock.Setup(service => service.PutViewInventario(It.IsAny<ViewInventarioPutRequestDto>()))
                .Returns(updatedDto);
            MockRequestPath("PUT", "/api/inventario/view_inventario/edit_book_inv");


            // Act
            var result = _controller.PutViewInventario(_samplePutRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<InventarioDto>(okResult.Value);
            Assert.Equal(updatedDto.InvId, returnValue.InvId);
            Assert.Equal(updatedDto.BookInv, returnValue.BookInv);
            _logServiceMock.Verify(log => log.AppendMessageAndItemToLog(It.IsAny<string>(), 200, "OK", It.Is<InventarioDto>(dto => dto.InvId == updatedDto.InvId), false), Times.Once);
        }

        [Fact]
        public void PutViewInventario_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            _inventarioRequestServiceMock.Setup(service => service.PutViewInventario(It.IsAny<ViewInventarioPutRequestDto>()))
                .Returns((InventarioDto)null);
            MockRequestPath("PUT", "/api/inventario/view_inventario/edit_book_inv");


            // Act
            var result = _controller.PutViewInventario(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        [Fact]
        public void PutViewInventario_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            MockRequestPath("PUT", "/api/inventario/view_inventario/edit_book_inv");

            // Act
            var result = _controller.PutViewInventario(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Request body is null or empty", badRequestResult.Value);
        }

        // Metodo helper per mockare HttpContext.Request.Method e Path per ogni test
        private void MockRequestPath(string method, string path)
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Method).Returns(method);
            httpRequestMock.Setup(r => r.Path).Returns(new PathString(path));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);
            // Se necessario, mockare anche User qui se l'autorizzazione è un problema per il test specifico
            // var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity()); // Utente anonimo ma non nullo
            // httpContextMock.Setup(c => c.User).Returns(claimsPrincipal);


            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
        }
    }
}