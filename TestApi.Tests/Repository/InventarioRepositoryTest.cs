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
    public class InventarioRepositoryTest
    {
        private readonly InventarioRepository _inventarioRepository;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<A3AppInventario>> _mockSet;

        private readonly List<A3AppInventario> _inventarioDataSample = new List<A3AppInventario> {
            new A3AppInventario 
            { 
                InvId = 1,
                WorkerId = 1,
                SavedDate = new DateTime(2025, 6, 1),
                Item = "ITEM001",
                Description = "Test Item 1",
                BarCode = "BC001",
                FiscalYear = 2025,
                Storage = "STG01",
                BookInv = 10.5,
                Imported = false,
                UserImp = "User001",
                DataImp = new DateTime(2025, 6, 1)
            },
            new A3AppInventario 
            { 
                InvId = 2,
                WorkerId = 2,
                SavedDate = new DateTime(2025, 6, 2),
                Item = "ITEM002",
                Description = "Test Item 2",
                BarCode = "BC002",
                FiscalYear = 2025,
                Storage = "STG02",
                BookInv = 20.0,
                Imported = false,
                UserImp = "User002",
                DataImp = new DateTime(2025, 6, 2)
            }
        };

        private readonly List<InventarioFilter> _inventarioFilterSample = new List<InventarioFilter> {
            new InventarioFilter 
            { 
                InvId = 1,
                WorkerId = 1,
                SavedDate = new DateTime(2025, 6, 1),
                Item = "ITEM001",
                Description = "Test Item 1",
                BarCode = "BC001",
                FiscalYear = 2025,
                Storage = "STG01",
                BookInv = 10.5,
                Imported = false,
                UserImp = "User001",
                DataImp = new DateTime(2025, 6, 1)
            },
            new InventarioFilter 
            { 
                InvId = 2,
                WorkerId = 2,
                SavedDate = new DateTime(2025, 6, 2),
                Item = "ITEM002",
                Description = "Test Item 2",
                BarCode = "BC002",
                FiscalYear = 2025,
                Storage = "STG02",
                BookInv = 20.0,
                Imported = false,
                UserImp = "User002",
                DataImp = new DateTime(2025, 6, 2)
            }
        };

        private readonly ViewInventarioRequestFilter _inventarioRequestFilterSample = new ViewInventarioRequestFilter
        {
            WorkerId = 1,
            FromDateTime = new DateTime(2025, 1, 1),
            ToDateTime = new DateTime(2025, 12, 31),
            Item = "ITEM001",
            BarCode = "BC001"
        };

        private readonly ViewInventarioPutFilter _inventarioPutFilterSample = new ViewInventarioPutFilter
        {
            InvId = 1,
            BookInv = 10.5
        };

        public InventarioRepositoryTest()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<A3AppInventario>>();
            _inventarioRepository = new InventarioRepository(_mockContext.Object);
        }

        [Fact]
        public void GetInventario_ShouldReturnListOfInventario_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(_inventarioDataSample.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(_inventarioDataSample.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(_inventarioDataSample.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(_inventarioDataSample.GetEnumerator());
            
            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);
            
            // Act
            var result = _inventarioRepository.GetInventario();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_inventarioDataSample.Count(), result.Count());
            Assert.Equal(result, _inventarioDataSample);
        }

        [Fact]
        public void GetInventario_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var inventarioData = new List<A3AppInventario>().AsQueryable();

            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(inventarioData.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(inventarioData.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(inventarioData.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(inventarioData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.GetInventario();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void PostInventarioList_ShouldAddNewInventario_WhenDataIsValid()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>
            {
                new InventarioFilter 
                { 
                    WorkerId = 5,
                    SavedDate = new DateTime(2025, 6, 3),
                    Item = "ITEM003",
                    Description = "Test Item 3",
                    BarCode = "BC003",
                    FiscalYear = 2025,
                    Storage = "STG03",
                    BookInv = 30.0,
                    Imported = false,
                    UserImp = "User003",
                    DataImp = new DateTime(2025, 6, 3)
                }
            };

            var inventarioData = _inventarioDataSample.AsQueryable();
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(inventarioData.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(inventarioData.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(inventarioData.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(inventarioData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.PostInventarioList(inventarioFilter.ToList());

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("ITEM003", result.First().Item);
        }

        [Fact]
        public void PostInventarioList_ShouldUpdateExistingInventario_WhenDataExists()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>
            {
                new InventarioFilter 
                { 
                    InvId = 1,
                    WorkerId = 1,
                    Item = "ITEM001",
                    Description = "Test Item 1",
                    BarCode = "BC001",
                    FiscalYear = 2025,
                    Storage = "STG01",
                    BookInv = 15.0
                }
            };
            var inventarioData = _inventarioDataSample.AsQueryable();

            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(inventarioData.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(inventarioData.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(inventarioData.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(inventarioData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.PostInventarioList(inventarioFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(15.0, result.First().BookInv);
        }

        [Fact]
        public void PostInventarioList_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>();

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.PostInventarioList(inventarioFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetInventarioByFilter_ShouldReturnFilteredInventario_WhenDataExists()
        {
            // Arrange
            var inventarioData = _inventarioDataSample.AsQueryable();

            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(_inventarioDataSample.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(_inventarioDataSample.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(_inventarioDataSample.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(_inventarioDataSample.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.GetViewInventario(_inventarioRequestFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_inventarioDataSample.First().Item, result.First().Item);
            Assert.Equal(_inventarioDataSample.First().BarCode, result.First().BarCode);
            Assert.Equal(_inventarioDataSample.First().BookInv, result.First().BookInv);
            Assert.Equal(_inventarioDataSample.First().SavedDate, result.First().SavedDate);
            Assert.Equal(_inventarioDataSample.First().Description, result.First().Description);
            Assert.Equal(_inventarioDataSample.First().Storage, result.First().Storage);
            Assert.Equal(_inventarioDataSample.First().FiscalYear, result.First().FiscalYear);
            Assert.Equal(_inventarioDataSample.First().WorkerId, result.First().WorkerId);
            Assert.Equal(_inventarioDataSample.First().Imported, result.First().Imported);
            Assert.Equal(_inventarioDataSample.First().UserImp, result.First().UserImp);
            Assert.Equal(_inventarioDataSample.First().DataImp, result.First().DataImp);
        }

        [Fact]
        public void GetInventarioByFilter_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var inventarioData = new List<A3AppInventario>().AsQueryable();

            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(inventarioData.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(inventarioData.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(inventarioData.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(inventarioData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.GetViewInventario(_inventarioRequestFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetInventarioByFilter_ShouldReturnFilteredInventario_WhenDataExistsWithBarCode()
        {
            // Arrange
            var inventarioData = _inventarioDataSample.AsQueryable();

            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(inventarioData.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(inventarioData.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(inventarioData.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(inventarioData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.GetViewInventario(_inventarioRequestFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetInventarioByFilter_ShouldReturnEmptyList_WhenNoDataExistsWithBarCode()
        {
            // Arrange
            var inventarioData = new List<A3AppInventario>().AsQueryable();

            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(inventarioData.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(inventarioData.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(inventarioData.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(inventarioData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.GetViewInventario(_inventarioRequestFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void PutInventario_ShoulReturnInventario_WhenDataExists()
        {
            // Arrange
            var inventarioData = _inventarioDataSample.AsQueryable();

            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(inventarioData.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(inventarioData.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(inventarioData.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(inventarioData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.PutViewInventario(_inventarioPutFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.InvId);
            Assert.Equal(10.5, result.BookInv);
        }

        [Fact]
        public void PutInventario_ShouldReturnNull_WhenDataDoesNotExist()
        {
            // Arrange
            var inventarioData = new List<A3AppInventario>().AsQueryable();

            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(inventarioData.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(inventarioData.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(inventarioData.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(inventarioData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);

            // Act
            var result = _inventarioRepository.PutViewInventario(_inventarioPutFilterSample);

            // Assert
            Assert.Null(result);
        }
    }
}