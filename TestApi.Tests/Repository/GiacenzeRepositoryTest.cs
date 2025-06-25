using System;
using System.Collections.Generic;
using System.Linq;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Implementation;
using apiPB.Utils.Implementation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestApi.Tests.Repository
{
    public class GiacenzeRepositoryTest
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<VwApiGiacenze>> _mockSet;
        private readonly GiacenzeRepository _giacenzeRepository;

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
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<VwApiGiacenze>>();
            _giacenzeRepository = new GiacenzeRepository(_mockContext.Object);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnListOfGiacenze_WhenDataExists()
        {
            // Arrange
            var giacenzeData = _giacenzeDataSample.AsQueryable();

            // Setup completo del mock DbSet per supportare LINQ queries
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Provider).Returns(giacenzeData.Provider);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Expression).Returns(giacenzeData.Expression);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.ElementType).Returns(giacenzeData.ElementType);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(() => giacenzeData.GetEnumerator());
            
            // Setup per supportare IEnumerable.GetEnumerator()
            _mockSet.As<IEnumerable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(() => giacenzeData.GetEnumerator());
            
            // Setup del context
            _mockContext.Setup(c => c.VwApiGiacenzes).Returns(_mockSet.Object);

            // Act
            var result = _giacenzeRepository.GetGiacenze();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, g => g.Item == "ITEM001");
            Assert.Contains(result, g => g.Item == "ITEM002");
            Assert.Contains(result, g => g.Description == "Test Item 1");
            Assert.Contains(result, g => g.Description == "Test Item 2");
            Assert.Contains(result, g => g.BookInv == 10.5);
            Assert.Contains(result, g => g.BookInv == 20.0);
        }

        [Fact]
        public void GetGiacenze_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            var emptyGiacenzeData = new List<VwApiGiacenze>().AsQueryable();

            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Provider).Returns(emptyGiacenzeData.Provider);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Expression).Returns(emptyGiacenzeData.Expression);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.ElementType).Returns(emptyGiacenzeData.ElementType);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(() => emptyGiacenzeData.GetEnumerator());
            
            // Setup per supportare IEnumerable.GetEnumerator()
            _mockSet.As<IEnumerable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(() => emptyGiacenzeData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiGiacenzes).Returns(_mockSet.Object);

            // Act & Assert
            Assert.Throws<EmptyListException>(() => _giacenzeRepository.GetGiacenze());
        }

        [Fact]
        public void GetGiacenze_ShouldThrowArgumentNullException_WhenDbSetIsNull()
        {
            // Arrange
            _mockContext.Setup(c => c.VwApiGiacenzes).Returns((DbSet<VwApiGiacenze>)null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _giacenzeRepository.GetGiacenze());
        }

        // NUOVO TEST: Verifica che il metodo funzioni correttamente con un singolo elemento
        [Fact]
        public void GetGiacenze_ShouldReturnSingleItem_WhenOnlyOneItemExists()
        {
            // Arrange
            var singleItemData = new List<VwApiGiacenze> { _giacenzeDataSample[0] }.AsQueryable();

            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Provider).Returns(singleItemData.Provider);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Expression).Returns(singleItemData.Expression);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.ElementType).Returns(singleItemData.ElementType);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(() => singleItemData.GetEnumerator());
            
            // Setup per supportare IEnumerable.GetEnumerator()
            _mockSet.As<IEnumerable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(() => singleItemData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiGiacenzes).Returns(_mockSet.Object);

            // Act
            var result = _giacenzeRepository.GetGiacenze();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("ITEM001", result.First().Item);
            Assert.Equal("Test Item 1", result.First().Description);
            Assert.Equal(10.5, result.First().BookInv);
        }

        [Fact]
        public void GetGiacenze_ShouldCallVwApiGiacenzes_OnContext()
        {
            // Arrange
            var giacenzeData = _giacenzeDataSample.AsQueryable();

            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Provider).Returns(giacenzeData.Provider);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.Expression).Returns(giacenzeData.Expression);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.ElementType).Returns(giacenzeData.ElementType);
            _mockSet.As<IQueryable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(() => giacenzeData.GetEnumerator());
            
            // Setup per supportare IEnumerable.GetEnumerator()
            _mockSet.As<IEnumerable<VwApiGiacenze>>().Setup(m => m.GetEnumerator()).Returns(() => giacenzeData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiGiacenzes).Returns(_mockSet.Object);

            // Act
            var result = _giacenzeRepository.GetGiacenze();

            // Assert
            _mockContext.Verify(c => c.VwApiGiacenzes, Times.Once);
            Assert.NotNull(result);
        }
    }
}