using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Data;
using apiPB.Models;
using apiPB.Repository.Implementation;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestApi.Tests.Repository
{
    public class SettingsRepositoryTest
    {
        private readonly SettingsRepository _settingsRepository;
        private readonly Mock<DbSet<A3AppSetting>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;

        private readonly List<A3AppSetting> _settings = new List<A3AppSetting>
        {
            new A3AppSetting
            {
                SettingsId = 1,
                MagoUrl = "http://test.mago.com",
                Username = "testuser",
                Password = "testpass",
                Company = "TestCompany",
                SpecificatorType = 1,
                Closed = false,
                RectificationReasonPositive = "Positive Reason",
                RectificationReasonNegative = "Negative Reason",
                Storage = "TestStorage",
                SyncGlobalActive = true
            }
        };

        private readonly SettingsFilter _settingsFilter = new SettingsFilter
        {
            MagoUrl = "http://updated.mago.com",
            Username = "updateduser",
            Password = "updatedpass",
            Company = "UpdatedCompany",
            SpecificatorType = 2,
            Closed = true,
            RectificationReasonPositive = "Updated Positive",
            RectificationReasonNegative = "Updated Negative",
            Storage = "UpdatedStorage",
            SyncGlobalActive = false
        };

        public SettingsRepositoryTest()
        {
            _mockSet = new Mock<DbSet<A3AppSetting>>();
            _mockContext = new Mock<ApplicationDbContext>();
            _settingsRepository = new SettingsRepository(_mockContext.Object);
        }

        #region GetSettings Tests

        [Fact]
        public void GetSettings_ShouldReturnSettingsDto_WhenSettingsExist()
        {
            // Arrange
            var queryableData = _settings.AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act
            var result = _settingsRepository.GetSettings();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SettingsDto>(result);
        }

        [Fact]
        public void GetSettings_ShouldThrowArgumentNullException_WhenNoSettingsExist()
        {
            // Arrange
            var emptyData = new List<A3AppSetting>().AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(emptyData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(emptyData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(emptyData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(emptyData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _settingsRepository.GetSettings());
            Assert.Equal("Nessun risultato per GetSettings in SettingsRepository", exception.ParamName);
        }

        [Fact]
        public void GetSettings_ShouldCallA3AppSettings_OnContext()
        {
            // Arrange
            var queryableData = _settings.AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act
            var result = _settingsRepository.GetSettings();

            // Assert
            _mockContext.Verify(c => c.A3AppSettings, Times.Once);
            Assert.NotNull(result);
        }

        #endregion

        #region GetSyncGlobalActive Tests

        [Fact]
        public void GetSyncGlobalActive_ShouldReturnSyncGobalActiveRequestDto_WhenSettingsExistAndSyncGlobalActiveIsNotNull()
        {
            // Arrange
            var queryableData = _settings.AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act
            var result = _settingsRepository.GetSyncGlobalActive();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<SyncGobalActiveRequestDto>(result);
        }

        [Fact]
        public void GetSyncGlobalActive_ShouldThrowArgumentNullException_WhenNoSettingsExist()
        {
            // Arrange
            var emptyData = new List<A3AppSetting>().AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(emptyData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(emptyData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(emptyData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(emptyData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _settingsRepository.GetSyncGlobalActive());
            Assert.Equal("A3AppSettings null e SyncGlobalActive null per GetSyncGlobalActive in SettingsRepository", exception.ParamName);
        }

        [Fact]
        public void GetSyncGlobalActive_ShouldThrowArgumentNullException_WhenSyncGlobalActiveIsNull()
        {
            // Arrange
            var settingsWithNullSync = new List<A3AppSetting>
            {
                new A3AppSetting
                {
                    SettingsId = 1,
                    MagoUrl = "http://test.mago.com",
                    Username = "testuser",
                    Password = "testpass",
                    Company = "TestCompany",
                    SpecificatorType = 1,
                    Closed = false,
                    RectificationReasonPositive = "Positive Reason",
                    RectificationReasonNegative = "Negative Reason",
                    Storage = "TestStorage",
                    SyncGlobalActive = null
                }
            };

            var queryableData = settingsWithNullSync.AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _settingsRepository.GetSyncGlobalActive());
            Assert.Equal("A3AppSettings null e SyncGlobalActive null per GetSyncGlobalActive in SettingsRepository", exception.ParamName);
        }

        #endregion

        #region EditSettings Tests

        [Fact]
        public void EditSettings_ShouldUpdateExistingSettings_WhenSettingsExist()
        {
            // Arrange
            var queryableData = _settings.AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act
            var result = _settingsRepository.EditSettings(_settingsFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_settingsFilter.MagoUrl, result.MagoUrl);
            Assert.Equal(_settingsFilter.Username, result.Username);
            Assert.Equal(_settingsFilter.Password, result.Password);
            Assert.Equal(_settingsFilter.Company, result.Company);
            Assert.Equal(_settingsFilter.SpecificatorType, result.SpecificatorType);
            Assert.Equal(_settingsFilter.Closed, result.Closed);
            Assert.Equal(_settingsFilter.RectificationReasonPositive, result.RectificationReasonPositive);
            Assert.Equal(_settingsFilter.RectificationReasonNegative, result.RectificationReasonNegative);
            Assert.Equal(_settingsFilter.Storage, result.Storage);
            Assert.Equal(_settingsFilter.SyncGlobalActive, result.SyncGlobalActive);
            
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void EditSettings_ShouldCreateNewSettings_WhenNoSettingsExist()
        {
            // Arrange
            var emptyData = new List<A3AppSetting>().AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(emptyData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(emptyData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(emptyData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(emptyData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act
            var result = _settingsRepository.EditSettings(_settingsFilter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_settingsFilter.MagoUrl, result.MagoUrl);
            Assert.Equal(_settingsFilter.Username, result.Username);
            Assert.Equal(_settingsFilter.Password, result.Password);
            Assert.Equal(_settingsFilter.Company, result.Company);
            Assert.Equal(_settingsFilter.SpecificatorType, result.SpecificatorType);
            Assert.Equal(_settingsFilter.Closed, result.Closed);
            Assert.Equal(_settingsFilter.RectificationReasonPositive, result.RectificationReasonPositive);
            Assert.Equal(_settingsFilter.RectificationReasonNegative, result.RectificationReasonNegative);
            Assert.Equal(_settingsFilter.Storage, result.Storage);
            Assert.Equal(_settingsFilter.SyncGlobalActive, result.SyncGlobalActive);
            
            _mockContext.Verify(c => c.A3AppSettings.Add(It.IsAny<A3AppSetting>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void EditSettings_ShouldCallA3AppSettings_OnContext()
        {
            // Arrange
            var queryableData = _settings.AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act
            var result = _settingsRepository.EditSettings(_settingsFilter);

            // Assert
            _mockContext.Verify(c => c.A3AppSettings, Times.Once);
            Assert.NotNull(result);
        }

        #endregion

        #region Additional Edge Case Tests

        [Fact]
        public void EditSettings_ShouldReturnA3AppSetting_Always()
        {
            // Arrange
            var queryableData = _settings.AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act
            var result = _settingsRepository.EditSettings(_settingsFilter);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<A3AppSetting>(result);
        }

        [Fact]
        public void EditSettings_ShouldHandleNullableProperties_Correctly()
        {
            // Arrange
            var filterWithNulls = new SettingsFilter
            {
                MagoUrl = null,
                Username = null,
                Password = null,
                Company = null,
                SpecificatorType = null,
                Closed = null,
                RectificationReasonPositive = null,
                RectificationReasonNegative = null,
                Storage = null,
                SyncGlobalActive = null
            };

            var queryableData = _settings.AsQueryable();
            
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<A3AppSetting>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            _mockContext.Setup(c => c.A3AppSettings).Returns(_mockSet.Object);

            // Act
            var result = _settingsRepository.EditSettings(filterWithNulls);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.MagoUrl);
            Assert.Null(result.Username);
            Assert.Null(result.Password);
            Assert.Null(result.Company);
            Assert.Null(result.SpecificatorType);
            Assert.Null(result.Closed);
            Assert.Null(result.RectificationReasonPositive);
            Assert.Null(result.RectificationReasonNegative);
            Assert.Null(result.Storage);
            Assert.Null(result.SyncGlobalActive);
        }

        #endregion
    }
}