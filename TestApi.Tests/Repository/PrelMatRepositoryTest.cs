using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Implementation;
using apiPB.Filters;
using apiPB.Utils.Implementation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestApi.Tests.Repository
{
    public class PrelMatRepositoryTest
    {
        private readonly PrelMatRepository _prelMatRepository;
        private readonly Mock<DbSet<A3AppPrelMat>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;

        private readonly List<A3AppPrelMat> _prelMats = new List<A3AppPrelMat>
        {
            new A3AppPrelMat
            {
                PrelMatId = 1,
                WorkerId = 1,
                SavedDate = new DateTime(2023, 6, 2),
                Job = "Job1",
                RtgStep = 1,
                Alternate = "Alt1",
                AltRtgStep = 2,
                Operation = "Op1",
                OperDesc = "Operation 1",
                Bom = "BOM1",
                Variant = "Variant1",
                Position = 1,
                Component = "Component1",
                ItemDesc = "Item Description 1",
                Moid = 1,
                Mono = "Mono1",
                CreationDate = new DateTime(2023, 10, 1),
                UoM = "UOM1",
                ProductionQty = 100,
                ProducedQty = 50,
                ResQty = 50,
                Storage = "Storage1",
                Wc = "WC1",
                BarCode = "BarCode1",
                PrelQty = 10,
                Imported = false,
                UserImp = "User1",
                DataImp = new DateTime(2023, 10, 1),
            },
            new A3AppPrelMat
            {
                PrelMatId = 2,
                WorkerId = 2,
                SavedDate = DateTime.Now.AddDays(-1),
                Job = "Job2",
                RtgStep = 2,
                Alternate = "Alt2",
                AltRtgStep = 3,
                Operation = "Op2",
                OperDesc = "Operation 2",
                Bom = "BOM2",
                Variant = "Variant2",
                ItemDesc = "Item Description 2",
                Moid = 2,
                Mono = "Mono2",
                CreationDate = DateTime.Now.AddDays(-1),
                UoM = "UOM2",
                ProductionQty = 200,
                ProducedQty = 100,
                ResQty = 100,
                Storage = "Storage2",
                Wc = "WC2",
                BarCode = "BarCode2",
                PrelQty = 20,
                Imported = true
            }
        };

        private readonly ViewPrelMatRequestFilter _viewPrelMatRequestFilter = new ViewPrelMatRequestFilter
        {
            WorkerId = 1,
            FromDateTime = new DateTime(2023, 1, 1),
            ToDateTime = new DateTime(2023, 12, 3),
            Job = "Job1",
            Operation = "Op1",
            Mono = "Mono1",
            Component = "Component1",
            BarCode = "BarCode1"
        };

        private readonly ViewPrelMatPutFilter _viewPrelMatPutFilter = new ViewPrelMatPutFilter
        {
            PrelMatId = 1,
            PrelQty = 15
        };

        public PrelMatRepositoryTest()
        {
            _mockSet = new Mock<DbSet<A3AppPrelMat>>();
            _mockContext = new Mock<ApplicationDbContext>();
            _prelMatRepository = new PrelMatRepository(_mockContext.Object);
        }

        private void SetupMockDbSet(IQueryable<A3AppPrelMat> data)
        {
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);
        }

        #region GetAppPrelMat Tests

        [Fact]
        public void GetAppPrelMat_ShouldReturnPrelMatList_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            // Act
            var result = _prelMatRepository.GetAppPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_prelMats.Count, result.Count());
            // Should be ordered by SavedDate descending
            Assert.Equal(2, result.First().PrelMatId); // Newer date first
        }

        [Fact]
        public void GetAppPrelMat_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(Enumerable.Empty<A3AppPrelMat>().AsQueryable());

            // Act
            var result = _prelMatRepository.GetAppPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region PostPrelMatList Tests

        [Fact]
        public void PostPrelMatList_ShouldAddNewPrelMat_WhenDataExists()
        {
            // Arrange
            var prelMatFilter = new PrelMatFilter
            {
                WorkerId = 1,
                Job = "Job1",
                RtgStep = 1,
                Alternate = "Alt1",
                AltRtgStep = 2,
                Operation = "Op1",
                OperDesc = "Operation 1",
                Bom = "BOM1",
                Variant = "Variant1",
                Position = 1,
                Component = "Component1",
                ItemDesc = "Item Description 1",
                Moid = 1,
                Mono = "Mono1",
                CreationDate = new DateTime(2023, 10, 1),
                UoM = "UOM1",
                ProductionQty = 100,
                ProducedQty = 50,
                ResQty = 50,
                Storage = "Storage1",
                Wc = "WC1",
                BarCode = "BarCode1",
                PrelQty = 10
            };

            SetupMockDbSet(_prelMats.AsQueryable());

            // Act
            var result = _prelMatRepository.PostPrelMatList(new List<PrelMatFilter> { prelMatFilter });

            // Assert
            _mockSet.Verify(m => m.AddRange(It.IsAny<IEnumerable<A3AppPrelMat>>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(prelMatFilter.PrelQty, result.First().PrelQty);
            Assert.Equal(prelMatFilter.Job, result.First().Job);
        }

        [Fact]
        public void PostPrelMatList_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                _prelMatRepository.PostPrelMatList(new List<PrelMatFilter>()));

            Assert.Equal(nameof(PrelMatRepository), exception.ClassName);
            Assert.Equal(nameof(_prelMatRepository.PostPrelMatList), exception.MethodName);
        }

        [Fact]
        public void PostPrelMatList_ShouldSetCurrentDateTime_ForSavedDate()
        {
            // Arrange
            var prelMatFilter = new PrelMatFilter
            {
                WorkerId = 1,
                Job = "Job1",
                PrelQty = 10
            };

            SetupMockDbSet(_prelMats.AsQueryable());
            var beforeTime = DateTime.Now.AddSeconds(-1);

            // Act
            var result = _prelMatRepository.PostPrelMatList(new List<PrelMatFilter> { prelMatFilter });

            // Assert
            var afterTime = DateTime.Now.AddSeconds(1);
            Assert.True(result.First().SavedDate >= beforeTime && result.First().SavedDate <= afterTime);
        }

        #endregion

        #region GetViewPrelMat Tests

        [Fact]
        public void GetViewPrelMat_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            // Act
            var result = _prelMatRepository.GetViewPrelMat(_viewPrelMatRequestFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Job1", result.First().Job);
        }

        [Fact]
        public void GetViewPrelMat_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            var emptyFilter = new ViewPrelMatRequestFilter
            {
                Job = "NonExistentJob"
            };

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                _prelMatRepository.GetViewPrelMat(emptyFilter));

            Assert.Equal(nameof(PrelMatRepository), exception.ClassName);
            Assert.Equal(nameof(_prelMatRepository.GetViewPrelMat), exception.MethodName);
        }

        [Fact]
        public void GetViewPrelMat_ShouldOrderByDataImp_WhenImportedIsTrue()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Imported = true;
            testData[0].DataImp = DateTime.Now.AddDays(-2);
            testData[1].Imported = true;
            testData[1].DataImp = DateTime.Now.AddDays(-1);

            SetupMockDbSet(testData.AsQueryable());

            var filter = new ViewPrelMatRequestFilter { Imported = true };

            // Act
            var result = _prelMatRepository.GetViewPrelMat(filter);

            // Assert
            var resultList = result.ToList();
            Assert.True(resultList[0].DataImp >= resultList[1].DataImp);
        }

        [Fact]
        public void GetViewPrelMat_ShouldOrderBySavedDate_WhenImportedIsFalseOrNull()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            var filter = new ViewPrelMatRequestFilter { Imported = false };

            // Act
            var result = _prelMatRepository.GetViewPrelMat(filter);

            // Assert
            var resultList = result.ToList();
            Assert.True(resultList.Count > 0);
            // Should be ordered by SavedDate descending
        }

        #endregion

        #region PutViewPrelMat Tests

        [Fact]
        public void PutViewPrelMat_ShouldUpdatePrelMat_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            // Act
            var result = _prelMatRepository.PutViewPrelMat(_viewPrelMatPutFilter);

            // Assert
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(_viewPrelMatPutFilter.PrelQty, result.PrelQty);
            Assert.Equal(1, result.PrelMatId);
        }

        [Fact]
        public void PutViewPrelMat_ShouldThrowArgumentNullException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(Enumerable.Empty<A3AppPrelMat>().AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _prelMatRepository.PutViewPrelMat(new ViewPrelMatPutFilter { PrelMatId = 999 }));

            Assert.Contains("999", exception.Message);
            Assert.Contains("PutViewPrelMat", exception.Message);
        }

        #endregion

        #region DeletePrelMatId Tests

        [Fact]
        public void DeletePrelMatId_ShouldDeletePrelMat_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] ids) =>
                _prelMats.FirstOrDefault(x => x.PrelMatId == (int)ids[0]));

            var deleteFilter = new ViewPrelMatDeleteFilter { PrelMatId = 1 };

            // Act
            var result = _prelMatRepository.DeletePrelMatId(deleteFilter);

            // Assert
            _mockSet.Verify(m => m.Remove(It.IsAny<A3AppPrelMat>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(1, result.PrelMatId);
        }

        [Fact]
        public void DeletePrelMatId_ShouldThrowArgumentNullException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(Enumerable.Empty<A3AppPrelMat>().AsQueryable());

            var deleteFilter = new ViewPrelMatDeleteFilter { PrelMatId = 999 };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _prelMatRepository.DeletePrelMatId(deleteFilter));

            Assert.Contains("999", exception.Message);
            Assert.Contains("DeletePrelMatId", exception.Message);
        }

        #endregion

        #region UpdatePrelMatImported Tests

        [Fact]
        public void UpdatePrelMatImported_ShouldUpdateNotImportedItems_WhenDataExists()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Imported = false;
            testData[1].Imported = false;
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new WorkerIdSyncFilter { WorkerId = "1" };

            // Act
            var result = _prelMatRepository.UpdatePrelMatImported(filter);

            // Assert
            _mockContext.Verify(m => m.SaveChanges(), Times.AtLeastOnce);
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdatePrelMatImported_ShouldThrowArgumentNullException_WhenFilterIsNull()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _prelMatRepository.UpdatePrelMatImported(null));

            Assert.Equal("filter", exception.ParamName);
        }

        [Fact]
        public void UpdatePrelMatImported_ShouldThrowArgumentNullException_WhenWorkerIdIsNull()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());
            var filter = new WorkerIdSyncFilter { WorkerId = null };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _prelMatRepository.UpdatePrelMatImported(filter));

            Assert.Equal("filter", exception.ParamName);
        }

        [Fact]
        public void UpdatePrelMatImported_ShouldThrowEmptyListException_WhenNoNotImportedItems()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData.ForEach(x => x.Imported = true);
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new WorkerIdSyncFilter { WorkerId = "1" };

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                _prelMatRepository.UpdatePrelMatImported(filter));

            Assert.Equal(nameof(PrelMatRepository), exception.ClassName);
            Assert.Equal(nameof(_prelMatRepository.UpdatePrelMatImported), exception.MethodName);
        }

        [Fact]
        public void UpdatePrelMatImported_ShouldSetImportedFieldsCorrectly()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Imported = false;
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new WorkerIdSyncFilter { WorkerId = "123" };
            var beforeTime = DateTime.Now.AddSeconds(-1);

            // Act
            _prelMatRepository.UpdatePrelMatImported(filter);

            // Assert
            var afterTime = DateTime.Now.AddSeconds(1);
            var updatedItem = testData[0];
            
            Assert.True(updatedItem.Imported);
            Assert.Equal("123", updatedItem.UserImp);
            Assert.True(updatedItem.DataImp >= beforeTime && updatedItem.DataImp <= afterTime);
        }

        #endregion

        #region GetPrelMatWithComponent Tests

        [Fact]
        public void GetPrelMatWithComponent_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Component = "TestComponent";
            testData[0].Imported = false;
            testData[1].Component = "TestComponent";
            testData[1].Imported = true; // This should be filtered out
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new ComponentFilter { Component = "TestComponent" };

            // Act
            var result = _prelMatRepository.GetPrelMatWithComponent(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TestComponent", result.First().Component);
            Assert.False(result.First().Imported);
        }

        [Fact]
        public void GetPrelMatWithComponent_ShouldReturnEmptyList_WhenNoMatchingData()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            var filter = new ComponentFilter { Component = "NonExistentComponent" };

            // Act
            var result = _prelMatRepository.GetPrelMatWithComponent(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetNotImportedPrelMat Tests

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnNotImportedItems_WhenDataExists()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Imported = false;
            testData[1].Imported = true;
            
            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _prelMatRepository.GetNotImportedPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.False(result.First().Imported);
        }

        [Fact]
        public void GetNotImportedPrelMat_ShouldReturnEmptyList_WhenAllItemsImported()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData.ForEach(x => x.Imported = true);
            
            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _prelMatRepository.GetNotImportedPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetNotImportedPrelMat_ShouldOrderBySavedDateDescending()
        {
            // Arrange
            var testData = new List<A3AppPrelMat>
            {
                new A3AppPrelMat { PrelMatId = 1, Imported = false, SavedDate = DateTime.Now.AddDays(-2) },
                new A3AppPrelMat { PrelMatId = 2, Imported = false, SavedDate = DateTime.Now.AddDays(-1) },
                new A3AppPrelMat { PrelMatId = 3, Imported = false, SavedDate = DateTime.Now }
            };
            
            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _prelMatRepository.GetNotImportedPrelMat().ToList();

            // Assert
            Assert.Equal(3, result[0].PrelMatId); // Most recent first
            Assert.Equal(2, result[1].PrelMatId);
            Assert.Equal(1, result[2].PrelMatId);
        }

        #endregion

        #region GetNotImportedAppPrelMatByFilter Tests

        [Fact]
        public void GetNotImportedAppPrelMatByFilter_ShouldReturnFilteredNotImportedItems()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Imported = false;
            testData[1].Imported = true;
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new ViewPrelMatRequestFilter
            {
                Job = "Job1"
            };

            // Act
            var result = _prelMatRepository.GetNotImportedAppPrelMatByFilter(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Job1", result.First().Job);
            Assert.False(result.First().Imported);
        }

        [Fact]
        public void GetNotImportedAppPrelMatByFilter_ShouldThrowEmptyListException_WhenNoResults()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData.ForEach(x => x.Imported = true);
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new ViewPrelMatRequestFilter
            {
                Job = "Job1"
            };

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                _prelMatRepository.GetNotImportedAppPrelMatByFilter(filter));

            Assert.Equal(nameof(PrelMatRepository), exception.ClassName);
            Assert.Equal(nameof(_prelMatRepository.GetNotImportedAppPrelMatByFilter), exception.MethodName);
        }

        #endregion

        #region UpdateImportedById Tests

        [Fact]
        public void UpdateImportedById_ShouldUpdateSpecificItem_WhenDataExists()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Imported = false;
            testData[1].Imported = false;
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new UpdateImportedIdFilter { Id = 1, WorkerId = "123" };

            // Act
            var result = _prelMatRepository.UpdateImportedById(filter);

            // Assert
            _mockContext.Verify(m => m.SaveChanges(), Times.AtLeastOnce);
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateImportedById_ShouldThrowArgumentNullException_WhenFilterIsNull()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _prelMatRepository.UpdateImportedById(null));

            Assert.Equal("filter", exception.ParamName);
        }

        [Fact]
        public void UpdateImportedById_ShouldThrowArgumentNullException_WhenWorkerIdIsNull()
        {
            // Arrange
            SetupMockDbSet(_prelMats.AsQueryable());
            var filter = new UpdateImportedIdFilter { Id = 1, WorkerId = null };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _prelMatRepository.UpdateImportedById(filter));

            Assert.Equal("filter", exception.ParamName);
        }

        [Fact]
        public void UpdateImportedById_ShouldThrowEmptyListException_WhenNoMatchingItem()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData.ForEach(x => x.Imported = true);
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new UpdateImportedIdFilter { Id = 1, WorkerId = "123" };

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                _prelMatRepository.UpdateImportedById(filter));

            Assert.Equal(nameof(PrelMatRepository), exception.ClassName);
            Assert.Equal(nameof(_prelMatRepository.UpdateImportedById), exception.MethodName);
        }

        [Fact]
        public void UpdateImportedById_ShouldSetImportedFieldsCorrectly()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Imported = false;
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new UpdateImportedIdFilter { Id = 1, WorkerId = "456" };
            var beforeTime = DateTime.Now.AddSeconds(-1);

            // Act
            _prelMatRepository.UpdateImportedById(filter);

            // Assert
            var afterTime = DateTime.Now.AddSeconds(1);
            var updatedItem = testData[0];
            
            Assert.True(updatedItem.Imported);
            Assert.Equal("456", updatedItem.UserImp);
            Assert.True(updatedItem.DataImp >= beforeTime && updatedItem.DataImp <= afterTime);
        }

        [Fact]
        public void UpdateImportedById_ShouldOnlyUpdateSpecificId()
        {
            // Arrange
            var testData = _prelMats.ToList();
            testData[0].Imported = false; // ID = 1
            testData[1].Imported = false; // ID = 2
            
            SetupMockDbSet(testData.AsQueryable());

            var filter = new UpdateImportedIdFilter { Id = 1, WorkerId = "789" };

            // Act
            _prelMatRepository.UpdateImportedById(filter);

            // Assert
            Assert.True(testData[0].Imported); // Should be updated
            Assert.False(testData[1].Imported); // Should remain unchanged
        }

        #endregion
    }
}