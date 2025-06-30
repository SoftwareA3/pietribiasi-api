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
using System.Text.Json;

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

        private readonly IEnumerable<SyncRegOreRequestDto> _syncRegOreRequests = new List<SyncRegOreRequestDto>
        {
            new SyncRegOreRequestDto
            {
                Closed = false,
                WorkerId = 1,
                MoId = 1001,
                RtgStep = 1,
                Alternate = "A1",
                AltRtgStep = 2,
                ProducedQty = 10,
                SecondRateQty = 5,
                ScrapQty = 2,
                ActualProcessingTime = new TimeSpan(1, 30, 0),
                ActualSetupTime = new TimeSpan(0, 15, 0),
                WorkerProcessingTime = new TimeSpan(1, 0, 0),
                WorkerSetupTime = new TimeSpan(0, 10, 0),
                SecondRate = "SR1",
                SecondRateStorage = "SEDE",
                SecondRateVariant = "VAR1",
                SecondRateNonConformityReason = "None",
                Scrap = "SCRAP1",
                ScrapStorage = "SCRAP_SEDE",
                ScrapVariant = "SCRAP_VAR1",
                ScrapNonConformityReason = "None",
                ProductionLotNumber = "LOT123",
                Variant = "VAR123",
                Bom = "BOM123",
                Storage = "STORAGE123",
                Wc = "WC123",
            },
            new SyncRegOreRequestDto
            {
                Closed = true,
                WorkerId = 2,
                MoId = 1002,
                RtgStep = 2,
                Alternate = "A2",
                AltRtgStep = 3,
                ProducedQty = 20,
                SecondRateQty = 10,
                ScrapQty = 5,
                ActualProcessingTime = new TimeSpan(2, 0, 0),
                ActualSetupTime = new TimeSpan(0, 30, 0),
                WorkerProcessingTime = new TimeSpan(1, 45, 0),
                WorkerSetupTime = new TimeSpan(0, 20, 0),
                SecondRate = "SR2",
                SecondRateStorage = "SEDE2",
                SecondRateVariant = "VAR2",
                SecondRateNonConformityReason = "None",
                Scrap = "SCRAP2",
                ScrapStorage = "SCRAP_SEDE2",
                ScrapVariant = "SCRAP_VAR2",
                ScrapNonConformityReason = "None",
                ProductionLotNumber = "LOT456",
                Variant = "VAR456",
                Bom = "BOM456",
                Storage = "STORAGE456",
                Wc = "WC456",
            }
        };

        private readonly IEnumerable<SyncPrelMatRequestDto> _syncPrelMatRequests = new List<SyncPrelMatRequestDto>
        {
            new SyncPrelMatRequestDto
            {
                MoId = 2001,
                RtgStep = 1,
                Alternate = "A1",
                AltRtgStep = 2,
                WorkerId = 1,
                ActionDetails = new List<SyncPrelMatDetailsRequestdto>
                {
                    new SyncPrelMatDetailsRequestdto
                    {
                        Position = 1,
                        PickedQty = 5,
                        Closed = false,
                        SpecificatorType = 1,
                        Specificator = "SPEC1",
                        Storage = "STORAGE1",
                        Lot = "LOT1",
                    }
                }
            },
            new SyncPrelMatRequestDto
            {
                MoId = 2002,
                RtgStep = 2,
                Alternate = "A2",
                AltRtgStep = 3,
                WorkerId = 2,
                ActionDetails = new List<SyncPrelMatDetailsRequestdto>
                {
                    new SyncPrelMatDetailsRequestdto
                    {
                        Position = 2,
                        PickedQty = 10,
                        Closed = true,
                        SpecificatorType = 2,
                        Specificator = "SPEC2",
                        Storage = "STORAGE2",
                        Lot = "LOT2",
                    }
                }
            }
        };

        private readonly IEnumerable<SyncInventarioRequestDto> _syncInventarioRequests = new List<SyncInventarioRequestDto>
        {
            new SyncInventarioRequestDto
            {
                MA_InventoryEntries = new MA_InventoryEntries
                {
                    InvRsn = "Reason1",
                    PostingDate = "2023-10-01",
                    PreprintedDocNo = "Doc123",
                    DocumentDate = "2023-10-01",
                    StoragePhase1 = "Storage1"
                },
                MA_InventoryEntriesDetail = new List<MA_InventoryEntriesDetail>
                {
                    new MA_InventoryEntriesDetail
                    {
                        Item = "ITEM1",
                        Qty = 100,
                        UoM = "PCS",
                        UnitValue = 0,
                        DocumentType = 3801188
                    }
                }
            },
            new SyncInventarioRequestDto
            {
                MA_InventoryEntries = new MA_InventoryEntries
                {
                    InvRsn = "Reason2",
                    PostingDate = "2023-10-02",
                    PreprintedDocNo = "Doc456",
                    DocumentDate = "2023-10-02",
                    StoragePhase1 = "Storage2"
                },
                MA_InventoryEntriesDetail = new List<MA_InventoryEntriesDetail>
                {
                    new MA_InventoryEntriesDetail
                    {
                        Item = "ITEM2",
                        Qty = 200,
                        UoM = "PCS",
                        UnitValue = 0,
                        DocumentType = 3801188
                    }
                }
            }
        };

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
        public async Task LoginAsync_SuccessfulLogin_ReturnsLoginResponse()
        {
            // Arrange
            var request = new MagoLoginRequestDto { Username = "test", Password = "test" };
            var expectedResponse = new MagoLoginResponseDto { Token = "test-token" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            var jsonContent = JsonSerializer.Serialize(expectedResponse);
            httpResponse.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            _mockMagoApiClient.Setup(x => x.SendPostAsync("account-manager/login", request))
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _magoRequestService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Token, result.Token);
        }

        [Fact]
        public async Task LoginAsync_SuccessfulRequestButNullContent_ThrowsException()
        {
            // Arrange
            var request = new MagoLoginRequestDto { Username = "test", Password = "test" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponse.Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json");

            _mockMagoApiClient.Setup(x => x.SendPostAsync("account-manager/login", request))
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _magoRequestService.LoginAsync(request));
            Assert.Equal("Login response is null", exception.Message);
        }

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

        [Fact]
        public async Task LoginWithWorkerIdAsync_SuccessfulLogin_ReturnsLoginResponseAndSettings()
        {
            // Arrange
            var request = new WorkerIdSyncRequestDto { WorkerId = 1 };
            var settings = new SettingsDto();
            var loginResponse = new MagoLoginResponseDto { Token = "test-token" };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            var jsonContent = JsonSerializer.Serialize(loginResponse);
            httpResponse.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            _mockSettingsRepository.Setup(x => x.GetSettings()).Returns(settings);
            _mockMagoApiClient.Setup(x => x.SendPostAsync("account-manager/login", It.IsAny<MagoLoginRequestDto>()))
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _magoRequestService.LoginWithWorkerIdAsync(request);

            // Assert
            Assert.NotNull(result.LoginResponse);
            Assert.NotNull(result.Settings);
            Assert.Equal(loginResponse.Token, result.LoginResponse.Token);
            Assert.Equal(settings, result.Settings);
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
        public async Task SyncPrelMat_NullToken_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new List<SyncPrelMatRequestDto> { new SyncPrelMatRequestDto() };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncPrelMat(request, null));
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

        [Fact]
        public async Task SyncPrelMat_FailedSync_ThrowsException()
        {
            // Arrange
            var request = new List<SyncPrelMatRequestDto> { new SyncPrelMatRequestDto() };
            var token = "test-token";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/materials-picking", request, token, It.IsAny<bool>()))
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _magoRequestService.SyncPrelMat(request, token));
            Assert.Equal("SyncPrelMat failed", exception.Message);
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
        public async Task SyncInventario_NullToken_ThrowsArgumentNullException()
        {
            // Arrange
            var request = new List<SyncInventarioRequestDto> { new SyncInventarioRequestDto() };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncInventario(request, null));
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

        [Fact]
        public async Task SyncInventario_FailedSync_ThrowsException()
        {
            // Arrange
            var request = new List<SyncInventarioRequestDto> { new SyncInventarioRequestDto() };
            var token = "test-token";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("ERPInventory/ImportInventoryEntries", request, token, false))
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _magoRequestService.SyncInventario(request, token));
            Assert.Equal("SyncInventario failed", exception.Message);
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
        public async Task SyncRegOreFiltered_NullSettings_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var request = new SyncRegOreFilteredDto { WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 } };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncRegOreFiltered(responseDto, null, request, true));
        }

        [Fact]
        public async Task SyncRegOreFiltered_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncRegOreFiltered(responseDto, settings, null, true));
        }

        [Fact]
        public async Task SyncRegOreFiltered_FilteredWithData_ProcessesFilteredList()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var regOreList = new List<RegOreDto> { new RegOreDto { RegOreId = 1 } };
            var request = new SyncRegOreFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                RegOreList = regOreList
            };

            var syncRegOreList = new List<SyncRegOreRequestDto> { new SyncRegOreRequestDto() };

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/mo-confirmation",
                It.IsAny<List<SyncRegOreRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _mockRegOreRequestService.Setup(x => x.UpdateImportedById(It.IsAny<UpdateImportedIdRequestDto>()));
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncRegOreRequestDto>>(It.IsAny<List<RegOreDto>>()))
                .Returns(syncRegOreList);

            // Act
            var result = await _magoRequestService.SyncRegOreFiltered(responseDto, settings, request, true);

            // Assert
            Assert.NotNull(result);
            _mockRegOreRequestService.Verify(x => x.UpdateImportedById(It.IsAny<UpdateImportedIdRequestDto>()), Times.Once);
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
            var syncRegOreList = new List<SyncRegOreRequestDto> { new SyncRegOreRequestDto() };

            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Returns(regOreList);

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/mo-confirmation",
                It.IsAny<List<SyncRegOreRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _mockRegOreRequestService.Setup(x => x.UpdateRegOreImported(It.IsAny<WorkerIdSyncRequestDto>()))
                .Returns(new List<RegOreDto>());
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncRegOreRequestDto>>(It.IsAny<List<RegOreDto>>()))
                .Returns(syncRegOreList);

            // Act
            var result = await _magoRequestService.SyncRegOreFiltered(responseDto, settings, request, false);

            // Assert
            Assert.NotNull(result);
            _mockRegOreRequestService.Verify(x => x.GetNotImportedAppRegOre(), Times.Once);
            _mockRegOreRequestService.Verify(x => x.UpdateRegOreImported(It.IsAny<WorkerIdSyncRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task SyncRegOreFiltered_NoRecordsFound_ThrowsException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncRegOreFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                RegOreList = new List<RegOreDto>()
            };

            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Returns((List<RegOreDto>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => 
                _magoRequestService.SyncRegOreFiltered(responseDto, settings, request, false));
            Assert.StartsWith("Value cannot be null.", exception.Message); 
        }

        [Fact]
        public async Task SyncRegOreFiltered_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncRegOreFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                RegOreList = new List<RegOreDto>()
            };

            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Returns(new List<RegOreDto> { });

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/mo-confirmation",
                It.IsAny<List<SyncRegOreRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act 
            var result = await _magoRequestService.SyncRegOreFiltered(responseDto, settings, request, false);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region SyncPrelMatFiltered Tests

        [Fact]
        public async Task SyncPrelMatFiltered_EmptyToken_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "" };
            var settings = new SettingsDto();
            var request = new SyncPrelMatFilteredDto { WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 } };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncPrelMatFiltered(responseDto, settings, request, true));
        }

        [Fact]
        public async Task SyncPrelMatFiltered_NullSettings_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var request = new SyncPrelMatFilteredDto { WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 } };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncPrelMatFiltered(responseDto, null, request, true));
        }

        [Fact]
        public async Task SyncPrelMatFiltered_FilteredWithData_ProcessesFilteredList()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var prelMatList = new List<PrelMatDto> { new PrelMatDto { PrelMatId = 1 } };
            var request = new SyncPrelMatFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                PrelMatList = prelMatList
            };

            var syncPrelMatList = new List<SyncPrelMatRequestDto> { new SyncPrelMatRequestDto() };

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/materials-picking",
                It.IsAny<List<SyncPrelMatRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            _mockPrelMatRequestService.Setup(x => x.UpdateImportedById(It.IsAny<UpdateImportedIdRequestDto>()));
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncPrelMatRequestDto>>(It.IsAny<List<PrelMatDto>>()))
                .Returns(syncPrelMatList);

            // Act
            var result = await _magoRequestService.SyncPrelMatFiltered(responseDto, settings, request, true);

            // Assert
            Assert.NotNull(result);
            _mockPrelMatRequestService.Verify(x => x.UpdateImportedById(It.IsAny<UpdateImportedIdRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task SyncPrelMatFiltered_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncPrelMatFiltered(responseDto, settings, null, true));
        }

        [Fact]
        public async Task SyncPrelMatFiltered_NoFilterApplied_RetrievesAllRecords()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncPrelMatFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                PrelMatList = new List<PrelMatDto>()
            };

            var prelMatList = new List<PrelMatDto> { new PrelMatDto { PrelMatId = 1 } };
            var syncPrelMatList = new List<SyncPrelMatRequestDto> { new SyncPrelMatRequestDto() };

            _mockPrelMatRequestService.Setup(x => x.GetNotImportedPrelMat())
                .Returns(prelMatList);

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/materials-picking",
                It.IsAny<List<SyncPrelMatRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _mockPrelMatRequestService.Setup(x => x.UpdatePrelMatImported(It.IsAny<WorkerIdSyncRequestDto>()))
                .Returns(new List<PrelMatDto>());
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncPrelMatRequestDto>>(It.IsAny<List<PrelMatDto>>()))
                .Returns(syncPrelMatList);

            // Act
            var result = await _magoRequestService.SyncPrelMatFiltered(responseDto, settings, request, false);

            // Assert
            Assert.NotNull(result);
            _mockPrelMatRequestService.Verify(x => x.GetNotImportedPrelMat(), Times.Once);
            _mockPrelMatRequestService.Verify(x => x.UpdatePrelMatImported(It.IsAny<WorkerIdSyncRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task SyncPrelMatFiltered_NoRecordsFound_ThrowsException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncPrelMatFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                PrelMatList = new List<PrelMatDto>()
            };

            _mockPrelMatRequestService.Setup(x => x.GetNotImportedPrelMat())
                .Returns((List<PrelMatDto>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncPrelMatFiltered(responseDto, settings, request, false));
            Assert.StartsWith("Value cannot be null.", exception.Message);
        }

        [Fact]
        public async Task SyncPrelMatFiltered_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncPrelMatFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                PrelMatList = new List<PrelMatDto>()
            };

            _mockPrelMatRequestService.Setup(x => x.GetNotImportedPrelMat())
                .Returns(new List<PrelMatDto> { });

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("openMes/materials-picking",
                It.IsAny<List<SyncPrelMatRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            var result = await _magoRequestService.SyncPrelMatFiltered(responseDto, settings, request, false);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region SyncInventarioFiltered Tests

        [Fact]
        public async Task SyncInventarioFiltered_EmptyToken_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "" };
            var settings = new SettingsDto();
            var request = new SyncInventarioFilteredDto { WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 } };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncInventarioFiltered(responseDto, settings, request, true));
        }

        [Fact]
        public async Task SyncInventarioFiltered_NullSettings_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var request = new SyncInventarioFilteredDto { WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 } };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncInventarioFiltered(responseDto, null, request, true));
        }

        [Fact]
        public async Task SyncInventarioFiltered_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncInventarioFiltered(responseDto, settings, null, true));
        }

        [Fact]
        public async Task SyncInventarioFiltered_FilteredWithData_ProcessesFilteredList()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var inventarioList = new List<InventarioDto> { new InventarioDto { InvId = 1 } };
            var request = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                InventarioList = inventarioList
            };

            var syncInventarioList = new List<SyncInventarioRequestDto> { new SyncInventarioRequestDto() };

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("ERPInventory/ImportInventoryEntries",
                It.IsAny<List<SyncInventarioRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            _mockInventarioRequestService.Setup(x => x.UpdateImportedById(It.IsAny<UpdateImportedIdRequestDto>())).Returns(new InventarioDto());
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncInventarioRequestDto>>(It.IsAny<List<InventarioDto>>()))
                .Returns(syncInventarioList);

            // Act
            var result = await _magoRequestService.SyncInventarioFiltered(responseDto, settings, request, true);

            // Assert
            Assert.NotNull(result);
            _mockInventarioRequestService.Verify(x => x.UpdateImportedById(It.IsAny<UpdateImportedIdRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task SyncInventarioFiltered_NoFilterApplied_RetrievesAllRecords()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                InventarioList = new List<InventarioDto>()
            };

            var inventarioList = new List<InventarioDto> { new InventarioDto { InvId = 1 } };
            var syncInventarioList = new List<SyncInventarioRequestDto> { new SyncInventarioRequestDto() };

            _mockInventarioRequestService.Setup(x => x.GetNotImportedInventario())
                .Returns(inventarioList);

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("ERPInventory/ImportInventoryEntries",
                It.IsAny<List<SyncInventarioRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _mockInventarioRequestService.Setup(x => x.UpdateInventarioImported(It.IsAny<WorkerIdSyncRequestDto>()))
                .Returns(new List<InventarioDto>());
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncInventarioRequestDto>>(It.IsAny<List<InventarioDto>>()))
                .Returns(syncInventarioList);

            // Act
            var result = await _magoRequestService.SyncInventarioFiltered(responseDto, settings, request, false);

            // Assert
            Assert.NotNull(result);
            _mockInventarioRequestService.Verify(x => x.GetNotImportedInventario(), Times.Once);
            _mockInventarioRequestService.Verify(x => x.UpdateInventarioImported(It.IsAny<WorkerIdSyncRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task SyncInventarioFiltered_NoRecordsFound_ThrowsException()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                InventarioList = new List<InventarioDto>()
            };

            _mockInventarioRequestService.Setup(x => x.GetNotImportedInventario())
                .Returns((List<InventarioDto>)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _magoRequestService.SyncInventarioFiltered(responseDto, settings, request, false));
            Assert.StartsWith("Value cannot be null.", exception.Message);
        }

        [Fact]
        public async Task SyncInventarioFiltered_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var request = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
                InventarioList = new List<InventarioDto>()
            };

            _mockInventarioRequestService.Setup(x => x.GetNotImportedInventario())
                .Returns(new List<InventarioDto> { });

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken("ERPInventory/ImportInventoryEntries",
                It.IsAny<List<SyncInventarioRequestDto>>(),
                responseDto.Token,
                It.IsAny<bool>())).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            var result = await _magoRequestService.SyncInventarioFiltered(responseDto, settings, request, false);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region SyncronizeAsync Tests

        [Fact]
        public async Task SynchronizeAsync_SuccessfulSynchronization_ReturnsSyncronizedDataDto()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var requestId = new WorkerIdSyncRequestDto { WorkerId = 1 };

            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Returns(new List<RegOreDto> { new RegOreDto { RegOreId = 1 } });
            _mockPrelMatRequestService.Setup(x => x.GetNotImportedPrelMat())
                .Returns(new List<PrelMatDto> { new PrelMatDto { PrelMatId = 1 } });
            _mockInventarioRequestService.Setup(x => x.GetNotImportedInventario())
                .Returns(new List<InventarioDto> { new InventarioDto { InvId = 1 } });

            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken(It.IsAny<string>(),
                It.IsAny<IEnumerable<SyncRegOreRequestDto>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken(It.IsAny<string>(),
                It.IsAny<IEnumerable<SyncPrelMatRequestDto>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            _mockMagoApiClient.Setup(x => x.SendPostAsyncWithToken(It.IsAny<string>(),
                It.IsAny<IEnumerable<SyncInventarioRequestDto>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            _mockRegOreRequestService.Setup(x => x.UpdateRegOreImported(It.IsAny<WorkerIdSyncRequestDto>()))
                .Returns(new List<RegOreDto>());
            _mockPrelMatRequestService.Setup(x => x.UpdatePrelMatImported(It.IsAny<WorkerIdSyncRequestDto>()))
                .Returns(new List<PrelMatDto>());
            _mockInventarioRequestService.Setup(x => x.UpdateInventarioImported(It.IsAny<WorkerIdSyncRequestDto>()))
                .Returns(new List<InventarioDto>());

            _mockMapper.Setup(m => m.Map<IEnumerable<SyncRegOreRequestDto>>(It.IsAny<List<RegOreDto>>()))
                .Returns(new List<SyncRegOreRequestDto> { new SyncRegOreRequestDto() });
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncPrelMatRequestDto>>(It.IsAny<List<PrelMatDto>>()))
                .Returns(new List<SyncPrelMatRequestDto> { new SyncPrelMatRequestDto() });
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncInventarioRequestDto>>(It.IsAny<List<InventarioDto>>()))
                .Returns(new List<SyncInventarioRequestDto> { new SyncInventarioRequestDto() });

            // Act
            var result = await _magoRequestService.SyncronizeAsync(responseDto, settings, requestId);

            // Assert
            Assert.NotNull(result);
            _mockRegOreRequestService.Verify(x => x.GetNotImportedAppRegOre(), Times.Once);
            _mockPrelMatRequestService.Verify(x => x.GetNotImportedPrelMat(), Times.Once);
            _mockInventarioRequestService.Verify(x => x.GetNotImportedInventario(), Times.Once);
            _mockMagoApiClient.Verify(x => x.SendPostAsyncWithToken("openMes/mo-confirmation", It.IsAny<IEnumerable<SyncRegOreRequestDto>>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _mockMagoApiClient.Verify(x => x.SendPostAsyncWithToken("openMes/materials-picking", It.IsAny<IEnumerable<SyncPrelMatRequestDto>>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _mockMagoApiClient.Verify(x => x.SendPostAsyncWithToken("ERPInventory/ImportInventoryEntries", It.IsAny<IEnumerable<SyncInventarioRequestDto>>(), It.IsAny<string>(), It.IsAny<bool>()), Times.AtLeastOnce); // Called in a loop
            _mockRegOreRequestService.Verify(x => x.UpdateRegOreImported(It.IsAny<WorkerIdSyncRequestDto>()), Times.Once);
            _mockPrelMatRequestService.Verify(x => x.UpdatePrelMatImported(It.IsAny<WorkerIdSyncRequestDto>()), Times.Once);
            _mockInventarioRequestService.Verify(x => x.UpdateInventarioImported(It.IsAny<WorkerIdSyncRequestDto>()), Times.Once);
        }

        [Fact]
        public async Task SynchronizeAsync_SyncRegOreFilteredThrowsException_LogsErrorAndThrows()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var requestId = new WorkerIdSyncRequestDto { WorkerId = 1 };

            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Throws(new Exception("RegOre sync failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _magoRequestService.SyncronizeAsync(responseDto, settings, requestId));
            Assert.Equal("RegOre sync failed", exception.Message);
            _mockLogService.Verify(x => x.AppendErrorToLog(It.Is<string>(s => s.Contains("Error in SynchronizeAsync"))), Times.Once);
        }

        [Fact]
        public async Task SynchronizeAsync_SyncPrelMatFilteredThrowsException_LogsErrorAndThrows()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var requestId = new WorkerIdSyncRequestDto { WorkerId = 1 };

            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Returns(new List<RegOreDto>());
            _mockPrelMatRequestService.Setup(x => x.GetNotImportedPrelMat())
                .Throws(new Exception("PrelMat sync failed"));

            _mockMapper.Setup(m => m.Map<IEnumerable<SyncRegOreRequestDto>>(It.IsAny<List<RegOreDto>>()))
                .Returns(new List<SyncRegOreRequestDto>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _magoRequestService.SyncronizeAsync(responseDto, settings, requestId));
            Assert.Equal("PrelMat sync failed", exception.Message);
            _mockLogService.Verify(x => x.AppendErrorToLog(It.Is<string>(s => s.Contains("Error in SynchronizeAsync"))), Times.Once);
        }

        [Fact]
        public async Task SynchronizeAsync_SyncInventarioFilteredThrowsException_LogsErrorAndThrows()
        {
            // Arrange
            var responseDto = new MagoLoginResponseDto { Token = "test-token" };
            var settings = new SettingsDto();
            var requestId = new WorkerIdSyncRequestDto { WorkerId = 1 };

            _mockRegOreRequestService.Setup(x => x.GetNotImportedAppRegOre())
                .Returns(new List<RegOreDto>());
            _mockPrelMatRequestService.Setup(x => x.GetNotImportedPrelMat())
                .Returns(new List<PrelMatDto>());
            _mockInventarioRequestService.Setup(x => x.GetNotImportedInventario())
                .Throws(new Exception("Inventario sync failed"));

            _mockMapper.Setup(m => m.Map<IEnumerable<SyncRegOreRequestDto>>(It.IsAny<List<RegOreDto>>()))
                .Returns(new List<SyncRegOreRequestDto>());
            _mockMapper.Setup(m => m.Map<IEnumerable<SyncPrelMatRequestDto>>(It.IsAny<List<PrelMatDto>>()))
                .Returns(new List<SyncPrelMatRequestDto>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _magoRequestService.SyncronizeAsync(responseDto, settings, requestId));
            Assert.Equal("Inventario sync failed", exception.Message);
            _mockLogService.Verify(x => x.AppendErrorToLog(It.Is<string>(s => s.Contains("Error in SynchronizeAsync"))), Times.Once);
        }

        #endregion
    }
}