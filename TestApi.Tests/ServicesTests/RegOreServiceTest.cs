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
    public class RegOreServiceTest
    {
        private readonly Mock<IRegOreRepository> _regOrerRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IRegOreRequestService _regOreService;

        private readonly A3AppRegOre _sampleRegOre = new A3AppRegOre
        {
            RegOreId = 1,
            WorkerId = 123,
            SavedDate = new DateTime(2023, 10, 1),
            Job = "Job1",
            RtgStep = 2,
            Alternate = "Alt1",
            AltRtgStep = 3,
            Operation = "Op1",
            OperDesc = "Operation Description",
            Bom = "BOM1",
            Variant = "Variant1",
            ItemDesc = "Item Description",
            Moid = 456,
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 10, 2),
            Uom = "UOM1",
            ProductionQty = 100.0,
            ProducedQty = 50.0,
            ResQty = 25.0,
            Storage = "Storage1",
            Wc = "WC1",
            WorkingTime = 3600,
            Imported = false,
            UserImp = "User1",
            DataImp = new DateTime(2023, 10, 3)
        };

        private readonly RegOreRequestDto _sampleRegOreRequest = new RegOreRequestDto
        {
            WorkerId = 123,
            Job = "Job1",
            RtgStep = 2,
            Alternate = "Alt1",
            AltRtgStep = 3,
            Operation = "Op1",
            OperDesc = "Operation Description",
            Bom = "BOM1",
            Variant = "Variant1",
            ItemDesc = "Item Description",
            Moid = 456,
            Mono = "Mono1",
            CreationDate = new DateTime(2023, 10, 2),
            Uom = "UOM1",
            ProductionQty = 100.0,
            ProducedQty = 50.0,
            ResQty = 25.0,
            Storage = "Storage1",
            Wc = "WC1",
            WorkingTime = 3600,
            Imported = false,
            UserImp = "User1",
            DataImp = new DateTime(2023, 10, 3)
        };

        private readonly ViewOreRequestDto _sampleViewOreRequest = new ViewOreRequestDto
        {
            WorkerId = 123,
            FromDateTime = new DateTime(2023, 10, 1),
            ToDateTime = new DateTime(2023, 10, 31),
            Job = "Job1",
            Operation = "Op1",
            Mono = "Mono1"
        };

        private readonly ViewOrePutRequestDto _sampleViewOrePutRequest = new ViewOrePutRequestDto
        {
            RegOreId = 1,
            WorkingTime = 7200
        };

        private readonly ViewOreDeleteRequestDto _sampleViewOreDeleteRequest = new ViewOreDeleteRequestDto
        {
            RegOreId = 1
        };

        private readonly WorkerIdSyncRequestDto _sampleWorkerIdSyncRequest = new WorkerIdSyncRequestDto
        {
            WorkerId = 123
        };

        private readonly UpdateImportedIdRequestDto _sampleUpdateImportedIdRequest = new UpdateImportedIdRequestDto
        {
            Id = 1
        };

        public RegOreServiceTest()
        {
            _regOrerRepositoryMock = new Mock<IRegOreRepository>();
            _mapperMock = new Mock<IMapper>();
            _regOreService = new RegOreRequestService(_regOrerRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetAppRegOre_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.GetAppRegOre()).Returns(regOreList);

            // Act
            var result = _regOreService.GetAppRegOre();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var firstResult = result.First();
            Assert.Equal(_sampleRegOre.RegOreId, firstResult.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, firstResult.WorkerId);
            Assert.Equal(_sampleRegOre.Job, firstResult.Job);
            _regOrerRepositoryMock.Verify(repo => repo.GetAppRegOre(), Times.Once);
        }

        [Fact]
        public void GetAppRegOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.GetAppRegOre()).Returns(regOreList);

            // Act
            var result = _regOreService.GetAppRegOre();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.GetAppRegOre(), Times.Once);
        }

        [Fact]
        public void PostAppRegOre_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            var regOreRequestList = new List<RegOreRequestDto> { _sampleRegOreRequest };
            _regOrerRepositoryMock.Setup(repo => repo.PostRegOreList(It.IsAny<List<RegOreFilter>>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<RegOreFilter>(It.IsAny<RegOreRequestDto>())).Returns(new RegOreFilter());

            // Act
            var result = _regOreService.PostAppRegOre(regOreRequestList);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var firstResult = result.First();
            Assert.Equal(_sampleRegOre.RegOreId, firstResult.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, firstResult.WorkerId);
            Assert.Equal(_sampleRegOre.Job, firstResult.Job);
            _regOrerRepositoryMock.Verify(repo => repo.PostRegOreList(It.IsAny<List<RegOreFilter>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<RegOreFilter>(It.IsAny<RegOreRequestDto>()), Times.Once);
        }

        [Fact]
        public void PostAppRegOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreRequestList = new List<RegOreRequestDto> { _sampleRegOreRequest };
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.PostRegOreList(It.IsAny<List<RegOreFilter>>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<RegOreFilter>(It.IsAny<RegOreRequestDto>())).Returns(new RegOreFilter());

            // Act
            var result = _regOreService.PostAppRegOre(regOreRequestList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.PostRegOreList(It.IsAny<List<RegOreFilter>>()), Times.Once);
        }

        [Fact]
        public void PostAppRegOre_ProcessesMultipleRequests_Correctly()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre, _sampleRegOre };
            var regOreRequestList = new List<RegOreRequestDto> { _sampleRegOreRequest, _sampleRegOreRequest };
            _regOrerRepositoryMock.Setup(repo => repo.PostRegOreList(It.IsAny<List<RegOreFilter>>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<RegOreFilter>(It.IsAny<RegOreRequestDto>())).Returns(new RegOreFilter());

            // Act
            var result = _regOreService.PostAppRegOre(regOreRequestList);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mapperMock.Verify(m => m.Map<RegOreFilter>(It.IsAny<RegOreRequestDto>()), Times.Exactly(2));
        }

        [Fact]
        public void GetAppViewOre_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.GetAppViewOre(It.IsAny<ViewOreRequestFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<ViewOreRequestFilter>(It.IsAny<ViewOreRequestDto>())).Returns(new ViewOreRequestFilter());

            // Act
            var result = _regOreService.GetAppViewOre(_sampleViewOreRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var firstResult = result.First();
            Assert.Equal(_sampleRegOre.RegOreId, firstResult.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, firstResult.WorkerId);
            _regOrerRepositoryMock.Verify(repo => repo.GetAppViewOre(It.IsAny<ViewOreRequestFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ViewOreRequestFilter>(It.IsAny<ViewOreRequestDto>()), Times.Once);
        }

        [Fact]
        public void GetAppViewOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.GetAppViewOre(It.IsAny<ViewOreRequestFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<ViewOreRequestFilter>(It.IsAny<ViewOreRequestDto>())).Returns(new ViewOreRequestFilter());

            // Act
            var result = _regOreService.GetAppViewOre(_sampleViewOreRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.GetAppViewOre(It.IsAny<ViewOreRequestFilter>()), Times.Once);
        }

        [Fact]
        public void PutAppViewOre_ReturnsRegOreDto_WhenDataExists()
        {
            // Arrange
            _regOrerRepositoryMock.Setup(repo => repo.PutAppViewOre(It.IsAny<ViewOrePutFilter>())).Returns(_sampleRegOre);
            _mapperMock.Setup(m => m.Map<ViewOrePutFilter>(It.IsAny<ViewOrePutRequestDto>())).Returns(new ViewOrePutFilter());

            // Act
            var result = _regOreService.PutAppViewOre(_sampleViewOrePutRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_sampleRegOre.RegOreId, result.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, result.WorkerId);
            Assert.Equal(_sampleRegOre.Job, result.Job);
            _regOrerRepositoryMock.Verify(repo => repo.PutAppViewOre(It.IsAny<ViewOrePutFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ViewOrePutFilter>(It.IsAny<ViewOrePutRequestDto>()), Times.Once);
        }

        [Fact]
        public void DeleteRegOreId_ReturnsRegOreDto_WhenDataExists()
        {
            // Arrange
            _regOrerRepositoryMock.Setup(repo => repo.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestFilter>())).Returns(_sampleRegOre);
            _mapperMock.Setup(m => m.Map<ViewOreDeleteRequestFilter>(It.IsAny<ViewOreDeleteRequestDto>())).Returns(new ViewOreDeleteRequestFilter());

            // Act
            var result = _regOreService.DeleteRegOreId(_sampleViewOreDeleteRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_sampleRegOre.RegOreId, result.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, result.WorkerId);
            _regOrerRepositoryMock.Verify(repo => repo.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ViewOreDeleteRequestFilter>(It.IsAny<ViewOreDeleteRequestDto>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppRegOre_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.GetNotImportedRegOre()).Returns(regOreList);

            // Act
            var result = _regOreService.GetNotImportedAppRegOre();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var firstResult = result.First();
            Assert.Equal(_sampleRegOre.RegOreId, firstResult.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, firstResult.WorkerId);
            _regOrerRepositoryMock.Verify(repo => repo.GetNotImportedRegOre(), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppRegOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.GetNotImportedRegOre()).Returns(regOreList);

            // Act
            var result = _regOreService.GetNotImportedAppRegOre();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.GetNotImportedRegOre(), Times.Once);
        }

        [Fact]
        public void UpdateRegOreImported_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.UpdateRegOreImported(It.IsAny<WorkerIdSyncFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<WorkerIdSyncFilter>(It.IsAny<WorkerIdSyncRequestDto>())).Returns(new WorkerIdSyncFilter());

            // Act
            var result = _regOreService.UpdateRegOreImported(_sampleWorkerIdSyncRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var firstResult = result.First();
            Assert.Equal(_sampleRegOre.RegOreId, firstResult.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, firstResult.WorkerId);
            _regOrerRepositoryMock.Verify(repo => repo.UpdateRegOreImported(It.IsAny<WorkerIdSyncFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<WorkerIdSyncFilter>(It.IsAny<WorkerIdSyncRequestDto>()), Times.Once);
        }

        [Fact]
        public void UpdateRegOreImported_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.UpdateRegOreImported(It.IsAny<WorkerIdSyncFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<WorkerIdSyncFilter>(It.IsAny<WorkerIdSyncRequestDto>())).Returns(new WorkerIdSyncFilter());

            // Act
            var result = _regOreService.UpdateRegOreImported(_sampleWorkerIdSyncRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.UpdateRegOreImported(It.IsAny<WorkerIdSyncFilter>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppRegOreByFilter_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.GetNotImportedAppRegOreByFilter(It.IsAny<ViewOreRequestFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<ViewOreRequestFilter>(It.IsAny<ViewOreRequestDto>())).Returns(new ViewOreRequestFilter());

            // Act
            var result = _regOreService.GetNotImportedAppRegOreByFilter(_sampleViewOreRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var firstResult = result.First();
            Assert.Equal(_sampleRegOre.RegOreId, firstResult.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, firstResult.WorkerId);
            _regOrerRepositoryMock.Verify(repo => repo.GetNotImportedAppRegOreByFilter(It.IsAny<ViewOreRequestFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ViewOreRequestFilter>(It.IsAny<ViewOreRequestDto>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppRegOreByFilter_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.GetNotImportedAppRegOreByFilter(It.IsAny<ViewOreRequestFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<ViewOreRequestFilter>(It.IsAny<ViewOreRequestDto>())).Returns(new ViewOreRequestFilter());

            // Act
            var result = _regOreService.GetNotImportedAppRegOreByFilter(_sampleViewOreRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.GetNotImportedAppRegOreByFilter(It.IsAny<ViewOreRequestFilter>()), Times.Once);
        }

        [Fact]
        public void UpdateImportedById_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.UpdateImportedById(It.IsAny<UpdateImportedIdFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<UpdateImportedIdFilter>(It.IsAny<UpdateImportedIdRequestDto>())).Returns(new UpdateImportedIdFilter());

            // Act
            var result = _regOreService.UpdateImportedById(_sampleUpdateImportedIdRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var firstResult = result.First();
            Assert.Equal(_sampleRegOre.RegOreId, firstResult.RegOreId);
            Assert.Equal(_sampleRegOre.WorkerId, firstResult.WorkerId);
            _regOrerRepositoryMock.Verify(repo => repo.UpdateImportedById(It.IsAny<UpdateImportedIdFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<UpdateImportedIdFilter>(It.IsAny<UpdateImportedIdRequestDto>()), Times.Once);
        }

        [Fact]
        public void UpdateImportedById_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.UpdateImportedById(It.IsAny<UpdateImportedIdFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<UpdateImportedIdFilter>(It.IsAny<UpdateImportedIdRequestDto>())).Returns(new UpdateImportedIdFilter());

            // Act
            var result = _regOreService.UpdateImportedById(_sampleUpdateImportedIdRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.UpdateImportedById(It.IsAny<UpdateImportedIdFilter>()), Times.Once);
        }

        // Test per validare comportamenti di errore
        [Fact]
        public void PostAppRegOre_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            var regOreRequestList = new List<RegOreRequestDto> { _sampleRegOreRequest };
            _regOrerRepositoryMock.Setup(repo => repo.PostRegOreList(It.IsAny<List<RegOreFilter>>()))
                .Throws(new Exception("Repository error"));
            _mapperMock.Setup(m => m.Map<RegOreFilter>(It.IsAny<RegOreRequestDto>())).Returns(new RegOreFilter());

            // Act & Assert
            Assert.Throws<Exception>(() => _regOreService.PostAppRegOre(regOreRequestList));
        }

        [Fact]
        public void PutAppViewOre_ThrowsException_WhenMapperThrows()
        {
            // Arrange
            _mapperMock.Setup(m => m.Map<ViewOrePutFilter>(It.IsAny<ViewOrePutRequestDto>()))
                .Throws(new ArgumentException("Mapping error"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _regOreService.PutAppViewOre(_sampleViewOrePutRequest));
        }
    }
}