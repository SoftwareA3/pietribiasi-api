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
    public class RegOreRepositoryTest
    {
        private readonly RegOreRepository _regOreRepository;
        private readonly Mock<DbSet<A3AppRegOre>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;

        private readonly List<A3AppRegOre> _regOres = new List<A3AppRegOre>
        {
            new A3AppRegOre
            {
                RegOreId = 2,
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
                Uom = "UOM2",
                ProductionQty = 200,
                ProducedQty = 100,
                ResQty = 100,
                Storage = "Storage2",
                Wc = "WC2",
                WorkingTime = 7200,
                Imported = true,
                UserImp = "User2",
                DataImp = DateTime.Now.AddDays(-1)
            },
            new A3AppRegOre
            {
                RegOreId = 1,
                WorkerId = 1,
                SavedDate = new DateTime(2023, 10, 1),
                Job = "Job1",
                RtgStep = 1,
                Alternate = "Alt1",
                AltRtgStep = 2,
                Operation = "Op1",
                OperDesc = "Operation 1",
                Bom = "BOM1",
                Variant = "Variant1",
                ItemDesc = "Item Description 1",
                Moid = 1,
                Mono = "Mono1",
                CreationDate = new DateTime(2023, 10, 1),
                Uom = "UOM1",
                ProductionQty = 100,
                ProducedQty = 50,
                ResQty = 50,
                Storage = "Storage1",
                Wc = "WC1",
                WorkingTime = 3600,
                Imported = false,
                UserImp = "User1",
                DataImp = new DateTime(2023, 10, 1)
            },
            new A3AppRegOre
            {
                RegOreId = 3,
                WorkerId = 3,
                SavedDate = DateTime.Now.AddDays(-2),
                Job = "Job3",
                RtgStep = 3,
                Alternate = "Alt3",
                AltRtgStep = 4,
                Operation = "Op3",
                OperDesc = "Operation 3",
                Bom = "BOM3",
                Variant = "Variant3",
                ItemDesc = "Item Description 3",
                Moid = 3,
                Mono = "Mono3",
                CreationDate = DateTime.Now.AddDays(-2),
                Uom = "UOM3",
                ProductionQty = 300,
                ProducedQty = 150,
                ResQty = 150,
                Storage = "Storage3",
                Wc = "WC3",
                WorkingTime = 5400,
                Imported = false,
                UserImp = null,
                DataImp = null
            }
        };

        private readonly JobFilter _jobFilter = new JobFilter
        {
            Job = "Job1"
        }; 

        private readonly MonoFilter _monoFilter = new MonoFilter
        {
            Job = "Job1",
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 10, 1)
        };

        private readonly OperationFilter _operationFilter = new OperationFilter
        {
            Job = "Job1",
            Operation = "Op1",
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 10, 1)
        };

        private readonly RegOreFilter _regOreFilter = new RegOreFilter
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
            ItemDesc = "Item Description 1",
            Moid = 1,
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 10, 1),
            Uom = "UOM1",
            ProductionQty = 100,
            ProducedQty = 50,
            ResQty = 50,
            Storage = "Storage1",
            Wc = "WC1",
            WorkingTime = 3600,
            Imported = false,
            UserImp = "User1",
            DataImp = new DateTime(2023, 10, 1)
        };

        private readonly ViewOreRequestFilter _viewOreRequestFilter = new ViewOreRequestFilter
        {
            WorkerId = 1,
            FromDateTime = new DateTime(2023, 1, 1),
            ToDateTime = new DateTime(2023, 12, 31),
            Job = "Job1",
            Operation = "Op1",
            Mono = "Mono1"
        };

        public RegOreRepositoryTest()
        {
            _mockSet = new Mock<DbSet<A3AppRegOre>>();
            _mockContext = new Mock<ApplicationDbContext>();
            _regOreRepository = new RegOreRepository(_mockContext.Object);
        }

        private void SetupMockDbSet(IQueryable<A3AppRegOre> data)
        {
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockContext.Setup(c => c.A3AppRegOres).Returns(_mockSet.Object);
        }

        [Fact]
        public void GetAppRegOre_ReturnsRegOreList_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_regOres.AsQueryable());

            // Act
            var result = _regOreRepository.GetAppRegOre();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal(_regOres.First().Job, result.First().Job);
        }

        [Fact]
        public void GetAppRegOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(Enumerable.Empty<A3AppRegOre>().AsQueryable());

            // Act
            var result = _regOreRepository.GetAppRegOre();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void PostRegOreList_AddsRegOre_WhenValidDataProvided()
        {
            // Arrange
            var filterList = new List<RegOreFilter> { _regOreFilter };
            SetupMockDbSet(_regOres.AsQueryable());
            _mockSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<A3AppRegOre>>()));
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _regOreRepository.PostRegOreList(filterList);

            // Assert
            Assert.Single(result);
            Assert.Equal(_regOreFilter.Job, result.First().Job);
        }

        [Fact]
        public void PostRegOreList_ThrowsEmptyListException_WhenNoDataProvided()
        {
            // Arrange
            SetupMockDbSet(_regOres.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() =>
                _regOreRepository.PostRegOreList(new List<RegOreFilter> { }));

            Assert.Equal(nameof(RegOreRepository), exception.ClassName);
            Assert.Equal(nameof(_regOreRepository.PostRegOreList), exception.MethodName);
        }

        [Fact]
        public void GetAppViewOre_ReturnsFilteredResults_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_regOres.AsQueryable());

            // Act
            var result = _regOreRepository.GetAppViewOre(_viewOreRequestFilter);

            // Assert
            Assert.Single(result);
            Assert.Equal(_viewOreRequestFilter.Job, result.First().Job);
        }

        [Fact]
        public void GetAppViewOre_ThrowsEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(Enumerable.Empty<A3AppRegOre>().AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => _regOreRepository.GetAppViewOre(new ViewOreRequestFilter()));
            Assert.Equal(nameof(RegOreRepository), exception.ClassName);
            Assert.Equal(nameof(_regOreRepository.GetAppViewOre), exception.MethodName);
        }

        [Fact]
        public void GetAppViewOre_OrdersByDataImp_WhenImportedIsTrue()
        {
            // Arrange
            var testData = _regOres.ToList();
            testData[0].Imported = true;
            testData[0].DataImp = DateTime.Now.AddDays(-2);
            testData[1].Imported = true;
            testData[1].DataImp = DateTime.Now.AddDays(-1);
            testData[2].Imported = true;
            testData[2].DataImp = DateTime.Now;

            SetupMockDbSet(testData.AsQueryable());

            var filter = new ViewOreRequestFilter { Imported = true };
            
            // Act
            var result = _regOreRepository.GetAppViewOre(filter);

            // Assert
            Assert.NotEmpty(result);
            var orderedResult = result.ToList();
            for (int i = 0; i < orderedResult.Count - 1; i++)
            {
                Assert.True(orderedResult[i].DataImp >= orderedResult[i + 1].DataImp);
            }
        }

        [Fact]
        public void GetAppViewOre_OrdersBySavedDate_WhenImportedIsFalseOrNull()
        {
            // Arrange
            var testData = _regOres.ToList();
            testData.ForEach(x => x.Imported = false);

            SetupMockDbSet(testData.AsQueryable());

            var filter = new ViewOreRequestFilter { Imported = false };
            
            // Act
            var result = _regOreRepository.GetAppViewOre(filter);

            // Assert
            Assert.NotEmpty(result);
            var orderedResult = result.ToList();
            for (int i = 0; i < orderedResult.Count - 1; i++)
            {
                Assert.True(orderedResult[i].SavedDate >= orderedResult[i + 1].SavedDate);
            }
        }

        [Fact]
        public void PutAppViewOre_UpdatesRegOre_WhenValidDataProvided()
        {
            // Arrange
            var filter = new ViewOrePutFilter
            {
                RegOreId = 1,
                WorkingTime = 7200
            };

            SetupMockDbSet(_regOres.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _regOreRepository.PutAppViewOre(filter);

            // Assert
            Assert.Equal(filter.WorkingTime, result.WorkingTime);
            Assert.NotNull(result);
        }

        [Fact]
        public void PutAppViewOre_ThrowsArgumentNullException_WhenRegOreNotFound()
        {
            // Arrange
            var filter = new ViewOrePutFilter
            {
                RegOreId = 999,
                WorkingTime = 7200
            };

            SetupMockDbSet(Enumerable.Empty<A3AppRegOre>().AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _regOreRepository.PutAppViewOre(filter));
            Assert.Contains("999", exception.Message);
            Assert.Contains("PutAppViewOre", exception.Message);
        }

        [Fact]
        public void DeleteRegOreId_ReturnsRegOre_WhenRegOreExists()
        {
            // Arrange
            var deleteFilter = new ViewOreDeleteRequestFilter
            {
                RegOreId = 1
            };
            
            SetupMockDbSet(_regOres.AsQueryable());
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] ids) =>
                _regOres.FirstOrDefault(x => x.RegOreId == (int)ids[0]));
            _mockSet.Setup(m => m.Remove(It.IsAny<A3AppRegOre>()));
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _regOreRepository.DeleteRegOreId(deleteFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deleteFilter.RegOreId, result.RegOreId);
            _mockSet.Verify(m => m.Remove(It.IsAny<A3AppRegOre>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteRegOreId_ThrowsArgumentNullException_WhenRegOreDoesNotExist()
        {
            // Arrange
            var deleteFilter = new ViewOreDeleteRequestFilter
            {
                RegOreId = 999
            };
            
            SetupMockDbSet(Enumerable.Empty<A3AppRegOre>().AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _regOreRepository.DeleteRegOreId(deleteFilter));
            Assert.Contains("999", exception.Message);
            Assert.Contains("DeleteRegOreId", exception.Message);
        }

        [Fact]
        public void GetNotImportedRegOre_ReturnsNotImportedRegOres_WhenDataExists()
        {
            // Arrange
            var testData = _regOres.ToList();
            testData[0].Imported = false;
            testData[1].Imported = false;
            testData[2].Imported = false;

            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _regOreRepository.GetNotImportedRegOre();

            // Assert
            Assert.Equal(3, result.Count()); // Tutti e 3 hanno Imported = false
            Assert.All(result, r => Assert.False(r.Imported));
        }

        [Fact]
        public void GetNotImportedRegOre_ReturnsEmptyList_WhenNoNotImportedDataExists()
        {
            // Arrange
            var testData = _regOres.ToList();
            testData.ForEach(x => x.Imported = true);

            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _regOreRepository.GetNotImportedRegOre();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UpdateRegOreImported_UpdatesNotImportedRegOres_WhenValidFilterProvided()
        {
            // Arrange
            var filter = new WorkerIdSyncFilter { WorkerId = "123" };
            var testData = _regOres.ToList();
            testData.ForEach(x => x.Imported = false);
            
            SetupMockDbSet(testData.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(testData.Count);

            // Act
            var result = _regOreRepository.UpdateRegOreImported(filter);

            // Assert
            Assert.NotNull(result);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateRegOreImported_ThrowsArgumentNullException_WhenFilterIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _regOreRepository.UpdateRegOreImported(null));
            Assert.Equal("filter", exception.ParamName);
        }

        [Fact]
        public void UpdateRegOreImported_ThrowsArgumentNullException_WhenWorkerIdIsNull()
        {
            // Arrange
            var filter = new WorkerIdSyncFilter { WorkerId = null };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _regOreRepository.UpdateRegOreImported(filter));
            Assert.Equal("filter", exception.ParamName);
        }

        [Fact]
        public void UpdateRegOreImported_ThrowsEmptyListException_WhenNoNotImportedDataExists()
        {
            // Arrange
            var filter = new WorkerIdSyncFilter { WorkerId = "123" };
            var testData = _regOres.ToList();
            testData.ForEach(x => x.Imported = true);

            SetupMockDbSet(testData.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => _regOreRepository.UpdateRegOreImported(filter));
            Assert.Equal(nameof(RegOreRepository), exception.ClassName);
            Assert.Equal(nameof(_regOreRepository.UpdateRegOreImported), exception.MethodName);
        }

        [Fact]
        public void UpdateRegOreImported_SetsImportedFieldsCorrectly()
        {
            // Arrange
            var filter = new WorkerIdSyncFilter { WorkerId = "456" };
            var testData = _regOres.ToList();
            testData.ForEach(x => x.Imported = false);
            
            SetupMockDbSet(testData.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(testData.Count);

            var beforeTime = DateTime.Now.AddSeconds(-1);

            // Act
            _regOreRepository.UpdateRegOreImported(filter);

            // Assert
            var afterTime = DateTime.Now.AddSeconds(1);
            
            Assert.All(testData, item => 
            {
                Assert.True(item.Imported);
                Assert.Equal("456", item.UserImp);
                Assert.True(item.DataImp >= beforeTime && item.DataImp <= afterTime);
            });
        }

        [Fact]
        public void GetNotImportedAppRegOreByFilter_ReturnsFilteredNotImportedRegOres_WhenDataExists()
        {
            // Arrange
            var filter = new ViewOreRequestFilter
            {
                WorkerId = 1,
                Job = "Job1"
            };

            var testData = _regOres.ToList();
            testData[1].Imported = false; // RegOreId 1 che corrisponde ai filtri

            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _regOreRepository.GetNotImportedAppRegOreByFilter(filter);

            // Assert
            Assert.Single(result);
            Assert.All(result, r => Assert.False(r.Imported));
            Assert.All(result, r => Assert.Equal(filter.WorkerId, r.WorkerId));
            Assert.All(result, r => Assert.Equal(filter.Job, r.Job));
        }

        [Fact]
        public void UpdateImportedById_UpdatesSpecificRegOre_WhenValidFilterProvided()
        {
            // Arrange
            var filter = new UpdateImportedIdFilter 
            { 
                Id = 1, 
                WorkerId = "123" 
            };
            
            var testData = _regOres.ToList();
            testData[1].Imported = false; // RegOreId 1

            SetupMockDbSet(testData.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _regOreRepository.UpdateImportedById(filter);

            // Assert
            Assert.NotNull(result);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateImportedById_ThrowsArgumentNullException_WhenFilterIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _regOreRepository.UpdateImportedById(null));
            Assert.Equal("filter", exception.ParamName);
        }

        [Fact]
        public void UpdateImportedById_ThrowsArgumentNullException_WhenWorkerIdIsNull()
        {
            // Arrange
            var filter = new UpdateImportedIdFilter 
            { 
                Id = 1, 
                WorkerId = null 
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _regOreRepository.UpdateImportedById(filter));
            Assert.Equal("filter", exception.ParamName);
        }

        [Fact]
        public void UpdateImportedById_ThrowsEmptyListException_WhenNoMatchingNotImportedRegOreExists()
        {
            // Arrange
            var filter = new UpdateImportedIdFilter 
            { 
                Id = 999,
                WorkerId = "123" 
            };

            SetupMockDbSet(_regOres.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => _regOreRepository.UpdateImportedById(filter));
            Assert.Equal(nameof(RegOreRepository), exception.ClassName);
            Assert.Equal(nameof(_regOreRepository.UpdateImportedById), exception.MethodName);
        }

        [Fact]
        public void UpdateImportedById_SetsImportedFieldsCorrectly()
        {
            // Arrange
            var filter = new UpdateImportedIdFilter 
            { 
                Id = 1, 
                WorkerId = "789" 
            };
            
            var testData = _regOres.ToList();
            testData[1].Imported = false; // RegOreId 1
            testData[2].Imported = false; // RegOreId 3

            SetupMockDbSet(testData.AsQueryable());
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            var beforeTime = DateTime.Now.AddSeconds(-1);

            // Act
            _regOreRepository.UpdateImportedById(filter);

            // Assert
            var afterTime = DateTime.Now.AddSeconds(1);
            var updatedItem = testData[1]; // RegOreId 1
            
            Assert.True(updatedItem.Imported);
            Assert.Equal("789", updatedItem.UserImp);
            Assert.True(updatedItem.DataImp >= beforeTime && updatedItem.DataImp <= afterTime);
            
            // Verifica che l'altro elemento non sia stato modificato
            Assert.False(testData[2].Imported); // RegOreId 3 should remain unchanged
        }
    }
}