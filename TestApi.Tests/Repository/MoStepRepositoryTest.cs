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
    public class MoStepRepositoryTest
    {
        private readonly MoStepRepository _moStepRepository;
        private readonly Mock<DbSet<VwApiMostep>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;

        private readonly List<VwApiMostep> _moSteps = new List<VwApiMostep>
        {
            new VwApiMostep
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
                Uom = "UOM1",
                ProductionQty = 100,
                ProducedQty = 50,
                ResQty = 50,
                Storage = "Storage1",
                Wc = "WC1"
            },
            new VwApiMostep
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
                Uom = "UOM2",
                ProductionQty = 200,
                ProducedQty = 100,
                ResQty = 100,
                Storage = "Storage2",
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

        public MoStepRepositoryTest()
        {
            _mockSet = new Mock<DbSet<VwApiMostep>>();
            _mockContext = new Mock<ApplicationDbContext>();
            _moStepRepository = new MoStepRepository(_mockContext.Object);
        }

        [Fact]
        public void GetMostepWithJob_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(() => _moSteps.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(() => _moSteps.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(() => _moSteps.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(() => _moSteps.AsQueryable().GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithJob(_jobFilter);

            // Assert
            Assert.Single(result);
            Assert.Equal(_jobFilter.Job, result.First().Job);
        }

        [Fact]
        public void GetMostepWithJob_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(_moSteps.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(_moSteps.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(_moSteps.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(_moSteps.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithJob(new JobFilter { Job = "NonExistentJob" });

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(_moSteps.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(_moSteps.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(_moSteps.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(_moSteps.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithMono(_monoFilter);

            // Assert
            Assert.Single(result);
            Assert.Equal(_monoFilter.Mono, result.First().Mono);
            Assert.Equal(_monoFilter.Job, result.First().Job);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(_moSteps.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(_moSteps.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(_moSteps.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(_moSteps.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithMono(new MonoFilter { Job = "NonExistentJob", Mono = "NonExistentMono", CreationDate = DateTime.Now });

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(_moSteps.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(_moSteps.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(_moSteps.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(_moSteps.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithOperation(_operationFilter);

            // Assert
            Assert.Single(result);
            Assert.Equal("Op1", result.First().Operation);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnEmpty_WhenNoDataExists()
        {
            // Arrange
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(_moSteps.AsQueryable().Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(_moSteps.AsQueryable().Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(_moSteps.AsQueryable().ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(_moSteps.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithOperation(new OperationFilter { Job = "NonExistentJob", Mono = "NonExistentMono", CreationDate = DateTime.Now, Operation = "NonExistentOperation" });

            // Assert
            Assert.Empty(result);
        }
    }
}