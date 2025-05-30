using apiPB.Models;
using apiPB.Dto.Models;
using apiPB.Dto.Request;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace apiPB.Mappers.Dto
{
    /// <summary>
    /// Mapper dei messaggi di log delle azioni.
    /// Vengono realizzati dei dizionari per il mapping dei codici numerici in stringhe leggibili.
    /// Vengono raggruppati i messaggi per informazioni comuni in modo da evitare duplicazioni.
    /// </summary>
    public static class ActionMessageMapperDto
    {
        // Dizionari di mapping per codici del log

        private static readonly Dictionary<int, string> actionStatusDictionary = new Dictionary<int, string>
        {
            {2051604480, "Da Fare"},
            {2051604481, "In Lavorazione"},
            {2051604482, "Eseguita"},
            {2051604483, "Errore"},
            {2051604484, "Tutte"},
            {2051604485, "WIP"},
            {2051604486, "In attesa"}
        };

        private static readonly Dictionary<int, string> actionTypeDictionary = new Dictionary<int, string>
        {
            {2051538944, "Lancio in Produzione"},
            {2051538945, "Consuntivazione OdP"},
            {2051538946, "Prelievo Materiale"},
            {2051538947, "Tutte"},
            {2051538948, "Creazione OdP"},
            {2051538949, "Movimentazione Distinta Base"}
        };

        private static readonly Dictionary<int, string> messageTypeDictionary = new Dictionary<int, string>
        {
            {2051342336, "Suggerimento"},
            {2051342337, "Attenzione"},
            {2051342338, "Errore"},
        };

        private static readonly Dictionary<int, string> moStatusDictionary = new Dictionary<int, string>
        {
            {20578304, "Lanciato"},
            {20578305, "In Lavorazione"},
            {20578306, "Terminata"},
            {20578307, "Creato"},
            {20578308, "Proposto da MRP"},
            {20578309, "Pianificato da CRP"},
            {20578310, "Proposto da MRP-CRP"},
        };

        public static ActionMessageListDto ToOmActionMessageDto(this List<VwOmActionMessage> model)
        {
            var actionMessage = model
                .GroupBy(x => new
                {
                    x.Moid,
                    x.RtgStep,
                    x.Alternate,
                    x.AltRtgStep,
                    x.Mono,
                    x.Bom,
                    x.Variant,
                    x.Wc,
                    x.Operation,
                    x.WorkerId,
                    x.ActionType,
                })
                .Select(group => new ActionMessageListDto
                {
                    ActionType = actionTypeDictionary.TryGetValue(group.Key.ActionType ?? 0, out var tipoAzione)
                            ? tipoAzione
                            : "Tipo Azione Sconosciuto",
                    Moid = group.Key.Moid,
                    RtgStep = group.Key.RtgStep,
                    Alternate = group.Key.Alternate,
                    AltRtgStep = group.Key.AltRtgStep,
                    Mono = group.Key.Mono,
                    Bom = group.Key.Bom,
                    Variant = group.Key.Variant,
                    Wc = group.Key.Wc,
                    Operation = group.Key.Operation,
                    WorkerId = group.Key.WorkerId,
                    OmMessageDetails = group
                        .Select(x => new OmMessageDetailsDto
                        {
                            MessageId = x.MessageId,
                            MessageDate = x.MessageDate,
                            MessageText = x.MessageText,
                            MessageType = messageTypeDictionary.TryGetValue(x.MessageType ?? 0, out var tipoMessaggio)
                                ? tipoMessaggio
                                : "Tipo Messaggio Sconosciuto",
                        })
                        .GroupBy(m => new { m.MessageId, m.MessageDate, m.MessageText, m.MessageType })
                        .Select(g => g.First())
                        .ToList(),
                    ActionMessageDetails = group
                        .Select(x => new ActionMessageDetailsDto
                        {
                            ActionId = x.ActionId,
                            Closed = x.Closed,
                            WorkerProcessingTime = x.WorkerProcessingTime,
                            WorkerSetupTime = x.WorkerSetupTime,
                            ActualProcessingTime = x.ActualProcessingTime,
                            ActualSetupTime = x.ActualSetupTime,
                            Storage = x.Storage,
                            SpecificatorType = x.SpecificatorType,
                            Specificator = x.Specificator,
                            ReturnMaterialQtyLower = x.ReturnMaterialQtyLower,
                            PickMaterialQtyGreater = x.PickMaterialQtyGreater,
                            ProductionLotNumber = x.ProductionLotNumber,
                            ProductionQty = x.ProductionQty,
                            DeliveryDate = x.DeliveryDate,
                            ConfirmChildMos = x.ConfirmChildMos,
                            ActionStatus = actionStatusDictionary.TryGetValue(x.ActionStatus ?? 0, out var statoAzione)
                                ? statoAzione
                                : "Azione Sconosciuta",
                            Mostatus = moStatusDictionary.TryGetValue(x.Mostatus ?? 0, out var statoMo)
                                ? statoMo
                                : "Stato MO Sconosciuto",
                        })
                        .GroupBy(a => new
                        {
                            a.ActionId,
                            a.Closed,
                            a.WorkerProcessingTime,
                            a.WorkerSetupTime,
                            a.ActualProcessingTime,
                            a.ActualSetupTime,
                            a.Storage,
                            a.SpecificatorType,
                            a.Specificator,
                            a.ReturnMaterialQtyLower,
                            a.PickMaterialQtyGreater,
                            a.ProductionLotNumber,
                            a.ProductionQty,
                            a.DeliveryDate,
                            a.ConfirmChildMos,
                            a.ActionStatus,
                            a.Mostatus
                        })
                        .Select(g => g.First())
                        .ToList(),
                }).ToList();
            return actionMessage.FirstOrDefault() ?? new ActionMessageListDto();


            // In caso di rollback, il codice originale è commentato qui sotto

            // return new ActionMessageListDto
            // {
            //     ActionId = model.ActionId,
            //     Moid = model.Moid,
            //     RtgStep = model.RtgStep,
            //     Alternate = model.Alternate,
            //     AltRtgStep = model.AltRtgStep,
            //     Mono = model.Mono,
            //     Bom = model.Bom,
            //     Variant = model.Variant,
            //     Wc = model.Wc,
            //     Operation = model.Operation,
            //     Job = model.Job,
            //     WorkerId = model.WorkerId,
            //     ActionMessage = model.ActionMessage,
            //     Closed = model.Closed,
            //     WorkerProcessingTime = model.WorkerProcessingTime,
            //     WorkerSetupTime = model.WorkerSetupTime,
            //     ActualProcessingTime = model.ActualProcessingTime,
            //     ActualSetupTime = model.ActualSetupTime,
            //     Storage = model.Storage,
            //     SpecificatorType = model.SpecificatorType,
            //     Specificator = model.Specificator,
            //     ReturnMaterialQtyLower = model.ReturnMaterialQtyLower,
            //     PickMaterialQtyGreater = model.PickMaterialQtyGreater,
            //     ProductionLotNumber = model.ProductionLotNumber,
            //     ProductionQty = model.ProductionQty,
            //     DeliveryDate = model.DeliveryDate,
            //     ConfirmChildMos = model.ConfirmChildMos,
            //     MessageId = model.MessageId,
            //     MessageDate = model.MessageDate,
            //     MessageText = model.MessageText,

            //     // Mappatura dei valori numerici in messaggi
            //     MessageType = model.MessageType.HasValue && messageTypeDictionary.TryGetValue(model.MessageType.Value, out var tipoMessaggio)
            //     ? tipoMessaggio
            //     : "Tipo Messaggio Sconosciuto",

            //     ActionStatus = model.ActionStatus.HasValue && actionStatusDictionary.TryGetValue(model.ActionStatus.Value, out var statoAzione)
            //     ? statoAzione
            //     : "Azione Sconosciuta",

            //     ActionType = model.ActionType.HasValue && actionTypeDictionary.TryGetValue(model.ActionType.Value, out var tipoAzione)
            //     ? tipoAzione
            //     : "Tipo Azione Sconosciuto",

            //     Mostatus = model.Mostatus.HasValue && moStatusDictionary.TryGetValue(model.Mostatus.Value, out var statoMo)
            //     ? statoMo
            //     : "Stato MO Sconosciuto"
            // };
        }
    }
}