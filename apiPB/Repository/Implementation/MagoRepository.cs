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

        public SettingsDto EditSettings(SettingsFilter settings)
        {
            var existingSettings = _context.A3AppSettings.FirstOrDefault();
            if (existingSettings != null)
            {
                existingSettings.MagoUrl = settings.MagoUrl;
                existingSettings.Username = settings.Username;
                existingSettings.Password = settings.Password;
                existingSettings.SpecificatorType = settings.SpecificatorType;
                existingSettings.Closed = settings.Closed;

                _context.A3AppSettings.Update(existingSettings);
                _context.SaveChanges();
                return existingSettings.ToSettingsDto();
            }
            else
            {
                var magoSettings = new A3AppSetting
                {
                    MagoUrl = settings.MagoUrl,
                    Username = settings.Username,
                    Password = settings.Password,
                    SpecificatorType = 6750211,
                    Closed = false
                };   
                _context.A3AppSettings.Add(magoSettings);
                _context.SaveChanges();
                return magoSettings.ToSettingsDto();
            }
        }
    }
}