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
    public class InventarioServiceTest
    {
        private readonly Mock<IInventarioRepository> _inventarioRepositoryMock;
        private readonly Mock<IMapper> _mapperMock; 
        private readonly InventarioRequestService _inventarioService;
        private readonly A3AppInventario _sampleVwApiInventario = new A3AppInventario // Modello restituito dal Repository
        {
            InvId = 1,
            WorkerId = 123,
            SavedDate = new DateTime(2023, 10, 1),
            Item = "SampleItem",
            Description = "SampleDescription",
            BarCode = "1234567890",
            FiscalYear = 2023,
            Storage = "SampleStorage",
            BookInv = 100.0,
            Imported = false,
            UserImp = "SampleUser",
            DataImp = new DateTime(2023, 9, 1)
        };

        private readonly InventarioDto _sampleInventarioDto = new InventarioDto // Modello restituito dal Service
        {
            InvId = 1,
            WorkerId = 123,
            SavedDate = new DateTime(2023, 10, 1),
            Item = "SampleItem",
            Description = "SampleDescription",
            BarCode = "1234567890",
            FiscalYear = 2023,
            Storage = "SampleStorage",
            BookInv = 100.0,
            Imported = false,
            UserImp = "SampleUser",
            DataImp = new DateTime(2023, 9, 1)
        };

        private readonly InventarioRequestDto _sampleInventarioRequestDto = new InventarioRequestDto // DTO di richiesta
        {
            InvId = 1,
            WorkerId = 123,
            SavedDate = new DateTime(2023, 10, 1),
            Item = "SampleItem",
            Description = "SampleDescription",
            BarCode = "1234567890",
            FiscalYear = 2023,
            Storage = "SampleStorage",
            BookInv = 100.0,
            Imported = false,
            UserImp = "SampleUser",
            DataImp = new DateTime(2023, 9, 1)
        };

        public InventarioServiceTest()
        {
            _inventarioRepositoryMock = new Mock<IInventarioRepository>();
            _mapperMock = new Mock<IMapper>();
            _inventarioService = new InventarioRequestService(_inventarioRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetInventario_ShouldReturnsListOfInventarioDto_WhenDataExists()
        {
            // Arrange
            var inventarioList = new List<A3AppInventario> { _sampleVwApiInventario };
            _inventarioRepositoryMock.Setup(repo => repo.GetInventario()).Returns(inventarioList);
            _mapperMock.Setup(m => m.Map<InventarioDto>(It.IsAny<A3AppInventario>())).Returns(_sampleInventarioDto);

            // Act
            var result = _inventarioService.GetInventario();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_sampleInventarioDto.InvId, result.First().InvId);
            Assert.Equal(_sampleInventarioDto.WorkerId, result.First().WorkerId);
            Assert.Equal(_sampleInventarioDto.SavedDate, result.First().SavedDate);
            Assert.Equal(_sampleInventarioDto.Item, result.First().Item);
            Assert.Equal(_sampleInventarioDto.Description, result.First().Description);
            Assert.Equal(_sampleInventarioDto.BarCode, result.First().BarCode);
            Assert.Equal(_sampleInventarioDto.FiscalYear, result.First().FiscalYear);
            Assert.Equal(_sampleInventarioDto.Storage, result.First().Storage);
            Assert.Equal(_sampleInventarioDto.BookInv, result.First().BookInv);
            Assert.Equal(_sampleInventarioDto.Imported, result.First().Imported);
            Assert.Equal(_sampleInventarioDto.UserImp, result.First().UserImp);
            Assert.Equal(_sampleInventarioDto.DataImp, result.First().DataImp);
            _inventarioRepositoryMock.Verify(repo => repo.GetInventario(), Times.Once);
        }

        [Fact]
        public void GetInventario_ShouldReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var emptyListFromRepository = new List<A3AppInventario>();
            _inventarioRepositoryMock.Setup(repo => repo.GetInventario()).Returns(emptyListFromRepository);

            // Act
            var result = _inventarioService.GetInventario();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void PostInventarioList_ShouldReturnsListOfInventarioDto_WhenDataExists()
        {
            // Arrange
            var inventarioList = new List<A3AppInventario> { _sampleVwApiInventario };
            var requestList = new List<InventarioRequestDto> { _sampleInventarioRequestDto };
            var filterList = new List<InventarioFilter> { new InventarioFilter() };
            _inventarioRepositoryMock.Setup(repo => repo.PostInventarioList(It.IsAny<IEnumerable<InventarioFilter>>())).Returns(inventarioList);
            _mapperMock.Setup(m => m.Map<InventarioFilter>(It.IsAny<InventarioRequestDto>())).Returns(new InventarioFilter());

            // Act
            var result = _inventarioService.PostInventarioList(requestList);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_sampleInventarioDto.InvId, result.First().InvId);
            Assert.Equal(_sampleInventarioDto.WorkerId, result.First().WorkerId);
            Assert.Equal(_sampleInventarioDto.SavedDate, result.First().SavedDate);
            Assert.Equal(_sampleInventarioDto.Item, result.First().Item);
            Assert.Equal(_sampleInventarioDto.Description, result.First().Description);
            Assert.Equal(_sampleInventarioDto.BarCode, result.First().BarCode);
            Assert.Equal(_sampleInventarioDto.FiscalYear, result.First().FiscalYear);
            Assert.Equal(_sampleInventarioDto.Storage, result.First().Storage);
            Assert.Equal(_sampleInventarioDto.BookInv, result.First().BookInv);
            Assert.Equal(_sampleInventarioDto.Imported, result.First().Imported);
            Assert.Equal(_sampleInventarioDto.UserImp, result.First().UserImp);
            Assert.Equal(_sampleInventarioDto.DataImp, result.First().DataImp);
            _inventarioRepositoryMock.Verify(repo => repo.PostInventarioList(It.IsAny<IEnumerable<InventarioFilter>>()), Times.Once);
        }

        [Fact]
        public void PostInventarioList_ShouldReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var inventarioList = new List<A3AppInventario>();
            var requestList = new List<InventarioRequestDto> { _sampleInventarioRequestDto };
            var filterList = new List<InventarioFilter> { new InventarioFilter() };
            _inventarioRepositoryMock.Setup(repo => repo.PostInventarioList(It.IsAny<IEnumerable<InventarioFilter>>())).Returns(inventarioList);
            _mapperMock.Setup(m => m.Map<InventarioFilter>(It.IsAny<InventarioRequestDto>())).Returns(new InventarioFilter());

            // Act
            var result = _inventarioService.PostInventarioList(requestList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetViewInventario_ShouldReturnsListOfInventarioDto_WhenDataExists()
        {
            // Arrange
            var inventarioList = new List<A3AppInventario> { _sampleVwApiInventario };
            var request = new ViewInventarioRequestDto
            {
                WorkerId = 123,
                FromDateTime = new DateTime(2023, 1, 1),
                ToDateTime = new DateTime(2023, 12, 31),
                Item = "SampleItem",
                BarCode = "1234567890"
            };
            var filter = new ViewInventarioRequestFilter();
            _inventarioRepositoryMock.Setup(repo => repo.GetViewInventario(It.IsAny<ViewInventarioRequestFilter>())).Returns(inventarioList);
            _mapperMock.Setup(m => m.Map<ViewInventarioRequestFilter>(It.IsAny<ViewInventarioRequestDto>())).Returns(filter);

            // Act
            var result = _inventarioService.GetViewInventario(request);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_sampleInventarioDto.InvId, result.First().InvId);
            Assert.Equal(_sampleInventarioDto.WorkerId, result.First().WorkerId);
            Assert.Equal(_sampleInventarioDto.SavedDate, result.First().SavedDate);
            Assert.Equal(_sampleInventarioDto.Item, result.First().Item);
            Assert.Equal(_sampleInventarioDto.Description, result.First().Description);
            Assert.Equal(_sampleInventarioDto.BarCode, result.First().BarCode);
            Assert.Equal(_sampleInventarioDto.FiscalYear, result.First().FiscalYear);
            Assert.Equal(_sampleInventarioDto.Storage, result.First().Storage);
            Assert.Equal(_sampleInventarioDto.BookInv, result.First().BookInv);
            Assert.Equal(_sampleInventarioDto.Imported, result.First().Imported);
            Assert.Equal(_sampleInventarioDto.UserImp, result.First().UserImp);
            Assert.Equal(_sampleInventarioDto.DataImp, result.First().DataImp);
            _inventarioRepositoryMock.Verify(repo => repo.GetViewInventario(It.IsAny<ViewInventarioRequestFilter>()), Times.Once);
        }

        [Fact]
        public void GetViewInventario_ShouldReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var inventarioList = new List<A3AppInventario>();
            var request = new ViewInventarioRequestDto
            {
                WorkerId = 123,
                FromDateTime = new DateTime(2023, 1, 1),
                ToDateTime = new DateTime(2023, 12, 31),
                Item = "SampleItem",
                BarCode = "1234567890"
            };
            var filter = new ViewInventarioRequestFilter();
            _inventarioRepositoryMock.Setup(repo => repo.GetViewInventario(It.IsAny<ViewInventarioRequestFilter>())).Returns(inventarioList);
            _mapperMock.Setup(m => m.Map<ViewInventarioRequestFilter>(It.IsAny<ViewInventarioRequestDto>())).Returns(filter);

            // Act
            var result = _inventarioService.GetViewInventario(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void PutViewInventario_ShouldReturnInventarioDto_WhenDataExists()
        {
            // Arrange
            var inventario = new A3AppInventario
            {
                InvId = 1,
                WorkerId = 123,
                SavedDate = new DateTime(2023, 10, 1),
                Item = "SampleItem",
                Description = "SampleDescription",
                BarCode = "1234567890",
                FiscalYear = 2023,
                Storage = "SampleStorage",
                BookInv = 100.0,
                Imported = false,
                UserImp = "SampleUser",
                DataImp = new DateTime(2023, 9, 1)
            };
            var request = new ViewInventarioPutRequestDto
            {
                InvId = 1,
                BookInv = 100.0
            };
            var filter = new ViewInventarioPutFilter();
            _inventarioRepositoryMock.Setup(repo => repo.PutViewInventario(It.IsAny<ViewInventarioPutFilter>())).Returns(inventario);
            _mapperMock.Setup(m => m.Map<ViewInventarioPutFilter>(It.IsAny<ViewInventarioPutRequestDto>())).Returns(filter);

            // Act
            var result = _inventarioService.PutViewInventario(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_sampleInventarioDto.InvId, result.InvId);
            Assert.Equal(_sampleInventarioDto.WorkerId, result.WorkerId);
            Assert.Equal(_sampleInventarioDto.SavedDate, result.SavedDate);
            Assert.Equal(_sampleInventarioDto.Item, result.Item);
            Assert.Equal(_sampleInventarioDto.Description, result.Description);
            Assert.Equal(_sampleInventarioDto.BarCode, result.BarCode);
            Assert.Equal(_sampleInventarioDto.FiscalYear, result.FiscalYear);
            Assert.Equal(_sampleInventarioDto.Storage, result.Storage);
            Assert.Equal(_sampleInventarioDto.BookInv, result.BookInv);
            Assert.Equal(_sampleInventarioDto.Imported, result.Imported);
            Assert.Equal(_sampleInventarioDto.UserImp, result.UserImp);
            Assert.Equal(_sampleInventarioDto.DataImp, result.DataImp);
            _inventarioRepositoryMock.Verify(repo => repo.PutViewInventario(It.IsAny<ViewInventarioPutFilter>()), Times.Once);
        }

        [Fact]
        public void PutViewInventario_ShouldReturnNull_WhenNoDataExists()
        {
            // Arrange
            var inventario = new A3AppInventario();
            var request = new ViewInventarioPutRequestDto
            {
                InvId = 1,
                BookInv = 100.0
            };
            var filter = new ViewInventarioPutFilter();
            _inventarioRepositoryMock.Setup(repo => repo.PutViewInventario(It.IsAny<ViewInventarioPutFilter>())).Returns(inventario);
            _mapperMock.Setup(m => m.Map<ViewInventarioPutFilter>(It.IsAny<ViewInventarioPutRequestDto>())).Returns(filter);

            // Act
            var result = _inventarioService.PutViewInventario(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.InvId);
            Assert.Equal(0, result.WorkerId);
            Assert.Null(result.SavedDate);
            Assert.Equal(string.Empty, result.Item);
            Assert.Equal(string.Empty, result.Description);
            Assert.Equal(string.Empty, result.BarCode);
            Assert.Equal(0, result.FiscalYear);
            Assert.Equal(string.Empty, result.Storage);
            Assert.Null(result.BookInv);
            Assert.False(result.Imported);
            Assert.Equal(string.Empty, result.UserImp);
            Assert.Null(result.DataImp);
            _inventarioRepositoryMock.Verify(repo => repo.PutViewInventario(It.IsAny<ViewInventarioPutFilter>()), Times.Once);
        }
    }
}