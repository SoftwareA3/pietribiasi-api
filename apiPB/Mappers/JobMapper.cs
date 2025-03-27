using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto;
using apiPB.Models;

namespace apiPB.Mappers
{
    public static class JobMapper
    {
        // Dto Modelli
        public static VwApiJobDto ToVwApiJobDto(this VwApiJob jobModel)
        {
            return new VwApiJobDto
            {
                Job = jobModel.Job ?? string.Empty,
                Description = jobModel.Description ?? string.Empty
            };
        }

        public static VwApiMoDto ToVwApiMoDto(this VwApiMo moModel)
        {
            return new VwApiMoDto
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

        public static VwApiMostepDto ToVwApiMoStepDto(this VwApiMostep mostepModel)
        {
            return new VwApiMostepDto
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

        public static VwApiMocomponentDto ToVwApiMocomponentDto(this VwApiMocomponent mocomponentModel)
        {
            return new VwApiMocomponentDto
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

        public static VwApiMoStepsComponentDto ToVwApiMoStepsComponentDto(this VwApiMoStepsComponent moStepsComponentModel)
        {
            return new VwApiMoStepsComponentDto
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
        public static VwApiMoRequestDto ToVwApiMoRequestDto(this VwApiMo moModel)
        {
            return new VwApiMoRequestDto
            {
                Job = moModel.Job ?? string.Empty,
                RtgStep = moModel.RtgStep,
                Alternate = moModel.Alternate ?? string.Empty,
                AltRtgStep = moModel.AltRtgStep
            };
        }

        public static VwApiMostepRequestDto ToVwApiMostepRequestDto(this VwApiMostep mostepModel)
        {
            return new VwApiMostepRequestDto
            {
                Job = mostepModel.Job ?? string.Empty
            };
        }

        public static VwApiMocomponentRequestDto ToVwApiMocomponentRequestDto(this VwApiMocomponent mocomponentModel)
        {
            return new VwApiMocomponentRequestDto
            {
                Job = mocomponentModel.Job ?? string.Empty
            };
        }

        public static VwApiMoStepsComponentRequestDto ToVwApiMoStepsComponentRequestDto(this VwApiMoStepsComponent moStepsComponentModel)
        {
            return new VwApiMoStepsComponentRequestDto
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