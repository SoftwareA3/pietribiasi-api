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
    public class PrelMatRepositoryTest
    {
        private readonly PrelMatRepository _prelMatRepository;
        private readonly Mock<DbSet<A3AppPrelMat>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;

        private readonly List<A3AppPrelMat> _prelMats = new List<A3AppPrelMat>
        {
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
            },
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
            }
        };

        private readonly List<PrelMatFilter> _prelMatFilterList = new List<PrelMatFilter>
        {
            new PrelMatFilter
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
                UoM = "UOM1",
                ProductionQty = 100,
                ProducedQty = 50,
                ResQty = 50,
                Storage = "Storage1",
                Wc = "WC1",
                BarCode = "BarCode1",
                PrelQty = 10
            },
            new PrelMatFilter
            {
                WorkerId = 2,
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
                PrelQty = 20
            }
        };

        private readonly PrelMatFilter _prelMatFilter = new PrelMatFilter
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
            Position = 1,
            Component = "Component1"
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

        [Fact]
        public void GetAppPrelMat_ShouldReturnPrelMatList_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(_prelMats.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(_prelMats.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(_prelMats.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(_prelMats.GetEnumerator());
            _mockSet.Setup(m => m.Remove(It.IsAny<A3AppPrelMat>())).Callback<A3AppPrelMat>(entity => _prelMats.Remove(entity));

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var result = _prelMatRepository.GetAppPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_prelMats.Count, result.Count());
            Assert.Equal(_prelMats, result);
        }

        [Fact]
        public void GetAppPrelMat_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<A3AppPrelMat>().GetEnumerator());

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var result = _prelMatRepository.GetAppPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void PostPrelMatList_ShouldAddNewPrelMat_WhereDataExists()
        {
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

            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(_prelMats.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(_prelMats.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(_prelMats.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(_prelMats.GetEnumerator());
            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var result = _prelMatRepository.PostPrelMatList(new List<PrelMatFilter> { prelMatFilter });
            _mockSet.Verify(m => m.AddRange(It.IsAny<IEnumerable<A3AppPrelMat>>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(prelMatFilter.PrelQty, result.First().PrelQty);
            Assert.Equal(prelMatFilter.Job, result.First().Job);
            Assert.Equal(prelMatFilter.RtgStep, result.First().RtgStep);
            Assert.Equal(prelMatFilter.Alternate, result.First().Alternate);
            Assert.Equal(prelMatFilter.AltRtgStep, result.First().AltRtgStep);
            Assert.Equal(prelMatFilter.Operation, result.First().Operation);
            Assert.Equal(prelMatFilter.OperDesc, result.First().OperDesc);
            Assert.Equal(prelMatFilter.Bom, result.First().Bom);
        }

        [Fact]
        public void PostPrelMatList_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<A3AppPrelMat>().GetEnumerator());

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var result = _prelMatRepository.PostPrelMatList(new List<PrelMatFilter>());

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetViewPrelMat_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(_prelMats.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(_prelMats.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(_prelMats.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(_prelMats.GetEnumerator());

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var result = _prelMatRepository.GetViewPrelMat(_viewPrelMatRequestFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_prelMats[1], result.First());
            Assert.Equal(_prelMats[1].Job, result.First().Job);
        }

        [Fact]
        public void GetViewPrelMat_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<A3AppPrelMat>().GetEnumerator());

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var result = _prelMatRepository.GetViewPrelMat(new ViewPrelMatRequestFilter());

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void PutPrelMat_ShouldUpdatePrelMat_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(_prelMats.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(_prelMats.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(_prelMats.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(_prelMats.GetEnumerator());

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var result = _prelMatRepository.PutViewPrelMat(_viewPrelMatPutFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_viewPrelMatPutFilter.PrelQty, result.PrelQty);
        }

        [Fact]
        public void PutPrelMat_ShouldReturnNull_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<A3AppPrelMat>().GetEnumerator());

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var result = _prelMatRepository.PutViewPrelMat(new ViewPrelMatPutFilter());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeletePrelMat_ShouldDeletePrelMat_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(_prelMats.AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(_prelMats.AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(_prelMats.AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(_prelMats.GetEnumerator());
            _mockSet.Setup(m => m.Remove(It.IsAny<A3AppPrelMat>())).Callback<A3AppPrelMat>(entity => _prelMats.Remove(entity));
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] ids) => _prelMats.FirstOrDefault(x => x.PrelMatId == (int)ids[0]));
            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var deleteFilter = new ViewPrelMatDeleteFilter
            {
                PrelMatId = 1
            };

            var result = _prelMatRepository.DeletePrelMatId(deleteFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PrelMatId);
        }

        [Fact]
        public void DeletePrelMat_ShouldReturnNull_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Provider).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Provider);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.Expression).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().Expression);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<A3AppPrelMat>().AsQueryable().ElementType);
            _mockSet.As<IQueryable<A3AppPrelMat>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<A3AppPrelMat>().GetEnumerator());
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((object[] ids) => null);
            _mockSet.Setup(m => m.Remove(It.IsAny<A3AppPrelMat>())).Callback<A3AppPrelMat>(entity => _prelMats.Remove(entity));

            _mockContext.Setup(c => c.A3AppPrelMats).Returns(_mockSet.Object);

            // Act
            var deleteFilter = new ViewPrelMatDeleteFilter
            {
                PrelMatId = 999 // Non-existent ID
            };

            var result = _prelMatRepository.DeletePrelMatId(deleteFilter);

            // Assert
            Assert.Null(result);
        }
    }
}