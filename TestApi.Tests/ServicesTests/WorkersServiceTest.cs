using Xunit;
using Moq;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private readonly VwApiWorkersfield _workerFieldSample = new VwApiWorkersfield
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

        #region GetWorkers Tests

        [Fact]
        public void GetWorkers_ReturnsListOfWorkerDto_WhenDataExists()
        {
            // Arrange
            var workers = new List<VwApiWorker> { _workerSample };
            _workerRepositoryMock.Setup(repo => repo.GetWorkers()).Returns(workers);

            // Act
            var result = _workerService.GetWorkers();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var resultWorker = result.First();
            Assert.Equal(_workerSample.WorkerId, resultWorker.WorkerId);
            Assert.Equal(_workerSample.Name, resultWorker.Name);
            Assert.Equal(_workerSample.LastName, resultWorker.LastName);
            Assert.Equal(_workerSample.Pin, resultWorker.Pin);
            Assert.Equal(_workerSample.Password, resultWorker.Password);
            Assert.Equal(_workerSample.TipoUtente, resultWorker.TipoUtente);
            Assert.Equal(_workerSample.StorageVersamenti, resultWorker.StorageVersamenti);
            Assert.Equal(_workerSample.Storage, resultWorker.Storage);
            Assert.Equal(_workerSample.LastLogin, resultWorker.LastLogin);
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
            _workerRepositoryMock.Verify(repo => repo.GetWorkers(), Times.Once);
        }
        #endregion

        #region GetWorkerByPassword Tests

        [Fact]
        public void GetWorkerByPassword_ReturnsWorkerDto_WhenDataExists()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>())).Returns(_workerSample);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetWorkerByPassword(_passwordWorkersRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workerSample.WorkerId, result.WorkerId);
            Assert.Equal(_workerSample.Name, result.Name);
            Assert.Equal(_workerSample.LastName, result.LastName);
            Assert.Equal(_workerSample.Pin, result.Pin);
            Assert.Equal(_workerSample.Password, result.Password);
            Assert.Equal(_workerSample.TipoUtente, result.TipoUtente);
            Assert.Equal(_workerSample.StorageVersamenti, result.StorageVersamenti);
            Assert.Equal(_workerSample.Storage, result.Storage);
            Assert.Equal(_workerSample.LastLogin, result.LastLogin);
            _workerRepositoryMock.Verify(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>()), Times.Once);
        }

        [Fact]
        public void GetWorkerByPassword_ThrowsArgumentNullException_WhenNoDataExists()
        {
            // Arrange
            var filter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>())).Returns((VwApiWorker)null);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample)).Returns(filter);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _workerService.GetWorkerByPassword(_passwordWorkersRequestDtoSample));
            Assert.Equal("worker", exception.ParamName);
            Assert.Contains("Nessun lavoratore trovato con la password fornita.", exception.Message);
        }
        #endregion

        #region CallStoredProcedure Tests

        [Fact]
        public async Task CallStoredProcedure_CallsRepositoryMethod()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workerDtoSample.WorkerId, FieldValue = _workerDtoSample.LastLogin };
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample)).Returns(filter);
            _workerRepositoryMock.Setup(repo => repo.CallStoredProcedure(It.IsAny<WorkerIdAndValueRequestFilter>())).Returns(Task.CompletedTask);

            // Act
            await _workerService.CallStoredProcedure(_workerDtoSample);

            // Assert
            _workerRepositoryMock.Verify(repo => repo.CallStoredProcedure(It.IsAny<WorkerIdAndValueRequestFilter>()), Times.Once);
        }
        #endregion

        #region GetWorkersFieldsById Tests

        [Fact]
        public void GetWorkersFieldsById_ReturnsListOfWorkersFieldDto_WhenDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workersFieldRequestDtoSample.WorkerId };
            var workersFields = new List<VwApiWorkersfield> { _workerFieldSample };
            _workerRepositoryMock.Setup(repo => repo.GetWorkersFieldsById(It.IsAny<WorkerIdAndValueRequestFilter>())).Returns(workersFields);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workersFieldRequestDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetWorkersFieldsById(_workersFieldRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var resultField = result.First();
            Assert.Equal(_workerFieldSample.WorkerId, resultField.WorkerId);
            Assert.Equal(_workerFieldSample.Line, resultField.Line);
            Assert.Equal(_workerFieldSample.FieldName, resultField.FieldName);
            Assert.Equal(_workerFieldSample.FieldValue, resultField.FieldValue);
            Assert.Equal(_workerFieldSample.Notes, resultField.Notes);
            Assert.Equal(_workerFieldSample.HideOnLayout, resultField.HideOnLayout);
            Assert.Equal(_workerFieldSample.Tbcreated, resultField.Tbcreated);
            Assert.Equal(_workerFieldSample.Tbmodified, resultField.Tbmodified);
            Assert.Equal(_workerFieldSample.TbcreatedId, resultField.TbcreatedId);
            Assert.Equal(_workerFieldSample.TbmodifiedId, resultField.TbmodifiedId);
            _workerRepositoryMock.Verify(repo => repo.GetWorkersFieldsById(It.IsAny<WorkerIdAndValueRequestFilter>()), Times.Once);
        }

        [Fact]
        public void GetWorkersFieldsById_ReturnsEmptyList_WhenNoDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workersFieldRequestDtoSample.WorkerId };
            var workersFields = new List<VwApiWorkersfield>();
            _workerRepositoryMock.Setup(repo => repo.GetWorkersFieldsById(It.IsAny<WorkerIdAndValueRequestFilter>())).Returns(workersFields);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workersFieldRequestDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetWorkersFieldsById(_workersFieldRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _workerRepositoryMock.Verify(repo => repo.GetWorkersFieldsById(It.IsAny<WorkerIdAndValueRequestFilter>()), Times.Once);
        }
        #endregion

        #region GetLastWorkerFieldLine Tests

        [Fact]
        public void GetLastWorkerFieldLine_ReturnsWorkersFieldDto_WhenDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workerFieldSample.WorkerId };
            _workerRepositoryMock.Setup(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>())).Returns(_workerFieldSample);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetLastWorkerFieldLine(_workerDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workerFieldSample.WorkerId, result.WorkerId);
            Assert.Equal(_workerFieldSample.Line, result.Line);
            Assert.Equal(_workerFieldSample.FieldName, result.FieldName);
            Assert.Equal(_workerFieldSample.FieldValue, result.FieldValue);
            Assert.Equal(_workerFieldSample.Notes, result.Notes);
            Assert.Equal(_workerFieldSample.HideOnLayout, result.HideOnLayout);
            Assert.Equal(_workerFieldSample.Tbcreated, result.Tbcreated);
            Assert.Equal(_workerFieldSample.Tbmodified, result.Tbmodified);
            Assert.Equal(_workerFieldSample.TbcreatedId, result.TbcreatedId);
            Assert.Equal(_workerFieldSample.TbmodifiedId, result.TbmodifiedId);
            _workerRepositoryMock.Verify(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>()), Times.Once);
        }

        [Fact]
        public void GetLastWorkerFieldLine_ReturnsNull_WhenNoDataExists()
        {
            // Arrange
            var filter = new WorkerIdAndValueRequestFilter { WorkerId = _workerFieldSample.WorkerId };
            _workerRepositoryMock.Setup(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>())).Returns((VwApiWorkersfield)null);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample)).Returns(filter);

            // Act
            var result = _workerService.GetLastWorkerFieldLine(_workerDtoSample);

            // Assert
            Assert.Null(result);
            _workerRepositoryMock.Verify(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>()), Times.Once);
        }
        #endregion

        #region UpdateOrCreateLastLogin Tests

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
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_workerDtoSample))
                .Returns(passwordFilter);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample))
                .Returns(workerIdValueFilter);

            // Setup per GetLastWorkerFieldLine
            _workerRepositoryMock.Setup(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>()))
                .Returns(_workerFieldSample);

            // Setup per CreateOrUpdateLastLogin
            _workerRepositoryMock.Setup(repo => repo.CreateOrUpdateLastLogin(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _workerService.UpdateOrCreateLastLogin(_passwordWorkersRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workerFieldSample.WorkerId, result.WorkerId);
            Assert.Equal(_workerFieldSample.Line, result.Line);
            Assert.Equal(_workerFieldSample.FieldName, result.FieldName);
            Assert.Equal(_workerFieldSample.FieldValue, result.FieldValue);
            Assert.Equal(_workerFieldSample.Notes, result.Notes);
            Assert.Equal(_workerFieldSample.HideOnLayout, result.HideOnLayout);
            Assert.Equal(_workerFieldSample.Tbcreated, result.Tbcreated);
            Assert.Equal(_workerFieldSample.Tbmodified, result.Tbmodified);
            Assert.Equal(_workerFieldSample.TbcreatedId, result.TbcreatedId);
            Assert.Equal(_workerFieldSample.TbmodifiedId, result.TbmodifiedId);

            _workerRepositoryMock.Verify(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>()), Times.Once);
            _workerRepositoryMock.Verify(repo => repo.CreateOrUpdateLastLogin(It.IsAny<PasswordWorkersRequestFilter>()), Times.Once);
            _workerRepositoryMock.Verify(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrCreateLastLogin_ThrowsException_WhenGetWorkerByPasswordFails()
        {
            // Arrange
            var passwordFilter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns((VwApiWorker)null);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample))
                .Returns(passwordFilter);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _workerService.UpdateOrCreateLastLogin(_passwordWorkersRequestDtoSample));
        }
        #endregion

        #region LoginWithPassword Tests

        [Fact]
        public void LoginWithPassword_ReturnsWorkerDto_WhenDataExists()
        {
            // Arrange
            var passwordFilter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            var workerIdValueFilter = new WorkerIdAndValueRequestFilter { WorkerId = _workerSample.WorkerId };

            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns(_workerSample);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample))
                .Returns(passwordFilter);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_workerDtoSample))
                .Returns(passwordFilter);
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample))
                .Returns(workerIdValueFilter);
            _workerRepositoryMock.Setup(repo => repo.CreateOrUpdateLastLogin(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns(Task.CompletedTask);
            _workerRepositoryMock.Setup(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>()))
                .Returns(_workerFieldSample);

            // Act
            var result = _workerService.LoginWithPassword(_passwordWorkersRequestDtoSample);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workerSample.WorkerId, result.WorkerId);
            Assert.Equal(_workerSample.Name, result.Name);
            Assert.Equal(_workerSample.LastName, result.LastName);
            Assert.Equal(_workerSample.Pin, result.Pin);
            Assert.Equal(_workerSample.Password, result.Password);
            Assert.Equal(_workerSample.TipoUtente, result.TipoUtente);
            Assert.Equal(_workerSample.StorageVersamenti, result.StorageVersamenti);
            Assert.Equal(_workerSample.Storage, result.Storage);
            Assert.Equal(_workerSample.LastLogin, result.LastLogin);
        }

        [Fact]
        public void LoginWithPassword_ThrowsException_WhenNoDataExists()
        {
            // Arrange
            var passwordFilter = new PasswordWorkersRequestFilter { Password = _passwordWorkersRequestDtoSample.Password };
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns((VwApiWorker)null);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_passwordWorkersRequestDtoSample))
                .Returns(passwordFilter);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _workerService.LoginWithPassword(_passwordWorkersRequestDtoSample));
        }
        #endregion

        #region GetWorkerByIdAndPassword Tests

        [Fact]
        public void GetWorkerByIdAndPassword_ReturnsWorkerDto_WhenDataExists()
        {
            // Arrange
            var request = _workerIdAndPasswordRequestDtoSample;

            var filter = new WorkerIdAndPasswordFilter { WorkerId = request.WorkerId, Password = request.Password };

            _workerRepositoryMock.Setup(repo => repo.GetWorkerByIdAndPassword(It.IsAny<WorkerIdAndPasswordFilter>()))
                .Returns(_workerSample);
            _mapperMock.Setup(m => m.Map<WorkerIdAndPasswordFilter>(request)).Returns(filter);
            _workerRepositoryMock.Setup(repo => repo.CreateOrUpdateLastLogin(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns(Task.CompletedTask);
            _workerRepositoryMock.Setup(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>()))
                .Returns(_workerFieldSample);
            _mapperMock.Setup(m => m.Map<PasswordWorkersRequestFilter>(_workerDtoSample))
                .Returns(new PasswordWorkersRequestFilter { Password = request.Password });
            _mapperMock.Setup(m => m.Map<WorkerIdAndValueRequestFilter>(_workerDtoSample))
                .Returns(new WorkerIdAndValueRequestFilter { WorkerId = _workerSample.WorkerId, FieldValue = _workerSample.LastLogin });
            _mapperMock.Setup(m => m.Map<WorkerDto>(_workerSample)).Returns(_workerDtoSample);
            _workerRepositoryMock.Setup(repo => repo.GetWorkerByPassword(It.IsAny<PasswordWorkersRequestFilter>()))
                .Returns(_workerSample);

            // Act
            var result = _workerService.GetWorkerByIdAndPassword(request);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(_workerSample.WorkerId, result.WorkerId);
            Assert.Equal(_workerSample.Name, result.Name);
            Assert.Equal(_workerSample.LastName, result.LastName);
            Assert.Equal(_workerSample.Pin, result.Pin);
            Assert.Equal(_workerSample.Password, result.Password);
            Assert.Equal(_workerSample.TipoUtente, result.TipoUtente);
            Assert.Equal(_workerSample.StorageVersamenti, result.StorageVersamenti);
            Assert.Equal(_workerSample.Storage, result.Storage);
            Assert.Equal(_workerSample.LastLogin, result.LastLogin);
            _workerRepositoryMock.Verify(repo => repo.GetWorkerByIdAndPassword(It.IsAny<WorkerIdAndPasswordFilter>()), Times.Once);
            _workerRepositoryMock.Verify(repo => repo.CreateOrUpdateLastLogin(It.IsAny<PasswordWorkersRequestFilter>()), Times.Once);
            _workerRepositoryMock.Verify(repo => repo.GetLastWorkerFieldLine(It.IsAny<WorkerIdAndValueRequestFilter>()), Times.Once);
        }
        #endregion
    }
}