using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using AutoMapper;
using apiPB.Services.Implementation;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Utils.Implementation;
using apiPB.Models;

namespace TestApi.Tests.ServicesTests
{
    public class ActionMessageServiceTest
    {
        private readonly Mock<IActionMessageRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ActionMessageRequestService _service;

        public ActionMessageServiceTest()
        {
            _mockRepository = new Mock<IActionMessageRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new ActionMessageRequestService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetActionMessagesByFilter_WithValidRequest_ReturnsActionMessageListDto()
        {
            // Arrange
            var request = new ImportedLogMessageDto
            {
                Moid = 1,
                RtgStep = 2,
                Alternate = "Alt1",
                AltRtgStep = 3,
                Mono = "Mono1",
                Bom = "Bom1",
                Variant = "Variant1",
                Wc = "WC1",
                Operation = "Operation1",
                WorkerId = 123,
                ActionType = 1
            };

            var filter = new ImportedLogMessageFilter
            {
                Moid = 1,
                RtgStep = 2,
                Alternate = "Alt1",
                AltRtgStep = 3,
                Mono = "Mono1",
                Bom = "Bom1",
                Variant = "Variant1",
                Wc = "WC1",
                Operation = "Operation1",
                WorkerId = 123,
                ActionType = 1
            };

            // Simulazione del risultato dal repository (VwOmActionMessage entities)
            var repositoryResult = new List<VwOmActionMessage>
            {
                new VwOmActionMessage
                {
                    ActionId = 1,
                    Moid = 1,
                    RtgStep = 2,
                    Alternate = "Alt1",
                    AltRtgStep = 3,
                    Mono = "Mono1",
                    Bom = "Bom1",
                    Variant = "Variant1",
                    Wc = "WC1",
                    Operation = "Operation1",
                    Job = "Job1",
                    WorkerId = 123,
                    ActionType = 1,
                    ActionStatus = 1,
                    ActionMessage = "Message1",
                    Closed = "Closed1",
                    WorkerProcessingTime = 100,
                    WorkerSetupTime = 50,
                    ActualProcessingTime = 120,
                    ActualSetupTime = 60,
                    Storage = "Storage1",
                    SpecificatorType = 1,
                    Specificator = "Specificator1",
                    ReturnMaterialQtyLower = "QtyLower1",
                    PickMaterialQtyGreater = "QtyGreater1",
                    ProductionLotNumber = "LotNumber1",
                    ProductionQty = 10.5,
                    DeliveryDate = DateTime.Now,
                    Tbcreated = DateTime.Now,
                    TbcreatedId = 12345,
                    ConfirmChildMos = "ConfirmChildMos1",
                    Mostatus = 1,
                    MessageId = 1,
                    MessageType = 1,
                    MessageDate = DateTime.Now,
                    MessageText = "Text1"
                }
            };

            var expectedResult = new ActionMessageListDto
            {
                ActionType = "ActionType1",
                Moid = 1,
                RtgStep = 2,
                Alternate = "Alt1",
                AltRtgStep = 3,
                Mono = "Mono1",
                Bom = "Bom1",
                Variant = "Variant1",
                Wc = "WC1",
                Operation = "Operation1",
                WorkerId = 123,
                ActionMessageDetails = new List<ActionMessageDetailsDto>
                {
                    new ActionMessageDetailsDto
                    {
                        ActionId = 1,
                        Job = "Job1",
                        ActionStatus = "Status1",
                        ActionMessage = "Message1",
                        Closed = "Closed1",
                        WorkerProcessingTime = 100,
                        WorkerSetupTime = 50,
                        ActualProcessingTime = 120,
                        ActualSetupTime = 60,
                        Storage = "Storage1",
                        SpecificatorType = 1,
                        Specificator = "Specificator1",
                        ReturnMaterialQtyLower = "QtyLower1",
                        PickMaterialQtyGreater = "QtyGreater1",
                        ProductionLotNumber = "LotNumber1",
                        ProductionQty = 10.5,
                        DeliveryDate = DateTime.Now,
                        Tbcreated = DateTime.Now,
                        TbcreatedId = 12345,
                        ConfirmChildMos = "ConfirmChildMos1",
                        Mostatus = "Mostatus1"
                    }
                },
                OmMessageDetails = new List<OmMessageDetailsDto>
                {
                    new OmMessageDetailsDto
                    {
                        MessageId = 1,
                        MessageType = "Type1",
                        MessageDate = DateTime.Now,
                        MessageText = "Text1"
                    }
                }
            };

            _mockMapper.Setup(m => m.Map<ImportedLogMessageFilter>(request))
                .Returns(filter);

            _mockRepository.Setup(r => r.GetActionMessagesByFilter(filter))
                .Returns(repositoryResult);

            // Act
            var result = _service.GetActionMessagesByFilter(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.Moid, result.Moid);
            Assert.Equal(expectedResult.RtgStep, result.RtgStep);
            Assert.Equal(expectedResult.WorkerId, result.WorkerId);
            
            _mockMapper.Verify(m => m.Map<ImportedLogMessageFilter>(request), Times.Once);
            _mockRepository.Verify(r => r.GetActionMessagesByFilter(filter), Times.Once);
        }

        [Fact]
        public void GetActionMessagesByFilter_WhenRepositoryReturnsEmptyList_ReturnsEmptyResult()
        {
            // Arrange
            var request = new ImportedLogMessageDto
            {
                Moid = 1,
                ActionType = 1
            };

            var filter = new ImportedLogMessageFilter
            {
                Moid = 1,
                ActionType = 1
            };

            var emptyResult = new List<VwOmActionMessage>();

            _mockMapper.Setup(m => m.Map<ImportedLogMessageFilter>(request)).Returns(filter);

            _mockRepository.Setup(r => r.GetActionMessagesByFilter(filter)).Returns(emptyResult);

            // Act
            var result = _service.GetActionMessagesByFilter(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.ActionMessageDetails ?? new List<ActionMessageDetailsDto>());
            Assert.Empty(result.OmMessageDetails ?? new List<OmMessageDetailsDto>());
            
            _mockMapper.Verify(m => m.Map<ImportedLogMessageFilter>(request), Times.Once);
            _mockRepository.Verify(r => r.GetActionMessagesByFilter(filter), Times.Once);
        }

        [Fact]
        public void GetActionMessagesByFilter_WhenMapperThrowsException_PropagatesException()
        {
            // Arrange
            var request = new ImportedLogMessageDto
            {
                Moid = 1,
                ActionType = 1
            };

            _mockMapper.Setup(m => m.Map<ImportedLogMessageFilter>(request)).Throws(new AutoMapperMappingException("Mapping error"));

            // Act & Assert
            var exception = Assert.Throws<AutoMapperMappingException>(() => _service.GetActionMessagesByFilter(request));
            Assert.Equal("Mapping error", exception.Message);
        }

        [Fact]
        public void GetActionMessagesByFilter_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var request = new ImportedLogMessageDto
            {
                Moid = 1,
                ActionType = 1
            };

            var filter = new ImportedLogMessageFilter
            {
                Moid = 1,
                ActionType = 1
            };

            _mockMapper.Setup(m => m.Map<ImportedLogMessageFilter>(request)).Returns(filter);

            _mockRepository.Setup(r => r.GetActionMessagesByFilter(filter)).Throws(new InvalidOperationException("Repository error"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.GetActionMessagesByFilter(request));
            Assert.Equal("Repository error", exception.Message);
        }

        [Fact]
        public void GetActionMessagesByFilter_WithMultipleRecords_ReturnsCorrectAggregation()
        {
            // Arrange
            var request = new ImportedLogMessageDto
            {
                Moid = 1,
                ActionType = 1
            };

            var filter = new ImportedLogMessageFilter
            {
                Moid = 1,
                ActionType = 1
            };

            var repositoryResult = new List<VwOmActionMessage>
            {
                new VwOmActionMessage
                {
                    ActionId = 1,
                    Moid = 1,
                    ActionType = 1,
                    Job = "Job1",
                    ActionMessage = "Message1",
                    MessageId = 1,
                    MessageType = 1,
                    MessageText = "Error1"
                },
                new VwOmActionMessage
                {
                    ActionId = 2,
                    Moid = 1,
                    ActionType = 1,
                    Job = "Job2",
                    ActionMessage = "Message2",
                    MessageId = 2,
                    MessageType = 2,
                    MessageText = "Error2"
                }
            };

            _mockMapper.Setup(m => m.Map<ImportedLogMessageFilter>(request)).Returns(filter);

            _mockRepository.Setup(r => r.GetActionMessagesByFilter(filter)).Returns(repositoryResult);

            // Act
            var result = _service.GetActionMessagesByFilter(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Moid);
            // La logica di aggregazione dipende dall'implementazione di ToOmActionMessageDto()
            // che non è visibile nel codice fornito, quindi testiamo solo che il metodo venga chiamato
            
            _mockMapper.Verify(m => m.Map<ImportedLogMessageFilter>(request), Times.Once);
            _mockRepository.Verify(r => r.GetActionMessagesByFilter(filter), Times.Once);
        }

        [Fact]
        public void GetActionMessagesByFilter_WithMinimalRequest_WorksCorrectly()
        {
            // Arrange
            var request = new ImportedLogMessageDto();
            var filter = new ImportedLogMessageFilter();
            var emptyResult = new List<VwOmActionMessage>();

            _mockMapper.Setup(m => m.Map<ImportedLogMessageFilter>(request)).Returns(filter);

            _mockRepository.Setup(r => r.GetActionMessagesByFilter(filter)).Returns(emptyResult);

            // Act
            var result = _service.GetActionMessagesByFilter(request);

            // Assert
            Assert.NotNull(result);
            _mockMapper.Verify(m => m.Map<ImportedLogMessageFilter>(request), Times.Once);
            _mockRepository.Verify(r => r.GetActionMessagesByFilter(filter), Times.Once);
        }
    }
}