using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Abstraction
{
    public interface IMagoRequestService
    {
        Task SyncronizeAsync(WorkerIdSyncRequestDto request);
        SettingsDto? EditSettings(SettingsDto settings);
        SettingsDto? GetSettings();
        Task SyncRegOre(IEnumerable<SyncRegOreRequestDto> request, string token);
        Task SyncPrelMat(IEnumerable<SyncPrelMatRequestDto> request, string token);
        Task<MagoLoginResponseDto?> LoginAsync(MagoLoginRequestDto request);
        Task LogoffAsync(MagoTokenRequestDto requestToken);
    }
}