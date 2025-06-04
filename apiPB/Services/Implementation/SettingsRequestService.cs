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
            try
            {
                var settings = _settingsRepository.GetSettings() ?? throw new ArgumentNullException("Nessun risultato per GetSettings in SettingsRequestService");
                return settings;
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in SettingsRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service SettingsRequestService", ex);
            }
        }

        public SyncGobalActiveRequestDto? GetSyncGlobalActive()
        {
            try
            {
                var appSetting = _settingsRepository.GetSyncGlobalActive();
                if (appSetting == null || appSetting.SyncGlobalActive == null)
                {
                    throw new ArgumentNullException("Nessun risultato per GetSyncGlobalActive in SettingsRequestService");
                }
                return appSetting;
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Repository o Mapper ritornano valore nullo in SettingsRequestService", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'esecuzione del Service SettingsRequestService", ex);
            }
        }
    }
}