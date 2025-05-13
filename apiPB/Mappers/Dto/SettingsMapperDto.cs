using apiPB.Models;
using apiPB.Dto.Request;
using apiPB.Dto.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace apiPB.Mappers.Dto
{
    public static class SettingsMapperDto
    {
        public static SettingsDto ToSettingsDto(this A3AppSetting settings)
        {
            return new SettingsDto
            {
                MagoUrl = settings.MagoUrl,
                Username = settings.Username,
                Password = settings.Password,
                SpecificatorType = settings.SpecificatorType,
                Closed = settings.Closed
            };
        }
    }
}