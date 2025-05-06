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
        // public static JobRequestDto ToMostepRequestDto(this VwApiMostep mostepModel)
        // {
        //     return new JobRequestDto
        //     {
        //         Job = mostepModel.Job ?? string.Empty
        //     };
        // }

        // public static MonoRequestDto ToMostepOdpRequestDto(this VwApiMostep mostepOdpModel)
        // {
        //     return new MonoRequestDto
        //     {
        //         Job = mostepOdpModel.Job ?? string.Empty,
        //         Mono = mostepOdpModel.Mono ?? string.Empty,
        //         CreationDate = mostepOdpModel.CreationDate ?? DateTime.MinValue
        //     };
        // }

        // public static OperationRequestDto ToMostepLavorazioniRequestDto(this VwApiMostep mostepLavorazioniModel)
        // {
        //     return new OperationRequestDto
        //     {
        //         Job = mostepLavorazioniModel.Job ?? string.Empty,
        //         Mono = mostepLavorazioniModel.Mono ?? string.Empty,
        //         CreationDate = mostepLavorazioniModel.CreationDate ?? DateTime.MinValue,
        //         Operation = mostepLavorazioniModel.Operation ?? string.Empty
        //     };
        // }
    }
}