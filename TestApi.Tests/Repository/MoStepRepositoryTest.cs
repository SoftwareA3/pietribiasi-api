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

        #region GetMostepWithJob Tests

        [Fact]
        public void GetMostepWithJob_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            var queryableData = _moSteps.AsQueryable();
            
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithJob(_jobFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_jobFilter.Job, result.First().Job);
            Assert.Equal("Mono1", result.First().Mono);
        }

        [Fact]
        public void GetMostepWithJob_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            var queryableData = _moSteps.AsQueryable();

            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act & Assert
            Assert.Throws<EmptyListException>(() =>
                _moStepRepository.GetMostepWithJob(new JobFilter { Job = "NonExistentJob" }));
        }

        #endregion

        #region GetMostepWithMono Tests

        [Fact]
        public void GetMostepWithMono_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            var queryableData = _moSteps.AsQueryable();
            
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithMono(_monoFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_monoFilter.Mono, result.First().Mono);
            Assert.Equal(_monoFilter.Job, result.First().Job);
            Assert.Equal(_monoFilter.CreationDate, result.First().CreationDate);
        }

        [Fact]
        public void GetMostepWithMono_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            var queryableData = _moSteps.AsQueryable();
            
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act & Assert
            Assert.Throws<EmptyListException>(() => 
                _moStepRepository.GetMostepWithMono(new MonoFilter 
                { 
                    Job = "NonExistentJob", 
                    Mono = "NonExistentMono", 
                    CreationDate = DateTime.Now 
                }));
        }

        #endregion

        #region GetMostepWithOperation Tests

        [Fact]
        public void GetMostepWithOperation_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            var queryableData = _moSteps.AsQueryable();
            
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithOperation(_operationFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Op1", result.First().Operation);
            Assert.Equal("Job1", result.First().Job);
            Assert.Equal("Mono1", result.First().Mono);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldThrowEmptyListException_WhenNoDataExists()
        {
            // Arrange
            var queryableData = _moSteps.AsQueryable();
            
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act & Assert
            Assert.Throws<EmptyListException>(() => 
                _moStepRepository.GetMostepWithOperation(new OperationFilter 
                { 
                    Job = "NonExistentJob", 
                    Mono = "NonExistentMono", 
                    CreationDate = DateTime.Now, 
                    Operation = "NonExistentOperation" 
                }));
        }

        #endregion

        #region Additional Edge Case Tests

        [Fact]
        public void GetMostepWithJob_ShouldReturnMultipleResults_WhenMultipleJobsMatch()
        {
            // Arrange
            var additionalData = new List<VwApiMostep>(_moSteps)
            {
                new VwApiMostep
                {
                    Job = "Job1", // Same job as first item
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
                    CreationDate = new DateTime(2023, 11, 1),
                    Uom = "UOM3",
                    ProductionQty = 150,
                    ProducedQty = 75,
                    ResQty = 75,
                    Storage = "Storage3",
                    Wc = "WC3"
                }
            };

            var queryableData = additionalData.AsQueryable();
            
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithJob(_jobFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal("Job1", item.Job));
        }

        [Fact]
        public void GetMostepWithJob_ShouldCallVwApiMosteps_OnContext()
        {
            // Arrange
            var queryableData = _moSteps.AsQueryable();
            
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwApiMostep>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwApiMosteps).Returns(_mockSet.Object);

            // Act
            var result = _moStepRepository.GetMostepWithJob(_jobFilter);

            // Assert
            _mockContext.Verify(c => c.VwApiMosteps, Times.Once);
            Assert.NotNull(result);
        }

        #endregion
    }
}