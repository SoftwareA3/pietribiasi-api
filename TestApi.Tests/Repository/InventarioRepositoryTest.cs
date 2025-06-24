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
                PrevBookInv = 8.0,
                BookInvDiff = 2.5,
                InvRsn = true,
                Imported = false,
                UserImp = "User001",
                DataImp = new DateTime(2025, 6, 1),
                UoM = "PCS"
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
                PrevBookInv = 25.0,
                BookInvDiff = 5.0,
                InvRsn = false,
                Imported = true,
                UserImp = "User002",
                DataImp = new DateTime(2025, 6, 2),
                UoM = "KG"
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
            BookInv = 15.0
        };

        private readonly WorkerIdSyncFilter _syncFilterSample = new WorkerIdSyncFilter
        {
            WorkerId = "1"
        };

        private readonly UpdateImportedIdFilter _updateImportedFilterSample = new UpdateImportedIdFilter
        {
            Id = 1,
            WorkerId = "1"
        };

        public InventarioRepositoryTest()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<A3AppInventario>>();
            _inventarioRepository = new InventarioRepository(_mockContext.Object);
        }

        private void SetupMockDbSet(IQueryable<A3AppInventario> data)
        {
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<A3AppInventario>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            _mockContext.Setup(c => c.A3AppInventarios).Returns(_mockSet.Object);
        }

        #region GetInventario Tests

        [Fact]
        public void GetInventario_ShouldReturnListOfInventario_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            
            // Act
            var result = _inventarioRepository.GetInventario();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            // Should be ordered by SavedDate descending
            Assert.Equal(_inventarioDataSample[1].InvId, result.First().InvId);
            Assert.Equal(_inventarioDataSample[0].InvId, result.Last().InvId);
        }

        [Fact]
        public void GetInventario_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(new List<A3AppInventario>().AsQueryable());

            // Act
            var result = _inventarioRepository.GetInventario();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region PostInventarioList Tests

        [Fact]
        public void PostInventarioList_ShouldAddNewInventario_WhenDataIsValid()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>
            {
                new InventarioFilter 
                { 
                    WorkerId = 3,
                    Item = "ITEM003",
                    Description = "Test Item 3",
                    BarCode = "BC003",
                    FiscalYear = 2025,
                    Storage = "STG03",
                    BookInv = 30.0,
                    PrevBookInv = 25.0,
                    UoM = "PCS"
                }
            };

            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _inventarioRepository.PostInventarioList(inventarioFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("ITEM003", result.First().Item);
            Assert.Equal(30.0, result.First().BookInv);
            Assert.Equal(25.0, result.First().PrevBookInv);
            Assert.Equal(5.0, result.First().BookInvDiff);
            Assert.True(result.First().InvRsn);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void PostInventarioList_ShouldUpdateExistingInventario_WhenItemExists()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>
            {
                new InventarioFilter 
                { 
                    WorkerId = 1,
                    Item = "ITEM001",
                    Description = "Test Item 1 Updated",
                    BarCode = "BC001",
                    BookInv = 15.0,
                    PrevBookInv = 10.0,
                    UoM = "PCS"
                }
            };

            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _inventarioRepository.PostInventarioList(inventarioFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("ITEM001", result.First().Item);
            Assert.Equal(15.0, result.First().BookInv);
            Assert.Equal(10.0, result.First().PrevBookInv);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void PostInventarioList_ShouldThrowEmptyListException_WhenEmptyList()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>();

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => _inventarioRepository.PostInventarioList(inventarioFilter));
            Assert.Equal("La collezione non può essere vuota", exception.Message);
        }

        [Fact]
        public void PostInventarioList_ShouldCalculateBookInvDiffCorrectly_WhenBookInvIsLower()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>
            {
                new InventarioFilter 
                { 
                    WorkerId = 3,
                    Item = "ITEM003",
                    Description = "Test Item 3",
                    BarCode = "BC003",
                    BookInv = 10.0,
                    PrevBookInv = 15.0,
                    UoM = "PCS"
                }
            };

            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _inventarioRepository.PostInventarioList(inventarioFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(5.0, result.First().BookInvDiff);
            Assert.False(result.First().InvRsn);
        }

        #endregion

        #region GetViewInventario Tests

        [Fact]
        public void GetViewInventario_ShouldReturnFilteredInventario_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_inventarioDataSample.AsQueryable());

            // Act
            var result = _inventarioRepository.GetViewInventario(_inventarioRequestFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("ITEM001", result.First().Item);
            Assert.Equal("BC001", result.First().BarCode);
            Assert.Equal(1, result.First().WorkerId);
        }

        [Fact]
        public void GetViewInventario_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(new List<A3AppInventario>().AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => _inventarioRepository.GetViewInventario(_inventarioRequestFilterSample));
            Assert.Equal("La collezione non può essere vuota", exception.Message);
        }

        [Fact]
        public void GetViewInventario_ShouldOrderByDataImp_WhenImportedIsTrue()
        {
            // Arrange
            var filter = new ViewInventarioRequestFilter
            {
                Imported = true
            };
            SetupMockDbSet(_inventarioDataSample.AsQueryable());

            // Act
            var result = _inventarioRepository.GetViewInventario(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            // Should be ordered by DataImp descending
            Assert.Equal(_inventarioDataSample[1].InvId, result.First().InvId);
        }

        [Fact]
        public void GetViewInventario_ShouldOrderBySavedDate_WhenImportedIsFalseOrNull()
        {
            // Arrange
            var filter = new ViewInventarioRequestFilter
            {
                Imported = false
            };
            SetupMockDbSet(_inventarioDataSample.AsQueryable());

            // Act
            var result = _inventarioRepository.GetViewInventario(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(_inventarioDataSample[0].InvId, result.Last().InvId);
            Assert.Equal(_inventarioDataSample[1].InvId, result.First().InvId);
        }

        #endregion

        #region PutViewInventario Tests

        [Fact]
        public void PutViewInventario_ShouldReturnUpdatedInventario_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _inventarioRepository.PutViewInventario(_inventarioPutFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.InvId);
            Assert.Equal(15.0, result.BookInv);
            Assert.Equal(7.0, result.BookInvDiff); // |15.0 - 8.0|
            Assert.True(result.InvRsn); // 15.0 > 8.0
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void PutViewInventario_ShouldThrowArgumentNullException_WhenDataDoesNotExist()
        {
            // Arrange
            SetupMockDbSet(new List<A3AppInventario>().AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _inventarioRepository.PutViewInventario(_inventarioPutFilterSample));
            Assert.Contains("Inventario non trovato", exception.Message);
        }

        #endregion

        #region GetNotImportedInventario Tests

        [Fact]
        public void GetNotImportedInventario_ShouldReturnNotImportedItems_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_inventarioDataSample.AsQueryable());

            // Act
            var result = _inventarioRepository.GetNotImportedInventario();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().InvId);
            Assert.False(result.First().Imported);
        }

        [Fact]
        public void GetNotImportedInventario_ShouldReturnEmptyList_WhenNoNotImportedItemsExist()
        {
            // Arrange
            var importedItems = _inventarioDataSample.Select(i => { i.Imported = true; return i; }).ToList();
            SetupMockDbSet(importedItems.AsQueryable());

            // Act
            var result = _inventarioRepository.GetNotImportedInventario();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region UpdateInventarioImported Tests

        [Fact]
        public void UpdateInventarioImported_ShouldUpdateImportedStatus_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _inventarioRepository.UpdateInventarioImported(_syncFilterSample);

            // Assert
            Assert.NotNull(result);
            _mockContext.Verify(c => c.SaveChanges(), Times.AtLeastOnce);
        }

        [Fact]
        public void UpdateInventarioImported_ShouldThrowArgumentNullException_WhenFilterIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _inventarioRepository.UpdateInventarioImported(null));
            Assert.Contains("Il filtro non può essere nullo", exception.Message);
        }

        [Fact]
        public void UpdateInventarioImported_ShouldThrowArgumentNullException_WhenWorkerIdIsNull()
        {
            // Arrange
            var filter = new WorkerIdSyncFilter { WorkerId = null };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _inventarioRepository.UpdateInventarioImported(filter));
            Assert.Contains("Il filtro non può essere nullo", exception.Message);
        }

        [Fact]
        public void UpdateInventarioImported_ShouldThrowEmptyListException_WhenNoNotImportedItemsExist()
        {
            // Arrange
            var importedItems = _inventarioDataSample.Select(i => { i.Imported = true; return i; }).ToList();
            SetupMockDbSet(importedItems.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => _inventarioRepository.UpdateInventarioImported(_syncFilterSample));
            Assert.Equal("La collezione non può essere vuota", exception.Message);
        }

        #endregion

        #region GetNotImportedAppInventarioByFilter Tests

        [Fact]
        public void GetNotImportedAppInventarioByFilter_ShouldReturnFilteredNotImportedItems_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_inventarioDataSample.AsQueryable());

            // Act
            var result = _inventarioRepository.GetNotImportedAppInventarioByFilter(_inventarioRequestFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().InvId);
            Assert.False(result.First().Imported);
        }

        [Fact]
        public void GetNotImportedAppInventarioByFilter_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(new List<A3AppInventario>().AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => _inventarioRepository.GetNotImportedAppInventarioByFilter(_inventarioRequestFilterSample));
            Assert.Equal("La collezione non può essere vuota", exception.Message);
        }

        #endregion

        #region UpdateImportedById Tests

        [Fact]
        public void UpdateImportedById_ShouldUpdateSpecificItem_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _inventarioRepository.UpdateImportedById(_updateImportedFilterSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.InvId);
            Assert.True(result.Imported);
            Assert.Equal("1", result.UserImp);
            Assert.NotNull(result.DataImp);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateImportedById_ShouldThrowArgumentNullException_WhenFilterIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _inventarioRepository.UpdateImportedById(null));
            Assert.Contains("Il filtro non può essere nullo", exception.Message);
        }

        [Fact]
        public void UpdateImportedById_ShouldThrowArgumentNullException_WhenWorkerIdIsNull()
        {
            // Arrange
            var filter = new UpdateImportedIdFilter { Id = 1, WorkerId = null };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _inventarioRepository.UpdateImportedById(filter));
            Assert.Contains("Il filtro non può essere nullo", exception.Message);
        }

        [Fact]
        public void UpdateImportedById_ShouldThrowException_WhenItemNotFound()
        {
            // Arrange
            SetupMockDbSet(new List<A3AppInventario>().AsQueryable());
            var filter = new UpdateImportedIdFilter { Id = 999, WorkerId = "1" };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _inventarioRepository.UpdateImportedById(filter));
            Assert.Contains("Inventario non trovato", exception.Message);
        }

        [Fact]
        public void UpdateImportedById_ShouldThrowException_WhenItemIsAlreadyImported()
        {
            // Arrange
            var importedItems = _inventarioDataSample.Select(i => { i.Imported = true; return i; }).ToList();
            SetupMockDbSet(importedItems.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _inventarioRepository.UpdateImportedById(_updateImportedFilterSample));
            Assert.Contains("Inventario non trovato", exception.Message);
        }

        #endregion

        #region CalculateBookInvDiff Tests (Private method testing through public methods)

        [Fact]
        public void CalculateBookInvDiff_ShouldSetInvRsnTrue_WhenBookInvIsGreaterThanPrevBookInv()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>
            {
                new InventarioFilter 
                { 
                    WorkerId = 3,
                    Item = "ITEM003",
                    BookInv = 20.0,
                    PrevBookInv = 15.0,
                    UoM = "PCS"
                }
            };

            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _inventarioRepository.PostInventarioList(inventarioFilter);

            // Assert
            Assert.True(result.First().InvRsn);
            Assert.Equal(5.0, result.First().BookInvDiff);
        }

        [Fact]
        public void CalculateBookInvDiff_ShouldThrowArgumentNullException_WhenInventarioFilterIsNull()
        {
            // Arrange
            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);
            var nullFilter = new List<InventarioFilter>
            {
                new InventarioFilter
                {
                    WorkerId = 3,
                    Item = "Item123", 
                    BookInv = null,
                    PrevBookInv = 15.0,
                    UoM = "PCS"
                }
            };


            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => _inventarioRepository.PostInventarioList(nullFilter));
            Assert.Contains("Il parametro 'inventario' non può essere nullo", exception.Message);
        }

        [Fact]
        public void CalculateBookInvDiff_ShouldSetInvRsnFalse_WhenBookInvIsLessThanPrevBookInv()
        {
            // Arrange
            var inventarioFilter = new List<InventarioFilter>
            {
                new InventarioFilter 
                { 
                    WorkerId = 3,
                    Item = "ITEM003",
                    BookInv = 10.0,
                    PrevBookInv = 15.0,
                    UoM = "PCS"
                }
            };

            SetupMockDbSet(_inventarioDataSample.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _inventarioRepository.PostInventarioList(inventarioFilter);

            // Assert
            Assert.False(result.First().InvRsn);
            Assert.Equal(5.0, result.First().BookInvDiff);
        }

        #endregion
    }
}