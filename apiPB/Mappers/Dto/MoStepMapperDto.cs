using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Mappers.Dto
{
    public static class MoStepMapperDto
    {
        // Da Model a Dto
        public static MostepDto ToMostepDtoFromModel(this VwApiMostep mostepModel)
        {
            return new MostepDto
            {
                Job = mostepModel.Job ?? string.Empty,
                RtgStep = mostepModel.RtgStep,
                Alternate = mostepModel.Alternate ?? string.Empty,
                AltRtgStep = mostepModel.AltRtgStep,
                Operation = mostepModel.Operation ?? string.Empty,
                OperDesc = mostepModel.OperDesc ?? string.Empty,
                Bom = mostepModel.Bom ?? string.Empty,
                Variant = mostepModel.Variant ?? string.Empty,
                ItemDesc = mostepModel.ItemDesc ?? string.Empty,
                Moid = mostepModel.Moid,
                Mono = mostepModel.Mono ?? string.Empty,
                CreationDate = mostepModel.CreationDate,
                Uom = mostepModel.Uom ?? string.Empty,
                ProductionQty = mostepModel.ProductionQty,
                ProducedQty = mostepModel.ProducedQty,
                ResQty = mostepModel.ResQty,
                Storage = mostepModel.Storage ?? string.Empty,
                Wc = mostepModel.Wc ?? string.Empty
            };
        }

        // Da Dto a Request
        public static MostepRequestDto ToMostepRequestDtoFromDto(this MostepDto mostepDto)
        {
            return new MostepRequestDto
            {
                Job = mostepDto.Job,
                RtgStep = mostepDto.RtgStep,
                Alternate = mostepDto.Alternate,
                AltRtgStep = mostepDto.AltRtgStep,
                Operation = mostepDto.Operation,
                OperDesc = mostepDto.OperDesc,
                Bom = mostepDto.Bom,
                Variant = mostepDto.Variant,
                ItemDesc = mostepDto.ItemDesc,
                Moid = mostepDto.Moid,
                Mono = mostepDto.Mono,
                CreationDate = mostepDto.CreationDate,
                Uom = mostepDto.Uom,
                ProductionQty = mostepDto.ProductionQty,
                ProducedQty = mostepDto.ProducedQty,
                ResQty = mostepDto.ResQty,
                Storage = mostepDto.Storage,
                Wc = mostepDto.Wc
            };
        }

        // Da Request a Dto
        public static MostepDto ToMostepDtoFromRequest(this MostepRequestDto requestDto)
        {
            return new MostepDto
            {
                Job = requestDto.Job ?? string.Empty,
                RtgStep = requestDto.RtgStep ?? 0,
                Alternate = requestDto.Alternate ?? string.Empty,
                AltRtgStep = requestDto.AltRtgStep ?? 0,
                Operation = requestDto.Operation ?? string.Empty,
                OperDesc = requestDto.OperDesc ?? string.Empty,
                Bom = requestDto.Bom ?? string.Empty,
                Variant = requestDto.Variant ?? string.Empty,
                ItemDesc = requestDto.ItemDesc ?? string.Empty,
                Moid = requestDto.Moid ?? 0,
                Mono = requestDto.Mono ?? string.Empty,
                CreationDate = requestDto.CreationDate,
                Uom = requestDto.Uom ?? string.Empty,
                ProductionQty = requestDto.ProductionQty ?? 0,
                ProducedQty = requestDto.ProducedQty ?? 0,
                ResQty = requestDto.ResQty ?? 0,
                Storage = requestDto.Storage ?? string.Empty,
                Wc = requestDto.Wc ?? string.Empty
            };
        }
    }
}