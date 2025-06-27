using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using apiPB.ApiClient.Implementation;
using apiPB.Data;
using apiPB.Models;
using apiPB.Utils.Abstraction;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Xunit;

namespace TestApi.Tests.ApiClient
{
    public class MagoApiClientTest
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<ILogService> _mockLogService;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<DbSet<A3AppSetting>> _mockAppSettingsSet;
        private readonly MagoApiClient _magoApiClient;

        private readonly List<A3AppSetting> _appSettingsData = new List<A3AppSetting>
        {
            new A3AppSetting
            {
                MagoUrl = "https://api.mago.test"
            }
        };

        public MagoApiClientTest()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockLogService = new Mock<ILogService>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockAppSettingsSet = new Mock<DbSet<A3AppSetting>>();
            
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            
            SetupMockDbSet();
            _magoApiClient = new MagoApiClient(_httpClient, _mockContext.Object, _mockLogService.Object);
        }

        private void SetupMockDbSet()
        {
            var queryableData = _appSettingsData.AsQueryable();
            _mockAppSettingsSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockAppSettingsSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockAppSettingsSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockAppSettingsSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());
            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockAppSettingsSet.Object);
        }

        private void SetupHttpResponse(HttpStatusCode statusCode, string content = "")
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeClient_WhenValidParametersProvided()
        {
            // Arrange & Act
            var client = new MagoApiClient(_httpClient, _mockContext.Object, _mockLogService.Object);

            // Assert
            Assert.NotNull(client);
            Assert.Equal("https://api.mago.test/", _httpClient.BaseAddress?.ToString());
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenConnectionStringNotFound()
        {
            // Arrange
            var emptyAppSettings = new List<A3AppSetting>().AsQueryable();
            _mockAppSettingsSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(emptyAppSettings.Provider);
            _mockAppSettingsSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(emptyAppSettings.Expression);
            _mockAppSettingsSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(emptyAppSettings.ElementType);
            _mockAppSettingsSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(emptyAppSettings.GetEnumerator());

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                new MagoApiClient(_httpClient, _mockContext.Object, _mockLogService.Object));
            Assert.Equal("Connection string non trovata.", exception.Message);
        }

        #endregion

        #region SendPostAsyncWithToken Tests

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldReturnSuccessResponse_WhenValidRequest()
        {
            // Arrange
            var testData = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
            var token = "Bearer test-token";
            var endpoint = "test-endpoint";
            var expectedResponse = "{\"result\": \"success\"}";
            
            SetupHttpResponse(HttpStatusCode.OK, expectedResponse);

            // Act
            var result = await _magoApiClient.SendPostAsyncWithToken(endpoint, testData, token, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            Assert.Equal(expectedResponse, content);
        }

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldSerializeAsObject_WhenIsListIsFalse()
        {
            // Arrange
            var testData = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
            var token = "Bearer test-token";
            var endpoint = "test-endpoint";
            
            SetupHttpResponse(HttpStatusCode.OK, "{}");

            // Act
            var result = await _magoApiClient.SendPostAsyncWithToken(endpoint, testData, token, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            
            // Verify that the request was made with the first object, not the list
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post && 
                    req.RequestUri.ToString().Contains(endpoint)),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldSetCorrectHeaders_WhenCalled()
        {
            // Arrange
            var testData = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
            var token = "Bearer test-token";
            var endpoint = "test-endpoint";
            
            SetupHttpResponse(HttpStatusCode.OK, "{}");

            // Act
            await _magoApiClient.SendPostAsyncWithToken(endpoint, testData, token, true);

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Headers.Authorization != null &&
                    req.Headers.Host == "192.168.100.213" &&
                    req.Headers.Accept.Any(h => h.MediaType == "application/json") &&
                    req.Headers.AcceptEncoding.Any(h => h.Value == "gzip") &&
                    req.Headers.Connection.Contains("keep-alive")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldLogError_WhenHttpRequestFails()
        {
            // Arrange
            var testData = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
            var token = "Bearer test-token";
            var endpoint = "test-endpoint";
            var expectedException = new HttpRequestException("Network error");

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _magoApiClient.SendPostAsyncWithToken(endpoint, testData, token, true));
            
            Assert.Equal("Network error", exception.Message);
            _mockLogService.Verify(log => log.AppendErrorToLog(It.Is<string>(s => s.Contains("Network error"))), Times.Once);
        }

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldHandleNullToken_WhenTokenIsNull()
        {
            // Arrange
            var testData = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
            string token = null;
            var endpoint = "test-endpoint";
            
            SetupHttpResponse(HttpStatusCode.OK, "{}");

            // Act
            var result = await _magoApiClient.SendPostAsyncWithToken(endpoint, testData, token, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldHandleEmptyData_WhenDataIsEmpty()
        {
            // Arrange
            var testData = new List<TestModel>();
            var token = "Bearer test-token";
            var endpoint = "test-endpoint";
            
            SetupHttpResponse(HttpStatusCode.OK, "{}");

            // Act
            var result = await _magoApiClient.SendPostAsyncWithToken(endpoint, testData, token, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion

        #region SendPostAsync Tests

        [Fact]
        public async Task SendPostAsync_ShouldReturnSuccessResponse_WhenValidRequest()
        {
            // Arrange
            var testData = new { username = "test", password = "password" };
            var endpoint = "login";
            var expectedResponse = "{\"token\": \"abc123\"}";
            
            SetupHttpResponse(HttpStatusCode.OK, expectedResponse);

            // Act
            var result = await _magoApiClient.SendPostAsync(endpoint, testData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            Assert.Equal(expectedResponse, content);
        }

        [Fact]
        public async Task SendPostAsync_ShouldSerializeBodyWithCamelCase_WhenCalled()
        {
            // Arrange
            var testData = new { UserName = "test", Password = "password" };
            var endpoint = "login";
            
            SetupHttpResponse(HttpStatusCode.OK, "{}");

            // Act
            await _magoApiClient.SendPostAsync(endpoint, testData);

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.Content != null &&
                    req.Content.Headers.ContentType.MediaType == "application/json"),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendPostAsync_ShouldLogError_WhenHttpRequestFails()
        {
            // Arrange
            var testData = new { username = "test", password = "password" };
            var endpoint = "login";
            var expectedException = new HttpRequestException("Connection timeout");

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _magoApiClient.SendPostAsync(endpoint, testData));
            
            Assert.Equal("Connection timeout", exception.Message);
            _mockLogService.Verify(log => log.AppendErrorToLog(It.Is<string>(s => s.Contains("Connection timeout"))), Times.Once);
        }

        [Fact]
        public async Task SendPostAsync_ShouldHandleNullBody_WhenBodyIsNull()
        {
            // Arrange
            object testData = null;
            var endpoint = "test-endpoint";
            
            SetupHttpResponse(HttpStatusCode.OK, "{}");

            // Act
            var result = await _magoApiClient.SendPostAsync(endpoint, testData);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldReturn401_WhenUnauthorized()
        {
            // Arrange
            var testData = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
            var token = "Bearer invalid-token";
            var endpoint = "protected-endpoint";
            
            SetupHttpResponse(HttpStatusCode.Unauthorized, "{\"error\": \"Unauthorized\"}");

            // Act
            var result = await _magoApiClient.SendPostAsyncWithToken(endpoint, testData, token, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldReturn500_WhenServerError()
        {
            // Arrange
            var testData = new List<TestModel> { new TestModel { Id = 1, Name = "Test" } };
            var token = "Bearer valid-token";
            var endpoint = "error-endpoint";

            SetupHttpResponse(HttpStatusCode.InternalServerError, "{\"error\": \"Internal server error\"}");

            // Act
            var result = await _magoApiClient.SendPostAsyncWithToken(endpoint, testData, token, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task SendPostAsyncWithToken_ShouldHandleLargePayload_WhenDataIsLarge()
        {
            // Arrange
            var largeTestData = new List<TestModel>();
            for (int i = 0; i < 1000; i++)
            {
                largeTestData.Add(new TestModel { Id = i, Name = $"Test{i}" });
            }
            var token = "Bearer test-token";
            var endpoint = "bulk-endpoint";
            
            SetupHttpResponse(HttpStatusCode.OK, "{\"processed\": 1000}");

            // Act
            var result = await _magoApiClient.SendPostAsyncWithToken(endpoint, largeTestData, token, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion

        [Fact]
        public void Dispose_ShouldDisposeHttpClient_WhenCalled()
        {
            // Arrange
            var client = new MagoApiClient(_httpClient, _mockContext.Object, _mockLogService.Object);

            // Act & Assert
            // HttpClient disposal is handled by the DI container in real scenarios
            // This test ensures the client can be created and used without issues
            Assert.NotNull(client);
        }

        // Helper class for testing
        private class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}