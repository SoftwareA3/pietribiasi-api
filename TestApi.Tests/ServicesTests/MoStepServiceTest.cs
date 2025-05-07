using Xunit;
using Moq;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using apiPB.Services.Request.Implementation; 
using apiPB.Services.Request.Abstraction;  
using apiPB.Repository.Abstraction;       
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;                   
using apiPB.Models;                        
using apiPB.Mappers.Dto;   

namespace TestApi.Tests.ServicesTests
{
    public class MoStepServiceTest
    {
        private readonly Mock<IMoStepRepository> _moStepRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IMoStepRequestService _moStepService;
        private readonly VwApiMostep _sampleVwApiMostep = new VwApiMostep
        {
            Job = "Job1",
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 1,
            Operation = "Op1",
            OperDesc = "Operation Description",
            Bom = "BOM1",
            Variant = "Variant1",
            ItemDesc = "Item Description",
            Moid = 123,
            Mono = "Mono1",
            CreationDate = DateTime.Now,
            Uom = "UOM1",
            ProductionQty = 100,
            ProducedQty = 50,
            ResQty = 25,
            Storage = "Storage1",
            Wc = "WC1"
        };

        private readonly MostepDto _sampleMostepDto = new MostepDto
        {
            Job = "Job1",
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 1,
            Operation = "Op1",
            OperDesc = "Operation Description",
            Bom = "BOM1",
            Variant = "Variant1",
            ItemDesc = "Item Description",
            Moid = 123,
            Mono = "Mono1",
            CreationDate = DateTime.Now,
            Uom = "UOM1",
            ProductionQty = 100,
            ProducedQty = 50,
            ResQty = 25,
            Storage = "Storage1",
            Wc = "WC1"
        };

        private readonly JobRequestDto _sampleJobRequest = new JobRequestDto
        {
            Job = "Job1"
        };

        private readonly MonoRequestDto _sampleMonoRequest = new MonoRequestDto
        {
            Job = "Job1",
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 1, 1)
        };

        private readonly OperationRequestDto _sampleOperationRequest = new OperationRequestDto
        {
            Job = "Job1",
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 1, 1),
            Operation = "Op1"
        };

        public MoStepServiceTest()
        {
            _moStepRepositoryMock = new Mock<IMoStepRepository>();
            _mapperMock = new Mock<IMapper>();
            _moStepService = new MoStepRequestService(_mapperMock.Object, _moStepRepositoryMock.Object);
        }

        [Fact]
        public void GetMostepWithJob_ShouldReturnListOfMostepDtoList_WhenDataExists()
        {
            // Arrange
            var filter = new JobFilter { Job = "Job1" };
            var vwApiMostepList = new List<VwApiMostep> { _sampleVwApiMostep };
            var expectedMostepDtoList = new List<MostepDto> { _sampleMostepDto };

            _mapperMock.Setup(m => m.Map<JobFilter>(_sampleJobRequest)).Returns(filter);
            _moStepRepositoryMock.Setup(r => r.GetMostepWithJob(filter)).Returns(vwApiMostepList);
            _mapperMock.Setup(m => m.Map<MostepDto>(It.IsAny<VwApiMostep>())).Returns(_sampleMostepDto);

            // Act
            var result = _moStepService.GetMostepWithJob(_sampleJobRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMostepDtoList.Count, result.Count());
            Assert.Equal(expectedMostepDtoList.First().Job, result.First().Job);
        }

        [Fact]
        public void GetMostepWithJob_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new JobFilter { Job = "Job1" };
            var vwApiMostepList = new List<VwApiMostep>();

            _mapperMock.Setup(m => m.Map<JobFilter>(_sampleJobRequest)).Returns(filter);
            _moStepRepositoryMock.Setup(r => r.GetMostepWithJob(filter)).Returns(vwApiMostepList);

            // Act
            var result = _moStepService.GetMostepWithJob(_sampleJobRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnListOfMostepDtoList_WhenDataExists()
        {
            // Arrange
            var filter = new MonoFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1) };
            var vwApiMostepList = new List<VwApiMostep> { _sampleVwApiMostep };
            var expectedMostepDtoList = new List<MostepDto> { _sampleMostepDto };

            _mapperMock.Setup(m => m.Map<MonoFilter>(_sampleMonoRequest)).Returns(filter);
            _moStepRepositoryMock.Setup(r => r.GetMostepWithMono(filter)).Returns(vwApiMostepList);
            _mapperMock.Setup(m => m.Map<MostepDto>(It.IsAny<VwApiMostep>())).Returns(_sampleMostepDto);

            // Act
            var result = _moStepService.GetMostepWithMono(_sampleMonoRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMostepDtoList.Count, result.Count());
            Assert.Equal(expectedMostepDtoList.First().Job, result.First().Job);
        }

        [Fact]
        public void GetMostepWithMono_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new MonoFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1) };
            var vwApiMostepList = new List<VwApiMostep>();

            _mapperMock.Setup(m => m.Map<MonoFilter>(_sampleMonoRequest)).Returns(filter);
            _moStepRepositoryMock.Setup(r => r.GetMostepWithMono(filter)).Returns(vwApiMostepList);

            // Act
            var result = _moStepService.GetMostepWithMono(_sampleMonoRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnListOfMostepDtoList_WhenDataExists()
        {
            // Arrange
            var filter = new OperationFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1), Operation = "Op1" };
            var vwApiMostepList = new List<VwApiMostep> { _sampleVwApiMostep };
            var expectedMostepDtoList = new List<MostepDto> { _sampleMostepDto };

            _mapperMock.Setup(m => m.Map<OperationFilter>(_sampleOperationRequest)).Returns(filter);
            _moStepRepositoryMock.Setup(r => r.GetMostepWithOperation(filter)).Returns(vwApiMostepList);
            _mapperMock.Setup(m => m.Map<MostepDto>(It.IsAny<VwApiMostep>())).Returns(_sampleMostepDto);

            // Act
            var result = _moStepService.GetMostepWithOperation(_sampleOperationRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMostepDtoList.Count, result.Count());
            Assert.Equal(expectedMostepDtoList.First().Job, result.First().Job);
        }

        [Fact]
        public void GetMostepWithOperation_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new OperationFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1), Operation = "Op1" };
            var vwApiMostepList = new List<VwApiMostep>();

            _mapperMock.Setup(m => m.Map<OperationFilter>(_sampleOperationRequest)).Returns(filter);
            _moStepRepositoryMock.Setup(r => r.GetMostepWithOperation(filter)).Returns(vwApiMostepList);

            // Act
            var result = _moStepService.GetMostepWithOperation(_sampleOperationRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}