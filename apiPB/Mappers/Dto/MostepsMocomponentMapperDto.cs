using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Mappers.Dto
{
    public static class MostepsMocomponentMapperDto
    {
        // Da Model a Dto
        public static MostepsMocomponentDto ToMostepsMocomponentDtoFromModel(this VwApiMostepsMocomponent mostepsMocomponentModel)
        {
            return new MostepsMocomponentDto
            {
                Job = mostepsMocomponentModel.Job ?? string.Empty,
                RtgStep = mostepsMocomponentModel.RtgStep,
                Alternate = mostepsMocomponentModel.Alternate ?? string.Empty,
                AltRtgStep = mostepsMocomponentModel.AltRtgStep,
                Operation = mostepsMocomponentModel.Operation ?? string.Empty,
                OperDesc = mostepsMocomponentModel.OperDesc ?? string.Empty,
                Position = mostepsMocomponentModel.Position,
                Component = mostepsMocomponentModel.Component ?? string.Empty,
                Bom = mostepsMocomponentModel.Bom ?? string.Empty,
                Variant = mostepsMocomponentModel.Variant ?? string.Empty,
                ItemDesc = mostepsMocomponentModel.ItemDesc ?? string.Empty,
                Moid = mostepsMocomponentModel.Moid,
                Mono = mostepsMocomponentModel.Mono ?? string.Empty,
                CreationDate = mostepsMocomponentModel.CreationDate,
                UoM = mostepsMocomponentModel.UoM ?? string.Empty,
                ProductionQty = mostepsMocomponentModel.ProductionQty,
                ProducedQty = mostepsMocomponentModel.ProducedQty,
                ResQty = mostepsMocomponentModel.ResQty,
                Storage = mostepsMocomponentModel.Storage ?? string.Empty,
                BarCode = mostepsMocomponentModel.BarCode ?? string.Empty
            };
        }

        // Da Dto a Request
        public static MostepsMocomponentRequestDto ToMostepsMocomponentRequestDtoFromDto(this MostepsMocomponentDto mostepsMocomponentDto)
        {
            return new MostepsMocomponentRequestDto
            {
                Job = mostepsMocomponentDto.Job,
                RtgStep = mostepsMocomponentDto.RtgStep,
                Alternate = mostepsMocomponentDto.Alternate,
                AltRtgStep = mostepsMocomponentDto.AltRtgStep,
                Operation = mostepsMocomponentDto.Operation,
                OperDesc = mostepsMocomponentDto.OperDesc,
                Position = mostepsMocomponentDto.Position,
                Component = mostepsMocomponentDto.Component,
                Bom = mostepsMocomponentDto.Bom,
                Variant = mostepsMocomponentDto.Variant,
                ItemDesc = mostepsMocomponentDto.ItemDesc,
                Moid = mostepsMocomponentDto.Moid,
                Mono = mostepsMocomponentDto.Mono,
                CreationDate = mostepsMocomponentDto.CreationDate,
                UoM = mostepsMocomponentDto.UoM,
                ProductionQty = mostepsMocomponentDto.ProductionQty,
                ProducedQty = mostepsMocomponentDto.ProducedQty,
                ResQty = mostepsMocomponentDto.ResQty,
                Storage = mostepsMocomponentDto.Storage,
                BarCode = mostepsMocomponentDto.BarCode
            };
        }

        // Da Request a Dto
        public static MostepsMocomponentDto ToMostepsMocomponentDtoFromRequest(this MostepsMocomponentRequestDto mostepsMocomponentRequestDto)
        {
            return new MostepsMocomponentDto
            {
                Job = mostepsMocomponentRequestDto.Job ?? string.Empty,
                RtgStep = mostepsMocomponentRequestDto.RtgStep ?? 0,
                Alternate = mostepsMocomponentRequestDto.Alternate ?? string.Empty,
                AltRtgStep = mostepsMocomponentRequestDto.AltRtgStep ?? 0,
                Operation = mostepsMocomponentRequestDto.Operation ?? string.Empty,
                OperDesc = mostepsMocomponentRequestDto.OperDesc ?? string.Empty,
                Position = mostepsMocomponentRequestDto.Position ?? 0,
                Component = mostepsMocomponentRequestDto.Component ?? string.Empty,
                Bom = mostepsMocomponentRequestDto.Bom ?? string.Empty,
                Variant = mostepsMocomponentRequestDto.Variant ?? string.Empty,
                ItemDesc = mostepsMocomponentRequestDto.ItemDesc ?? string.Empty,
                Moid = mostepsMocomponentRequestDto.Moid ?? 0,
                Mono = mostepsMocomponentRequestDto.Mono ?? string.Empty,
                CreationDate = mostepsMocomponentRequestDto.CreationDate,
                UoM = mostepsMocomponentRequestDto.UoM ?? string.Empty,
                ProductionQty = mostepsMocomponentRequestDto.ProductionQty ?? 0.0,
                ProducedQty = mostepsMocomponentRequestDto.ProducedQty ?? 0.0,
                ResQty = mostepsMocomponentRequestDto.ResQty ?? 0.0,
                Storage = mostepsMocomponentRequestDto.Storage ?? string.Empty,
                BarCode = mostepsMocomponentRequestDto.BarCode
            };
        }
    }
    
}