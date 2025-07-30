using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net;
using apiPB.ApiClient.Abstraction;
using apiPB.Mappers.Dto;
using apiPB.Repository.Abstraction;
using AutoMapper;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Services.Abstraction;
using apiPB.Dto.Request;
using Azure.Core;
using apiPB.Utils.Abstraction;

namespace apiPB.Services.Implementation
{
    public class SettingsRequestService : ISettingsRequestService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IMapper _mapper;

        public SettingsRequestService(ISettingsRepository settingsRepository, IMapper mapper)
        {
            _settingsRepository = settingsRepository;
            _mapper = mapper;
        }

        public SettingsDto EditSettings(SettingsDto settings)
        {
            var filter = _mapper.Map<SettingsFilter>(settings);
            var editedSettings = _settingsRepository.EditSettings(filter);

            return editedSettings.ToSettingsDto();
        }

        public SettingsDto? GetSettings()
        {
            var settings = _settingsRepository.GetSettings();
            return settings;
        }

        public SyncGobalActiveRequestDto? GetSyncGlobalActive()
        {
            var appSetting = _settingsRepository.GetSyncGlobalActive();
            return appSetting;
        }

        public TerminaLavorazioniUtenteRequestDto? GetTerminaLavorazioniUtente()
        {
            var appSetting = _settingsRepository.GetTerminaLavorazioniUtente();
            return appSetting;
        }
    }
}