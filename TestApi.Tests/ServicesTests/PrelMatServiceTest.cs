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
    public class PrelMatServiceTest
    {
        private readonly Mock<IPrelMatRepository> _prelMatRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IPrelMatRequestService _prelMatService;

        private readonly A3AppPrelMat _sampleVwApiPrelMat = new A3AppPrelMat
        {
            PrelMatId = 1,
            WorkerId = 1,
            SavedDate = new DateTime(2022, 1, 1),
            Job = "Job1",
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 1,
            Operation = "Op1",
            OperDesc = "Operation Description",
            Position = 1,
            Component = "Component1",
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
            BarCode = "BarCode1",
            Wc = "WC1",
            PrelQty = 10,
            Imported = false,
            UserImp = "User1",
            DataImp = new DateTime(2023, 1, 1)
        };

        private readonly PrelMatDto _samplePrelMatDto = new PrelMatDto
        {
            PrelMatId = 1,
            WorkerId = 1,
            SavedDate = new DateTime(2022, 1, 1),
            Job = "Job1",
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 1,
            Operation = "Op1",
            OperDesc = "Operation Description",
            Position = 1,
            Component = "Component1",
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
            BarCode = "BarCode1",
            Wc = "WC1",
            PrelQty = 10,
            Imported = false,
            UserImp = "User1",
            DataImp = new DateTime(2023, 1, 1)
        };

        private readonly PrelMatRequestDto _samplePrelMatRequestDto = new PrelMatRequestDto
        {
            WorkerId = 1,
            Job = "Job1",
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 1,
            Operation = "Op1",
            OperDesc = "Operation Description",
            Position = 1,
            Component = "Component1",
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
            BarCode = "BarCode1",
            Wc = "WC1",
            PrelQty = 10,
            Imported = false,
            UserImp = "User1",
            DataImp = new DateTime(2023, 1, 1)
        };

        private readonly ViewPrelMatRequestDto _sampleViewPrelMatRequestDto = new ViewPrelMatRequestDto
        {
            WorkerId = 1,
            FromDateTime = new DateTime(2022, 1, 1),
            ToDateTime = new DateTime(2023, 1, 2),
            Job = "Job1",
            Operation = "Op1",
            Mono = "Mono1",
            Component = "Component1",
            BarCode = "BarCode1"
        };

        private readonly ViewPrelMatPutRequestDto _sampleViewPrelMatPutRequestDto = new ViewPrelMatPutRequestDto
        {
            PrelMatId = 1,
            PrelQty = 10,
        };

        public PrelMatServiceTest()
        {
            _prelMatRepositoryMock = new Mock<IPrelMatRepository>();
            _mapperMock = new Mock<IMapper>();
            _prelMatService = new PrelMatRequestService(_prelMatRepositoryMock.Object, _mapperMock.Object);
        }

        #region GetAppPrelMat Tests

        [Fact]
        public void GetAppPrelMat_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            _prelMatRepositoryMock.Setup(repo => repo.GetAppPrelMat()).Returns(sampleList);

            // Act
            var result = _prelMatService.GetAppPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.GetAppPrelMat(), Times.Once);
        }

        [Fact]
        public void GetAppPrelMat_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            _prelMatRepositoryMock.Setup(repo => repo.GetAppPrelMat()).Returns(sampleList);

            // Act
            var result = _prelMatService.GetAppPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.GetAppPrelMat(), Times.Once);
        }

        #endregion

        #region PostPrelMatList Tests

        [Fact]
        public void PostPrelMatList_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var requestList = new List<PrelMatRequestDto> { _samplePrelMatRequestDto };
            var filterList = new List<PrelMatFilter> { new PrelMatFilter() };

            _prelMatRepositoryMock.Setup(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<PrelMatFilter>(It.IsAny<PrelMatRequestDto>())).Returns(new PrelMatFilter());

            // Act
            var result = _prelMatService.PostPrelMatList(requestList);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>()), Times.Once);
            _mapperMock.Verify(m => m.Map<PrelMatFilter>(It.IsAny<PrelMatRequestDto>()), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var requestList = new List<PrelMatRequestDto> { _samplePrelMatRequestDto };

            _prelMatRepositoryMock.Setup(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<PrelMatFilter>(It.IsAny<PrelMatRequestDto>())).Returns(new PrelMatFilter());

            // Act
            var result = _prelMatService.PostPrelMatList(requestList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>()), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ReturnsEmptyList_WhenRequestListIsEmpty()
        {
            // Arrange
            var requestList = new List<PrelMatRequestDto>();
            var sampleList = new List<A3AppPrelMat>();

            _prelMatRepositoryMock.Setup(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>())).Returns(sampleList);

            // Act
            var result = _prelMatService.PostPrelMatList(requestList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>()), Times.Once);
        }

        #endregion

        #region GetViewPrelMatList Tests

        [Fact]
        public void GetViewPrelMatList_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var filter = new ViewPrelMatRequestFilter();
            var request = _sampleViewPrelMatRequestDto;

            _prelMatRepositoryMock.Setup(repo => repo.GetViewPrelMat(It.IsAny<ViewPrelMatRequestFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ViewPrelMatRequestFilter>(It.IsAny<ViewPrelMatRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetViewPrelMatList(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.GetViewPrelMat(It.IsAny<ViewPrelMatRequestFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ViewPrelMatRequestFilter>(It.IsAny<ViewPrelMatRequestDto>()), Times.Once);
        }

        [Fact]
        public void GetViewPrelMatList_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var filter = new ViewPrelMatRequestFilter();
            var request = _sampleViewPrelMatRequestDto;

            _prelMatRepositoryMock.Setup(repo => repo.GetViewPrelMat(It.IsAny<ViewPrelMatRequestFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ViewPrelMatRequestFilter>(It.IsAny<ViewPrelMatRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetViewPrelMatList(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.GetViewPrelMat(It.IsAny<ViewPrelMatRequestFilter>()), Times.Once);
        }

        #endregion

        #region PutViewPrelMat Tests

        [Fact]
        public void PutViewPrelMat_ReturnsPrelMatDto_WhenDataExists()
        {
            // Arrange
            var filter = new ViewPrelMatPutFilter();
            var request = _sampleViewPrelMatPutRequestDto;

            _prelMatRepositoryMock.Setup(repo => repo.PutViewPrelMat(It.IsAny<ViewPrelMatPutFilter>())).Returns(_sampleVwApiPrelMat);
            _mapperMock.Setup(m => m.Map<ViewPrelMatPutFilter>(It.IsAny<ViewPrelMatPutRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.PutViewPrelMat(request);

            // Assert
            Assert.NotNull(result);
            AssertPrelMatDto(result, _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.PutViewPrelMat(It.IsAny<ViewPrelMatPutFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ViewPrelMatPutFilter>(It.IsAny<ViewPrelMatPutRequestDto>()), Times.Once);
        }

        #endregion

        #region DeletePrelMatId Tests

        [Fact]
        public void DeletePrelMatId_ReturnsPrelMatDto_WhenDataExists()
        {
            // Arrange
            var filter = new ViewPrelMatDeleteFilter();
            var request = new ViewPrelMatDeleteRequestDto();

            _prelMatRepositoryMock.Setup(repo => repo.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteFilter>())).Returns(_sampleVwApiPrelMat);
            _mapperMock.Setup(m => m.Map<ViewPrelMatDeleteFilter>(It.IsAny<ViewPrelMatDeleteRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.DeletePrelMatId(request);

            // Assert
            Assert.NotNull(result);
            AssertPrelMatDto(result, _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ViewPrelMatDeleteFilter>(It.IsAny<ViewPrelMatDeleteRequestDto>()), Times.Once);
        }

        #endregion

        #region GetPrelMatWithComponent Tests

        [Fact]
        public void GetPrelMatWithComponent_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var request = new ComponentRequestDto();
            var filter = new ComponentFilter();

            _prelMatRepositoryMock.Setup(repo => repo.GetPrelMatWithComponent(It.IsAny<ComponentFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ComponentFilter>(It.IsAny<ComponentRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetPrelMatWithComponent(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.GetPrelMatWithComponent(It.IsAny<ComponentFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ComponentFilter>(It.IsAny<ComponentRequestDto>()), Times.Once);
        }

        [Fact]
        public void GetPrelMatWithComponent_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var request = new ComponentRequestDto();
            var filter = new ComponentFilter();

            _prelMatRepositoryMock.Setup(repo => repo.GetPrelMatWithComponent(It.IsAny<ComponentFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ComponentFilter>(It.IsAny<ComponentRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetPrelMatWithComponent(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.GetPrelMatWithComponent(It.IsAny<ComponentFilter>()), Times.Once);
        }

        [Fact]
        public void GetPrelMatWithComponent_WithNullRequest_ReturnsListOfPrelMatDto()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var filter = new ComponentFilter();

            _prelMatRepositoryMock.Setup(repo => repo.GetPrelMatWithComponent(It.IsAny<ComponentFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ComponentFilter>(It.IsAny<ComponentRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetPrelMatWithComponent(null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.GetPrelMatWithComponent(It.IsAny<ComponentFilter>()), Times.Once);
        }

        #endregion

        #region GetNotImportedPrelMat Tests

        [Fact]
        public void GetNotImportedPrelMat_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            _prelMatRepositoryMock.Setup(repo => repo.GetNotImportedPrelMat()).Returns(sampleList);

            // Act
            var result = _prelMatService.GetNotImportedPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.GetNotImportedPrelMat(), Times.Once);
        }

        [Fact]
        public void GetNotImportedPrelMat_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            _prelMatRepositoryMock.Setup(repo => repo.GetNotImportedPrelMat()).Returns(sampleList);

            // Act
            var result = _prelMatService.GetNotImportedPrelMat();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.GetNotImportedPrelMat(), Times.Once);
        }

        #endregion

        #region UpdatePrelMatImported Tests

        [Fact]
        public void UpdatePrelMatImported_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var request = new WorkerIdSyncRequestDto();
            var filter = new WorkerIdSyncFilter();

            _prelMatRepositoryMock.Setup(repo => repo.UpdatePrelMatImported(It.IsAny<WorkerIdSyncFilter>(), It.IsAny<bool>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<WorkerIdSyncFilter>(It.IsAny<WorkerIdSyncRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.UpdatePrelMatImported(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.UpdatePrelMatImported(It.IsAny<WorkerIdSyncFilter>(), It.IsAny<bool>()), Times.Once);
            _mapperMock.Verify(m => m.Map<WorkerIdSyncFilter>(It.IsAny<WorkerIdSyncRequestDto>()), Times.Once);
        }

        [Fact]
        public void UpdatePrelMatImported_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var request = new WorkerIdSyncRequestDto();
            var filter = new WorkerIdSyncFilter();

            _prelMatRepositoryMock.Setup(repo => repo.UpdatePrelMatImported(It.IsAny<WorkerIdSyncFilter>(), It.IsAny<bool>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<WorkerIdSyncFilter>(It.IsAny<WorkerIdSyncRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.UpdatePrelMatImported(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.UpdatePrelMatImported(It.IsAny<WorkerIdSyncFilter>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetNotImportedAppPrelMatByFilter Tests

        [Fact]
        public void GetNotImportedAppPrelMatByFilter_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var request = _sampleViewPrelMatRequestDto;
            var filter = new ViewPrelMatRequestFilter();

            _prelMatRepositoryMock.Setup(repo => repo.GetNotImportedAppPrelMatByFilter(It.IsAny<ViewPrelMatRequestFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ViewPrelMatRequestFilter>(It.IsAny<ViewPrelMatRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetNotImportedAppPrelMatByFilter(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.GetNotImportedAppPrelMatByFilter(It.IsAny<ViewPrelMatRequestFilter>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ViewPrelMatRequestFilter>(It.IsAny<ViewPrelMatRequestDto>()), Times.Once);
        }

        [Fact]
        public void GetNotImportedAppPrelMatByFilter_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var request = _sampleViewPrelMatRequestDto;
            var filter = new ViewPrelMatRequestFilter();

            _prelMatRepositoryMock.Setup(repo => repo.GetNotImportedAppPrelMatByFilter(It.IsAny<ViewPrelMatRequestFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ViewPrelMatRequestFilter>(It.IsAny<ViewPrelMatRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetNotImportedAppPrelMatByFilter(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.GetNotImportedAppPrelMatByFilter(It.IsAny<ViewPrelMatRequestFilter>()), Times.Once);
        }

        #endregion

        #region UpdateImportedById Tests

        [Fact]
        public void UpdateImportedById_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var request = new UpdateImportedIdRequestDto();
            var filter = new UpdateImportedIdFilter();

            _prelMatRepositoryMock.Setup(repo => repo.UpdateImportedById(It.IsAny<UpdateImportedIdFilter>(), It.IsAny<bool>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<UpdateImportedIdFilter>(It.IsAny<UpdateImportedIdRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.UpdateImportedById(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            AssertPrelMatDto(result.First(), _samplePrelMatDto);
            _prelMatRepositoryMock.Verify(repo => repo.UpdateImportedById(It.IsAny<UpdateImportedIdFilter>(), It.IsAny<bool>()), Times.Once);
            _mapperMock.Verify(m => m.Map<UpdateImportedIdFilter>(It.IsAny<UpdateImportedIdRequestDto>()), Times.Once);
        }

        [Fact]
        public void UpdateImportedById_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var request = new UpdateImportedIdRequestDto();
            var filter = new UpdateImportedIdFilter();

            _prelMatRepositoryMock.Setup(repo => repo.UpdateImportedById(It.IsAny<UpdateImportedIdFilter>(), It.IsAny<bool>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<UpdateImportedIdFilter>(It.IsAny<UpdateImportedIdRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.UpdateImportedById(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.UpdateImportedById(It.IsAny<UpdateImportedIdFilter>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region Helper Methods

        private void AssertPrelMatDto(PrelMatDto actual, PrelMatDto expected)
        {
            Assert.Equal(expected.PrelMatId, actual.PrelMatId);
            Assert.Equal(expected.WorkerId, actual.WorkerId);
            Assert.Equal(expected.SavedDate, actual.SavedDate);
            Assert.Equal(expected.Job, actual.Job);
            Assert.Equal(expected.RtgStep, actual.RtgStep);
            Assert.Equal(expected.Alternate, actual.Alternate);
            Assert.Equal(expected.AltRtgStep, actual.AltRtgStep);
            Assert.Equal(expected.Operation, actual.Operation);
            Assert.Equal(expected.OperDesc, actual.OperDesc);
            Assert.Equal(expected.Position, actual.Position);
            Assert.Equal(expected.Component, actual.Component);
            Assert.Equal(expected.Bom, actual.Bom);
            Assert.Equal(expected.Variant, actual.Variant);
            Assert.Equal(expected.ItemDesc, actual.ItemDesc);
            Assert.Equal(expected.Moid, actual.Moid);
            Assert.Equal(expected.Mono, actual.Mono);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.UoM, actual.UoM);
            Assert.Equal(expected.ProductionQty, actual.ProductionQty);
            Assert.Equal(expected.ProducedQty, actual.ProducedQty);
            Assert.Equal(expected.ResQty, actual.ResQty);
            Assert.Equal(expected.Storage, actual.Storage);
            Assert.Equal(expected.BarCode, actual.BarCode);
            Assert.Equal(expected.Wc, actual.Wc);
            Assert.Equal(expected.PrelQty, actual.PrelQty);
            Assert.Equal(expected.Imported, actual.Imported);
            Assert.Equal(expected.UserImp, actual.UserImp);
            Assert.Equal(expected.DataImp, actual.DataImp);
        }

        #endregion
    }
}