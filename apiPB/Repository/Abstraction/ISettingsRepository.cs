using apiPB.Models;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Repository.Abstraction
{
    public interface ISettingsRepository
    {
        A3AppSetting EditSettings(SettingsFilter settings);
        SettingsDto? GetSettings();

        SyncGobalActiveRequestDto? GetSyncGlobalActive();

        void IncrementExternalReferenceCounter();
    }
}