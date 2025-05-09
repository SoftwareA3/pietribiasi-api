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
    public class RegOreServiceTest
    {
        private readonly Mock<IRegOreRepository> _regOrerRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IRegOreRequestService _regOreServiceMock;

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

        public RegOreServiceTest()
        {
            _regOrerRepositoryMock = new Mock<IRegOreRepository>();
            _mapperMock = new Mock<IMapper>();
            _regOreServiceMock = new RegOreRequestService(_regOrerRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetAppRegOre_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.GetAppRegOre()).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<RegOreDto>(It.IsAny<A3AppRegOre>())).Returns(new RegOreDto());

            // Act
            var result = _regOreServiceMock.GetAppRegOre();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(result.First().RegOreId, _sampleRegOre.RegOreId);
            Assert.Equal(result.First().WorkerId, _sampleRegOre.WorkerId);
            Assert.Equal(result.First().Job, _sampleRegOre.Job);
            Assert.Equal(result.First().RtgStep, _sampleRegOre.RtgStep);
            Assert.Equal(result.First().Alternate, _sampleRegOre.Alternate);
            Assert.Equal(result.First().AltRtgStep, _sampleRegOre.AltRtgStep);
            Assert.Equal(result.First().Operation, _sampleRegOre.Operation);
            Assert.Equal(result.First().OperDesc, _sampleRegOre.OperDesc);
            Assert.Equal(result.First().Bom, _sampleRegOre.Bom);
            Assert.Equal(result.First().Variant, _sampleRegOre.Variant);
            Assert.Equal(result.First().ItemDesc, _sampleRegOre.ItemDesc);
            Assert.Equal(result.First().Moid, _sampleRegOre.Moid);
            Assert.Equal(result.First().Mono, _sampleRegOre.Mono);
            Assert.Equal(result.First().CreationDate, _sampleRegOre.CreationDate);
            Assert.Equal(result.First().Uom, _sampleRegOre.Uom);
            Assert.Equal(result.First().ProductionQty, _sampleRegOre.ProductionQty);
            Assert.Equal(result.First().ProducedQty, _sampleRegOre.ProducedQty);
            Assert.Equal(result.First().ResQty, _sampleRegOre.ResQty);
            Assert.Equal(result.First().Storage, _sampleRegOre.Storage);
            Assert.Equal(result.First().Wc, _sampleRegOre.Wc);
            Assert.Equal(result.First().WorkingTime, _sampleRegOre.WorkingTime);
            Assert.Equal(result.First().Imported, _sampleRegOre.Imported);
            Assert.Equal(result.First().UserImp, _sampleRegOre.UserImp);
            Assert.Equal(result.First().DataImp, _sampleRegOre.DataImp);
            _regOrerRepositoryMock.Verify(repo => repo.GetAppRegOre(), Times.Once);
        }

        [Fact]
        public void GetAppRegOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.GetAppRegOre()).Returns(regOreList);

            // Act
            var result = _regOreServiceMock.GetAppRegOre();

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
            var result = _regOreServiceMock.PostAppRegOre(regOreRequestList);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(result.First().RegOreId, _sampleRegOre.RegOreId);
            Assert.Equal(result.First().WorkerId, _sampleRegOre.WorkerId);
            Assert.Equal(result.First().Job, _sampleRegOre.Job);
            Assert.Equal(result.First().RtgStep, _sampleRegOre.RtgStep);
            Assert.Equal(result.First().Alternate, _sampleRegOre.Alternate);
            Assert.Equal(result.First().AltRtgStep, _sampleRegOre.AltRtgStep);
            Assert.Equal(result.First().Operation, _sampleRegOre.Operation);
            Assert.Equal(result.First().OperDesc, _sampleRegOre.OperDesc);
            Assert.Equal(result.First().Bom, _sampleRegOre.Bom);
            Assert.Equal(result.First().Variant, _sampleRegOre.Variant);
            Assert.Equal(result.First().ItemDesc, _sampleRegOre.ItemDesc);
            Assert.Equal(result.First().Moid, _sampleRegOre.Moid);
            Assert.Equal(result.First().Mono, _sampleRegOre.Mono);
            Assert.Equal(result.First().CreationDate, _sampleRegOre.CreationDate);
            Assert.Equal(result.First().Uom, _sampleRegOre.Uom);
            Assert.Equal(result.First().ProductionQty, _sampleRegOre.ProductionQty);
            Assert.Equal(result.First().ProducedQty, _sampleRegOre.ProducedQty);
            Assert.Equal(result.First().ResQty, _sampleRegOre.ResQty);
            Assert.Equal(result.First().Storage, _sampleRegOre.Storage);
            Assert.Equal(result.First().Wc, _sampleRegOre.Wc);
            Assert.Equal(result.First().WorkingTime, _sampleRegOre.WorkingTime);
            Assert.Equal(result.First().Imported, _sampleRegOre.Imported);
            Assert.Equal(result.First().UserImp, _sampleRegOre.UserImp);
            Assert.Equal(result.First().DataImp, _sampleRegOre.DataImp);
            _regOrerRepositoryMock.Verify(repo => repo.PostRegOreList(It.IsAny<List<RegOreFilter>>()), Times.Once);
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
            var result = _regOreServiceMock.PostAppRegOre(regOreRequestList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.PostRegOreList(It.IsAny<List<RegOreFilter>>()), Times.Once);
        }

        [Fact]
        public void GetAppViewOre_ReturnsListOfRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.GetAppViewOre(It.IsAny<ViewOreRequestFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<ViewOreRequestFilter>(It.IsAny<ViewOreRequestDto>())).Returns(new ViewOreRequestFilter());
            _mapperMock.Setup(m => m.Map<RegOreDto>(It.IsAny<A3AppRegOre>())).Returns(new RegOreDto());

            // Act
            var result = _regOreServiceMock.GetAppViewOre(_sampleViewOreRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(result.First().RegOreId, _sampleRegOre.RegOreId);
            Assert.Equal(result.First().WorkerId, _sampleRegOre.WorkerId);
            Assert.Equal(result.First().Job, _sampleRegOre.Job);
            Assert.Equal(result.First().RtgStep, _sampleRegOre.RtgStep);
            Assert.Equal(result.First().Alternate, _sampleRegOre.Alternate);
            Assert.Equal(result.First().AltRtgStep, _sampleRegOre.AltRtgStep);
            Assert.Equal(result.First().Operation, _sampleRegOre.Operation);
            Assert.Equal(result.First().OperDesc, _sampleRegOre.OperDesc);
            Assert.Equal(result.First().Bom, _sampleRegOre.Bom);
            Assert.Equal(result.First().Variant, _sampleRegOre.Variant);
            Assert.Equal(result.First().ItemDesc, _sampleRegOre.ItemDesc);
            Assert.Equal(result.First().Moid, _sampleRegOre.Moid);
            Assert.Equal(result.First().Mono, _sampleRegOre.Mono);
            Assert.Equal(result.First().CreationDate, _sampleRegOre.CreationDate);
            Assert.Equal(result.First().Uom, _sampleRegOre.Uom);
            Assert.Equal(result.First().ProductionQty, _sampleRegOre.ProductionQty);
            Assert.Equal(result.First().ProducedQty, _sampleRegOre.ProducedQty);
            Assert.Equal(result.First().ResQty, _sampleRegOre.ResQty);
            Assert.Equal(result.First().Storage, _sampleRegOre.Storage);
            Assert.Equal(result.First().Wc, _sampleRegOre.Wc);
            Assert.Equal(result.First().WorkingTime, _sampleRegOre.WorkingTime);
            Assert.Equal(result.First().Imported, _sampleRegOre.Imported);
            Assert.Equal(result.First().UserImp, _sampleRegOre.UserImp);
            Assert.Equal(result.First().DataImp, _sampleRegOre.DataImp);
            _regOrerRepositoryMock.Verify(repo => repo.GetAppViewOre(It.IsAny<ViewOreRequestFilter>()), Times.Once);
        }

        [Fact]
        public void GetAppViewOre_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre>();
            _regOrerRepositoryMock.Setup(repo => repo.GetAppViewOre(It.IsAny<ViewOreRequestFilter>())).Returns(regOreList);
            _mapperMock.Setup(m => m.Map<ViewOreRequestFilter>(It.IsAny<ViewOreRequestDto>())).Returns(new ViewOreRequestFilter());

            // Act
            var result = _regOreServiceMock.GetAppViewOre(_sampleViewOreRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _regOrerRepositoryMock.Verify(repo => repo.GetAppViewOre(It.IsAny<ViewOreRequestFilter>()), Times.Once);
        }

        [Fact]
        public void PutAppViewOre_ReturnsRegOreDto_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.PutAppViewOre(It.IsAny<ViewOrePutFilter>())).Returns(_sampleRegOre);
            _mapperMock.Setup(m => m.Map<ViewOrePutFilter>(It.IsAny<ViewOrePutRequestDto>())).Returns(new ViewOrePutFilter());
            _mapperMock.Setup(m => m.Map<RegOreDto>(It.IsAny<A3AppRegOre>())).Returns(new RegOreDto());

            // Act
            var result = _regOreServiceMock.PutAppViewOre(_sampleViewOrePutRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.RegOreId, _sampleRegOre.RegOreId);
            Assert.Equal(result.WorkerId, _sampleRegOre.WorkerId);
            Assert.Equal(result.Job, _sampleRegOre.Job);
            Assert.Equal(result.RtgStep, _sampleRegOre.RtgStep);
            Assert.Equal(result.Alternate, _sampleRegOre.Alternate);
            Assert.Equal(result.AltRtgStep, _sampleRegOre.AltRtgStep);
            Assert.Equal(result.Operation, _sampleRegOre.Operation);
            Assert.Equal(result.OperDesc, _sampleRegOre.OperDesc);
            Assert.Equal(result.Bom, _sampleRegOre.Bom);
            Assert.Equal(result.Variant, _sampleRegOre.Variant);
            Assert.Equal(result.ItemDesc, _sampleRegOre.ItemDesc);
            Assert.Equal(result.Moid, _sampleRegOre.Moid);
            Assert.Equal(result.Mono, _sampleRegOre.Mono);
            Assert.Equal(result.CreationDate, _sampleRegOre.CreationDate);
            Assert.Equal(result.Uom, _sampleRegOre.Uom);
            Assert.Equal(result.ProductionQty, _sampleRegOre.ProductionQty);
            Assert.Equal(result.ProducedQty, _sampleRegOre.ProducedQty);
            Assert.Equal(result.ResQty, _sampleRegOre.ResQty);
            Assert.Equal(result.Storage, _sampleRegOre.Storage);
            Assert.Equal(result.Wc, _sampleRegOre.Wc);
            Assert.Equal(result.WorkingTime, _sampleRegOre.WorkingTime); 
            Assert.Equal(result.Imported, _sampleRegOre.Imported);
            Assert.Equal(result.UserImp, _sampleRegOre.UserImp);
            Assert.Equal(result.DataImp, _sampleRegOre.DataImp);
            _regOrerRepositoryMock.Verify(repo => repo.PutAppViewOre(It.IsAny<ViewOrePutFilter>()), Times.Once);
        }

        [Fact]
        public void PutAppViewOre_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            _regOrerRepositoryMock.Setup(repo => repo.PutAppViewOre(It.IsAny<ViewOrePutFilter>())).Returns((A3AppRegOre)null);
            _mapperMock.Setup(m => m.Map<ViewOrePutFilter>(It.IsAny<ViewOrePutRequestDto>())).Returns(new ViewOrePutFilter());

            // Act
            var result = _regOreServiceMock.PutAppViewOre(_sampleViewOrePutRequest);

            // Assert
            Assert.Null(result);
            _regOrerRepositoryMock.Verify(repo => repo.PutAppViewOre(It.IsAny<ViewOrePutFilter>()), Times.Once);
        }

        [Fact]
        public void DeleteAppViewOre_ReturnsTrue_WhenDataExists()
        {
            // Arrange
            var regOreList = new List<A3AppRegOre> { _sampleRegOre };
            _regOrerRepositoryMock.Setup(repo => repo.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestFilter>())).Returns(regOreList.First());
            _mapperMock.Setup(m => m.Map<ViewOreDeleteRequestFilter>(It.IsAny<ViewOreDeleteRequestDto>())).Returns(new ViewOreDeleteRequestFilter());

            // Act
            var result = _regOreServiceMock.DeleteRegOreId(_sampleViewOreDeleteRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.RegOreId, _sampleRegOre.RegOreId);
            Assert.Equal(result.WorkerId, _sampleRegOre.WorkerId);
            Assert.Equal(result.Job, _sampleRegOre.Job);
            Assert.Equal(result.RtgStep, _sampleRegOre.RtgStep);
            Assert.Equal(result.Alternate, _sampleRegOre.Alternate);
            Assert.Equal(result.AltRtgStep, _sampleRegOre.AltRtgStep);
            Assert.Equal(result.Operation, _sampleRegOre.Operation);
            Assert.Equal(result.OperDesc, _sampleRegOre.OperDesc);
            Assert.Equal(result.Bom, _sampleRegOre.Bom);
            Assert.Equal(result.Variant, _sampleRegOre.Variant);
            Assert.Equal(result.ItemDesc, _sampleRegOre.ItemDesc);
            Assert.Equal(result.Moid, _sampleRegOre.Moid);
            Assert.Equal(result.Mono, _sampleRegOre.Mono);
            Assert.Equal(result.CreationDate, _sampleRegOre.CreationDate);
            Assert.Equal(result.Uom, _sampleRegOre.Uom);
            Assert.Equal(result.ProductionQty, _sampleRegOre.ProductionQty);
            Assert.Equal(result.ProducedQty, _sampleRegOre.ProducedQty);
            Assert.Equal(result.ResQty, _sampleRegOre.ResQty);
            Assert.Equal(result.Storage, _sampleRegOre.Storage);
            Assert.Equal(result.Wc, _sampleRegOre.Wc);
            Assert.Equal(result.WorkingTime, _sampleRegOre.WorkingTime);
            Assert.Equal(result.Imported, _sampleRegOre.Imported);
            Assert.Equal(result.UserImp, _sampleRegOre.UserImp);
            Assert.Equal(result.DataImp, _sampleRegOre.DataImp);
            _regOrerRepositoryMock.Verify(repo => repo.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestFilter>()), Times.Once);
        }

        [Fact]
        public void DeleteAppViewOre_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var request = new ViewOreDeleteRequestDto();
            _regOrerRepositoryMock.Setup(repo => repo.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestFilter>())).Returns((A3AppRegOre)null);
            _mapperMock.Setup(m => m.Map<ViewOreDeleteRequestFilter>(It.IsAny<ViewOreDeleteRequestDto>())).Returns(new ViewOreDeleteRequestFilter());

            // Act
            var result = _regOreServiceMock.DeleteRegOreId(request);

            // Assert
            Assert.Null(result);
            _regOrerRepositoryMock.Verify(repo => repo.DeleteRegOreId(It.IsAny<ViewOreDeleteRequestFilter>()), Times.Once);
        }
    }
}