using System;
using Xunit;
using Moq;
using AutoMapper;
using apiPB.Services.Implementation;
using apiPB.Repository.Abstraction;
using apiPB.Dto.Models;
using apiPB.Filters;
using apiPB.Dto.Request;
using apiPB.Models;

namespace TestApi.Tests.ServicesTests
{
    public class SettingsTest
    {
        private readonly Mock<ISettingsRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SettingsRequestService _service;

        private readonly SettingsDto _settingsDto = new SettingsDto
        {
            MagoUrl = "http://example.com",
            Username = "testuser",
            Password = "password123",
            Company = "Test Company",
            SpecificatorType = 1,
            Closed = false,
            RectificationReasonPositive = "Positive Reason",
            RectificationReasonNegative = "Negative Reason",
            Storage = "Test Storage",
            SyncGlobalActive = true
        };

        private readonly SettingsFilter _settingsFilter = new SettingsFilter
        {
            MagoUrl = "http://example.com",
            Username = "testuser",
            Password = "password123",
            Company = "Test Company",
            SpecificatorType = 1,
            Closed = false,
            RectificationReasonPositive = "Positive Reason",
            RectificationReasonNegative = "Negative Reason",
            Storage = "Test Storage",
            SyncGlobalActive = true
        };

        private readonly A3AppSetting _repositoryResult = new A3AppSetting
        {
            MagoUrl = "http://example.com",
            Username = "updatedtestuser",
            Password = "updatedpassword123",
            Company = "Test Company",
            SpecificatorType = 1,
            Closed = false,
            RectificationReasonPositive = "Positive Reason1",
            RectificationReasonNegative = "Negative Reason1",
            Storage = "Test Storage",
            SyncGlobalActive = true
        };

        private readonly SettingsDto _expectedResult = new SettingsDto
        {
            MagoUrl = "http://example.com",
            Username = "updatedtestuser",
            Password = "updatedpassword123",
            Company = "Test Company",
            SpecificatorType = 1,
            Closed = false,
            RectificationReasonPositive = "Positive Reason1",
            RectificationReasonNegative = "Negative Reason1",
            Storage = "Test Storage",
            SyncGlobalActive = true
        };

        public SettingsTest()
        {
            _mockRepository = new Mock<ISettingsRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new SettingsRequestService(_mockRepository.Object, _mockMapper.Object);
        }

        #region EditSettings Tests

        [Fact]
        public void EditSettings_WithValidSettings_ReturnsSettingsDto()
        {
            // Arrange

            _mockMapper.Setup(m => m.Map<SettingsFilter>(_settingsDto))
                .Returns(_settingsFilter);

            _mockRepository.Setup(r => r.EditSettings(_settingsFilter))
                .Returns(_repositoryResult);

            // Act
            var result = _service.EditSettings(_settingsDto);

            // Assert
            Assert.NotNull(result);
            _mockMapper.Verify(m => m.Map<SettingsFilter>(_settingsDto), Times.Once);
            _mockRepository.Verify(r => r.EditSettings(_settingsFilter), Times.Once);
        }

        [Fact]
        public void EditSettings_WhenMapperThrowsException_PropagatesException()
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<SettingsFilter>(_settingsDto)).Throws(new AutoMapperMappingException("Mapping error"));

            // Act & Assert
            Assert.Throws<AutoMapperMappingException>(() => _service.EditSettings(_settingsDto));
        }

        [Fact]
        public void EditSettings_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<SettingsFilter>(_settingsDto))
                .Returns(_settingsFilter);

            _mockRepository.Setup(r => r.EditSettings(_settingsFilter))
                .Throws(new Exception("Repository error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _service.EditSettings(_settingsDto));
        }

        #endregion

        #region GetSettings Tests

        [Fact]
        public void GetSettings_WhenSettingsExist_ReturnsSettingsDto()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetSettings()).Returns(_expectedResult);

            // Act
            var result = _service.GetSettings();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_expectedResult.MagoUrl, result.MagoUrl);
            Assert.Equal(_expectedResult.Username, result.Username);
            Assert.Equal(_expectedResult.Password, result.Password);
            Assert.Equal(_expectedResult.Company, result.Company);
            Assert.Equal(_expectedResult.SpecificatorType, result.SpecificatorType);
            Assert.Equal(_expectedResult.Closed, result.Closed);
            Assert.Equal(_expectedResult.RectificationReasonPositive, result.RectificationReasonPositive);
            Assert.Equal(_expectedResult.RectificationReasonNegative, result.RectificationReasonNegative);
            Assert.Equal(_expectedResult.Storage, result.Storage);
            Assert.Equal(_expectedResult.SyncGlobalActive, result.SyncGlobalActive);
            _mockRepository.Verify(r => r.GetSettings(), Times.Once);
        }

        [Fact]
        public void GetSettings_WhenNoSettingsExist_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetSettings())
                .Returns((SettingsDto)null);

            // Act
            var result = _service.GetSettings();

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetSettings(), Times.Once);
        }

        [Fact]
        public void GetSettings_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetSettings())
                .Throws(new Exception("Repository error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _service.GetSettings());
        }

        #endregion

        #region GetSyncGlobalActive Tests

        [Fact]
        public void GetSyncGlobalActive_WhenSettingExists_ReturnsSyncGobalActiveRequestDto()
        {
            // Arrange
            var expectedResult = new SyncGobalActiveRequestDto
            {
                SyncGlobalActive = true,
            };

            _mockRepository.Setup(r => r.GetSyncGlobalActive())
                .Returns(expectedResult);

            // Act
            var result = _service.GetSyncGlobalActive();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResult.SyncGlobalActive, result.SyncGlobalActive);
            _mockRepository.Verify(r => r.GetSyncGlobalActive(), Times.Once);
        }

        [Fact]
        public void GetSyncGlobalActive_WhenNoSettingExists_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetSyncGlobalActive()).Returns((SyncGobalActiveRequestDto)null);

            // Act
            var result = _service.GetSyncGlobalActive();

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetSyncGlobalActive(), Times.Once);
        }

        [Fact]
        public void GetSyncGlobalActive_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetSyncGlobalActive()).Throws(new Exception("Repository error"));

            // Act & Assert
            Assert.Throws<Exception>(() => _service.GetSyncGlobalActive());
        }

        #endregion
    }
}