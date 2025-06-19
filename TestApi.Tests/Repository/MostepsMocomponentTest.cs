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
    public class MostepsMocomponentTest
    {
        private readonly MostepsMocomponentRepository _mostepsMocomponentRepository;
        private readonly Mock<DbSet<VwApiMostepsMocomponent>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;

        private readonly List<VwApiMostepsMocomponent> _mostepsMocomponents = new List<VwApiMostepsMocomponent>
        {
            new VwApiMostepsMocomponent
            {
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
                BarCode = "BarCode1",
                Wc = "WC1"
            },
            new VwApiMostepsMocomponent
            {
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
                BarCode = "BarCode2",
                Wc = "WC2"
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
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 10, 1),
            Operation = "Op1"
        };

        private readonly BarCodeFilter _barCodeFilter = new BarCodeFilter
        {
            Job = "Job1",
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 10, 1),
            Operation = "Op1",
            BarCode = "BarCode1"
        };  

        public MostepsMocomponentTest()
        {
            _mockSet = new Mock<DbSet<VwApiMostepsMocomponent>>();
            _mockContext = new Mock<ApplicationDbContext>();
            _mostepsMocomponentRepository = new MostepsMocomponentRepository(_mockContext.Object);
        }

        [Fact]
        public void GetMostepsMocomponentJob_ReturnsDistinctResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(_mostepsMocomponents.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(_mostepsMocomponents.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(_mostepsMocomponents.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(_mostepsMocomponents.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentJob(_jobFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetMostepsMocomponentJobDistinct_ReturnsEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(_mostepsMocomponents.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(_mostepsMocomponents.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(_mostepsMocomponents.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(_mostepsMocomponents.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentJob(new JobFilter { Job = "NonExistentJob" });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepsMocomponentMonoDistinct_ReturnsDistinctResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(_mostepsMocomponents.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(_mostepsMocomponents.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(_mostepsMocomponents.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(_mostepsMocomponents.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentMono(_monoFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetMostepsMocomponentMonoDistinct_ReturnsEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(_mostepsMocomponents.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(_mostepsMocomponents.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(_mostepsMocomponents.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(_mostepsMocomponents.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentMono(new MonoFilter { Job = "NonExistentJob", Mono = "NonExistentMono", CreationDate = DateTime.Now });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepsMocomponentOperationDistinct_ReturnsDistinctResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(_mostepsMocomponents.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(_mostepsMocomponents.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(_mostepsMocomponents.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(_mostepsMocomponents.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentOperation(_operationFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetMostepsMocomponentOperationDistinct_ReturnsEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(_mostepsMocomponents.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(_mostepsMocomponents.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(_mostepsMocomponents.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(_mostepsMocomponents.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentOperation(new OperationFilter { Job = "NonExistentJob", Mono = "NonExistentMono", CreationDate = DateTime.Now, Operation = "NonExistentOperation" });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepsMocomponentBarCodeDistinct_ReturnsDistinctResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(_mostepsMocomponents.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(_mostepsMocomponents.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(_mostepsMocomponents.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(_mostepsMocomponents.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentBarCode(_barCodeFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetMostepsMocomponentBarCodeDistinct_ReturnsEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(_mostepsMocomponents.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(_mostepsMocomponents.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(_mostepsMocomponents.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(_mostepsMocomponents.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentBarCode(new BarCodeFilter { Job = "NonExistentJob", Mono = "NonExistentMono", CreationDate = DateTime.Now, Operation = "NonExistentOperation", BarCode = "NonExistentBarCode" });

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}