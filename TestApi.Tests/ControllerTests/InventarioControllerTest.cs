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
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;


namespace TestApi.Tests.ControllerTests
{
    public class InventarioControllerTests
    {
        private readonly Mock<IInventarioRequestService> _inventarioRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
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
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new InventarioController(_responseHandlerMock.Object, _inventarioRequestServiceMock.Object);

            var httpContextMock = new Mock<HttpContext>();

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "La richiesta non può essere vuota."))
                .Returns(new BadRequestObjectResult("La richiesta non può essere vuota."));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Non risultato trovato."))
                .Returns(new NotFoundObjectResult("Non risultato trovato."));
        }

        // --- Test per GetInventario ---
        [Fact]
        public void GetInventario_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<InventarioDto> { _sampleInventarioDto };
            _inventarioRequestServiceMock.Setup(service => service.GetInventario()).Returns(mockData);
            _responseHandlerMock.Setup(x => x.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<InventarioDto>>(), It.IsAny<bool>(), "Ok"))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetInventario();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InventarioDto>>(okResult.Value);
            Assert.Single(returnValue); 
            Assert.Equal(_sampleInventarioDto.InvId, returnValue.First().InvId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<InventarioDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void GetInventario_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<InventarioDto>();
            _inventarioRequestServiceMock.Setup(service => service.GetInventario()).Returns(emptyList);
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetInventario();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"), Times.Once);
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
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<InventarioDto>>(), It.IsAny<bool>(), "Ok"))
                .Returns(new OkObjectResult(responseDtoList));


            // Act
            var result = _controller.PostInventarioList(requestDtoList);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode); 
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InventarioDto>>(okResult.Value);
            Assert.Equal(responseDtoList.Count, returnValue.Count());
            Assert.Equal(responseDtoList.First().InvId, returnValue.First().InvId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<InventarioDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void PostInventarioList_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var requestDtoList = new List<InventarioRequestDto> { _sampleInventarioRequestDto };
            _inventarioRequestServiceMock.Setup(service => service.PostInventarioList(It.IsAny<IEnumerable<InventarioRequestDto>>()))
                .Returns(new List<InventarioDto>()); 
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"))
                .Returns(new NotFoundObjectResult("Not Found"));


            // Act
            var result = _controller.PostInventarioList(requestDtoList);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"), Times.Once);
        }

        [Fact]
        public void PostInventarioList_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _inventarioRequestServiceMock.Setup(service => service.PostInventarioList(It.IsAny<IEnumerable<InventarioRequestDto>>()))
                .Returns((List<InventarioDto>)null);
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Bad Request"))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.PostInventarioList(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Bad Request"))
                .Returns(new BadRequestObjectResult("Bad Request"));
        }


        // --- Test per GetViewInventario ---
        [Fact]
        public void GetViewInventario_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<InventarioDto> { _sampleInventarioDto };
            _inventarioRequestServiceMock.Setup(service => service.GetViewInventario(It.IsAny<ViewInventarioRequestDto>()))
                .Returns(mockData);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<InventarioDto>>(), It.IsAny<bool>(), "Ok"))
                .Returns(new OkObjectResult(mockData));

            // Act
            var result = _controller.GetViewInventario(_sampleViewRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<InventarioDto>>(okResult.Value);
            Assert.Equal(mockData.Count, returnValue.Count());
            Assert.Equal(mockData.First().InvId, returnValue.First().InvId);
            _responseHandlerMock.Verify(log => log.HandleOkAndList(It.IsAny<HttpContext>(), It.IsAny<List<InventarioDto>>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void GetViewInventario_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            _inventarioRequestServiceMock.Setup(service => service.GetViewInventario(It.IsAny<ViewInventarioRequestDto>()))
                .Returns(new List<InventarioDto>());
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.GetViewInventario(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"), Times.Once);
        }

        [Fact]
        public void GetViewInventario_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _inventarioRequestServiceMock.Setup(service => service.GetViewInventario(It.IsAny<ViewInventarioRequestDto>()))
                .Returns((List<InventarioDto>)null);
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Bad Request"))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = _controller.GetViewInventario(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Bad Request"), Times.Once);
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
            _responseHandlerMock.Setup(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<InventarioDto>(), It.IsAny<bool>(), "Ok"))
                .Returns(new OkObjectResult(updatedDto));


            // Act
            var result = _controller.PutViewInventario(_samplePutRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<InventarioDto>(okResult.Value);
            Assert.Equal(updatedDto.InvId, returnValue.InvId);
            Assert.Equal(updatedDto.BookInv, returnValue.BookInv);
            _responseHandlerMock.Verify(log => log.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<InventarioDto>(), It.IsAny<bool>(), "Ok"), Times.Once);
        }

        [Fact]
        public void PutViewInventario_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            _inventarioRequestServiceMock.Setup(service => service.PutViewInventario(It.IsAny<ViewInventarioPutRequestDto>()))
                .Returns((InventarioDto)null);
            _responseHandlerMock.Setup(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = _controller.PutViewInventario(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Not Found"), Times.Once);
        }

        [Fact]
        public void PutViewInventario_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _inventarioRequestServiceMock.Setup(service => service.PutViewInventario(It.IsAny<ViewInventarioPutRequestDto>()))
                .Returns((InventarioDto)null);
            _responseHandlerMock.Setup(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Bad Request"))
                .Returns(new BadRequestObjectResult("Bad Request"));
            
            // Act
            var result = _controller.PutViewInventario(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _responseHandlerMock.Verify(log => log.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), "Bad Request"), Times.Once);
        }
    }
}