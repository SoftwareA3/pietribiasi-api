using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Implementation;
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
            }
        };

        public JobRepositoryTest()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<VwApiJob>>();
            _jobRepository = new JobRepository(_mockContext.Object);
        }

        [Fact]
        public void GetJobs_ShouldReturnListOfJobs_WhenDataExists()
        {
            // Arrange
            var data = _jobDataSample.AsQueryable();

            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockContext.Setup(c => c.VwApiJobs).Returns(_mockSet.Object);

            // Act
            var result = _jobRepository.GetJobs();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(_jobDataSample.First().Job, result.First().Job);
            Assert.Equal(_jobDataSample.First().Description, result.First().Description);
        }

        [Fact]
        public void GetJobs_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var data = new List<VwApiJob>().AsQueryable();

            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<VwApiJob>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockContext.Setup(c => c.VwApiJobs).Returns(_mockSet.Object);

            // Act
            var result = _jobRepository.GetJobs();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}