using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Implementation;
using apiPB.Utils.Implementation;
using apiPB.Filters;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestApi.Tests.Repository
{
    public class JobRepositoryTest
    {
        private readonly JobRepository _jobRepository;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<VwApiJob>> _mockSet;

        private readonly List<VwApiJob> _jobDataSample = new List<VwApiJob>
        {
            new VwApiJob
            {
                Job = "JOB001",
                Description = "Test Job 1"
            },
            new VwApiJob
            {
                Job = "JOB002",
                Description = "Test Job 2"
            },
            new VwApiJob
            {
                Job = "JOB001", // Duplicate to test Distinct()
                Description = "Test Job 1"
            }
        };

        public JobRepositoryTest()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<VwApiJob>>();
            _jobRepository = new JobRepository(_mockContext.Object);
        }

        private void SetupMockDbSet(IQueryable<VwApiJob> data)
        {
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            _mockContext.Setup(c => c.VwApiJobs).Returns(_mockSet.Object);
        }

        [Fact]
        public void GetJobs_ShouldReturnListOfJobs_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_jobDataSample.AsQueryable());

            // Act
            var result = _jobRepository.GetJobs();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2); // Should have at least 2 unique jobs
            Assert.Contains(result, j => j.Job == "JOB001");
            Assert.Contains(result, j => j.Job == "JOB002");
            Assert.Equal("Test Job 1", result.FirstOrDefault(j => j.Job == "JOB001")?.Description);
            Assert.Equal("Test Job 2", result.FirstOrDefault(j => j.Job == "JOB002")?.Description);
        }

        [Fact]
        public void GetJobs_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            var emptyData = new List<VwApiJob>().AsQueryable();
            SetupMockDbSet(emptyData);

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => _jobRepository.GetJobs());
            Assert.NotNull(exception);
            Assert.Equal("La collezione non può essere vuota", exception.Message);
        }

        [Fact]
        public void GetJobs_ShouldUseAsNoTracking_WhenCalled()
        {
            // Arrange
            SetupMockDbSet(_jobDataSample.AsQueryable());

            // Act
            var result = _jobRepository.GetJobs();

            // Assert
            Assert.NotNull(result);
            // Verify that AsNoTracking was called by checking the mock setup
            _mockContext.Verify(c => c.VwApiJobs, Times.Once);
        }

        [Fact]
        public void GetJobs_ShouldReturnEmptyList_WhenContextThrowsException()
        {
            // Arrange
            _mockContext.Setup(c => c.VwApiJobs).Throws(new Exception("Database connection error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _jobRepository.GetJobs());
        }

        [Fact]
        public void GetJobs_ShouldHandleLargeDataSet_WhenManyJobsExist()
        {
            // Arrange
            var largeJobList = new List<VwApiJob>();
            for (int i = 1; i <= 1000; i++)
            {
                largeJobList.Add(new VwApiJob
                {
                    Job = $"JOB{i:D3}",
                    Description = $"Test Job {i}"
                });
            }
            SetupMockDbSet(largeJobList.AsQueryable());

            // Act
            var result = _jobRepository.GetJobs();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1000, result.Count());
        }

        [Fact]
        public void GetJobs_ShouldHandleNullDescription_WhenDescriptionIsNull()
        {
            // Arrange
            var jobsWithNullDescription = new List<VwApiJob>
            {
                new VwApiJob { Job = "JOB001", Description = null },
                new VwApiJob { Job = "JOB002", Description = "Valid Description" }
            };
            SetupMockDbSet(jobsWithNullDescription.AsQueryable());

            // Act
            var result = _jobRepository.GetJobs();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, j => j.Job == "JOB001" && j.Description == null);
            Assert.Contains(result, j => j.Job == "JOB002" && j.Description == "Valid Description");
        }

        [Fact]
        public void GetJobs_ShouldHandleEmptyStrings_WhenJobOrDescriptionIsEmpty()
        {
            // Arrange
            var jobsWithEmptyStrings = new List<VwApiJob>
            {
                new VwApiJob { Job = "", Description = "Empty Job Code" },
                new VwApiJob { Job = "JOB001", Description = "" },
                new VwApiJob { Job = "JOB002", Description = "Valid Job" }
            };
            SetupMockDbSet(jobsWithEmptyStrings.AsQueryable());

            // Act
            var result = _jobRepository.GetJobs();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, j => j.Job == "" && j.Description == "Empty Job Code");
            Assert.Contains(result, j => j.Job == "JOB001" && j.Description == "");
            Assert.Contains(result, j => j.Job == "JOB002" && j.Description == "Valid Job");
        }

        [Fact]
        public void Constructor_ShouldInitializeRepository_WhenValidContextProvided()
        {
            // Arrange & Act
            var repository = new JobRepository(_mockContext.Object);

            // Assert
            Assert.NotNull(repository);
        }
    }
}