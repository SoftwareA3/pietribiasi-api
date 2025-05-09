using System;
using System.Collections.Generic;
using apiPB.Utils.Abstraction;
using apiPB.Utils.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace TestApi.Tests.UtilsTests
{
    public class ResponseHandlerTest
    {
        private readonly Mock<ILogService> _mockLogService;
        private readonly ResponseHandler _responseHandler;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<HttpRequest> _mockHttpRequest;

        public ResponseHandlerTest()
        {
            // Setup mocks
            _mockLogService = new Mock<ILogService>();
            _responseHandler = new ResponseHandler(_mockLogService.Object);
            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpRequest = new Mock<HttpRequest>();
            
            // Setup HttpContext mock
            _mockHttpContext.Setup(c => c.Request).Returns(_mockHttpRequest.Object);
        }

        [Fact]
        public void HandleBadRequest_ShouldReturnBadRequestResult()
        {
            // Arrange
            _mockHttpRequest.Setup(r => r.Method).Returns("GET");
            _mockHttpRequest.Setup(r => r.Path).Returns(new PathString("/api/test"));
            bool isLogActive = true;

            // Act
            var result = _responseHandler.HandleBadRequest(_mockHttpContext.Object, isLogActive);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("La richiesta non può essere vuota.", result.Value);
            
            // Verify log service was called
            _mockLogService.Verify(
                l => l.AppendMessageToLog(
                    It.Is<string>(s => s == "GET api/test"),
                    It.Is<int?>(sc => sc == StatusCodes.Status400BadRequest),
                    It.Is<string>(sm => sm == "La richiesta non può essere vuota"),
                    It.Is<bool>(la => la == true)
                ),
                Times.Once
            );
        }

        [Fact]
        public void HandleNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            _mockHttpRequest.Setup(r => r.Method).Returns("GET");
            _mockHttpRequest.Setup(r => r.Path).Returns(new PathString("/api/test"));
            bool isLogActive = true;

            // Act
            var result = _responseHandler.HandleNotFound(_mockHttpContext.Object, isLogActive);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Non risultato trovato.", result.Value);
            
            // Verify log service was called
            _mockLogService.Verify(
                l => l.AppendMessageToLog(
                    It.Is<string>(s => s == "GET api/test"),
                    It.Is<int?>(sc => sc == StatusCodes.Status404NotFound),
                    It.Is<string>(sm => sm == "Nessun Risultato trovato"),
                    It.Is<bool>(la => la == true)
                ),
                Times.Once
            );
        }

        [Fact]
        public void HandleOkAndItem_ShouldReturnOkResult()
        {
            // Arrange
            _mockHttpRequest.Setup(r => r.Method).Returns("GET");
            _mockHttpRequest.Setup(r => r.Path).Returns(new PathString("/api/test"));
            bool isLogActive = true;
            var testItem = new TestItem { Id = 1, Name = "Test" };

            // Act
            var result = _responseHandler.HandleOkAndItem(_mockHttpContext.Object, testItem, isLogActive);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(testItem, result.Value);
            
            // Verify log service was called
            _mockLogService.Verify(
                l => l.AppendMessageAndItemToLog(
                    It.Is<string>(s => s == "GET api/test"),
                    It.Is<int?>(sc => sc == StatusCodes.Status200OK),
                    It.Is<string>(sm => sm == "Ok"),
                    It.Is<TestItem>(i => i.Id == 1 && i.Name == "Test"),
                    It.Is<bool>(la => la == true)
                ),
                Times.Once
            );
        }

        [Fact]
        public void HandleOkAndList_ShouldReturnOkResult()
        {
            // Arrange
            _mockHttpRequest.Setup(r => r.Method).Returns("GET");
            _mockHttpRequest.Setup(r => r.Path).Returns(new PathString("/api/test"));
            bool isLogActive = true;
            var testList = new List<TestItem> 
            { 
                new TestItem { Id = 1, Name = "Test1" },
                new TestItem { Id = 2, Name = "Test2" }
            };

            // Act
            var result = _responseHandler.HandleOkAndList(_mockHttpContext.Object, testList, isLogActive);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(testList, result.Value);
            
            // Verify log service was called
            _mockLogService.Verify(
                l => l.AppendMessageAndListToLog(
                    It.Is<string>(s => s == "GET api/test"),
                    It.Is<int?>(sc => sc == StatusCodes.Status200OK),
                    It.Is<string>(sm => sm == "Ok"),
                    It.Is<List<TestItem>>(list => list.Count == 2 && list[0].Name == "Test1" && list[1].Name == "Test2"),
                    It.Is<bool>(la => la == true)
                ),
                Times.Once
            );
        }

        [Fact]
        public void HandleCreated_ShouldReturnCreatedResult()
        {
            // Arrange
            _mockHttpRequest.Setup(r => r.Method).Returns("POST");
            _mockHttpRequest.Setup(r => r.Path).Returns(new PathString("/api/test"));
            bool isLogActive = true;
            var itemList = new List<TestItem> 
            { 
                new TestItem { Id = 1, Name = "Test1" },
                new TestItem { Id = 2, Name = "Test2" }
            };

            // Act
            var result = _responseHandler.HandleCreated(_mockHttpContext.Object, itemList, isLogActive);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(itemList, result.Value);
            Assert.Equal(nameof(itemList), result.ActionName);
            
            // Verify log service was called
            _mockLogService.Verify(
                l => l.AppendMessageAndListToLog(
                    It.Is<string>(s => s == "POST api/test"),
                    It.Is<int?>(sc => sc == StatusCodes.Status201Created),
                    It.Is<string>(sm => sm == "Created"),
                    It.Is<List<TestItem>>(list => list.Count == 2 && list[0].Name == "Test1" && list[1].Name == "Test2"),
                    It.Is<bool>(la => la == true)
                ),
                Times.Once
            );
        }

        [Fact]
        public void HandleBadRequest_WithLogDisabled_ShouldNotLog()
        {
            // Arrange
            _mockHttpRequest.Setup(r => r.Method).Returns("GET");
            _mockHttpRequest.Setup(r => r.Path).Returns(new PathString("/api/test"));
            bool isLogActive = false;

            // Act
            var result = _responseHandler.HandleBadRequest(_mockHttpContext.Object, isLogActive);

            // Assert
            Assert.NotNull(result);
            
            // Verify log service was called with isLogActive = false
            _mockLogService.Verify(
                l => l.AppendMessageToLog(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<string>(),
                    It.Is<bool>(la => la == false)
                ),
                Times.Once
            );
        }

        [Fact]
        public void HttpContextString_WithNullValues_ShouldHandleGracefully()
        {
            // Arrange
            _mockHttpRequest.Setup(r => r.Method).Returns((string)null);
            _mockHttpRequest.Setup(r => r.Path).Returns((PathString)null);
            bool isLogActive = true;

            // Act
            var result = _responseHandler.HandleBadRequest(_mockHttpContext.Object, isLogActive);

            // Assert
            Assert.NotNull(result);
            
            // Verify log service was called with empty string for request path
            _mockLogService.Verify(
                l => l.AppendMessageToLog(
                    It.Is<string>(s => s == " "),
                    It.IsAny<int?>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()
                ),
                Times.Once
            );
        }
        
        // Test helper class
        private class TestItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}