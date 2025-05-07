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
            Assert.Equal(_samplePrelMatDto.PrelMatId, result.First().PrelMatId);
            Assert.Equal(_samplePrelMatDto.Job, result.First().Job);
            Assert.Equal(_samplePrelMatDto.RtgStep, result.First().RtgStep);
            Assert.Equal(_samplePrelMatDto.Alternate, result.First().Alternate);
            Assert.Equal(_samplePrelMatDto.AltRtgStep, result.First().AltRtgStep);
            Assert.Equal(_samplePrelMatDto.Operation, result.First().Operation);
            Assert.Equal(_samplePrelMatDto.OperDesc, result.First().OperDesc);
            Assert.Equal(_samplePrelMatDto.Position, result.First().Position);
            Assert.Equal(_samplePrelMatDto.Component, result.First().Component);
            Assert.Equal(_samplePrelMatDto.Bom, result.First().Bom);
            Assert.Equal(_samplePrelMatDto.Variant, result.First().Variant);
            Assert.Equal(_samplePrelMatDto.ItemDesc, result.First().ItemDesc);
            Assert.Equal(_samplePrelMatDto.Moid, result.First().Moid);
            Assert.Equal(_samplePrelMatDto.Mono, result.First().Mono);
            Assert.Equal(_samplePrelMatDto.CreationDate, result.First().CreationDate);
            Assert.Equal(_samplePrelMatDto.UoM, result.First().UoM);
            Assert.Equal(_samplePrelMatDto.ProductionQty, result.First().ProductionQty);
            Assert.Equal(_samplePrelMatDto.ProducedQty, result.First().ProducedQty);
            Assert.Equal(_samplePrelMatDto.ResQty, result.First().ResQty);
            Assert.Equal(_samplePrelMatDto.Storage, result.First().Storage);
            Assert.Equal(_samplePrelMatDto.BarCode, result.First().BarCode);
            Assert.Equal(_samplePrelMatDto.Wc, result.First().Wc);
            Assert.Equal(_samplePrelMatDto.PrelQty, result.First().PrelQty);
            Assert.Equal(_samplePrelMatDto.Imported, result.First().Imported);
            Assert.Equal(_samplePrelMatDto.UserImp, result.First().UserImp);
            Assert.Equal(_samplePrelMatDto.DataImp, result.First().DataImp);
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

        [Fact]
        public void PostPrelMatList_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var requestList = new List<PrelMatRequestDto> { _samplePrelMatRequestDto };
            var filterList = new List<PrelMatFilter> { new PrelMatFilter() };

            _prelMatRepositoryMock.Setup(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<List<PrelMatFilter>>(It.IsAny<List<PrelMatRequestDto>>())).Returns(filterList);

            // Act
            var result = _prelMatService.PostPrelMatList(requestList);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_samplePrelMatDto.PrelMatId, result.First().PrelMatId);
            Assert.Equal(_samplePrelMatDto.Job, result.First().Job);
            Assert.Equal(_samplePrelMatDto.RtgStep, result.First().RtgStep);
            Assert.Equal(_samplePrelMatDto.Alternate, result.First().Alternate);
            Assert.Equal(_samplePrelMatDto.AltRtgStep, result.First().AltRtgStep);
            Assert.Equal(_samplePrelMatDto.Operation, result.First().Operation);
            Assert.Equal(_samplePrelMatDto.OperDesc, result.First().OperDesc);
            Assert.Equal(_samplePrelMatDto.Position, result.First().Position);
            Assert.Equal(_samplePrelMatDto.Component, result.First().Component);
            Assert.Equal(_samplePrelMatDto.Bom, result.First().Bom);
            Assert.Equal(_samplePrelMatDto.Variant, result.First().Variant);
            Assert.Equal(_samplePrelMatDto.ItemDesc, result.First().ItemDesc);
            Assert.Equal(_samplePrelMatDto.Moid, result.First().Moid);
            Assert.Equal(_samplePrelMatDto.Mono, result.First().Mono);
            Assert.Equal(_samplePrelMatDto.CreationDate, result.First().CreationDate);
            Assert.Equal(_samplePrelMatDto.UoM, result.First().UoM);
            Assert.Equal(_samplePrelMatDto.ProductionQty, result.First().ProductionQty);
            Assert.Equal(_samplePrelMatDto.ProducedQty, result.First().ProducedQty);
            Assert.Equal(_samplePrelMatDto.ResQty, result.First().ResQty);
            Assert.Equal(_samplePrelMatDto.Storage, result.First().Storage);
            Assert.Equal(_samplePrelMatDto.BarCode, result.First().BarCode);
            Assert.Equal(_samplePrelMatDto.Wc, result.First().Wc);
            Assert.Equal(_samplePrelMatDto.PrelQty, result.First().PrelQty);
            Assert.Equal(_samplePrelMatDto.Imported, result.First().Imported);
            Assert.Equal(_samplePrelMatDto.UserImp, result.First().UserImp);
            Assert.Equal(_samplePrelMatDto.DataImp, result.First().DataImp);
            _prelMatRepositoryMock.Verify(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>()), Times.Once);
        }

        [Fact]
        public void PostPrelMatList_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var requestList = new List<PrelMatRequestDto> { _samplePrelMatRequestDto };
            var filterList = new List<PrelMatFilter> { new PrelMatFilter() };

            _prelMatRepositoryMock.Setup(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<List<PrelMatFilter>>(It.IsAny<List<PrelMatRequestDto>>())).Returns(filterList);

            // Act
            var result = _prelMatService.PostPrelMatList(requestList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.PostPrelMatList(It.IsAny<List<PrelMatFilter>>()), Times.Once);
        }

        [Fact]
        public void GetViewPrelMatList_ReturnsListOfPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var filter = new ViewPrelMatRequestFilter();
            var request = new ViewPrelMatRequestDto();

            _prelMatRepositoryMock.Setup(repo => repo.GetViewPrelMat(It.IsAny<ViewPrelMatRequestFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ViewPrelMatRequestFilter>(It.IsAny<ViewPrelMatRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetViewPrelMatList(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_samplePrelMatDto.PrelMatId, result.First().PrelMatId);
            Assert.Equal(_samplePrelMatDto.Job, result.First().Job);
            Assert.Equal(_samplePrelMatDto.RtgStep, result.First().RtgStep);
            Assert.Equal(_samplePrelMatDto.Alternate, result.First().Alternate);
            Assert.Equal(_samplePrelMatDto.AltRtgStep, result.First().AltRtgStep);
            Assert.Equal(_samplePrelMatDto.Operation, result.First().Operation);
            Assert.Equal(_samplePrelMatDto.OperDesc, result.First().OperDesc);
            Assert.Equal(_samplePrelMatDto.Position, result.First().Position);
            Assert.Equal(_samplePrelMatDto.Component, result.First().Component);
            Assert.Equal(_samplePrelMatDto.Bom, result.First().Bom);
            Assert.Equal(_samplePrelMatDto.Variant, result.First().Variant);
            Assert.Equal(_samplePrelMatDto.ItemDesc, result.First().ItemDesc);
            Assert.Equal(_samplePrelMatDto.Moid, result.First().Moid);
            Assert.Equal(_samplePrelMatDto.Mono, result.First().Mono);
            Assert.Equal(_samplePrelMatDto.CreationDate, result.First().CreationDate);
            Assert.Equal(_samplePrelMatDto.UoM, result.First().UoM);
            Assert.Equal(_samplePrelMatDto.ProductionQty, result.First().ProductionQty);
            Assert.Equal(_samplePrelMatDto.ProducedQty, result.First().ProducedQty);
            Assert.Equal(_samplePrelMatDto.ResQty, result.First().ResQty);
            Assert.Equal(_samplePrelMatDto.Storage, result.First().Storage);
            Assert.Equal(_samplePrelMatDto.BarCode, result.First().BarCode);
            Assert.Equal(_samplePrelMatDto.Wc, result.First().Wc);
            Assert.Equal(_samplePrelMatDto.PrelQty, result.First().PrelQty);
            Assert.Equal(_samplePrelMatDto.Imported, result.First().Imported);
            Assert.Equal(_samplePrelMatDto.UserImp, result.First().UserImp);
            Assert.Equal(_samplePrelMatDto.DataImp, result.First().DataImp);
            _prelMatRepositoryMock.Verify(repo => repo.GetViewPrelMat(It.IsAny<ViewPrelMatRequestFilter>()), Times.Once);
        }

        [Fact]
        public void GetViewPrelMatList_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var filter = new ViewPrelMatRequestFilter();
            var request = new ViewPrelMatRequestDto();

            _prelMatRepositoryMock.Setup(repo => repo.GetViewPrelMat(It.IsAny<ViewPrelMatRequestFilter>())).Returns(sampleList);
            _mapperMock.Setup(m => m.Map<ViewPrelMatRequestFilter>(It.IsAny<ViewPrelMatRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.GetViewPrelMatList(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _prelMatRepositoryMock.Verify(repo => repo.GetViewPrelMat(It.IsAny<ViewPrelMatRequestFilter>()), Times.Once);
        }

        [Fact]
        public void PutViewPrelMat_ReturnsPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var filter = new ViewPrelMatPutFilter();
            var request = new ViewPrelMatPutRequestDto();

            _prelMatRepositoryMock.Setup(repo => repo.PutViewPrelMat(It.IsAny<ViewPrelMatPutFilter>())).Returns(sampleList.First());
            _mapperMock.Setup(m => m.Map<ViewPrelMatPutFilter>(It.IsAny<ViewPrelMatPutRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.PutViewPrelMat(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_samplePrelMatDto.PrelMatId, result.PrelMatId);
            Assert.Equal(_samplePrelMatDto.Job, result.Job);
            Assert.Equal(_samplePrelMatDto.RtgStep, result.RtgStep);
            Assert.Equal(_samplePrelMatDto.Alternate, result.Alternate);
            Assert.Equal(_samplePrelMatDto.AltRtgStep, result.AltRtgStep);
            Assert.Equal(_samplePrelMatDto.Operation, result.Operation);
            Assert.Equal(_samplePrelMatDto.OperDesc, result.OperDesc);
            Assert.Equal(_samplePrelMatDto.Position, result.Position);
            Assert.Equal(_samplePrelMatDto.Component, result.Component);
            Assert.Equal(_samplePrelMatDto.Bom, result.Bom);
            Assert.Equal(_samplePrelMatDto.Variant, result.Variant);
            Assert.Equal(_samplePrelMatDto.ItemDesc, result.ItemDesc);
            Assert.Equal(_samplePrelMatDto.Moid, result.Moid);
            Assert.Equal(_samplePrelMatDto.Mono, result.Mono);
            Assert.Equal(_samplePrelMatDto.CreationDate, result.CreationDate);
            Assert.Equal(_samplePrelMatDto.UoM, result.UoM);
            Assert.Equal(_samplePrelMatDto.ProductionQty, result.ProductionQty);
            Assert.Equal(_samplePrelMatDto.ProducedQty, result.ProducedQty);
            Assert.Equal(_samplePrelMatDto.ResQty, result.ResQty);
            Assert.Equal(_samplePrelMatDto.Storage, result.Storage);
            Assert.Equal(_samplePrelMatDto.BarCode, result.BarCode);
            Assert.Equal(_samplePrelMatDto.Wc, result.Wc);
            Assert.Equal(_samplePrelMatDto.PrelQty, result.PrelQty);
            Assert.Equal(_samplePrelMatDto.Imported, result.Imported);
            Assert.Equal(_samplePrelMatDto.UserImp, result.UserImp);
            Assert.Equal(_samplePrelMatDto.DataImp, result.DataImp);
            _prelMatRepositoryMock.Verify(repo => repo.PutViewPrelMat(It.IsAny<ViewPrelMatPutFilter>()), Times.Once);
        }

        [Fact]
        public void PutViewPrelMat_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var filter = new ViewPrelMatPutFilter();
            var request = new ViewPrelMatPutRequestDto();

            _prelMatRepositoryMock.Setup(repo => repo.PutViewPrelMat(It.IsAny<ViewPrelMatPutFilter>())).Returns((A3AppPrelMat)null);
            _mapperMock.Setup(m => m.Map<ViewPrelMatPutFilter>(It.IsAny<ViewPrelMatPutRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.PutViewPrelMat(request);

            // Assert
            Assert.Null(result);
            _prelMatRepositoryMock.Verify(repo => repo.PutViewPrelMat(It.IsAny<ViewPrelMatPutFilter>()), Times.Once);
        }

        [Fact]
        public void DeletePrelMatId_ReturnsPrelMatDto_WhenDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat> { _sampleVwApiPrelMat };
            var filter = new ViewPrelMatDeleteFilter();
            var request = new ViewPrelMatDeleteRequestDto();

            _prelMatRepositoryMock.Setup(repo => repo.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteFilter>())).Returns(sampleList.First());
            _mapperMock.Setup(m => m.Map<ViewPrelMatDeleteFilter>(It.IsAny<ViewPrelMatDeleteRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.DeletePrelMatId(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_samplePrelMatDto.PrelMatId, result.PrelMatId);
            Assert.Equal(_samplePrelMatDto.Job, result.Job);
            Assert.Equal(_samplePrelMatDto.RtgStep, result.RtgStep);
            Assert.Equal(_samplePrelMatDto.Alternate, result.Alternate);
            Assert.Equal(_samplePrelMatDto.AltRtgStep, result.AltRtgStep);
            Assert.Equal(_samplePrelMatDto.Operation, result.Operation);
            Assert.Equal(_samplePrelMatDto.OperDesc, result.OperDesc);
            Assert.Equal(_samplePrelMatDto.Position, result.Position);
            Assert.Equal(_samplePrelMatDto.Component, result.Component);
            Assert.Equal(_samplePrelMatDto.Bom, result.Bom);
            Assert.Equal(_samplePrelMatDto.Variant, result.Variant);
            Assert.Equal(_samplePrelMatDto.ItemDesc, result.ItemDesc);
            Assert.Equal(_samplePrelMatDto.Moid, result.Moid);
            Assert.Equal(_samplePrelMatDto.Mono, result.Mono);
            Assert.Equal(_samplePrelMatDto.CreationDate, result.CreationDate);
            Assert.Equal(_samplePrelMatDto.UoM, result.UoM);
            Assert.Equal(_samplePrelMatDto.ProductionQty, result.ProductionQty);
            Assert.Equal(_samplePrelMatDto.ProducedQty, result.ProducedQty);
            Assert.Equal(_samplePrelMatDto.ResQty, result.ResQty);
            Assert.Equal(_samplePrelMatDto.Storage, result.Storage);
            Assert.Equal(_samplePrelMatDto.BarCode, result.BarCode);
            Assert.Equal(_samplePrelMatDto.Wc, result.Wc);
            Assert.Equal(_samplePrelMatDto.PrelQty, result.PrelQty);
            Assert.Equal(_samplePrelMatDto.Imported, result.Imported);
            Assert.Equal(_samplePrelMatDto.UserImp, result.UserImp);
            Assert.Equal(_samplePrelMatDto.DataImp, result.DataImp);
            _prelMatRepositoryMock.Verify(repo => repo.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteFilter>()), Times.Once);  
        }

        [Fact]
        public void DeletePrelMatId_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var sampleList = new List<A3AppPrelMat>();
            var filter = new ViewPrelMatDeleteFilter();
            var request = new ViewPrelMatDeleteRequestDto();

            _prelMatRepositoryMock.Setup(repo => repo.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteFilter>())).Returns((A3AppPrelMat)null);
            _mapperMock.Setup(m => m.Map<ViewPrelMatDeleteFilter>(It.IsAny<ViewPrelMatDeleteRequestDto>())).Returns(filter);

            // Act
            var result = _prelMatService.DeletePrelMatId(request);

            // Assert
            Assert.Null(result);
            _prelMatRepositoryMock.Verify(repo => repo.DeletePrelMatId(It.IsAny<ViewPrelMatDeleteFilter>()), Times.Once);
        }
    }
}