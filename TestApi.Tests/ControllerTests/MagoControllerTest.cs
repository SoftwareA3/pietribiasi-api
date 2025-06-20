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
            RtgStep = 2,
            Alternate = "Alt1",
            AltRtgStep = 3,
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
                .ReturnsAsync(_testMagoLoginResponseWithError);
            _responseHandlerMock.Setup(rh => rh.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(new BadRequestObjectResult("Bad Request"));

            // Act
            var result = await _controller.Login(_testMagoLoginRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bad Request", badRequestResult.Value);
        }

        
    }
}