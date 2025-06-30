using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Implementation;
using apiPB.Filters;
using apiPB.Utils.Implementation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestApi.Tests.Repository
{
    public class ActionMessageRepositoryTest
    {
        private readonly ActionMessageRepository _actionMessageRepository;
        private readonly Mock<DbSet<VwOmActionMessage>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;

        private readonly List<VwOmActionMessage> _actionMessages = new List<VwOmActionMessage>
        {
            new VwOmActionMessage
            {
                ActionId = 1,
                Moid = 100,
                RtgStep = 1,
                Alternate = "Alt1",
                Job = "Job1",
                AltRtgStep = 2,
                Mono = "Mono1",
                Bom = "BOM1",
                Variant = "Variant1",
                Wc = "WC1",
                Operation = "Op1",
                WorkerId = 1001,
                ActionType = 1,
                ActionStatus = 0,
                ActionMessage = "Action message 1",
                Closed = "No",
                WorkerProcessingTime = 30,
                WorkerSetupTime = 10,
                ActualProcessingTime = 25,
                ActualSetupTime = 5,
                Storage = "Storage1",
                SpecificatorType = 1,
                Specificator = "Spec1",
                ReturnMaterialQtyLower = "RMQL1",
                PickMaterialQtyGreater = "PMQG1",
                ProductionLotNumber = "PLN1",
                ProductionQty = 100,
                Tbcreated = DateTime.Now,
                TbcreatedId = 1,
                DeliveryDate = DateTime.Now.AddDays(1),
                ConfirmChildMos = "Yes",
                Mostatus = 1,
                MessageId = 1,
                MessageType = 1,
                MessageDate = DateTime.Now,
                MessageText = "Message text 1"
            },
            new VwOmActionMessage
            {
                ActionId = 2,
                Moid = 100,
                RtgStep = 1,
                Alternate = "Alt1",
                AltRtgStep = 2,
                Mono = "Mono1",
                Bom = "BOM1",
                Variant = "Variant1",
                Wc = "WC1",
                Operation = "Op1",
                WorkerId = 1001,
                ActionType = 1,
            },
            new VwOmActionMessage
            {
                ActionId = 3,
                Moid = 200,
                RtgStep = 2,
                Alternate = "Alt2",
                AltRtgStep = 3,
                Mono = "Mono2",
                Bom = "BOM2",
                Variant = "Variant2",
                Wc = "WC2",
                Operation = "Op2",
                WorkerId = 1002,
                ActionType = 2
            }
        };

        private readonly ImportedLogMessageFilter _filter = new ImportedLogMessageFilter
        {
            Moid = 100,
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 2,
            Mono = "Mono1",
            Bom = "BOM1",
            Variant = "Variant1",
            Wc = "WC1",
            Operation = "Op1",
            WorkerId = 1001,
            ActionType = 1
        };

        private readonly ImportedLogMessageFilter _filterWithOptionalNulls = new ImportedLogMessageFilter
        {
            Moid = 100,
            RtgStep = 1,
            Alternate = "Alt1",
            AltRtgStep = 2,
            Mono = null,
            Bom = null,
            Variant = null,
            Wc = null,
            Operation = null,
            WorkerId = 1001,
            ActionType = 1
        };

        public ActionMessageRepositoryTest()
        {
            _mockSet = new Mock<DbSet<VwOmActionMessage>>();
            _mockContext = new Mock<ApplicationDbContext>();
            _actionMessageRepository = new ActionMessageRepository(_mockContext.Object);
        }

        #region GetActionMessagesByFilter Tests

        [Fact]
        public void GetActionMessagesByFilter_ShouldReturnFilteredResults_WhenDataExists()
        {
            // Arrange
            var queryableData = _actionMessages.AsQueryable();

            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwOmActionMessages).Returns(_mockSet.Object);

            // Act
            var result = _actionMessageRepository.GetActionMessagesByFilter(_filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item =>
            {
                Assert.Equal(_filter.Moid, item.Moid);
                Assert.Equal(_filter.RtgStep, item.RtgStep);
                Assert.Equal(_filter.Alternate, item.Alternate);
                Assert.Equal(_filter.AltRtgStep, item.AltRtgStep);
                Assert.Equal(_filter.WorkerId, item.WorkerId);
                Assert.Equal(_filter.ActionType, item.ActionType);
            });
        }

        [Fact]
        public void GetActionMessagesByFilter_ShouldReturnOrderedByActionIdDescending()
        {
            // Arrange
            var queryableData = _actionMessages.AsQueryable();

            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwOmActionMessages).Returns(_mockSet.Object);

            // Act
            var result = _actionMessageRepository.GetActionMessagesByFilter(_filter);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.True(resultList[0].ActionId > resultList[1].ActionId);
        }

        [Fact]
        public void GetActionMessagesByFilter_ShouldHandleNullOptionalFields_Correctly()
        {
            // Arrange
            var queryableData = _actionMessages.AsQueryable();

            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwOmActionMessages).Returns(_mockSet.Object);

            // Act
            var result = _actionMessageRepository.GetActionMessagesByFilter(_filterWithOptionalNulls);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item =>
            {
                Assert.Equal(_filterWithOptionalNulls.Moid, item.Moid);
                Assert.Equal(_filterWithOptionalNulls.RtgStep, item.RtgStep);
                Assert.Equal(_filterWithOptionalNulls.Alternate, item.Alternate);
                Assert.Equal(_filterWithOptionalNulls.AltRtgStep, item.AltRtgStep);
                Assert.Equal(_filterWithOptionalNulls.WorkerId, item.WorkerId);
                Assert.Equal(_filterWithOptionalNulls.ActionType, item.ActionType);
            });
        }

        [Fact]
        public void GetActionMessagesByFilter_ShouldCallVwOmActionMessages_OnContext()
        {
            // Arrange
            var queryableData = _actionMessages.AsQueryable();

            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwOmActionMessages).Returns(_mockSet.Object);

            // Act
            var result = _actionMessageRepository.GetActionMessagesByFilter(_filter);

            // Assert
            _mockContext.Verify(c => c.VwOmActionMessages, Times.Once);
            Assert.NotNull(result);
        }

        #endregion

        #region Additional Edge Case Tests

        [Fact]
        public void GetActionMessagesByFilter_ShouldReturnSingleResult_WhenOnlyOneMatchExists()
        {
            // Arrange
            var queryableData = _actionMessages.AsQueryable();

            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.VwOmActionMessages).Returns(_mockSet.Object);

            var specificFilter = new ImportedLogMessageFilter
            {
                Moid = 200,
                RtgStep = 2,
                Alternate = "Alt2",
                AltRtgStep = 3,
                Mono = "Mono2",
                Bom = "BOM2",
                Variant = "Variant2",
                Wc = "WC2",
                Operation = "Op2",
                WorkerId = 1002,
                ActionType = 2
            };

            // Act
            var result = _actionMessageRepository.GetActionMessagesByFilter(specificFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(3, result.First().ActionId);
        }

        [Fact]
        public void GetActionMessagesByFilter_ShouldReturnEmpty_WhenNoMatchesForOptionalFields()
        {
            // Arrange
            var queryableData = _actionMessages.AsQueryable();
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<VwOmActionMessage>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());
            _mockContext.Setup(c => c.VwOmActionMessages).Returns(_mockSet.Object);

            var filterWithNonMatchingOptionalFields = new ImportedLogMessageFilter
            {
                Moid = 100,
                RtgStep = 1,
                Alternate = "Alt1",
                AltRtgStep = 2,
                Mono = "NonExistentMono",
                Bom = "NonExistentBOM",
                Variant = "NonExistentVariant",
                Wc = "WC1",
                Operation = "Op1",
                WorkerId = 1001,
                ActionType = 1
            };

            // Act
            var result = _actionMessageRepository.GetActionMessagesByFilter(filterWithNonMatchingOptionalFields);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion
    }
}