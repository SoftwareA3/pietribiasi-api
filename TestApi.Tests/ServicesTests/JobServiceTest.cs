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
    public class JobServiceTest
    {
        private readonly Mock<IJobRepository> _jobRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly JobRequestService _jobService;

        // DTO e Modelli di esempio utilizzati per i test
        private readonly VwApiJob _sampleVwApiJob = new VwApiJob // Modello restituito dal Repository
        {
            Job = "JOB001",
            Description = "Descrizione Job 1"
        };

        private readonly JobDto _sampleJobDto = new JobDto // DTO restituito dal Service
        {
            Job = "JOB001",
            Description = "Descrizione Job 1"
        };

        public JobServiceTest()
        {
            _jobRepositoryMock = new Mock<IJobRepository>();
            _mapperMock = new Mock<IMapper>(); // Inizializza il mock di IMapper

            _jobService = new JobRequestService(_jobRepositoryMock.Object, _mapperMock.Object);
        }

        // --- Test per GetJob ---
        [Fact]
        public void GetJob_ShouldReturnListOfJobDto_WhenDataExists()
        {
            // Arrange
            var mockDataDalRepository = new List<VwApiJob> { _sampleVwApiJob };
            _jobRepositoryMock.Setup(repo => repo.GetJobs()).Returns(mockDataDalRepository);

            // Act
            var result = _jobService.GetJobs();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_sampleJobDto.Job, result.First().Job);
            Assert.Equal(_sampleJobDto.Description, result.First().Description);
        }

        [Fact]
        public void GetJob_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var mockDataDalRepository = new List<VwApiJob>();
            _jobRepositoryMock.Setup(repo => repo.GetJobs()).Returns(mockDataDalRepository);

            // Act
            var result = _jobService.GetJobs();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}