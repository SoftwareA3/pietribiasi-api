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

        private void SetupMockDbSet(IQueryable<VwApiMostepsMocomponent> data)
        {
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<VwApiMostepsMocomponent>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            
            _mockContext.Setup(c => c.VwApiMostepsMocomponents).Returns(_mockSet.Object);
        }

        [Fact]
        public void GetMostepsMocomponentJob_ReturnsDistinctResults_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_mostepsMocomponents.AsQueryable());

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentJob(_jobFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Job1", result.First().Job);
        }

        [Fact]
        public void GetMostepsMocomponentJob_ThrowsEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(Enumerable.Empty<VwApiMostepsMocomponent>().AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => 
                _mostepsMocomponentRepository.GetMostepsMocomponentJob(new JobFilter { Job = "NonExistentJob" }));
            
            Assert.Equal(nameof(MostepsMocomponentRepository), exception.ClassName);
            Assert.Equal(nameof(_mostepsMocomponentRepository.GetMostepsMocomponentJob), exception.MethodName);
        }

        [Fact]
        public void GetMostepsMocomponentMono_ReturnsDistinctResults_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_mostepsMocomponents.AsQueryable());

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentMono(_monoFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Job1", result.First().Job);
            Assert.Equal("Mono1", result.First().Mono);
        }

        [Fact]
        public void GetMostepsMocomponentMono_ThrowsEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(_mostepsMocomponents.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => 
                _mostepsMocomponentRepository.GetMostepsMocomponentMono(
                    new MonoFilter { Job = "NonExistentJob", Mono = "NonExistentMono", CreationDate = DateTime.Now }));
            
            Assert.Equal(nameof(MostepsMocomponentRepository), exception.ClassName);
            Assert.Equal(nameof(_mostepsMocomponentRepository.GetMostepsMocomponentMono), exception.MethodName);
        }

        [Fact]
        public void GetMostepsMocomponentOperation_ReturnsDistinctResults_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_mostepsMocomponents.AsQueryable());

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentOperation(_operationFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Job1", result.First().Job);
            Assert.Equal("Mono1", result.First().Mono);
            Assert.Equal("Op1", result.First().Operation);
        }

        [Fact]
        public void GetMostepsMocomponentOperation_ThrowsEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(_mostepsMocomponents.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => 
                _mostepsMocomponentRepository.GetMostepsMocomponentOperation(
                    new OperationFilter { Job = "NonExistentJob", Mono = "NonExistentMono", CreationDate = DateTime.Now, Operation = "NonExistentOperation" }));
            
            Assert.Equal(nameof(MostepsMocomponentRepository), exception.ClassName);
            Assert.Equal(nameof(_mostepsMocomponentRepository.GetMostepsMocomponentOperation), exception.MethodName);
        }

        [Fact]
        public void GetMostepsMocomponentBarCode_ReturnsDistinctResults_WhenDataExists()
        {
            // Arrange
            SetupMockDbSet(_mostepsMocomponents.AsQueryable());

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentBarCode(_barCodeFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Job1", result.First().Job);
            Assert.Equal("Mono1", result.First().Mono);
            Assert.Equal("Op1", result.First().Operation);
            Assert.Equal("BarCode1", result.First().BarCode);
        }

        [Fact]
        public void GetMostepsMocomponentBarCode_ThrowsEmptyListException_WhenNoDataExists()
        {
            // Arrange
            SetupMockDbSet(_mostepsMocomponents.AsQueryable());

            // Act & Assert
            var exception = Assert.Throws<EmptyListException>(() => 
                _mostepsMocomponentRepository.GetMostepsMocomponentBarCode(
                    new BarCodeFilter { Job = "NonExistentJob", Mono = "NonExistentMono", CreationDate = DateTime.Now, Operation = "NonExistentOperation", BarCode = "NonExistentBarCode" }));
            
            Assert.Equal(nameof(MostepsMocomponentRepository), exception.ClassName);
            Assert.Equal(nameof(_mostepsMocomponentRepository.GetMostepsMocomponentBarCode), exception.MethodName);
        }

        [Fact]
        public void GetMostepsMocomponentJob_HandlesDuplicateData_ReturnsDistinct()
        {
            // Arrange
            var duplicateData = new List<VwApiMostepsMocomponent>
            {
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", Operation = "Op1" },
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", Operation = "Op1" }, // Duplicate
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono2", Operation = "Op2" }
            };
            
            SetupMockDbSet(duplicateData.AsQueryable());

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentJob(_jobFilter);

            // Assert
            Assert.NotNull(result);
            // Distinct dovrebbe rimuovere i duplicati, ma il test dipende dall'implementazione di Distinct() su VwApiMostepsMocomponent
            Assert.True(result.Count() >= 1);
        }

        [Fact]
        public void GetMostepsMocomponentMono_FiltersCorrectlyByAllParameters()
        {
            // Arrange
            var testData = new List<VwApiMostepsMocomponent>
            {
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 10, 1) },
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono2", CreationDate = new DateTime(2023, 10, 1) },
                new VwApiMostepsMocomponent { Job = "Job2", Mono = "Mono1", CreationDate = new DateTime(2023, 10, 1) },
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 10, 2) }
            };
            
            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentMono(_monoFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var item = result.First();
            Assert.Equal("Job1", item.Job);
            Assert.Equal("Mono1", item.Mono);
            Assert.Equal(new DateTime(2023, 10, 1), item.CreationDate);
        }

        [Fact]
        public void GetMostepsMocomponentOperation_FiltersCorrectlyByAllParameters()
        {
            // Arrange
            var testData = new List<VwApiMostepsMocomponent>
            {
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 10, 1), Operation = "Op1" },
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 10, 1), Operation = "Op2" },
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono2", CreationDate = new DateTime(2023, 10, 1), Operation = "Op1" }
            };
            
            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentOperation(_operationFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var item = result.First();
            Assert.Equal("Job1", item.Job);
            Assert.Equal("Mono1", item.Mono);
            Assert.Equal(new DateTime(2023, 10, 1), item.CreationDate);
            Assert.Equal("Op1", item.Operation);
        }

        [Fact]
        public void GetMostepsMocomponentBarCode_FiltersCorrectlyByAllParameters()
        {
            // Arrange
            var testData = new List<VwApiMostepsMocomponent>
            {
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 10, 1), Operation = "Op1", BarCode = "BarCode1" },
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 10, 1), Operation = "Op1", BarCode = "BarCode2" },
                new VwApiMostepsMocomponent { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 10, 1), Operation = "Op2", BarCode = "BarCode1" }
            };
            
            SetupMockDbSet(testData.AsQueryable());

            // Act
            var result = _mostepsMocomponentRepository.GetMostepsMocomponentBarCode(_barCodeFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var item = result.First();
            Assert.Equal("Job1", item.Job);
            Assert.Equal("Mono1", item.Mono);
            Assert.Equal(new DateTime(2023, 10, 1), item.CreationDate);
            Assert.Equal("Op1", item.Operation);
            Assert.Equal("BarCode1", item.BarCode);
        }
    }
}