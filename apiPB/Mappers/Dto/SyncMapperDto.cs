using apiPB.Models;
using apiPB.Dto.Request;
using apiPB.Dto.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json.Serialization;

namespace apiPB.Mappers.Dto
{
    /// <summary>
    /// Mapper per la conversione dei dati di richiesta e sincronizzazione a Mago4.
    /// Alcuni metodi sono specifici per la sincronizzazione dei dati delle operazioni.
    /// </summary>
    public static class SyncMapperDto
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
                RectificationReasonPositive = settings.RectificationReasonPositive,
                RectificationReasonNegative = settings.RectificationReasonNegative,
                Storage = settings.Storage,
                TerminaLavorazioniUtente = settings.TerminaLavorazioniUtente,
                SyncGlobalActive = settings.SyncGlobalActive,
                ExternalReferences = settings.ExternalReferences.ToString()
            };
        }

        public static SyncGobalActiveRequestDto ToSyncGobalActiveRequestDto(this A3AppSetting settings)
        {
            return new SyncGobalActiveRequestDto
            {
                SyncGlobalActive = settings.SyncGlobalActive
            };
        }

        public static TerminaLavorazioniUtenteRequestDto ToTerminaLavorazioniUtenteRequestDto(this A3AppSetting settings)
        {
            return new TerminaLavorazioniUtenteRequestDto
            {
                TerminaLavorazioniUtente = settings.TerminaLavorazioniUtente
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

        public static SyncronizedDataDto ToSyncronizedDataDto(this SyncronizedDataDto sincData, List<SyncRegOreRequestDto>? syncRegOreList, List<SyncPrelMatRequestDto>? syncPrelMatList, List<SyncInventarioRequestDto>? syncInventarioList)
        {
            if (syncInventarioList != null && syncInventarioList.Count > 0) sincData.InventarioRequest = syncInventarioList;
            if (syncRegOreList != null && syncRegOreList.Count > 0) sincData.RegOreRequest = syncRegOreList;
            if (syncPrelMatList != null && syncPrelMatList.Count > 0) sincData.PrelMatRequest = syncPrelMatList;
            return sincData;
        }

        public static List<SyncRegOreRequestDto> ToSyncregOreRequestDto(this SettingsDto settings, List<RegOreDto> regOreList)
        {
            List<SyncRegOreRequestDto> syncRegOreList = new List<SyncRegOreRequestDto>();

            foreach (var regOre in regOreList)
            {
                long workingTimeInSeconds = regOre.WorkingTime ?? 0; // esempio: 542439
                TimeSpan workingTimeSpan = TimeSpan.FromSeconds(workingTimeInSeconds);

                SyncRegOreRequestDto syncRegOre = new SyncRegOreRequestDto
                {
                    Closed = regOre.Closed,
                    WorkerId = regOre.WorkerId,
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

        public static List<SyncPrelMatRequestDto> ToSyncPrelMatRequestDto(this SettingsDto settings, List<PrelMatDto> prelMatList)
        {
            var syncPrelMatList = prelMatList
                .GroupBy(p => new { p.Moid, p.WorkerId })
                .Select(group => new SyncPrelMatRequestDto
                {
                    MoId = group.Key.Moid,
                    WorkerId = group.Key.WorkerId,
                    ExternalReferences = "PB000" + settings.ExternalReferences?.ToString(),
                    ActionDetails = group
                        .Select(p => new SyncPrelMatDetailsRequestdto
                        {
                            Position = p.Position,
                            PickedQty = p.PrelQty,
                            SpecificatorType = settings.SpecificatorType,
                            Storage = p.Storage,
                            NeededQty = p.NeededQty,
                            Component = p.NeededQty == 0 ? string.Empty : p.Component,
                            ExternalProgram = settings.ExternalProgram
                        }).ToList(),
                    ExternalProgram = settings.ExternalProgram
                })
                .ToList();

            return syncPrelMatList;
        }

        public static List<DeleteMoComponentRequestDto> ToDeleteMoComponentRequestDto(this SettingsDto settings, List<PrelMatDto> prelMatList)
        {
            var deleteMoComponentList = new List<DeleteMoComponentRequestDto>();

            foreach (var item in prelMatList)
            {
                var deleteMoComponent = new DeleteMoComponentRequestDto
                {
                    MoId = item.Moid ?? 0,
                    Position = item.Position ?? 0,
                    ExternalReferences = "PB000" + settings.ExternalReferences?.ToString(),
                    ExternalProgram = settings.ExternalProgram
                };

                deleteMoComponentList.Add(deleteMoComponent);
            }

            return deleteMoComponentList;
        }

        public static List<SyncInventarioRequestDto> ToSyncInventarioRequestDto(this SettingsDto settings, List<InventarioDto> inventarioList)
        {
            var syncInventarioList = new List<SyncInventarioRequestDto>();

            foreach (var inventario in inventarioList)
            {
                var syncInventario = new SyncInventarioRequestDto
                {
                    MA_InventoryEntries = new MA_InventoryEntries
                    {
                        InvRsn = inventario.InvRsn == true ? settings.RectificationReasonPositive : settings.RectificationReasonNegative,
                        PostingDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        PreprintedDocNo = "PB000" + inventario.InvId.ToString(),
                        DocumentDate = inventario.SavedDate?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        StoragePhase1 = settings.Storage
                    },
                    MA_InventoryEntriesDetail = new List<MA_InventoryEntriesDetail>()
                    {
                        new MA_InventoryEntriesDetail
                        {
                            Item = inventario.Item,
                            Qty = inventario.BookInvDiff,
                            UoM = inventario.UoM,
                            UnitValue = 0,
                            DocumentType = 3801188
                        }
                    }
                };

                syncInventarioList.Add(syncInventario);
            }

            return syncInventarioList;
        }
    }
}