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
        // Dto Modelli
        public static MostepDto ToMostepDto(this VwApiMostep mostepModel)
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

        // Dto Richieste
        public static MostepRequestDto ToMostepRequestDto(this VwApiMostep mostepModel)
        {
            return new MostepRequestDto
            {
                Job = mostepModel.Job ?? string.Empty
            };
        }

        public static MostepOdpRequestDto ToMostepOdpRequestDto(this VwApiMostep mostepOdpModel)
        {
            return new MostepOdpRequestDto
            {
                Job = mostepOdpModel.Job ?? string.Empty,
                Mono = mostepOdpModel.Mono ?? string.Empty,
                CreationDate = mostepOdpModel.CreationDate
            };
        }

        public static MostepLavorazioniRequestDto ToMostepLavorazioniRequestDto(this VwApiMostep mostepLavorazioniModel)
        {
            return new MostepLavorazioniRequestDto
            {
                Job = mostepLavorazioniModel.Job ?? string.Empty,
                Mono = mostepLavorazioniModel.Mono ?? string.Empty,
                CreationDate = mostepLavorazioniModel.CreationDate,
                Operation = mostepLavorazioniModel.Operation ?? string.Empty
            };
        }
    }
}