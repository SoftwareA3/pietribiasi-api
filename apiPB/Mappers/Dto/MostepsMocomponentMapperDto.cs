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
                Position = mostepsMocomponentModel.Position,
                Component = mostepsMocomponentModel.Component ?? string.Empty,
                Bom = mostepsMocomponentModel.Bom ?? string.Empty,
                Variant = mostepsMocomponentModel.Variant ?? string.Empty,
                ItemDesc = mostepsMocomponentModel.ItemDesc ?? string.Empty,
                Moid = mostepsMocomponentModel.Moid,
                Mono = mostepsMocomponentModel.Mono ?? string.Empty,
                Uom = mostepsMocomponentModel.Uom ?? string.Empty,
                ProductionQty = mostepsMocomponentModel.ProductionQty,
                ProducedQty = mostepsMocomponentModel.ProducedQty,
                ResQty = mostepsMocomponentModel.ResQty,
                Storage = mostepsMocomponentModel.Storage ?? string.Empty
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
    }
    
}