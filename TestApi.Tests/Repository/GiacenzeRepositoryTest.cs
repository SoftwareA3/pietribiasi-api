using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Abstraction;
using apiPB.Repository.Implementation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestApi.Tests.Repository
{
    public class GiacenzeRepositoryTest
    {
        private readonly GiacenzeRepository _giacenzeRepository;
        private readonly Mock<ApplicationDbContext> _mockContext = new Mock<ApplicationDbContext>();
        private readonly Mock<DbSet<VwApiGiacenze>> _mockSet = new Mock<DbSet<VwApiGiacenze>>();

        private readonly List<VwApiGiacenze> _giacenzeDataSample = new List<VwApiGiacenze>
        {
            new VwApiGiacenze 
            { 
                Item = "ITEM001", 
                Description = "Test Item 1", 
                BarCode = "BC001", 
                FiscalYear = 2025, 
                Storage = "STG01", 
                BookInv = 10.5 
            },
            new VwApiGiacenze 
            { 
                Item = "ITEM002", 
                Description = "Test Item 2", 
                BarCode = "BC002", 
                FiscalYear = 2025, 
                Storage = "STG02", 
                BookInv = 20.0 
            }
        };

        public GiacenzeRepositoryTest()
        {
            _giacenzeRepository = new GiacenzeRepository(_mockContext.Object);
            _mockSet = new Mock<DbSet<VwApiGiacenze>>();
        }

        [Fact]
        public void GetGiacenze_ShouldReturnListOfGiacenze()
        {
            // Arrange
            var giacenzeData = _giacenzeDataSample.AsQueryable();

            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Provider).Returns(giacenzeData.Provider);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Expression).Returns(giacenzeData.Expression);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.ElementType).Returns(giacenzeData.ElementType);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(giacenzeData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiGiacenzes).Returns(_mockSet.Object);

            // Act
            var result = _giacenzeRepository.GetGiacenze();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, g => g.Item == "ITEM001");
            Assert.Contains(result, g => g.Item == "ITEM002");
        }

        [Fact]
        public void GetGiacenze_ShouldReturnEmptyList_WhenNoDataExist()
        {
            // Arrange
            var giacenzeData = new List<VwApiGiacenze>().AsQueryable();

            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Provider).Returns(giacenzeData.Provider);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Expression).Returns(giacenzeData.Expression);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.ElementType).Returns(giacenzeData.ElementType);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(giacenzeData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiGiacenzes).Returns(_mockSet.Object);

            // Act
            var result = _giacenzeRepository.GetGiacenze();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GiacenzeRepository_ShouldInitializeWithContext()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();

            // Act & Assert
            var exception = Record.Exception(() => new GiacenzeRepository(mockContext.Object));
            Assert.Null(exception);
        }
    }
}