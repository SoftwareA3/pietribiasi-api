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
    public class RegOreRepositoryTest
    {
        private readonly RegOreRepository _regOreRepository;
        private readonly Mock<DbSet<A3AppRegOre>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;

        private readonly List<A3AppRegOre> _regOres = new List<A3AppRegOre>
        {
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
            WorkingTime = 3600
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

            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Provider).Returns(_regOres.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Expression).Returns(_regOres.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.ElementType).Returns(_regOres.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.GetEnumerator()).Returns(_regOres.GetEnumerator());
            _mockContext.Setup(c => c.A3AppRegOres).Returns(_mockSet.Object);
        }

        [Fact]
        public void GetAppRegOre_ReturnsRegOreList_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Provider).Returns(_regOres.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Expression).Returns(_regOres.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.ElementType).Returns(_regOres.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.GetEnumerator()).Returns(_regOres.GetEnumerator());

            _mockContext.Setup(c => c.A3AppRegOres).Returns(_mockSet.Object);

            // Act
            var result = _regOreRepository.GetAppRegOre();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(_regOres.First().Job, result.First().Job);
            Assert.Equal(_regOres.Last().Job, result.Last().Job);
        }

        [Fact]
        public void GetAppRegOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Provider).Returns(new List<A3AppRegOre>().AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Expression).Returns(new List<A3AppRegOre>().AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.ElementType).Returns(new List<A3AppRegOre>().AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.GetEnumerator()).Returns(new List<A3AppRegOre>().GetEnumerator());

            _mockContext.Setup(c => c.A3AppRegOres).Returns(_mockSet.Object);

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
            _mockSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<A3AppRegOre>>()));
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _regOreRepository.PostRegOreList(filterList);

            // Assert
            Assert.Single(result);
            Assert.Equal(_regOreFilter.Job, result.First().Job);
        }

        [Fact]
        public void PostRegOreList_DoesNotAddRegOre_WhenNoDataProvided()
        {
            // Arrange
            var filterList = new List<RegOreFilter>();
            _mockSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<A3AppRegOre>>()));
            _mockContext.Setup(c => c.SaveChanges()).Returns(0);

            // Act
            var result = _regOreRepository.PostRegOreList(filterList);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetAppViewOre_ReturnsFilteredResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Provider).Returns(_regOres.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Expression).Returns(_regOres.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.ElementType).Returns(_regOres.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.GetEnumerator()).Returns(_regOres.GetEnumerator());

            _mockContext.Setup(c => c.A3AppRegOres).Returns(_mockSet.Object);

            // Act
            var result = _regOreRepository.GetAppViewOre(_viewOreRequestFilter);

            // Assert
            Assert.Single(result);
            Assert.Equal(_viewOreRequestFilter.Job, result.First().Job);
        }

        [Fact]
        public void GetAppViewOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Provider).Returns(new List<A3AppRegOre>().AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Expression).Returns(new List<A3AppRegOre>().AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.ElementType).Returns(new List<A3AppRegOre>().AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.GetEnumerator()).Returns(new List<A3AppRegOre>().GetEnumerator());

            _mockContext.Setup(c => c.A3AppRegOres).Returns(_mockSet.Object);

            // Act
            var result = _regOreRepository.GetAppViewOre(new ViewOreRequestFilter());

            // Assert
            Assert.Empty(result);
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

            var regOre = _regOres.First(m => m.RegOreId == filter.RegOreId);
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(regOre);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _regOreRepository.PutAppViewOre(filter);

            // Assert
            Assert.Equal(filter.WorkingTime, result.WorkingTime);
        }

        [Fact]
        public void PutAppViewOre_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var filter = new ViewOrePutFilter
            {
                RegOreId = 999,
                WorkingTime = 7200
            };

            _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((A3AppRegOre)null);
            _mockContext.Setup(c => c.SaveChanges()).Returns(0);

            // Act
            var result = _regOreRepository.PutAppViewOre(filter);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeleteRegOre_ShouldReturnRegOre_WhenRegOreExists()
        {
            // Arrange
            var deleteRegOreId = new ViewOreDeleteRequestFilter
            {
                RegOreId = 1
            };
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Provider).Returns(_regOres.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Expression).Returns(_regOres.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.ElementType).Returns(_regOres.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.GetEnumerator()).Returns(_regOres.GetEnumerator());
            _mockContext.Setup(c => c.A3AppRegOres).Returns(_mockSet.Object);
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(_regOres.FirstOrDefault(m => m.RegOreId == deleteRegOreId.RegOreId));

            // Act
            var result = _regOreRepository.DeleteRegOreId(deleteRegOreId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deleteRegOreId.RegOreId, result.RegOreId);
        }

        [Fact]
        public void DeleteRegOre_ShouldReturnNull_WhenRegOreDoesNotExist()
        {
            // Arrange
            var deleteRegOreId = new ViewOreDeleteRequestFilter
            {
                RegOreId = 999
            };
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Provider).Returns(_regOres.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.Expression).Returns(_regOres.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.ElementType).Returns(_regOres.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppRegOre>>().Setup(m => m.GetEnumerator()).Returns(_regOres.GetEnumerator());
            _mockContext.Setup(c => c.A3AppRegOres).Returns(_mockSet.Object);
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(_regOres.FirstOrDefault(m => m.RegOreId == deleteRegOreId.RegOreId));

            // Act
            var result = _regOreRepository.DeleteRegOreId(deleteRegOreId);

            // Assert
            Assert.Null(result);
        }
    }
}