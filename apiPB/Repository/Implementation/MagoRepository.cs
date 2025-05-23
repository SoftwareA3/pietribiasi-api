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
    public class MagoRepository : IMagoRepository
    {
        private readonly ApplicationDbContext _context;
        
        public MagoRepository(ApplicationDbContext context)
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
                return null;
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
                return null;
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
                existingSettings.Closed = settings.Closed;
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
                    Closed = settings.Closed,
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
    }
}