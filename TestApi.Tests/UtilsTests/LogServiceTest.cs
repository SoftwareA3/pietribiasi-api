using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Net;
using System.Linq;
using apiPB.Utils.Implementation;
using apiPB.Utils.Abstraction;

namespace TestApi.Tests.UtilsTests
{
    public class LogServiceTest : IDisposable
    {
        private readonly string _testLogFolderPath = "TestLogs\\";
        private readonly string _testLogFilePath;
        private readonly LogService _logService;
        private readonly Type _logServiceType;
        private readonly FieldInfo _logFolderPathField;
        private readonly FieldInfo _logFilePathField;
        private readonly FieldInfo _logErrorFilePathField;

        // Model utilizzato per simulare gli oggetti nei test
        private class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public LogServiceTest()
        {
            // Setup test environment
            _testLogFilePath = Path.Combine(_testLogFolderPath, "API.log");
            _logService = new LogService();

            // Use reflection to access and modify private fields for testing
            _logServiceType = typeof(LogService);
            _logFolderPathField = _logServiceType.GetField("_logFolderPath", BindingFlags.NonPublic | BindingFlags.Instance);
            _logFilePathField = _logServiceType.GetField("_logFilePath", BindingFlags.NonPublic | BindingFlags.Instance);
            _logErrorFilePathField = _logServiceType.GetField("_logErrorFilePath", BindingFlags.NonPublic | BindingFlags.Instance);


            // Set test paths for the log service
            _logFolderPathField.SetValue(_logService, _testLogFolderPath);
            _logFilePathField.SetValue(_logService, _testLogFilePath);
            _logErrorFilePathField.SetValue(_logService, Path.Combine(_testLogFolderPath, "APIErrors.log"));


            // Ensure clean test environment
            CleanupTestEnvironment();
        }

        public void Dispose()
        {
            // Cleanup after tests
            CleanupTestEnvironment();
        }

        private void CleanupTestEnvironment()
        {
            // Delete test log file if it exists
            if (File.Exists(_testLogFilePath))
            {
                File.Delete(_testLogFilePath);
            }

            // Delete test log directory if it exists
            if (Directory.Exists(_testLogFolderPath))
            {
                Directory.Delete(_testLogFolderPath, true);
            }
        }

        #region AppendMessageToLog Tests
        [Fact]
        public void AppendMessageToLog_CreatesLogFileAndAddsMessage_WhenIsActiveIsTrue()
        {
            // Arrange
            string requestType = "GET api/test";
            int statusCode = 200;
            string statusMessage = "Ok";
            bool isActive = true;

            // Act
            _logService.AppendMessageToLog(requestType, statusCode, statusMessage, isActive);

            // Assert
            Assert.True(File.Exists(_testLogFilePath));
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.Contains(requestType, logContent);
            Assert.Contains($"StatusCode: {statusCode}", logContent);
            Assert.Contains($"Message: {statusMessage}", logContent);
        }

        [Fact]
        public void AppendMessageToLog_DoesNotCreateLogFile_WhenIsActiveIsFalse()
        {
            // Arrange
            string requestType = "GET api/test";
            int statusCode = 200;
            string statusMessage = "Ok";
            bool isActive = false;

            // Act
            _logService.AppendMessageToLog(requestType, statusCode, statusMessage, isActive);

            // Assert
            Assert.False(File.Exists(_testLogFilePath));
        }
        #endregion

        #region AppendMessageAndListToLog Tests

        [Fact]
        public void AppendMessageAndListToLog_AddsMessageAndListToLogFile_WhenIsActiveIsTrue()
        {
            // Arrange
            string requestType = "GET api/test";
            int statusCode = 200;
            string statusMessage = "Ok";
            bool isActive = true;

            var testList = new List<TestModel>
            {
                new TestModel { Id = 1, Name = "Test1" },
                new TestModel { Id = 2, Name = "Test2" }
            };

            // Act
            _logService.AppendMessageAndListToLog(requestType, statusCode, statusMessage, testList, isActive);

            // Assert
            Assert.True(File.Exists(_testLogFilePath));
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.Contains(requestType, logContent);
            Assert.Contains($"StatusCode: {statusCode}", logContent);
            Assert.Contains($"Message: {statusMessage}", logContent);
            Assert.Contains("Id: 1", logContent);
            Assert.Contains("Name: Test1", logContent);
            Assert.Contains("Id: 2", logContent);
            Assert.Contains("Name: Test2", logContent);
        }

        [Fact]
        public void AppendMessageAndListToLog_DoesNothing_WhenIsActiveIsFalse()
        {
            // Arrange
            string requestType = "GET api/test";
            int statusCode = 200;
            string statusMessage = "Ok";
            bool isActive = false;

            var testList = new List<TestModel>
            {
                new TestModel { Id = 1, Name = "Test1" },
                new TestModel { Id = 2, Name = "Test2" }
            };

            // Act
            _logService.AppendMessageAndListToLog(requestType, statusCode, statusMessage, testList, isActive);

            // Assert
            Assert.False(File.Exists(_testLogFilePath));
        }

        [Fact]
        public void AppendMessageAndListToLog_HandlesNullList_WhenIsActiveIsTrue()
        {
            // Arrange
            string requestType = "GET api/test";
            int statusCode = 200;
            string statusMessage = "Ok";
            bool isActive = true;
            List<TestModel> testList = null;

            // Act
            _logService.AppendMessageAndListToLog(requestType, statusCode, statusMessage, testList, isActive);

            // Assert
            Assert.True(File.Exists(_testLogFilePath));
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.Contains(requestType, logContent);
            Assert.Contains($"StatusCode: {statusCode}", logContent);
            Assert.Contains($"Message: {statusMessage}", logContent);
            // Should not contain list entries since list is null
            Assert.DoesNotContain("Id: 1", logContent);
        }
        #endregion

        #region AppendMessageAndItemToLog Tests

        [Fact]
        public void AppendMessageAndItemToLog_AddsMessageAndItemToLogFile_WhenIsActiveIsTrue()
        {
            // Arrange
            string requestType = "GET api/test";
            int statusCode = 200;
            string statusMessage = "Ok";
            bool isActive = true;

            var testItem = new TestModel { Id = 1, Name = "Test1" };

            // Act
            _logService.AppendMessageAndItemToLog(requestType, statusCode, statusMessage, testItem, isActive);

            // Assert
            Assert.True(File.Exists(_testLogFilePath));
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.Contains(requestType, logContent);
            Assert.Contains($"StatusCode: {statusCode}", logContent);
            Assert.Contains($"Message: {statusMessage}", logContent);
            Assert.Contains("Id: 1", logContent);
            Assert.Contains("Name: Test1", logContent);
        }

        [Fact]
        public void AppendMessageAndItemToLog_DoesNothing_WhenIsActiveIsFalse()
        {
            // Arrange
            string requestType = "GET api/test";
            int statusCode = 200;
            string statusMessage = "Ok";
            bool isActive = false;

            var testItem = new TestModel { Id = 1, Name = "Test1" };

            // Act
            _logService.AppendMessageAndItemToLog(requestType, statusCode, statusMessage, testItem, isActive);

            // Assert
            Assert.False(File.Exists(_testLogFilePath));
        }

        [Fact]
        public void AppendMessageAndItemToLog_HandlesNullItem_WhenIsActiveIsTrue()
        {
            // Arrange
            string requestType = "GET api/test";
            int statusCode = 200;
            string statusMessage = "Ok";
            bool isActive = true;
            TestModel testItem = null;

            // Act
            _logService.AppendMessageAndItemToLog(requestType, statusCode, statusMessage, testItem, isActive);

            // Assert
            Assert.True(File.Exists(_testLogFilePath));
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.Contains(requestType, logContent);
            Assert.Contains($"StatusCode: {statusCode}", logContent);
            Assert.Contains($"Message: {statusMessage}", logContent);
            // Should not contain item entries since item is null
            Assert.DoesNotContain("Id: 1", logContent);
        }
        #endregion

        #region Test Methods for LogService

        [Fact]
        public void CreateLogFile_CreatesDirectoryAndFile_WhenTheyDoNotExist()
        {
            // Arrange
            MethodInfo createLogFileMethod = _logServiceType.GetMethod("CreateLogFile",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            createLogFileMethod.Invoke(_logService, null);

            // Assert
            Assert.True(Directory.Exists(_testLogFolderPath));
            Assert.True(File.Exists(_testLogFilePath));
        }

        [Fact]
        public void AppendIpAddress_ReturnsNonEmptyString()
        {
            // Arrange
            MethodInfo appendIpAddressMethod = _logServiceType.GetMethod("AppendIpAddress",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            string result = (string)appendIpAddressMethod.Invoke(_logService, null);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(string.Empty, result);
            // Check if the result is in a valid IP format
            Assert.True(System.Net.IPAddress.TryParse(result, out _));
        }
        #endregion

        #region AppendErrorToLog and AppendWarningToLog Tests

        [Fact]
        public void AppendErrorToLog_ShouldCreateErrorLogFileAndWriteMessage()
        {
            // Arrange
            string errorMessage = "Test error message";
            string errorLogPath = Path.Combine(_testLogFolderPath, "APIErrors.log");
            if (!Directory.Exists(_testLogFolderPath))
                Directory.CreateDirectory(_testLogFolderPath);
            if (File.Exists(errorLogPath))
                File.Delete(errorLogPath);

            // Act
            _logService.AppendErrorToLog(errorMessage);

            // Assert
            Assert.True(File.Exists(errorLogPath));
            string errorLogContent = File.ReadAllText(errorLogPath);
            Assert.Contains("===== ERROR =====", errorLogContent);
            Assert.Contains(errorMessage, errorLogContent);
            Assert.Contains("===== END ERROR =====", errorLogContent);

            // Cleanup
            if (File.Exists(errorLogPath))
                File.Delete(errorLogPath);
        }

        [Fact]
        public void AppendWarningToLog_ShouldCreateErrorLogFileAndWriteWarning()
        {
            // Arrange
            string warningMessage = "Test warning message";

            // Act
            _logService.AppendWarningToLog(warningMessage);

            // Assert
            Assert.True(File.Exists(Path.Combine(_testLogFolderPath, "APIErrors.log")));
            string errorLogContent = File.ReadAllText(Path.Combine(_testLogFolderPath, "APIErrors.log"));
            Assert.Contains("===== WARNING =====", errorLogContent);
            Assert.Contains(warningMessage, errorLogContent);
            Assert.Contains("===== END WARNING =====", errorLogContent);
        }
        #endregion
    }
}