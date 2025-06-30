using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using apiPB.Utils.Implementation;

namespace TestApi.Tests.UtilsTests
{
    public class ApplicationExceptionTest
    {
        private const string TestClassName = "TestClass";
        private const string TestMethodName = "TestMethod";

        #region ValidateCollection Tests

        [Fact]
        public void ValidateCollection_WithNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            List<string> nullCollection = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ApplicationExceptionHandler.ValidateCollection(nullCollection, TestClassName, TestMethodName));
            
            Assert.Equal("collection", exception.ParamName);
            Assert.Contains("La collezione non può essere null", exception.Message);
        }

        [Fact]
        public void ValidateCollection_WithNullCollectionAndCustomMessage_ShouldThrowArgumentNullExceptionWithCustomMessage()
        {
            // Arrange
            List<string> nullCollection = null;
            string customMessage = "Custom null message";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ApplicationExceptionHandler.ValidateCollection(nullCollection, TestClassName, TestMethodName, false, customMessage));
            
            Assert.Equal("collection", exception.ParamName);
            Assert.Contains(customMessage, exception.Message);
        }

        [Fact]
        public void ValidateCollection_WithEmptyCollectionExpectingNotEmpty_ShouldThrowEmptyListException()
        {
            // Arrange
            var emptyCollection = new List<string>();

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                ApplicationExceptionHandler.ValidateCollection(emptyCollection, TestClassName, TestMethodName, expectEmpty: false));
            
            Assert.Equal(TestClassName, exception.ClassName);
            Assert.Equal(TestMethodName, exception.MethodName);
            Assert.Contains("La collezione non può essere vuota", exception.Message);
        }

        [Fact]
        public void ValidateCollection_WithEmptyCollectionAndCustomMessage_ShouldThrowEmptyListExceptionWithCustomMessage()
        {
            // Arrange
            var emptyCollection = new List<string>();
            string customMessage = "Custom empty message";

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                ApplicationExceptionHandler.ValidateCollection(emptyCollection, TestClassName, TestMethodName, expectEmpty: false, customMessage));
            
            Assert.Equal(TestClassName, exception.ClassName);
            Assert.Equal(TestMethodName, exception.MethodName);
            Assert.Contains(customMessage, exception.Message);
        }

        [Fact]
        public void ValidateCollection_WithEmptyCollectionExpectingEmpty_ShouldNotThrow()
        {
            // Arrange
            var emptyCollection = new List<string>();

            // Act & Assert
            var exception = Record.Exception(() =>
                ApplicationExceptionHandler.ValidateCollection(emptyCollection, TestClassName, TestMethodName, expectEmpty: true));
            
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateCollection_WithValidCollectionExpectingNotEmpty_ShouldNotThrow()
        {
            // Arrange
            var validCollection = new List<string> { "item1", "item2" };

            // Act & Assert
            var exception = Record.Exception(() =>
                ApplicationExceptionHandler.ValidateCollection(validCollection, TestClassName, TestMethodName, expectEmpty: false));
            
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateCollection_WithValidCollectionExpectingEmpty_ShouldNotThrow()
        {
            // Arrange
            var validCollection = new List<string> { "item1", "item2" };

            // Act & Assert
            var exception = Record.Exception(() =>
                ApplicationExceptionHandler.ValidateCollection(validCollection, TestClassName, TestMethodName, expectEmpty: true));
            
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateCollection_WithIEnumerableNotList_ShouldWork()
        {
            // Arrange
            IEnumerable<int> enumerable = new int[] { 1, 2, 3 }.Where(x => x > 0);

            // Act & Assert
            var exception = Record.Exception(() =>
                ApplicationExceptionHandler.ValidateCollection(enumerable, TestClassName, TestMethodName));
            
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateCollection_WithEmptyIEnumerable_ShouldThrowEmptyListException()
        {
            // Arrange
            IEnumerable<int> emptyEnumerable = new int[] { 1, 2, 3 }.Where(x => x > 10);

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                ApplicationExceptionHandler.ValidateCollection(emptyEnumerable, TestClassName, TestMethodName));
            
            Assert.Equal(TestClassName, exception.ClassName);
            Assert.Equal(TestMethodName, exception.MethodName);
        }

        #endregion

        #region ValidateNotNullOrEmptyList Tests

        [Fact]
        public void ValidateNotNullOrEmptyList_WithNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            List<string> nullCollection = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ApplicationExceptionHandler.ValidateNotNullOrEmptyList(nullCollection, TestClassName, TestMethodName));
            
            Assert.Equal("collection", exception.ParamName);
        }

        [Fact]
        public void ValidateNotNullOrEmptyList_WithEmptyCollection_ShouldThrowEmptyListException()
        {
            // Arrange
            var emptyCollection = new List<string>();

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                ApplicationExceptionHandler.ValidateNotNullOrEmptyList(emptyCollection, TestClassName, TestMethodName));
            
            Assert.Equal(TestClassName, exception.ClassName);
            Assert.Equal(TestMethodName, exception.MethodName);
        }

        [Fact]
        public void ValidateNotNullOrEmptyList_WithValidCollection_ShouldNotThrow()
        {
            // Arrange
            var validCollection = new List<string> { "item1" };

            // Act & Assert
            var exception = Record.Exception(() =>
                ApplicationExceptionHandler.ValidateNotNullOrEmptyList(validCollection, TestClassName, TestMethodName));
            
            Assert.Null(exception);
        }

        #endregion

        #region ValidateEmptyList Tests

        [Fact]
        public void ValidateEmptyList_WithNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            List<string> nullCollection = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ApplicationExceptionHandler.ValidateEmptyList(nullCollection, TestClassName, TestMethodName));
            
            Assert.Equal("collection", exception.ParamName);
        }

        [Fact]
        public void ValidateEmptyList_WithEmptyCollection_ShouldNotThrow()
        {
            // Arrange
            var emptyCollection = new List<string>();

            // Act & Assert
            var exception = Record.Exception(() =>
                ApplicationExceptionHandler.ValidateEmptyList(emptyCollection, TestClassName, TestMethodName));
            
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateEmptyList_WithValidCollection_ShouldNotThrow()
        {
            // Arrange
            var validCollection = new List<string> { "item1" };

            // Act & Assert
            var exception = Record.Exception(() =>
                ApplicationExceptionHandler.ValidateEmptyList(validCollection, TestClassName, TestMethodName));
            
            Assert.Null(exception);
        }

        #endregion

        #region Custom Exception Tests

        [Fact]
        public void DatabaseException_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            string expectedClassName = "TestClass";
            string expectedMethodName = "TestMethod";
            string expectedMessage = "Database error occurred";

            // Act
            var exception = new DatabaseExeption(expectedClassName, expectedMethodName, expectedMessage);

            // Assert
            Assert.Equal(expectedClassName, exception.ClassName);
            Assert.Equal(expectedMethodName, exception.MethodName);
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void DatabaseException_ShouldSetPropertiesCorrectlyWithInnerException()
        {
            // Arrange
            string expectedClassName = "TestClass";
            string expectedMethodName = "TestMethod";
            string expectedMessage = "Database error occurred";
            var innerException = new Exception("Inner exception");

            // Act
            var exception = new DatabaseExeption(expectedClassName, expectedMethodName, expectedMessage, innerException);

            // Assert
            Assert.Equal(expectedClassName, exception.ClassName);
            Assert.Equal(expectedMethodName, exception.MethodName);
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void EmptyListException_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            string expectedClassName = "TestClass";
            string expectedMethodName = "TestMethod";
            string expectedMessage = "List is empty";

            // Act
            var exception = new EmptyListException(expectedClassName, expectedMethodName, expectedMessage);

            // Assert
            Assert.Equal(expectedClassName, exception.ClassName);
            Assert.Equal(expectedMethodName, exception.MethodName);
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void ExpectedEmptyListException_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            string expectedClassName = "TestClass";
            string expectedMethodName = "TestMethod";
            string expectedMessage = "List is expected to be empty";

            // Act
            var exception = new ExpectedEmptyListException(expectedClassName, expectedMethodName, expectedMessage);

            // Assert
            Assert.Equal(expectedClassName, exception.ClassName);
            Assert.Equal(expectedMethodName, exception.MethodName);
            Assert.Equal(expectedMessage, exception.Message);
        }
        #endregion
    }
}