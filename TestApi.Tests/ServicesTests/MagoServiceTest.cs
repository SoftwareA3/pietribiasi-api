using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Xunit;
using Moq;
using apiPB.Services.Implementation;
using apiPB.Services.Abstraction;
using apiPB.ApiClient.Abstraction;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Utils.Abstraction;
using AutoMapper;

namespace TestApi.Tests.ServicesTests
{
    public class MagoServiceTest
    {
        private readonly Mock<IMagoApiClient> _mockMagoApiClient;
        private readonly Mock<IRegOreRequestService> _mockRegOreRequestService;
        private readonly Mock<IPrelMatRequestService> _mockPrelMatRequestService;
        private readonly Mock<IInventarioRequestService> _mockInventarioRequestService;
        private readonly Mock<ISettingsRepository> _mockSettingsRepository;
        private readonly Mock<ILogService> _mockLogService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MagoRequestService _magoRequestService;

        public MagoServiceTest()
        {
            _mockMagoApiClient = new Mock<IMagoApiClient>();
            _mockRegOreRequestService = new Mock<IRegOreRequestService>();
            _mockPrelMatRequestService = new Mock<IPrelMatRequestService>();
            _mockInventarioRequestService = new Mock<IInventarioRequestService>();
            _mockSettingsRepository = new Mock<ISettingsRepository>();
            _mockLogService = new Mock<ILogService>();
            _mockMapper = new Mock<IMapper>();

            _magoRequestService = new MagoRequestService(
                _mockMagoApiClient.Object,
                _mockMapper.Object,
                _mockSettingsRepository.Object,
                _mockRegOreRequestService.Object,
                _mockPrelMatRequestService.Object,
                _mockInventarioRequestService.Object,
                _mockLogService.Object
            );
        }

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_FailedRequest_ThrowsException()
        {
            // Arrange
            var request = new MagoLoginRequestDto { Username = "test", Password = "test" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            
            _mockMagoApiClient.Setup(x => x.SendPostAsync("account-manager/login", request))
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _magoRequestService.LoginAsync(request));
            Assert.Equal("Login failed", exception.Message);
        }

        #endregion

        #region LoginWithWorkerIdAsync Tests

        [Fact]
        public async Task LoginWithWorkerIdAsync_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _magoRequestService.LoginWithWorkerIdAsync(null));
        }

        [Fact]
        public async Task LoginWithWorkerIdAsync_SettingsNotFound_ThrowsException()
        {
            // Arrange
            var request = new WorkerIdSyncRequestDto { WorkerId = 1 };
            _mockSettingsRepository.Setup(x => x.GetSettings()).Returns((SettingsDto)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _magoRequestService.LoginWithWorkerIdAsync(request));
            Assert.Equal("Settings not found", exception.Message);
        }

        #endregion

        #region LogoffAsync Tests

        [Fact]
        public async Task LogoffAsync_SuccessfulLogoff_CompletesSuccessfully()
        {
            // Arrange
            var dto = new MagoTokenRequestDto { Token = "test-token" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            
            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken<MagoTokenRequestDto>(
                "account-manager/logout", 
                It.IsAny<List<MagoTokenRequestDto>>(), 
                "test-token", It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);

            // Act
            await _magoRequestService.LogoffAsync(dto);

            // Assert
            _mockMagoApiClient.Verify(x => x.SendPostAsyncWithToken<MagoTokenRequestDto>(
                "account-manager/logout", 
                It.IsAny<List<MagoTokenRequestDto>>(), 
                "test-token", It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task LogoffAsync_FailedLogoff_ThrowsHttpRequestException()
        {
            // Arrange
            var dto = new MagoTokenRequestDto { Token = "test-token" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            
            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken<MagoTokenRequestDto>(
                "account-manager/logout", 
                It.IsAny<List<MagoTokenRequestDto>>(), 
                "test-token", It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _magoRequestService.LogoffAsync(dto));
        }

        #endregion

        #region SyncRegOre Tests

        [Fact]
        public async Task SyncRegOre_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _magoRequestService.SyncRegOre(null, "token"));
        }

        [Fact]
        public async Task SyncRegOre_NullToken_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new List<SyncRegOreRequestDto> { new SyncRegOreRequestDto() };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _magoRequestService.SyncRegOre(request, null));
        }

        [Fact]
        public async Task SyncRegOre_SuccessfulSync_ReturnsHttpResponse()
        {
            // Arrange
            var request = new List<SyncRegOreRequestDto> { new SyncRegOreRequestDto() };
            var token = "test-token";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            
            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/mo-confirmation", request, token, It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _magoRequestService.SyncRegOre(request, token);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            _mockMagoApiClient.Verify(x => x.SendPostAsyncWithToken("openMes/mo-confirmation", request, token, It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task SyncRegOre_FailedSync_ThrowsException()
        {
            // Arrange
            var request = new List<SyncRegOreRequestDto> { new SyncRegOreRequestDto() };
            var token = "test-token";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            
            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/mo-confirmation", request, token, It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _magoRequestService.SyncRegOre(request, token));
            Assert.Equal("SyncRegOre failed", exception.Message);
        }

        #endregion

        #region SyncPrelMat Tests

        [Fact]
        public async Task SyncPrelMat_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _magoRequestService.SyncPrelMat(null, "token"));
        }

        [Fact]
        public async Task SyncPrelMat_SuccessfulSync_ReturnsHttpResponse()
        {
            // Arrange
            var request = new List<SyncPrelMatRequestDto> { new SyncPrelMatRequestDto() };
            var token = "test-token";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            
            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/materials-picking", request, token, It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _magoRequestService.SyncPrelMat(request, token);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion

        #region SyncInventario Tests

        [Fact]
        public async Task SyncInventario_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _magoRequestService.SyncInventario(null, "token"));
        }

        [Fact]
        public async Task SyncInventario_SuccessfulSync_ReturnsHttpResponse()
        {
            // Arrange
            var request = new List<SyncInventarioRequestDto> { new SyncInventarioRequestDto() };
            var token = "test-token";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            
            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("ERPInventory/ImportInventoryEntries", request, token, false))
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _magoRequestService.SyncInventario(request, token);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        #endregion

        #region SyncRegOreFiltered Tests

        [Fact]
        public async Task SyncRegOreFiltered_EmptyToken_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "" };
            var settings = new SettingsDto();
            var request = new SyncRegOreFilteredDto { WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 } };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _magoRequestService.SyncRegOreFiltered(responseDto, settings, request, true));
        }

        [Fact]
        public async Task SyncRegOreFiltered_NoFilterApplied_RetrievesAllRecords()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncRegOreFilteredDto 
            { 
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                RegOreList = new List<RegOreDto>()
            };
            
            var regOreList = new List<RegOreDto> { new RegOreDto { RegOreId = 1 } };
            
            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Returns(regOreList);

            // Act & Assert
            // Questo test richiederà ulteriori setup per il mapping e l'API call
            // La logica completa dipende dalle implementazioni di ToSyncregOreRequestDto
        }

        #endregion

        #region SyncronizeAsync Tests

        [Fact]
        public async Task SyncronizeAsync_AllSyncOperationsSucceed_ReturnsSyncronizedDataDto()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var requestId = new WorkerIdSyncRequestDto { WorkerId = 1 };

            // Setup per tutti i servizi di sync
            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Returns(new List<RegOreDto>());
            _mockPrelMatRequestService.Setup(x => x.GetNotImportedPrelMat())
                .Returns(new List<PrelMatDto>());
            _mockInventarioRequestService.Setup(x => x.GetNotImportedInventario())
                .Returns(new List<InventarioDto>());

            // Act
            var result = await _magoRequestService.SyncronizeAsync(responseDto, settings, requestId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SyncronizedDataDto>(result);
        }

        [Fact]
        public async Task SyncronizeAsync_ExceptionThrown_LogsErrorAndRethrows()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var requestId = new WorkerIdSyncRequestDto { WorkerId = 1 };

            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Throws(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _magoRequestService.SyncronizeAsync(responseDto, settings, requestId));

            _mockLogService.Verify(x => x.AppendErrorToLog(It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}