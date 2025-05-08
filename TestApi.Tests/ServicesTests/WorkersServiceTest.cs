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
    public class WorkersServiceTest
    {
        private readonly Mock<IWorkerRepository> _workerRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IWorkersRequestService _workerService;

        private readonly VwApiWorker _workerSample = new VwApiWorker
        {
            WorkerId = 1,
            Name = "Nome",
            LastName = "Cognome",
            Pin = "1234",
            Password = "Password",
            TipoUtente = "TipoUtente",
            StorageVersamenti = "StorageVersamenti",
            Storage = "Storage",
            LastLogin = "2025-05-07 12:00:00"
        };

        private readonly RmWorkersField _workerFieldSample = new RmWorkersField
        {
            WorkerId = 1,
            Line = 4,
            FieldName = "Last Login",
            FieldValue = "2025-05-07 12:00:00",
            Notes = "Notes",
            HideOnLayout = "HideOnLayout",
            Tbcreated = new DateTime(2023, 10, 1),
            Tbmodified = new DateTime(2023, 10, 1),
            TbcreatedId = 1,
            TbmodifiedId = 1
        };

        private readonly WorkerDto _workerDtoSample = new WorkerDto
        {
            WorkerId = 1,
            Name = "Nome",
            LastName = "Cognome",
            Pin = "1234",
            Password = "Password",
            TipoUtente = "TipoUtente",
            StorageVersamenti = "StorageVersamenti",
            Storage = "Storage",
            LastLogin = "2025-05-07 12:00:00"
        };

        private readonly PasswordWorkersRequestDto _passwordWorkersRequestDtoSample = new PasswordWorkersRequestDto
        {
            Password = "Password"
        };

        private readonly WorkersFieldRequestDto _workersFieldRequestDtoSample = new WorkersFieldRequestDto
        {
            WorkerId = 1,
            Line = 4,
            FieldName = "Last Login",
            FieldValue = "2025-05-07 12:00:00",
            Notes = "Notes",
            HideOnLayout = "HideOnLayout",
            Tbcreated = new DateTime(2023, 10, 1),
            Tbmodified = new DateTime(2023, 10, 1),
            TbcreatedId = 1,
            TbmodifiedId = 1
        };

        private readonly WorkersFieldDto _workersFieldDtoSample = new WorkersFieldDto
        {
            WorkerId = 1,
            Line = 4,
            FieldName = "Last Login",
            FieldValue = "2025-05-07 12:00:00",
            Notes = "Notes",
            HideOnLayout = "HideOnLayout",
            Tbcreated = new DateTime(2023, 10, 1),
            Tbmodified = new DateTime(2023, 10, 1),
            TbcreatedId = 1,
            TbmodifiedId = 1
        };

        private readonly WorkerIdAndPasswordRequestDto _workerIdAndPasswordRequestDtoSample = new WorkerIdAndPasswordRequestDto
        {
            WorkerId = 1,
            Password = "Password"
        };

        public WorkersServiceTest()
        {
            _workerRepositoryMock = new Mock<IWorkerRepository>();
            _mapperMock = new Mock<IMapper>();
            _workerService = new WorkersRequestService(_workerRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetWorkers_ReturnsListOfWorkerDto_WhenDataExists()
        {
            // Arrange
            var workers = new List<VwApiWorker> { _workerSample };
            _workerRepositoryMock.Setup(repo => repo.GetWorkers()).Returns(workers);
            _mapperMock.Setup(m => m.Map<WorkerDto>(It.IsAny<VwApiWorker>())).Returns(_workerDtoSample);

            // Act
            var result = _workerService.GetWorkers();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_workerDtoSample.WorkerId, result.First().WorkerId);
            Assert.Equal(_workerDtoSample.Name, result.First().Name);
            Assert.Equal(_workerDtoSample.LastName, result.First().LastName);
            Assert.Equal(_workerDtoSample.Pin, result.First().Pin);
            Assert.Equal(_workerDtoSample.Password, result.First().Password);
            Assert.Equal(_workerDtoSample.TipoUtente, result.First().TipoUtente);
            Assert.Equal(_workerDtoSample.StorageVersamenti, result.First().StorageVersamenti);
            Assert.Equal(_workerDtoSample.Storage, result.First().Storage);
            Assert.Equal(_workerDtoSample.LastLogin, result.First().LastLogin);
            _workerRepositoryMock.Verify(repo => repo.GetWorkers(), Times.Once);
        }

        [Fact]
        public void GetWorkers_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var workers = new List<VwApiWorker>();
            _workerRepositoryMock.Setup(repo => repo.GetWorkers()).Returns(workers);

            // Act
            var result = _workerService.GetWorkers();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetWorkerByPassword_ReturnsWorkerDto_WhenDataExists()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(filter)).Returns(_workerSample);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample)).Returns(filter);
            _mapperMock.Setup(m => m.Map<WorkerDto>(_workerSample)).Returns(_workerDtoSample);

            // Act
            var result = _workerService.GetWorkerByPassword(_passwordWorkersRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workerDtoSample.WorkerId, result.WorkerId);
            Assert.Equal(_workerDtoSample.Name, result.Name);
            Assert.Equal(_workerDtoSample.LastName, result.LastName);
            Assert.Equal(_workerDtoSample.Pin, result.Pin);
            Assert.Equal(_workerDtoSample.Password, result.Password);
            Assert.Equal(_workerDtoSample.TipoUtente, result.TipoUtente);
            Assert.Equal(_workerDtoSample.StorageVersamenti, result.StorageVersamenti);
            Assert.Equal(_workerDtoSample.Storage, result.Storage);
            Assert.Equal(_workerDtoSample.LastLogin, result.LastLogin);
            _workerRepositoryMock.Verify(repo => repo.GetWorkerByPassword(filter), Times.Once);
        }

        [Fact]
        public void GetWorkerByPassword_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(filter)).Returns((VwApiWorker)null);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetWorkerByPassword(_passwordWorkersRequestDtoSample);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CallStoredProcedure_CallsRepositoryMethod()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workerDtoSample.WorkerId, FieldValue = _workerDtoSample.LastLogin };
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample)).Returns(filter);

            // Act
            _workerService.CallStoredProcedure(_workerDtoSample);

            // Assert
            _workerRepositoryMock.Verify(repo => repo.CallStoredProcedure(filter), Times.Once);
        }

        [Fact]
        public void GetWorkersFieldsById_ReturnsListOfWorkersFieldDto_WhenDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workersFieldRequestDtoSample.WorkerId };
            var workersFields = new List<RmWorkersField> { _workerFieldSample };
            _workerRepositoryMock.Setup(repo => repo.GetWorkersFieldsById(filter)).Returns(workersFields);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workersFieldRequestDtoSample)).Returns(filter);
            _mapperMock.Setup(m => m.Map<WorkersFieldDto>(_workerFieldSample)).Returns(_workersFieldDtoSample);

            // Act
            var result = _workerService.GetWorkersFieldsById(_workersFieldRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_workersFieldDtoSample.WorkerId, result.First().WorkerId);
            Assert.Equal(_workersFieldDtoSample.Line, result.First().Line);
            Assert.Equal(_workersFieldDtoSample.FieldName, result.First().FieldName);
            Assert.Equal(_workersFieldDtoSample.FieldValue, result.First().FieldValue);
            Assert.Equal(_workersFieldDtoSample.Notes, result.First().Notes);
            Assert.Equal(_workersFieldDtoSample.HideOnLayout, result.First().HideOnLayout);
            Assert.Equal(_workersFieldDtoSample.Tbcreated, result.First().Tbcreated);
            Assert.Equal(_workersFieldDtoSample.Tbmodified, result.First().Tbmodified);
            Assert.Equal(_workersFieldDtoSample.TbcreatedId, result.First().TbcreatedId);
            Assert.Equal(_workersFieldDtoSample.TbmodifiedId, result.First().TbmodifiedId);
            _workerRepositoryMock.Verify(repo => repo.GetWorkersFieldsById(filter), Times.Once);
        }

        [Fact]
        public void GetWorkersFieldsById_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workersFieldRequestDtoSample.WorkerId };
            var workersFields = new List<RmWorkersField>();
            _workerRepositoryMock.Setup(repo => repo.GetWorkersFieldsById(filter)).Returns(workersFields);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workersFieldRequestDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetWorkersFieldsById(_workersFieldRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetLastWorkerFieldLine_ReturnsWorkersFieldDto_WhenDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workerFieldSample.WorkerId };
            _workerRepositoryMock.Setup(repo => repo.GetLastWorkerFieldLine(filter)).Returns(_workerFieldSample);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample)).Returns(filter);
            _mapperMock.Setup(m => m.Map<WorkersFieldDto>(_workerFieldSample)).Returns(_workersFieldDtoSample);

            // Act
            var result = _workerService.GetLastWorkerFieldLine(_workerDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workersFieldDtoSample.WorkerId, result.WorkerId);
            Assert.Equal(_workersFieldDtoSample.Line, result.Line);
            Assert.Equal(_workersFieldDtoSample.FieldName, result.FieldName);
            Assert.Equal(_workersFieldDtoSample.FieldValue, result.FieldValue);
            Assert.Equal(_workersFieldDtoSample.Notes, result.Notes);
            Assert.Equal(_workersFieldDtoSample.HideOnLayout, result.HideOnLayout);
            Assert.Equal(_workersFieldDtoSample.Tbcreated, result.Tbcreated);
            Assert.Equal(_workersFieldDtoSample.Tbmodified, result.Tbmodified);
            Assert.Equal(_workersFieldDtoSample.TbcreatedId, result.TbcreatedId);
            Assert.Equal(_workersFieldDtoSample.TbmodifiedId, result.TbmodifiedId);
            _workerRepositoryMock.Verify(repo => repo.GetLastWorkerFieldLine(filter), Times.Once);
        }

        [Fact]
        public void GetLastWorkerFieldLine_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workerFieldSample.WorkerId };
            _workerRepositoryMock.Setup(repo => repo.GetLastWorkerFieldLine(filter)).Returns((RmWorkersField)null);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetLastWorkerFieldLine(_workerDtoSample);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateOrCreateLastLogin_ReturnsWorkersFieldDto_WhenDataExists()
        {
            // Arrange
            var passwordFilter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            var workerIdValueFilter = new WorkerIdAndValueRequestFilter { WorkerId = _workerSample.WorkerId };
            
            // Setup per GetWorkerByPassword
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns(_workerSample);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample))
                .Returns(passwordFilter);
            _mapperMock.Setup(m => m.Map<WorkerDto>(_workerSample))
                .Returns(_workerDtoSample);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample))
                .Returns(workerIdValueFilter);
                
            // Setup per GetLastWorkerFieldLine
            _workerRepositoryMock.Setup(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>()))
                .Returns(_workerFieldSample);
            _mapperMock.Setup(m => m.Map<WorkersFieldDto>(_workerFieldSample))
                .Returns(_workersFieldDtoSample);
                
            // Setup per CreateOrUpdateLastLogin
            _workerRepositoryMock.Setup(repo => repo.CreateOrUpdateLastLogin(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _workerService.UpdateOrCreateLastLogin(_passwordWorkersRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workersFieldDtoSample.WorkerId, result.WorkerId);
            Assert.Equal(_workersFieldDtoSample.Line, result.Line);
            Assert.Equal(_workersFieldDtoSample.FieldName, result.FieldName);
            Assert.Equal(_workersFieldDtoSample.FieldValue, result.FieldValue);
            Assert.Equal(_workersFieldDtoSample.Notes, result.Notes);
            Assert.Equal(_workersFieldDtoSample.HideOnLayout, result.HideOnLayout);
            Assert.Equal(_workersFieldDtoSample.Tbcreated, result.Tbcreated);
            Assert.Equal(_workersFieldDtoSample.Tbmodified, result.Tbmodified);
            Assert.Equal(_workersFieldDtoSample.TbcreatedId, result.TbcreatedId);
            Assert.Equal(_workersFieldDtoSample.TbmodifiedId, result.TbmodifiedId);
        }

        [Fact]
        public void UpdateOrCreateLastLogin_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(filter)).Returns((VwApiWorker)null);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample)).Returns(filter);

            // Act
            var result = _workerService.UpdateOrCreateLastLogin(_passwordWorkersRequestDtoSample).Result;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LoginWithPassword_ReturnsWorkerDto_WhenDataExists()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(filter)).Returns(_workerSample);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample)).Returns(filter);
            _workerRepositoryMock.Setup(repo => repo.CreateOrUpdateLastLogin(filter)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<WorkerDto>(_workerSample)).Returns(_workerDtoSample);

            // Act
            var result = _workerService.LoginWithPassword(_passwordWorkersRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workerDtoSample.WorkerId, result.WorkerId);
            Assert.Equal(_workerDtoSample.Name, result.Name);
            Assert.Equal(_workerDtoSample.LastName, result.LastName);
            Assert.Equal(_workerDtoSample.Pin, result.Pin);
            Assert.Equal(_workerDtoSample.Password, result.Password);
            Assert.Equal(_workerDtoSample.TipoUtente, result.TipoUtente);
            Assert.Equal(_workerDtoSample.StorageVersamenti, result.StorageVersamenti);
            Assert.Equal(_workerDtoSample.Storage, result.Storage);
            Assert.Equal(_workerDtoSample.LastLogin, result.LastLogin);
            _workerRepositoryMock.Verify(repo => repo.GetWorkerByPassword(filter), Times.AtLeast(1));
        }

        [Fact]
        public void LoginWithPassword_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(filter)).Returns((VwApiWorker)null);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample)).Returns(filter);

            // Act
            var result = _workerService.LoginWithPassword(_passwordWorkersRequestDtoSample);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetWorkerByIdAndPassword_ReturnsWorkerDto_WhenDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndPasswordFilter { WorkerId = _workerIdAndPasswordRequestDtoSample.WorkerId, Password = _workerIdAndPasswordRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByIdAndPassword(filter)).Returns(_workerSample);
            _mapperMock.Setup(m => m.Map<WorkerIdAndPasswordFilter>(_workerIdAndPasswordRequestDtoSample)).Returns(filter);
            _mapperMock.Setup(m => m.Map<WorkerDto>(_workerSample)).Returns(_workerDtoSample);

            // Act
            var result = _workerService.GetWorkerByIdAndPassword(_workerIdAndPasswordRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workerDtoSample.WorkerId, result.WorkerId);
            Assert.Equal(_workerDtoSample.Name, result.Name);
            Assert.Equal(_workerDtoSample.LastName, result.LastName);
            Assert.Equal(_workerDtoSample.Pin, result.Pin);
            Assert.Equal(_workerDtoSample.Password, result.Password);
            Assert.Equal(_workerDtoSample.TipoUtente, result.TipoUtente);
            Assert.Equal(_workerDtoSample.StorageVersamenti, result.StorageVersamenti);
            Assert.Equal(_workerDtoSample.Storage, result.Storage);
            Assert.Equal(_workerDtoSample.LastLogin, result.LastLogin);
            _workerRepositoryMock.Verify(repo => repo.GetWorkerByIdAndPassword(filter), Times.Once);
        }

        [Fact]
        public void GetWorkerByIdAndPassword_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndPasswordFilter { WorkerId = _workerIdAndPasswordRequestDtoSample.WorkerId, Password = _workerIdAndPasswordRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByIdAndPassword(filter)).Returns((VwApiWorker)null);
            _mapperMock.Setup(m => m.Map<WorkerIdAndPasswordFilter>(_workerIdAndPasswordRequestDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetWorkerByIdAndPassword(_workerIdAndPasswordRequestDtoSample);

            // Assert
            Assert.Null(result);
        }
    }
}