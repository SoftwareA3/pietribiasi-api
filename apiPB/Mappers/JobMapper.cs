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

        public static VwApiMostepRequestDto ToVwMostepRequestDto(this VwApiMostep mostepModel)
        {
            return new VwApiMostepRequestDto
            {
                Job = mostepModel.Job ?? string.Empty
            };
        }
    }
}