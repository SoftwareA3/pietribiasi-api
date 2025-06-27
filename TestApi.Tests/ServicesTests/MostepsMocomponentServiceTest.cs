using Xunit;
using Moq;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using apiPB.Services.Implementation; 
using apiPB.Services.Abstraction;  
using apiPB.Repository.Abstraction;       
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;                   
using apiPB.Models;                        
using apiPB.Mappers.Dto;   

namespace TestApi.Tests.ServicesTests
{
    public class MostepsMocomponentServiceTest
    {
        private readonly Mock<IMostepsMocomponentRepository> _mostepsMocomponentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IMostepsMocomponentRequestService _mostepsMocomponentService;
        private readonly VwApiMostepsMocomponent _sampleVwApiMostepsMocomponent = new VwApiMostepsMocomponent
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
            CreationDate = new DateTime(2023, 1, 1),
            UoM = "UOM1",
            ProductionQty = 100,
            ProducedQty = 50,
            ResQty = 25,
            Storage = "Storage1",
            Wc = "WC1"
        };

        private readonly MostepsMocomponentDto _sampleMostepsMocomponentDto = new MostepsMocomponentDto
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
            CreationDate = new DateTime(2023, 1, 1),
            UoM = "UOM1",
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

        private readonly BarCodeRequestDto _sampleBarCodeRequest = new BarCodeRequestDto
        {
            Job = "Job1",
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 1, 1),
            Operation = "Op1",
            BarCode = "BarCode1"
        };

        public MostepsMocomponentServiceTest()
        {
            _mostepsMocomponentRepositoryMock = new Mock<IMostepsMocomponentRepository>();
            _mapperMock = new Mock<IMapper>();
            _mostepsMocomponentService = new MostepsMocomponentRequestService(_mostepsMocomponentRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetMostepsMocomponentJobDistinct_ShouldReturnListOfMostepsMocomponentDto_WhenDataExists()
        {
            // Arrange
            var vwApiMostepsMocomponents = new List<VwApiMostepsMocomponent> { _sampleVwApiMostepsMocomponent };
            _mostepsMocomponentRepositoryMock.Setup(repo => repo.GetMostepsMocomponentJob(It.IsAny<JobFilter>()))
                .Returns(vwApiMostepsMocomponents);

            _mapperMock.Setup(m => m.Map<JobFilter>(It.IsAny<JobRequestDto>()))
                .Returns(new JobFilter());

            _mapperMock.Setup(m => m.Map<MostepsMocomponentDto>(It.IsAny<VwApiMostepsMocomponent>()))
                .Returns(_sampleMostepsMocomponentDto);

            // Act
            var result = _mostepsMocomponentService.GetMostepsMocomponentJobDistinct(_sampleJobRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_sampleMostepsMocomponentDto.Job, result.First().Job);
            Assert.Equal(_sampleMostepsMocomponentDto.RtgStep, result.First().RtgStep);
            Assert.Equal(_sampleMostepsMocomponentDto.Alternate, result.First().Alternate);
            Assert.Equal(_sampleMostepsMocomponentDto.AltRtgStep, result.First().AltRtgStep);
            Assert.Equal(_sampleMostepsMocomponentDto.Operation, result.First().Operation);
            Assert.Equal(_sampleMostepsMocomponentDto.OperDesc, result.First().OperDesc);
            Assert.Equal(_sampleMostepsMocomponentDto.Bom, result.First().Bom);
            Assert.Equal(_sampleMostepsMocomponentDto.Variant, result.First().Variant);
            Assert.Equal(_sampleMostepsMocomponentDto.ItemDesc, result.First().ItemDesc);
            Assert.Equal(_sampleMostepsMocomponentDto.Moid, result.First().Moid);
            Assert.Equal(_sampleMostepsMocomponentDto.UoM, result.First().UoM);
            Assert.Equal(_sampleMostepsMocomponentDto.Mono, result.First().Mono);
            Assert.Equal(_sampleMostepsMocomponentDto.CreationDate, result.First().CreationDate);
            Assert.Equal(_sampleMostepsMocomponentDto.ProductionQty, result.First().ProductionQty);
            Assert.Equal(_sampleMostepsMocomponentDto.ProducedQty, result.First().ProducedQty);
            Assert.Equal(_sampleMostepsMocomponentDto.ResQty, result.First().ResQty);
            Assert.Equal(_sampleMostepsMocomponentDto.Storage, result.First().Storage);
            Assert.Equal(_sampleMostepsMocomponentDto.Wc, result.First().Wc);
        }

        [Fact]
        public void GetMostepsMocomponentJobDistinct_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new JobFilter { Job = "Job1" };
            var vwApiMostepsMocomponents = new List<VwApiMostepsMocomponent>();
            _mostepsMocomponentRepositoryMock.Setup(repo => repo.GetMostepsMocomponentJob(It.IsAny<JobFilter>()))
                .Returns(vwApiMostepsMocomponents);

            _mapperMock.Setup(m => m.Map<JobFilter>(It.IsAny<JobRequestDto>()))
                .Returns(filter);

            // Act
            var result = _mostepsMocomponentService.GetMostepsMocomponentJobDistinct(_sampleJobRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepsMocomponentMonoDistinct_ShouldReturnListOfMostepsMocomponentDto_WhenDataExists()
        {
            // Arrange
            var filter = new MonoFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1) };
            var vwApiMostepsMocomponents = new List<VwApiMostepsMocomponent> { _sampleVwApiMostepsMocomponent };
            _mostepsMocomponentRepositoryMock.Setup(repo => repo.GetMostepsMocomponentMono(It.IsAny<MonoFilter>()))
                .Returns(vwApiMostepsMocomponents);

            _mapperMock.Setup(m => m.Map<MonoFilter>(It.IsAny<MonoRequestDto>()))
                .Returns(filter);

            _mapperMock.Setup(m => m.Map<MostepsMocomponentDto>(It.IsAny<VwApiMostepsMocomponent>()))
                .Returns(_sampleMostepsMocomponentDto);

            // Act
            var result = _mostepsMocomponentService.GetMostepsMocomponentMonoDistinct(_sampleMonoRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_sampleMostepsMocomponentDto.Job, result.First().Job);
            Assert.Equal(_sampleMostepsMocomponentDto.RtgStep, result.First().RtgStep);
            Assert.Equal(_sampleMostepsMocomponentDto.Alternate, result.First().Alternate);
            Assert.Equal(_sampleMostepsMocomponentDto.AltRtgStep, result.First().AltRtgStep);
            Assert.Equal(_sampleMostepsMocomponentDto.Operation, result.First().Operation);
            Assert.Equal(_sampleMostepsMocomponentDto.OperDesc, result.First().OperDesc);
            Assert.Equal(_sampleMostepsMocomponentDto.Bom, result.First().Bom);
            Assert.Equal(_sampleMostepsMocomponentDto.Variant, result.First().Variant);
            Assert.Equal(_sampleMostepsMocomponentDto.ItemDesc, result.First().ItemDesc);
            Assert.Equal(_sampleMostepsMocomponentDto.Moid, result.First().Moid);
            Assert.Equal(_sampleMostepsMocomponentDto.UoM, result.First().UoM);
            Assert.Equal(_sampleMostepsMocomponentDto.Mono, result.First().Mono);
            Assert.Equal(_sampleMostepsMocomponentDto.CreationDate, result.First().CreationDate);
            Assert.Equal(_sampleMostepsMocomponentDto.ProductionQty, result.First().ProductionQty);
            Assert.Equal(_sampleMostepsMocomponentDto.ProducedQty, result.First().ProducedQty);
            Assert.Equal(_sampleMostepsMocomponentDto.ResQty, result.First().ResQty);
            Assert.Equal(_sampleMostepsMocomponentDto.Storage, result.First().Storage);
            Assert.Equal(_sampleMostepsMocomponentDto.Wc, result.First().Wc);
        }

        [Fact]
        public void GetMostepsMocomponentMonoDistinct_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new MonoFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1) };
            var vwApiMostepsMocomponents = new List<VwApiMostepsMocomponent>();
            _mostepsMocomponentRepositoryMock.Setup(repo => repo.GetMostepsMocomponentMono(It.IsAny<MonoFilter>()))
                .Returns(vwApiMostepsMocomponents);

            _mapperMock.Setup(m => m.Map<MonoFilter>(It.IsAny<MonoRequestDto>()))
                .Returns(filter);

            // Act
            var result = _mostepsMocomponentService.GetMostepsMocomponentMonoDistinct(_sampleMonoRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepsMocomponentOperationDistinct_ListOfMocomponentDto_WhenDataExists()
        {
            // Arrange
            var filter = new OperationFilter {Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1), Operation = "Op1"};
            var vwApiMostepsMocomponents = new List<VwApiMostepsMocomponent> { _sampleVwApiMostepsMocomponent };
            _mostepsMocomponentRepositoryMock.Setup(repo => repo.GetMostepsMocomponentOperation(It.IsAny<OperationFilter>()))
                .Returns(vwApiMostepsMocomponents);

            _mapperMock.Setup(m => m.Map<OperationFilter>(It.IsAny<OperationRequestDto>()))
                .Returns(filter);

            _mapperMock.Setup(m => m.Map<MostepsMocomponentDto>(It.IsAny<VwApiMostepsMocomponent>()))
                .Returns(_sampleMostepsMocomponentDto);

            // Act
            var result = _mostepsMocomponentService.GetMostepsMocomponentOperationDistinct(_sampleOperationRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_sampleMostepsMocomponentDto.Job, result.First().Job);
            Assert.Equal(_sampleMostepsMocomponentDto.RtgStep, result.First().RtgStep);
            Assert.Equal(_sampleMostepsMocomponentDto.Alternate, result.First().Alternate);
            Assert.Equal(_sampleMostepsMocomponentDto.AltRtgStep, result.First().AltRtgStep);
            Assert.Equal(_sampleMostepsMocomponentDto.Operation, result.First().Operation);
            Assert.Equal(_sampleMostepsMocomponentDto.OperDesc, result.First().OperDesc);
            Assert.Equal(_sampleMostepsMocomponentDto.Bom, result.First().Bom);
            Assert.Equal(_sampleMostepsMocomponentDto.Variant, result.First().Variant);
            Assert.Equal(_sampleMostepsMocomponentDto.ItemDesc, result.First().ItemDesc);
            Assert.Equal(_sampleMostepsMocomponentDto.Moid, result.First().Moid);
            Assert.Equal(_sampleMostepsMocomponentDto.UoM, result.First().UoM);
            Assert.Equal(_sampleMostepsMocomponentDto.Mono, result.First().Mono);
            Assert.Equal(_sampleMostepsMocomponentDto.CreationDate, result.First().CreationDate);
            Assert.Equal(_sampleMostepsMocomponentDto.ProductionQty, result.First().ProductionQty);
            Assert.Equal(_sampleMostepsMocomponentDto.ProducedQty, result.First().ProducedQty);
            Assert.Equal(_sampleMostepsMocomponentDto.ResQty, result.First().ResQty);
            Assert.Equal(_sampleMostepsMocomponentDto.Storage, result.First().Storage);
            Assert.Equal(_sampleMostepsMocomponentDto.Wc, result.First().Wc);
        }

        [Fact]
        public void GetMostepsMocomponentOperationDistinct_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new OperationFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1), Operation = "Op1" };
            var vwApiMostepsMocomponents = new List<VwApiMostepsMocomponent>();
            _mostepsMocomponentRepositoryMock.Setup(repo => repo.GetMostepsMocomponentOperation(It.IsAny<OperationFilter>()))
                .Returns(vwApiMostepsMocomponents);

            _mapperMock.Setup(m => m.Map<OperationFilter>(It.IsAny<OperationRequestDto>()))
                .Returns(filter);

            // Act
            var result = _mostepsMocomponentService.GetMostepsMocomponentOperationDistinct(_sampleOperationRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMostepsMocomponentBarCodeDistinct_ShouldReturnListOfMostepsMocomponentDto_WhenDataExists()
        {
            // Arrange
            var filter = new BarCodeFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1), Operation = "Op1", BarCode = "BarCode1" };
            var vwApiMostepsMocomponents = new List<VwApiMostepsMocomponent> { _sampleVwApiMostepsMocomponent };
            _mostepsMocomponentRepositoryMock.Setup(repo => repo.GetMostepsMocomponentBarCode(It.IsAny<BarCodeFilter>()))
                .Returns(vwApiMostepsMocomponents);

            _mapperMock.Setup(m => m.Map<BarCodeFilter>(It.IsAny<BarCodeRequestDto>()))
                .Returns(filter);

            _mapperMock.Setup(m => m.Map<MostepsMocomponentDto>(It.IsAny<VwApiMostepsMocomponent>()))
                .Returns(_sampleMostepsMocomponentDto);

            // Act
            var result = _mostepsMocomponentService.GetMostepsMocomponentBarCodeDistinct(_sampleBarCodeRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_sampleMostepsMocomponentDto.Job, result.First().Job);
            Assert.Equal(_sampleMostepsMocomponentDto.RtgStep, result.First().RtgStep);
            Assert.Equal(_sampleMostepsMocomponentDto.Alternate, result.First().Alternate);
            Assert.Equal(_sampleMostepsMocomponentDto.AltRtgStep, result.First().AltRtgStep);
            Assert.Equal(_sampleMostepsMocomponentDto.Operation, result.First().Operation);
            Assert.Equal(_sampleMostepsMocomponentDto.OperDesc, result.First().OperDesc);
            Assert.Equal(_sampleMostepsMocomponentDto.Bom, result.First().Bom);
            Assert.Equal(_sampleMostepsMocomponentDto.Variant, result.First().Variant);
            Assert.Equal(_sampleMostepsMocomponentDto.ItemDesc, result.First().ItemDesc);
            Assert.Equal(_sampleMostepsMocomponentDto.Moid, result.First().Moid);
            Assert.Equal(_sampleMostepsMocomponentDto.UoM, result.First().UoM);
            Assert.Equal(_sampleMostepsMocomponentDto.Mono, result.First().Mono);
            Assert.Equal(_sampleMostepsMocomponentDto.CreationDate, result.First().CreationDate);
            Assert.Equal(_sampleMostepsMocomponentDto.ProductionQty, result.First().ProductionQty);
            Assert.Equal(_sampleMostepsMocomponentDto.ProducedQty, result.First().ProducedQty);
            Assert.Equal(_sampleMostepsMocomponentDto.ResQty, result.First().ResQty);
            Assert.Equal(_sampleMostepsMocomponentDto.Storage, result.First().Storage);
            Assert.Equal(_sampleMostepsMocomponentDto.Wc, result.First().Wc);
        }
        
        [Fact]
        public void GetMostepsMocomponentBarCodeDistinct_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new BarCodeFilter { Job = "Job1", Mono = "Mono1", CreationDate = new DateTime(2023, 1, 1), Operation = "Op1", BarCode = "BarCode1" };
            var vwApiMostepsMocomponents = new List<VwApiMostepsMocomponent>();
            _mostepsMocomponentRepositoryMock.Setup(repo => repo.GetMostepsMocomponentBarCode(It.IsAny<BarCodeFilter>()))
                .Returns(vwApiMostepsMocomponents);

            _mapperMock.Setup(m => m.Map<BarCodeFilter>(It.IsAny<BarCodeRequestDto>()))
                .Returns(filter);

            // Act
            var result = _mostepsMocomponentService.GetMostepsMocomponentBarCodeDistinct(_sampleBarCodeRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}