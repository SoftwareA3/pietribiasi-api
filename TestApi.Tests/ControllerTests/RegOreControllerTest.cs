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
using apiPB.Services.Request.Abstraction;
using apiPB.Services.Utils.Abstraction;

namespace TestApi.Tests.ControllerTests
{
    public class RegOreControllerTests
    {
        private readonly Mock<IRegOreRequestService> _regOreRequestServiceMock;
        private readonly Mock<ILogService> _logServiceMock;
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
            _logServiceMock = new Mock<ILogService>();

            _controller = new RegOreController(_logServiceMock.Object, _regOreRequestServiceMock.Object);

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

            _logServiceMock.Setup(x => x.AppendMessageToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndListToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<List<RegOreDto>>(),
                It.IsAny<bool>()));

            _logServiceMock.Setup(x => x.AppendMessageAndItemToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<RegOreDto>(),
                It.IsAny<bool>()));
        }

        // --- Test per getAllRegOre ---
        [Fact]
        public void GetAllRegOre_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<RegOreDto> { _sampleRegOreDto };
            _regOreRequestServiceMock.Setup(service => service.GetAppRegOre()).Returns(mockData);

            // Act
            var result = _controller.getAllRegOre();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<RegOreDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(_sampleRegOreDto.RegOreId, returnValue.First().RegOreId);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", It.Is<List<RegOreDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetAllRegOre_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            var emptyList = new List<RegOreDto>();
            _regOreRequestServiceMock.Setup(service => service.GetAppRegOre()).Returns(emptyList);

            // Act
            var result = _controller.getAllRegOre();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
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
            MockRequestPath("POST", "/api/reg_ore/post_reg_ore");

            // Act
            var result = _controller.PostRegOreList(requestDtoList);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<RegOreDto>>(createdResult.Value);
            Assert.Equal(responseDtoList.Count, returnValue.Count());
            Assert.Equal(responseDtoList.First().RegOreId, returnValue.First().RegOreId);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 201, "Created", It.Is<List<RegOreDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void PostRegOreList_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var requestDtoList = new List<RegOreRequestDto> { _sampleRegOreRequestDto };
            _regOreRequestServiceMock.Setup(service => service.PostAppRegOre(It.IsAny<IEnumerable<RegOreRequestDto>>()))
                .Returns(new List<RegOreDto>());
            MockRequestPath("POST", "/api/reg_ore/post_reg_ore");

            // Act
            var result = _controller.PostRegOreList(requestDtoList);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        [Fact]
        public void PostRegOreList_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            MockRequestPath("POST", "/api/reg_ore/post_reg_ore");

            // Act
            var result = _controller.PostRegOreList(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("La richiesta non può essere vuota.", badRequestResult.Value);
        }

        [Fact]
        public void PostRegOreList_ShouldReturnBadRequest_WhenRequestIsEmptyList()
        {
            // Arrange
            MockRequestPath("POST", "/api/reg_ore/post_reg_ore");

            // Act
            var result = _controller.PostRegOreList(new List<RegOreRequestDto>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("La richiesta non può essere vuota.", badRequestResult.Value);
        }

        // --- Test per GetA3AppRegOre ---
        [Fact]
        public void GetA3AppRegOre_ShouldReturnOkResult_WhenDataExists()
        {
            // Arrange
            var mockData = new List<RegOreDto> { _sampleRegOreDto };
            _regOreRequestServiceMock.Setup(service => service.GetAppViewOre(It.IsAny<ViewOreRequestDto>()))
                .Returns(mockData);
            MockRequestPath("POST", "/api/reg_ore/view_ore");

            // Act
            var result = _controller.GetA3AppRegOre(_sampleViewRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<RegOreDto>>(okResult.Value);
            Assert.Equal(mockData.Count, returnValue.Count());
            Assert.Equal(mockData.First().RegOreId, returnValue.First().RegOreId);
            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(It.IsAny<string>(), 200, "OK", It.Is<List<RegOreDto>>(list => list.Count == 1), false), Times.Once);
        }

        [Fact]
        public void GetA3AppRegOre_ShouldReturnNotFound_WhenNoDataExists()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.GetAppViewOre(It.IsAny<ViewOreRequestDto>()))
                .Returns(new List<RegOreDto>());
            MockRequestPath("POST", "/api/reg_ore/view_ore");

            // Act
            var result = _controller.GetA3AppRegOre(_sampleViewRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
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
            MockRequestPath("PUT", "/api/reg_ore/view_ore/edit_working_time");

            // Act
            var result = _controller.PutA3AppRegOre(_samplePutRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<RegOreDto>(okResult.Value);
            Assert.Equal(updatedDto.RegOreId, returnValue.RegOreId);
            Assert.Equal(updatedDto.WorkingTime, returnValue.WorkingTime);
            _logServiceMock.Verify(log => log.AppendMessageAndItemToLog(It.IsAny<string>(), 200, "OK", It.Is<RegOreDto>(dto => dto.RegOreId == updatedDto.RegOreId), false), Times.Once);
        }

        [Fact]
        public void PutA3AppRegOre_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.PutAppViewOre(It.IsAny<ViewOrePutRequestDto>()))
                .Returns((RegOreDto)null);
            MockRequestPath("PUT", "/api/reg_ore/view_ore/edit_working_time");

            // Act
            var result = _controller.PutA3AppRegOre(_samplePutRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        // --- Test per DeleteRegOreId ---
        [Fact]
        public void DeleteRegOreId_ShouldReturnOkResult_WhenDataIsDeleted()
        {
            // Arrange
            var deletedDto = _sampleRegOreDto;
            _regOreRequestServiceMock.Setup(service => service.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestDto>()))
                .Returns(deletedDto);
            MockRequestPath("DELETE", "/api/reg_ore/view_ore/delete_reg_ore_id");

            // Act
            var result = _controller.DeleteRegOreId(_sampleDeleteRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsType<RegOreDto>(okResult.Value);
            Assert.Equal(deletedDto.RegOreId, returnValue.RegOreId);
            _logServiceMock.Verify(log => log.AppendMessageAndItemToLog(It.IsAny<string>(), 200, "Deleted", It.Is<RegOreDto>(dto => dto.RegOreId == deletedDto.RegOreId), false), Times.Once);
        }

        [Fact]
        public void DeleteRegOreId_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            _regOreRequestServiceMock.Setup(service => service.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestDto>()))
                .Returns((RegOreDto)null);
            MockRequestPath("DELETE", "/api/reg_ore/view_ore/delete_reg_ore_id");

            // Act
            var result = _controller.DeleteRegOreId(_sampleDeleteRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _logServiceMock.Verify(log => log.AppendMessageToLog(It.IsAny<string>(), 404, "Not Found", false), Times.Once);
        }

        // Metodo helper per mockare HttpContext.Request.Method e Path per ogni test
        private void MockRequestPath(string method, string path)
        {
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Method).Returns(method);
            httpRequestMock.Setup(r => r.Path).Returns(new PathString(path));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
        }
    }
}