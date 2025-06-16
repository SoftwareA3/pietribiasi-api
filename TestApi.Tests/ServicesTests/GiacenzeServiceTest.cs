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
using apiPB.Models;                        
using apiPB.Mappers.Dto;                  

namespace TestApi.Tests.ServicesTests
{
    public class GiacenzeServiceTest
    {
        private readonly Mock<IGiacenzeRepository> _giacenzeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock; 
        private readonly GiacenzeRequestService _giacenzeService;

        // DTO e Modelli di esempio utilizzati per i test
        private readonly VwApiGiacenze _sampleVwApiGiacenza = new VwApiGiacenze // Modello restituito dal Repository
        {
            Item = "ITEM001",
            Description = "Descrizione Articolo 1",
            BarCode = "1234567890123",
            FiscalYear = 2023,
            Storage = "MAG01",
            BookInv = 100.50
        };

        private readonly GiacenzeDto _sampleGiacenzeDto = new GiacenzeDto // DTO restituito dal Service
        {
            Item = "ITEM001",
            Description = "Descrizione Articolo 1",
            BarCode = "1234567890123",
            FiscalYear = 2023,
            Storage = "MAG01",
            BookInv = 100.50
        };

        public GiacenzeServiceTest()
        {
            _giacenzeRepositoryMock = new Mock<IGiacenzeRepository>();
            _mapperMock = new Mock<IMapper>(); // Inizializza il mock di IMapper

            _giacenzeService = new GiacenzeRequestService(_giacenzeRepositoryMock.Object, _mapperMock.Object);
        }

        // --- Test per GetGiacenze ---
        [Fact]
        public void GetGiacenze_ShouldReturnListOfGiacenzeDto_WhenDataExists()
        {
            // Arrange
            var mockDataDalRepository = new List<VwApiGiacenze> { _sampleVwApiGiacenza };
            _giacenzeRepositoryMock.Setup(repo => repo.GetGiacenze()).Returns(mockDataDalRepository);

            // Act
            var result = _giacenzeService.GetGiacenze();

            // Assert
            Assert.NotNull(result);
            var giacenzeDtoList = result.ToList();
            Assert.Single(giacenzeDtoList);
            var giacenzaDto = giacenzeDtoList.First();

            Assert.Equal(_sampleGiacenzeDto.Item, giacenzaDto.Item);
            Assert.Equal(_sampleGiacenzeDto.Description, giacenzaDto.Description);
            Assert.Equal(_sampleGiacenzeDto.BarCode, giacenzaDto.BarCode);
            Assert.Equal(_sampleGiacenzeDto.FiscalYear, giacenzaDto.FiscalYear);
            Assert.Equal(_sampleGiacenzeDto.Storage, giacenzaDto.Storage);
            Assert.Equal(_sampleGiacenzeDto.BookInv, giacenzaDto.BookInv);

            _giacenzeRepositoryMock.Verify(repo => repo.GetGiacenze(), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var emptyListFromRepository = new List<VwApiGiacenze>();
            _giacenzeRepositoryMock.Setup(repo => repo.GetGiacenze()).Returns(emptyListFromRepository);

            // Act
            var result = _giacenzeService.GetGiacenze();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _giacenzeRepositoryMock.Verify(repo => repo.GetGiacenze(), Times.Once);
        }

        [Fact]
        public void GetGiacenze_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var detailedVwApiGiacenza = new VwApiGiacenze
            {
                Item = "ITEM002",
                Description = "Another Item Description",
                BarCode = "9876543210987",
                FiscalYear = 2024,
                Storage = "MAG02",
                BookInv = 15.75
            };
            var mockDataDalRepository = new List<VwApiGiacenze> { detailedVwApiGiacenza };
            _giacenzeRepositoryMock.Setup(repo => repo.GetGiacenze()).Returns(mockDataDalRepository);

            var expectedDto = new GiacenzeDto
            {
                Item = "ITEM002",
                Description = "Another Item Description",
                BarCode = "9876543210987",
                FiscalYear = 2024,
                Storage = "MAG02",
                BookInv = 15.75
            };

            // Act
            var result = _giacenzeService.GetGiacenze().ToList();

            // Assert
            Assert.Single(result);
            var actualDto = result.First();

            Assert.Equal(expectedDto.Item, actualDto.Item);
            Assert.Equal(expectedDto.Description, actualDto.Description);
            Assert.Equal(expectedDto.BarCode, actualDto.BarCode);
            Assert.Equal(expectedDto.FiscalYear, actualDto.FiscalYear);
            Assert.Equal(expectedDto.Storage, actualDto.Storage);
            Assert.Equal(expectedDto.BookInv, actualDto.BookInv);
        }

        [Fact]
        public void GetGiacenze_ShouldHandleNullsInRepositoryModelGracefully()
        {
            // Arrange
            var vwApiGiacenzaWithNulls = new VwApiGiacenze // Modello dal Repository con possibili null
            {
                Item = "ITEM003", 
                Description = null,
                BarCode = null,
                FiscalYear = 2023,
                Storage = "MAG03", // Storage è non nullo
                BookInv = null
            };
            var mockDataDalRepository = new List<VwApiGiacenze> { vwApiGiacenzaWithNulls };
            _giacenzeRepositoryMock.Setup(repo => repo.GetGiacenze()).Returns(mockDataDalRepository);

            // Il mapper ToGiacenzeDto gestisce i null convertendoli in string.Empty o mantenendo il null per BookInv
            var expectedDto = new GiacenzeDto
            {
                Item = "ITEM003",
                Description = string.Empty, 
                BarCode = string.Empty,     
                FiscalYear = 2023,
                Storage = "MAG03",
                BookInv = null
            };

            // Act
            var result = _giacenzeService.GetGiacenze().ToList();

            // Assert
            Assert.Single(result);
            var actualDto = result.First();

            Assert.Equal(expectedDto.Item, actualDto.Item);
            Assert.Equal(expectedDto.Description, actualDto.Description);
            Assert.Equal(expectedDto.BarCode, actualDto.BarCode);
            Assert.Equal(expectedDto.FiscalYear, actualDto.FiscalYear);
            Assert.Equal(expectedDto.Storage, actualDto.Storage);
            Assert.Equal(expectedDto.BookInv, actualDto.BookInv);
        }
    }
}