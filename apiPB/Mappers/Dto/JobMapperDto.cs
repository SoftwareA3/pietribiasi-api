using apiPB.Dto.Request;
using apiPB.Dto.Models;
using apiPB.Models;

namespace apiPB.Mappers.Dto
{
    public static class JobMapperDto
    {
        // Dto Modelli
        public static JobDto ToJobDto(this VwApiJob jobModel)
        {
            return new JobDto
            {
                Job = jobModel.Job ?? string.Empty,
                Description = jobModel.Description ?? string.Empty
            };
        }

        public static MoDto ToMoDto(this VwApiMo moModel)
        {
            return new MoDto
            {
                Job = moModel.Job ?? string.Empty,
                RtgStep = moModel.RtgStep,
                Alternate = moModel.Alternate ?? string.Empty,
                AltRtgStep = moModel.AltRtgStep,
                Bom = moModel.Bom ?? string.Empty,
                Variant = moModel.Variant ?? string.Empty,
                ItemDesc = moModel.ItemDesc ?? string.Empty,
                Moid = moModel.Moid,
                Mono = moModel.Mono ?? string.Empty,
                Uom = moModel.Uom ?? string.Empty,
                ProductionQty = moModel.ProductionQty,
                ProducedQty = moModel.ProducedQty
            };
        }

        public static MostepDto ToMostepDto(this VwApiMostep mostepModel)
        {
            return new MostepDto
            {
                Job = mostepModel.Job ?? string.Empty,
                RtgStep = mostepModel.RtgStep,
                Alternate = mostepModel.Alternate ?? string.Empty,
                AltRtgStep = mostepModel.AltRtgStep,
                Wc = mostepModel.Wc ?? string.Empty,
                Operation = mostepModel.Operation ?? string.Empty,
                Storage = mostepModel.Storage ?? string.Empty
            };
        }

        public static MocomponentDto ToMocomponentDto(this VwApiMocomponent mocomponentModel)
        {
            return new MocomponentDto
            {
                Job = mocomponentModel.Job ?? string.Empty,
                Moid = mocomponentModel.Moid,
                Mono = mocomponentModel.Mono ?? string.Empty,
                Line = mocomponentModel.Line,
                Position = mocomponentModel.Position,
                ReferredPosition = mocomponentModel.ReferredPosition,
                Component = mocomponentModel.Component ?? string.Empty,
                ComponentDesc = mocomponentModel.ComponentDesc ?? string.Empty,
                UoM = mocomponentModel.UoM ?? string.Empty,
                NeededQty = mocomponentModel.NeededQty,
                AssignedQuantity = mocomponentModel.AssignedQuantity,
                PickedQuantity = mocomponentModel.PickedQuantity,
                Storage = mocomponentModel.Storage ?? string.Empty,
                Lot = mocomponentModel.Lot ?? string.Empty,
                SpecificatorType = mocomponentModel.SpecificatorType,
                Specificator = mocomponentModel.Specificator ?? string.Empty,
                Closed = mocomponentModel.Closed ?? string.Empty
            };
        }

        public static MoStepsComponentDto ToMoStepsComponentDto(this VwApiMoStepsComponent moStepsComponentModel)
        {
            return new MoStepsComponentDto
            {
                Job = moStepsComponentModel.Job ?? string.Empty,
                RtgStep = moStepsComponentModel.RtgStep,
                Alternate = moStepsComponentModel.Alternate ?? string.Empty,
                AltRtgStep = moStepsComponentModel.AltRtgStep,
                Position = moStepsComponentModel.Position,
                Component = moStepsComponentModel.Component ?? string.Empty,
                Bom = moStepsComponentModel.Bom ?? string.Empty,
                Variant = moStepsComponentModel.Variant ?? string.Empty,
                ItemDesc = moStepsComponentModel.ItemDesc ?? string.Empty,
                Moid = moStepsComponentModel.Moid,
                Mono = moStepsComponentModel.Mono ?? string.Empty,
                Uom = moStepsComponentModel.Uom ?? string.Empty,
                ProductionQty = moStepsComponentModel.ProductionQty,
                ProducedQty = moStepsComponentModel.ProducedQty
            };
        }

        // Dto Richieste
        public static MoRequestDto ToMoRequestDto(this VwApiMo moModel)
        {
            return new MoRequestDto
            {
                Job = moModel.Job ?? string.Empty,
                RtgStep = moModel.RtgStep,
                Alternate = moModel.Alternate ?? string.Empty,
                AltRtgStep = moModel.AltRtgStep
            };
        }

        public static MostepRequestDto ToMostepRequestDto(this VwApiMostep mostepModel)
        {
            return new MostepRequestDto
            {
                Job = mostepModel.Job ?? string.Empty
            };
        }

        public static MocomponentRequestDto ToMocomponentRequestDto(this VwApiMocomponent mocomponentModel)
        {
            return new MocomponentRequestDto
            {
                Job = mocomponentModel.Job ?? string.Empty
            };
        }

        public static MoStepsComponentRequestDto ToMoStepsComponentRequestDto(this VwApiMoStepsComponent moStepsComponentModel)
        {
            return new MoStepsComponentRequestDto
            {
                Job = moStepsComponentModel.Job ?? string.Empty,
                RtgStep = moStepsComponentModel.RtgStep,
                Alternate = moStepsComponentModel.Alternate ?? string.Empty,
                AltRtgStep = moStepsComponentModel.AltRtgStep,
                Position = moStepsComponentModel.Position,
                Component = moStepsComponentModel.Component ?? string.Empty
            };
        }
    }
}