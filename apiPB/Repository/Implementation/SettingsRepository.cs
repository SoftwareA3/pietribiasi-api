using apiPB.Data;
using apiPB.Models;
using apiPB.Filters;
using apiPB.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using apiPB.Dto.Models;
using apiPB.Mappers.Dto;
using apiPB.Dto.Request;

namespace apiPB.Repository.Implementation
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public SettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public SettingsDto? GetSettings()
        {
            var settings = _context.A3AppSettings.FirstOrDefault();
            if (settings != null)
            {
                return settings.ToSettingsDto();
            }
            else
            {
                throw new ArgumentNullException("Nessun risultato per GetSettings in SettingsRepository");
            }
        }

        public SyncGobalActiveRequestDto? GetSyncGlobalActive()
        {
            var appSetting = _context.A3AppSettings.FirstOrDefault();
            if (appSetting != null && appSetting.SyncGlobalActive != null)
            {
                return appSetting.ToSyncGobalActiveRequestDto();
            }
            else
            {
                throw new ArgumentNullException("A3AppSettings null e SyncGlobalActive null per GetSyncGlobalActive in SettingsRepository");
            }
        }

        public TerminaLavorazioniUtenteRequestDto? GetTerminaLavorazioniUtente()
        {
            var appSetting = _context.A3AppSettings.FirstOrDefault();
            if (appSetting != null && appSetting.TerminaLavorazioniUtente != null)
            {
                return appSetting.ToTerminaLavorazioniUtenteRequestDto();
            }
            else
            {
                throw new ArgumentNullException("A3AppSettings null e TerminaLavorazioniUtente null per GetTerminaLavorazioniUtente in SettingsRepository");
            }
        }

        public A3AppSetting EditSettings(SettingsFilter settings)
        {
            var existingSettings = _context.A3AppSettings.FirstOrDefault();
            if (existingSettings != null)
            {
                existingSettings.MagoUrl = settings.MagoUrl;
                existingSettings.Username = settings.Username;
                existingSettings.Password = settings.Password;
                existingSettings.Company = settings.Company;
                existingSettings.SpecificatorType = settings.SpecificatorType;
                existingSettings.RectificationReasonPositive = settings.RectificationReasonPositive;
                existingSettings.RectificationReasonNegative = settings.RectificationReasonNegative;
                existingSettings.Storage = settings.Storage;
                existingSettings.TerminaLavorazioniUtente = settings.TerminaLavorazioniUtente;
                existingSettings.SyncGlobalActive = settings.SyncGlobalActive;

                //_context.A3AppSettings.Update(existingSettings);
                _context.SaveChanges();
                return existingSettings;
            }
            else
            {
                var magoSettings = new A3AppSetting
                {
                    MagoUrl = settings.MagoUrl,
                    Username = settings.Username,
                    Password = settings.Password,
                    Company = settings.Company,
                    SpecificatorType = settings.SpecificatorType,
                    TerminaLavorazioniUtente = settings.TerminaLavorazioniUtente,
                    RectificationReasonPositive = settings.RectificationReasonPositive,
                    RectificationReasonNegative = settings.RectificationReasonNegative,
                    Storage = settings.Storage,
                    SyncGlobalActive = settings.SyncGlobalActive
                };
                _context.A3AppSettings.Add(magoSettings);
                _context.SaveChanges();
                return magoSettings;
            }
        }
        
        public void IncrementExternalReferenceCounter()
        {
            var settings = _context.A3AppSettings.FirstOrDefault();
            if (settings != null)
            {
                settings.ExternalReferences = settings.ExternalReferences + 1;
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentNullException("Nessun risultato per IncrementExternalReferenceCounter in SettingsRepository");
            }
        }
    }
}