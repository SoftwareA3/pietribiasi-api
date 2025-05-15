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
                Company = settings.Company,
                SpecificatorType = settings.SpecificatorType,
                Closed = settings.Closed
            };
        }

        public static MagoLoginRequestDto ToMagoLoginRequestDto(this SettingsDto settings)
        {
            return new MagoLoginRequestDto
            {
                Username = settings.Username,
                Password = settings.Password,
                Company = settings.Company
            };
        }

        public static SyncSettingsRequestDto ToSyncSettingsRequestDto(this SettingsDto settings, int? workerId)
        {
            return new SyncSettingsRequestDto
            {
                MagoUrl = settings.MagoUrl,
                Username = settings.Username,
                Password = settings.Password,
                Company = settings.Company,
                SpecificatorType = settings.SpecificatorType,
                Closed = settings.Closed,
                WorkerId = workerId
            };
        }

        public static List<SyncRegOreRequestDto> ToSyncregOreRequestDto(this SyncSettingsRequestDto settings, List<RegOreDto> regOreList)
        {
            List<SyncRegOreRequestDto> syncRegOreList = new List<SyncRegOreRequestDto>();

            foreach (var regOre in regOreList)
            {
                long workingTimeInSeconds = (long)regOre.WorkingTime; // esempio: 542439
                TimeSpan workingTimeSpan = TimeSpan.FromSeconds(workingTimeInSeconds);

                SyncRegOreRequestDto syncRegOre = new SyncRegOreRequestDto
                {
                    Closed = settings.Closed,
                    WorkerId = settings.WorkerId,
                    MoId = regOre.Moid,
                    RtgStep = regOre.RtgStep,
                    Alternate = regOre.Alternate,
                    AltRtgStep = regOre.AltRtgStep,
                    ActualProcessingTime = workingTimeSpan,
                    WorkerProcessingTime = workingTimeSpan,
                    SecondRateVariant = regOre.Variant,
                    ScrapVariant = regOre.Variant,
                    Variant = regOre.Variant,
                    Bom = regOre.Bom,
                    Storage = regOre.Storage,
                    ScrapStorage = regOre.Storage,
                    Wc = regOre.Wc,
                };

                syncRegOreList.Add(syncRegOre);
            }

            return syncRegOreList;
        }

        public static List<SyncPrelMatRequestDto> ToSyncPrelMatRequestDto(this SyncSettingsRequestDto settings, List<PrelMatDto> prelMatList)
        {
            var syncPrelMatList = prelMatList
                .GroupBy(p => new { p.Moid, p.RtgStep, p.Alternate, p.AltRtgStep, p.WorkerId })
                .Select(group => new SyncPrelMatRequestDto
                {
                    MoId = group.Key.Moid,
                    RtgStep = group.Key.RtgStep,
                    Alternate = group.Key.Alternate,
                    AltRtgStep = group.Key.AltRtgStep,
                    WorkerId = group.Key.WorkerId,
                    ActionDetails = group
                        .Select(p => new SyncPrelMatDetailsRequestdto
                        {
                            Position = p.Position,
                            PickedQty = p.PrelQty,
                            Closed = settings.Closed,
                            SpecificatorType = settings.SpecificatorType,
                            Storage = p.Storage
                        }).ToList()
                })
                .ToList();

            return syncPrelMatList;
        }
    }
}