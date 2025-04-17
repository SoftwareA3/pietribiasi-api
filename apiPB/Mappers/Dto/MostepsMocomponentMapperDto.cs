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
        // Dto Modelli
        public static MostepsMocomponentDto ToMostepsMocomponentDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
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
        // Dto Richieste
        public static MostepsMocomponentRequestDto ToMostepsMocomponentRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        {
            return new MostepsMocomponentRequestDto
            {
                Job = mostepsMocomponentModel.Job ?? string.Empty,
                RtgStep = mostepsMocomponentModel.RtgStep,
                Alternate = mostepsMocomponentModel.Alternate ?? string.Empty,
                AltRtgStep = mostepsMocomponentModel.AltRtgStep,
                Position = mostepsMocomponentModel.Position,
                Component = mostepsMocomponentModel.Component ?? string.Empty
            };
        }

        public static MostepsMocomponentJobRequestDto ToMostepsMocomponentJobRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        {
            return new MostepsMocomponentJobRequestDto
            {
                Job = mostepsMocomponentModel.Job ?? string.Empty
            };
        }
        public static MostepsMocomponentMonoRequestDto ToMostepsMocomponentMonoRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        {
            return new MostepsMocomponentMonoRequestDto
            {
                Job = mostepsMocomponentModel.Job ?? string.Empty,
                Mono = mostepsMocomponentModel.Mono ?? string.Empty,
                CreationDate = mostepsMocomponentModel.CreationDate
            };
        }

        public static MostepsMocomponentOperationRequestDto ToMostepsMocomponentOperationRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        {
            return new MostepsMocomponentOperationRequestDto
            {
                Job = mostepsMocomponentModel.Job ?? string.Empty,
                Mono = mostepsMocomponentModel.Mono ?? string.Empty,
                CreationDate = mostepsMocomponentModel.CreationDate,
                Operation = mostepsMocomponentModel.Operation ?? string.Empty
            };
        }

        public static MostepsMocomponentBarCodeRequestDto ToMostepsMocomponentBarCodeRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        {
            return new MostepsMocomponentBarCodeRequestDto
            {
                Job = mostepsMocomponentModel.Job ?? string.Empty,
                Mono = mostepsMocomponentModel.Mono ?? string.Empty,
                CreationDate = mostepsMocomponentModel.CreationDate,
                Operation = mostepsMocomponentModel.Operation ?? string.Empty,
                BarCode = mostepsMocomponentModel.BarCode ?? string.Empty
            };
        }
    }
    
}