using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Implementation;
using apiPB.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Xunit;
using apiPB.Utils.Implementation;

namespace TestApi.Tests.Repository
{
    public class WorkerRepositoryTest
    {
        private readonly WorkerRepository _workerRepository;
        private readonly Mock<DbSet<VwApiWorker>> _mockWorkerSet;
        private readonly Mock<DbSet<VwApiWorkersfield>> _mockWorkersFieldSet;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DatabaseFacade> _mockDatabase;

        private readonly List<VwApiWorker> _workers = new List<VwApiWorker>
        {
            new VwApiWorker
            {
                WorkerId = 1,
                Password = "password123",
                Name = "Test Worker 1"
            },
            new VwApiWorker
            {
                WorkerId = 2,
                Password = "password456",
                Name = "Test Worker 2"
            }
        };

        private readonly List<VwApiWorkersfield> _workersFields = new List<VwApiWorkersfield>
        {
            new VwApiWorkersfield
            {
                WorkerId = 1,
                Line = 1,
                FieldValue = "Field Value 1"
            },
            new VwApiWorkersfield
            {
                WorkerId = 1,
                Line = 2,
                FieldValue = "Field Value 2"
            },
            new VwApiWorkersfield
            {
                WorkerId = 2,
                Line = 1,
                FieldValue = "Field Value 3"
            }
        };

        public WorkerRepositoryTest()
        {
            _mockWorkerSet = new Mock<DbSet<VwApiWorker>>();
            _mockWorkersFieldSet = new Mock<DbSet<VwApiWorkersfield>>();
            _mockContext = new Mock<ApplicationDbContext>();
            _mockDatabase = new Mock<DatabaseFacade>(_mockContext.Object);
            _workerRepository = new WorkerRepository(_mockContext.Object);
        }

        #region GetWorkers Tests

        [Fact]
        public void GetWorkers_ShouldReturnWorkersList_WhenWorkersExist()
        {
            // Arrange
            var queryableData = _workers.AsQueryable();
            
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkers).Returns(_mockWorkerSet.Object);

            // Act
            var result = _workerRepository.GetWorkers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.IsAssignableFrom<IEnumerable<VwApiWorker>>(result);
        }

        [Fact]
        public void GetWorkers_ShouldThrowException_WhenNoWorkersExist()
        {
            // Arrange
            var emptyData = new List<VwApiWorker>().AsQueryable();
            
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Provider).Returns(emptyData.Provider);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Expression).Returns(emptyData.Expression);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.ElementType).Returns(emptyData.ElementType);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.GetEnumerator()).Returns(emptyData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkers).Returns(_mockWorkerSet.Object);

            // Act & Assert
            // Modified to expect a generic Exception as per ApplicationExceptionHandler's behavior
            Assert.Throws<EmptyListException>(_workerRepository.GetWorkers);
        }

        #endregion

        #region GetWorkerByPassword Tests

        [Fact]
        public void GetWorkerByPassword_ShouldReturnWorker_WhenPasswordMatches()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = "password123" };
            var queryableData = _workers.AsQueryable();
            
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkers).Returns(_mockWorkerSet.Object);

            // Act
            var result = _workerRepository.GetWorkerByPassword(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.WorkerId);
            Assert.Equal("password123", result.Password);
        }

        [Fact]
        public void GetWorkerByPassword_ShouldThrowArgumentNullException_WhenPasswordNotFound()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = "nonexistentpassword" };
            var queryableData = _workers.AsQueryable();
            
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkers).Returns(_mockWorkerSet.Object);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _workerRepository.GetWorkerByPassword(filter));
            Assert.StartsWith("Value cannot be null", exception.Message);
        }

        #endregion

        #region GetWorkerByIdAndPassword Tests

        [Fact]
        public void GetWorkerByIdAndPassword_ShouldReturnWorker_WhenIdAndPasswordMatch()
        {
            // Arrange
            var filter = new WorkerIdAndPasswordFilter { WorkerId = 1, Password = "password123" };
            var queryableData = _workers.AsQueryable();
            
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkers).Returns(_mockWorkerSet.Object);

            // Act
            var result = _workerRepository.GetWorkerByIdAndPassword(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.WorkerId);
            Assert.Equal("password123", result.Password);
        }

        [Fact]
        public void GetWorkerByIdAndPassword_ShouldThrowArgumentNullException_WhenNoMatch()
        {
            // Arrange
            var filter = new WorkerIdAndPasswordFilter { WorkerId = 1, Password = "wrongpassword" };
            var queryableData = _workers.AsQueryable();
            
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkers).Returns(_mockWorkerSet.Object);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _workerRepository.GetWorkerByIdAndPassword(filter));
            Assert.StartsWith("Value cannot be null", exception.Message);
        }

        #endregion

        #region GetWorkersFieldsById Tests

        [Fact]
        public void GetWorkersFieldsById_ShouldReturnWorkerFields_WhenWorkerIdExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = 1 };
            var queryableData = _workersFields.AsQueryable();
            
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkersfields).Returns(_mockWorkersFieldSet.Object);

            // Act
            var result = _workerRepository.GetWorkersFieldsById(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, field => Assert.Equal(1, field.WorkerId));
        }

        [Fact]
        public void GetWorkersFieldsById_ShouldThrowException_WhenNoFieldsFound()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = 999 };
            var queryableData = _workersFields.AsQueryable();
            
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkersfields).Returns(_mockWorkersFieldSet.Object);

            // Act & Assert
            // Modified to expect a generic Exception as per ApplicationExceptionHandler's behavior
            Assert.Throws<EmptyListException>(() => _workerRepository.GetWorkersFieldsById(filter));
        }

        #endregion

        #region GetLastWorkerFieldLine Tests

        [Fact]
        public void GetLastWorkerFieldLine_ShouldReturnLastField_WhenFieldsExist()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = 1 };
            var queryableData = _workersFields.AsQueryable();
            
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkersFieldSet.As<IQueryable<VwApiWorkersfield>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkersfields).Returns(_mockWorkersFieldSet.Object);

            // Act
            var result = _workerRepository.GetLastWorkerFieldLine(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.WorkerId);
            Assert.Equal(2, result.Line); // Should return the highest line number
            Assert.Equal("Field Value 2", result.FieldValue);
        }

        #endregion

        #region CreateOrUpdateLastLogin Tests

        [Fact]
        public async Task CreateOrUpdateLastLogin_ShouldThrowInvalidOperationException_WhenWorkerNotFound()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = "nonexistentpassword" };
            var queryableData = _workers.AsQueryable();
            
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkers).Returns(_mockWorkerSet.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _workerRepository.CreateOrUpdateLastLogin(filter));
            Assert.StartsWith("Value cannot be null", exception.Message);
        }

        #endregion

        #region Additional Edge Case Tests

        [Fact]
        public void GetWorkers_ShouldCallVwApiWorkers_OnContext()
        {
            // Arrange
            var queryableData = _workers.AsQueryable();
            
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockWorkerSet.As<IQueryable<VwApiWorker>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiWorkers).Returns(_mockWorkerSet.Object);

            // Act
            var result = _workerRepository.GetWorkers();

            // Assert
            _mockContext.Verify(c => c.VwApiWorkers, Times.Once);
            Assert.NotNull(result);
        }

        #endregion
    }
}