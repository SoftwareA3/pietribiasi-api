using apiPB.Models;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Repository.Abstraction
{
    public interface IMagoRepository
    {
        SettingsDto EditSettings(SettingsFilter settings);
    }
}