using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using apiPB.Controllers;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Services.Abstraction;
using apiPB.Utils.Abstraction;
using apiPB.Utils.Implementation;

namespace TestApi.Tests.ControllerTests
{
    public class MagoControllerTests
    {
        private readonly Mock<IMagoRequestService> _magoRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
        private readonly MagoController _controller;

        private readonly MagoLoginResponseDto _testMagoLoginResponse = new MagoLoginResponseDto
        {
            Token = "test_token",
            Subscription = "test_subscription_id",
            IsAdmin = true,
        };

        private readonly MagoLoginRequestDto _testMagoLoginRequest = new MagoLoginRequestDto
        {
            Username = "test_user",
            Password = "test_password",
            Company = "test_company",
        };

        private readonly MagoLoginResponseDto _testMagoLoginResponseWithError = new MagoLoginResponseDto
        {
            Token = null,
            Subscription = null,
            IsAdmin = false,
        };

        private readonly MagoTokenRequestDto _testMagoTokenRequest = new MagoTokenRequestDto
        {
            Token = "test_token",
        };

        private readonly WorkerIdSyncRequestDto _testWorkerIdSyncRequest = new WorkerIdSyncRequestDto
        {
            WorkerId = 1
        };

        private readonly SettingsDto _testSettingsDto = new SettingsDto
        {
            MagoUrl = "http://example.com",
            Username = "testUser",
            Password = "testPassword",
            Company = "TestCompany",
            SpecificatorType = 1,
            Closed = false,
            RectificationReasonPositive = "Positive Reason",
            RectificationReasonNegative = "Negative Reason",
            Storage = "TestStorage",
            SyncGlobalActive = true
        };

        private readonly SyncRegOreRequestDto _testSyncRegOreRequest = new SyncRegOreRequestDto
        {
            Closed = false,
            WorkerId = 1,
            MoId = 123,
            RtgStep = 2,
            Alternate = "Alt1",
            AltRtgStep = 3,
            ProducedQty = 100,
            SecondRateQty = 10,
            ScrapQty = 5,
            ActualProcessingTime = TimeSpan.FromHours(2),
            ActualSetupTime = TimeSpan.FromHours(1),
            WorkerProcessingTime = TimeSpan.FromHours(2),
            WorkerSetupTime = TimeSpan.FromHours(1),
            SecondRate = "SR1",
            SecondRateStorage = "SEDE",
            SecondRateVariant = "Variant1",
            SecondRateNonConformityReason = "NonConformity1",
            Scrap = "Scrap1",
            ScrapStorage = "Storage1",
            ScrapVariant = "Variant2",
            ScrapNonConformityReason = "NonConformity2",
            ProductionLotNumber = "Lot123",
            Variant = "Variant1",
            Bom = "BOM123",
            Storage = "Storage1",
            Wc = "WC1"
        };

        private readonly SyncPrelMatRequestDto _testSyncPrelMatRequest = new SyncPrelMatRequestDto
        {
            MoId = 123,
            WorkerId = 1,
            ActionDetails = new List<SyncPrelMatDetailsRequestdto>
            {
                new SyncPrelMatDetailsRequestdto
                {
                    Position = 1,
                    PickedQty = 10.5,
                    Closed = false,
                    SpecificatorType = 1,
                    Specificator = "Spec1",
                    Storage = "Storage1",
                    Lot = "Lot1"
                },
            }
        };

        private readonly SyncRegOreFilteredDto _testSyncRegOreFilteredDto = new SyncRegOreFilteredDto
        {
            WorkerIdSyncRequestDto = new WorkerIdSyncRequestDto { WorkerId = 1 },
            RegOreList = new List<RegOreDto>
            {
                new RegOreDto
                {
                    RegOreId = 1,
                    WorkerId = 43,
                    SavedDate = new DateTime(2023, 10, 1, 12, 0, 0),
                    Job = "JOB123",
                    RtgStep = 1,
                    Alternate = "ALT1",
                    AltRtgStep = 2,
                    Operation = "OP123",
                    OperDesc = "Operation Description",
                    Bom = "BOM123",
                    Variant = "VAR1",
                    ItemDesc = "Item Description",
                    Moid = 100,
                    Mono = "MO123",
                    CreationDate = new DateTime(2023, 9, 1),
                    Uom = "HR",
                    ProductionQty = 100,
                    ProducedQty = 75,
                    ResQty = 25,
                    Storage = "WH01",
                    Wc = "WC01",
                    WorkingTime = 480,
                    Imported = false,
                    UserImp = "User123",
                    DataImp = new DateTime(2023, 10, 1, 12, 0, 0),
                }
            }
        };

        private readonly SyncInventarioRequestDto _testSyncInventarioRequest = new SyncInventarioRequestDto
        {
            MA_InventoryEntries = new MA_InventoryEntries
            {
                InvRsn = "TestReason",
                PostingDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                PreprintedDocNo = "Doc123",
                DocumentDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                StoragePhase1 = "Storage1"
            },
            MA_InventoryEntriesDetail = new List<MA_InventoryEntriesDetail>
            {
                new MA_InventoryEntriesDetail
                {
                    Item = "Item123",
                    Qty = 100.0,
                    UoM = "pcs",
                    UnitValue = 0,
                    DocumentType = 3801188
                }
            }
        };

        private readonly (MagoLoginResponseDto, SettingsDto) _loginResponseTuple;

        private readonly SyncronizedDataDto _testSyncronizedData;

        public MagoControllerTests()
        {
            _magoRequestServiceMock = new Mock<IMagoRequestService>();
            _responseHandlerMock = new Mock<IResponseHandler>();
            _controller = new MagoController(_magoRequestServiceMock.Object, _responseHandlerMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();

            httpContextMock.Setup(ctx => ctx.Request).Returns(httpRequestMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            _loginResponseTuple = (_testMagoLoginResponse, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(_loginResponseTuple));
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);

            _responseHandlerMock.Setup(rh => rh.HandleNoContent(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NoContentResult());
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            _testSyncronizedData = new SyncronizedDataDto
            {
                PrelMatRequest = new List<SyncPrelMatRequestDto> { _testSyncPrelMatRequest },
                RegOreRequest = new List<SyncRegOreRequestDto> { _testSyncRegOreRequest },
                InventarioRequest = new List<SyncInventarioRequestDto> { _testSyncInventarioRequest }
            };
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            _magoRequestServiceMock.Setup(s => s.LoginAsync(_testMagoLoginRequest))
                .ReturnsAsync(_testMagoLoginResponse);
            _responseHandlerMock.Setup(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<MagoLoginResponseDto>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(_testMagoLoginResponse));

            // Act
            var result = await _controller.Login(_testMagoLoginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MagoLoginResponseDto>(okResult.Value);
            Assert.Equal(_testMagoLoginResponse.Token, response.Token);
            Assert.Equal(_testMagoLoginResponse.Subscription, response.Subscription);
            Assert.True(response.IsAdmin);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenLoginFails()
        {
            // Arrange
            _magoRequestServiceMock.Setup(s => s.LoginAsync(_testMagoLoginRequest))
                .ReturnsAsync((MagoLoginResponseDto)null);
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = await _controller.Login(_testMagoLoginRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not Found", notFoundResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenRequestIsInvalid()
        {
            // Arrange
            _magoRequestServiceMock.Setup(s => s.LoginAsync(It.IsAny<MagoLoginRequestDto>()))
                .Throws(new ArgumentException("Invalid request"));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.Login(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        // -- Test per Logoff --
        [Fact]
        public async Task Logoff_ShouldReturnOk_WhenLogoffIsSuccessful()
        {
            // Arrange
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(_testMagoTokenRequest));
            _responseHandlerMock.Setup(rh => rh.HandleOk(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult(""));

            // Act
            var result = await _controller.LogOff(_testMagoTokenRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _magoRequestServiceMock.Verify(s => s.LogoffAsync(_testMagoTokenRequest), Times.Once);
        }

        [Fact]
        public async Task Logoff_ShouldReturnBadRequest_WhenLogoffFails()
        {
            // Arrange
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.IsAny<MagoTokenRequestDto>()));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.LogOff(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task Logoff_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.IsAny<MagoTokenRequestDto>()))
                .Throws(new Exception("Logoff failed"));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.LogOff(_testMagoTokenRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task Syncronize_ShouldReturnOk_WhenSyncronizationIsSuccessful()
        {
            // Arrange
            var loginResponseTuple = (_testMagoLoginResponse, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));

            _magoRequestServiceMock.Setup(s => s.SyncronizeAsync(_testMagoLoginResponse, _testSettingsDto, _testWorkerIdSyncRequest))
                .ReturnsAsync(_testSyncronizedData);

            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);

            _responseHandlerMock.Setup(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), _testSyncronizedData, It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult("Syncronization successful"));

            // Act
            var result = await _controller.Syncronize(_testWorkerIdSyncRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Syncronization successful", okResult.Value);

            // Verify that all expected methods were called
            _magoRequestServiceMock.Verify(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest), Times.Once);
            _magoRequestServiceMock.Verify(s => s.SyncronizeAsync(_testMagoLoginResponse, _testSettingsDto, _testWorkerIdSyncRequest), Times.Once);
            _magoRequestServiceMock.Verify(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)), Times.Once);
        }

        [Fact]
        public async Task Syncronize_ShouldReturnBadRequest_WhenReqestIsInvalid()
        {
            // Arrange
            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(It.IsAny<WorkerIdSyncRequestDto>()))
                .Throws(new ArgumentException("Invalid request"));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.Syncronize(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task Syncronize_ShouldReturnBadRequest_WhenLoginFails()
        {
            // Arrange
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.Syncronize(null);

            // Assert
            var notFoundResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", notFoundResult.Value);
        }

        [Fact]
        public async Task Syncronize_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var loginResponseTuple = (_testMagoLoginResponse, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .ReturnsAsync(loginResponseTuple);

            _magoRequestServiceMock.Setup(s => s.SyncronizeAsync(_testMagoLoginResponse, _testSettingsDto, _testWorkerIdSyncRequest))
                .Throws(new Exception("Syncronization failed"));

            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.Syncronize(_testWorkerIdSyncRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task Syncronize_ShouldReturBadRequest_WhenLoginResponseIsNull()
        {
            // Arrange
            (MagoLoginResponseDto, SettingsDto) loginResponseTuple = (null, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));

            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.Syncronize(_testWorkerIdSyncRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task Syncronize_ShouldReturBadRequest_WhenSettingsIsNull()
        {
            // Arrange
            (MagoLoginResponseDto, SettingsDto) loginResponseTuple = (_testMagoLoginResponse, null);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));

            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.Syncronize(_testWorkerIdSyncRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        // -- Test per SyncRegOreFiltered --
        [Fact]
        public async Task SyncRegOreFiltered_ShouldReturnOk_WhenSyncIsSuccessful()
        {
            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testSyncRegOreFilteredDto.WorkerIdSyncRequestDto))
                .Returns(Task.FromResult((_testMagoLoginResponse, _testSettingsDto)));
            _magoRequestServiceMock.Setup(s => s.SyncRegOreFiltered(_testMagoLoginResponse, _testSettingsDto, _testSyncRegOreFilteredDto, true))
                .Returns(Task.FromResult<IEnumerable<SyncRegOreRequestDto>>(new List<SyncRegOreRequestDto> { _testSyncRegOreRequest }));
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);
            _responseHandlerMock.Setup(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<object>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult("Sync successful"));

            // Act
            var result = await _controller.SyncRegOreFiltered(_testSyncRegOreFilteredDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Sync successful", okResult.Value);
            _magoRequestServiceMock.Verify(s => s.SyncRegOreFiltered(_testMagoLoginResponse, _testSettingsDto, _testSyncRegOreFilteredDto, true), Times.Once);
        }

        [Fact]
        public async Task SyncRegOreFiltered_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncRegOreFiltered(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncRegOreFiltered_ShouldReturnNotFound_WhenLoginFails()
        {
            // Arrange
            (MagoLoginResponseDto, SettingsDto) loginResponseTuple = (null, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncRegOreFiltered(_testSyncRegOreFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncRegOreFiltered_ShouldReturnBadRequest_WhenSettingsAreNull()
        {
            // Arrange
            (MagoLoginResponseDto, SettingsDto) loginResponseTuple = (_testMagoLoginResponse, null);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncRegOreFiltered(_testSyncRegOreFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncRegOreFiltered_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var loginResponseTuple = (_testMagoLoginResponse, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testSyncRegOreFilteredDto.WorkerIdSyncRequestDto))
                .Returns(Task.FromResult(loginResponseTuple));
            _magoRequestServiceMock.Setup(s => s.SyncRegOreFiltered(_testMagoLoginResponse, _testSettingsDto, _testSyncRegOreFilteredDto, It.IsAny<bool>()))
                .Throws(new Exception("Sync failed"));
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncRegOreFiltered(_testSyncRegOreFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
            
            // Verify logoff was called even on exception
            _magoRequestServiceMock.Verify(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)), Times.Once);
        }

        // Test per SyncPrelMatFiltered
        [Fact]
        public async Task SyncPrelMatFiltered_ShouldReturnOk_WhenSyncIsSuccessful()
        {
            // Arrange
            var testSyncPrelMatFilteredDto = new SyncPrelMatFilteredDto
            {
                WorkerIdSyncRequestDto = _testWorkerIdSyncRequest,
                PrelMatList = new List<PrelMatDto>()
            };

            var loginResponseTuple = (_testMagoLoginResponse, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);
            _magoRequestServiceMock.Setup(s => s.SyncPrelMatFiltered(_testMagoLoginResponse, _testSettingsDto, testSyncPrelMatFilteredDto, true))
                .Returns(Task.FromResult<IEnumerable<SyncPrelMatRequestDto>>(new List<SyncPrelMatRequestDto> { _testSyncPrelMatRequest }));
            _responseHandlerMock.Setup(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<object>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult("Sync successful"));

            // Act
            var result = await _controller.SyncPrelMatFiltered(testSyncPrelMatFilteredDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Sync successful", okResult.Value);
        }

        [Fact]
        public async Task SyncPrelMatFiltered_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncPrelMatFiltered(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncPrelMatFiltered_ShouldReturnNotFound_WhenLoginFails()
        {
            // Arrange
            var testSyncPrelMatFilteredDto = new SyncPrelMatFilteredDto
            {
                WorkerIdSyncRequestDto = _testWorkerIdSyncRequest,
                PrelMatList = new List<PrelMatDto>()
            };

            (MagoLoginResponseDto, SettingsDto) loginResponseTuple = (null, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncPrelMatFiltered(testSyncPrelMatFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncPrelMatFiltered_ShouldReturnBadRequest_WhenSettingsAreNull()
        {
            // Arrange
            var testSyncPrelMatFilteredDto = new SyncPrelMatFilteredDto
            {
                WorkerIdSyncRequestDto = _testWorkerIdSyncRequest,
                PrelMatList = new List<PrelMatDto>()
            };

            (MagoLoginResponseDto, SettingsDto) loginResponseTuple = (_testMagoLoginResponse, null);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncPrelMatFiltered(testSyncPrelMatFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncPrelMatFiltered_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var testSyncPrelMatFilteredDto = new SyncPrelMatFilteredDto
            {
                WorkerIdSyncRequestDto = _testWorkerIdSyncRequest,
                PrelMatList = new List<PrelMatDto>()
            };

            var loginResponseTuple = (_testMagoLoginResponse, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _magoRequestServiceMock.Setup(s => s.SyncPrelMatFiltered(_testMagoLoginResponse, _testSettingsDto, testSyncPrelMatFilteredDto, true))
                .Throws(new Exception("Sync failed"));
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncPrelMatFiltered(testSyncPrelMatFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
            
            // Verify logoff was called even on exception
            _magoRequestServiceMock.Verify(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)), Times.Once);
        }

        // Test per SyncInventarioFiltered
        [Fact]
        public async Task SyncInventarioFiltered_ShouldReturnOk_WhenSyncIsSuccessful()
        {
            // Arrange
            var testSyncInventarioFilteredDto = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = _testWorkerIdSyncRequest,
                InventarioList = new List<InventarioDto>()
            };

            var loginResponseTuple = (_testMagoLoginResponse, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);
            _magoRequestServiceMock.Setup(s => s.SyncInventarioFiltered(_testMagoLoginResponse, _testSettingsDto, testSyncInventarioFilteredDto, true))
                .Returns(Task.FromResult<IEnumerable<SyncInventarioRequestDto>>(new List<SyncInventarioRequestDto> { _testSyncInventarioRequest }));
            _responseHandlerMock.Setup(rh => rh.HandleOkAndItem(It.IsAny<HttpContext>(), It.IsAny<object>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new OkObjectResult("Sync successful"));

            // Act
            var result = await _controller.SyncInventarioFiltered(testSyncInventarioFilteredDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Sync successful", okResult.Value);
        }

        [Fact]
        public async Task SyncInventarioFiltered_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Arrange
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncInventarioFiltered(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncInventarioFiltered_ShouldReturnBadRequest_WhenWorkerIdSyncRequestDtoIsNull()
        {
            // Arrange
            var testSyncInventarioFilteredDto = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = null,
                InventarioList = new List<InventarioDto>()
            };

            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncInventarioFiltered(testSyncInventarioFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncInventarioFiltered_ShouldReturnNotFound_WhenLoginFails()
        {
            // Arrange
            var testSyncInventarioFilteredDto = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = _testWorkerIdSyncRequest,
                InventarioList = new List<InventarioDto>()
            };

            (MagoLoginResponseDto, SettingsDto) loginResponseTuple = (null, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _responseHandlerMock.Setup(rh => rh.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new NotFoundObjectResult("Not Found"));

            // Act
            var result = await _controller.SyncInventarioFiltered(testSyncInventarioFilteredDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not Found", notFoundResult.Value);
        }

        [Fact]
        public async Task SyncInventarioFiltered_ShouldReturnBadRequest_WhenSettingsAreNull()
        {
            // Arrange
            var testSyncInventarioFilteredDto = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = _testWorkerIdSyncRequest,
                InventarioList = new List<InventarioDto>()
            };

            (MagoLoginResponseDto, SettingsDto) loginResponseTuple = (_testMagoLoginResponse, null);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncInventarioFiltered(testSyncInventarioFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        [Fact]
        public async Task SyncInventarioFiltered_ShouldReturnBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var testSyncInventarioFilteredDto = new SyncInventarioFilteredDto
            {
                WorkerIdSyncRequestDto = _testWorkerIdSyncRequest,
                InventarioList = new List<InventarioDto>()
            };

            var loginResponseTuple = (_testMagoLoginResponse, _testSettingsDto);

            _magoRequestServiceMock.Setup(s => s.LoginWithWorkerIdAsync(_testWorkerIdSyncRequest))
                .Returns(Task.FromResult(loginResponseTuple));
            _magoRequestServiceMock.Setup(s => s.SyncInventarioFiltered(_testMagoLoginResponse, _testSettingsDto, testSyncInventarioFilteredDto, true))
                .Throws(new Exception("Sync failed"));
            _magoRequestServiceMock.Setup(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)))
                .Returns(Task.CompletedTask);
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.SyncInventarioFiltered(testSyncInventarioFilteredDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
            _magoRequestServiceMock.Verify(s => s.LogoffAsync(It.Is<MagoTokenRequestDto>(dto => dto.Token == _testMagoLoginResponse.Token)), Times.Once);
        }

    }
}