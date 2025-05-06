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
                BarCode = mostepsMocomponentModel.BarCode ?? string.Empty,
                Wc = mostepsMocomponentModel.Wc ?? string.Empty
            };
        }

        // public static JobRequestDto ToMostepsMocomponentJobRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        // {
        //     return new JobRequestDto
        //     {
        //         Job = mostepsMocomponentModel.Job ?? string.Empty
        //     };
        // }
        // public static MonoRequestDto ToMostepsMocomponentMonoRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        // {
        //     return new MonoRequestDto
        //     {
        //         Job = mostepsMocomponentModel.Job ?? string.Empty,
        //         Mono = mostepsMocomponentModel.Mono ?? string.Empty,
        //         CreationDate = mostepsMocomponentModel.CreationDate ?? DateTime.MinValue
        //     };
        // }

        // public static OperationRequestDto ToMostepsMocomponentOperationRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        // {
        //     return new OperationRequestDto
        //     {
        //         Job = mostepsMocomponentModel.Job ?? string.Empty,
        //         Mono = mostepsMocomponentModel.Mono ?? string.Empty,
        //         CreationDate = mostepsMocomponentModel.CreationDate ?? DateTime.MinValue,
        //         Operation = mostepsMocomponentModel.Operation ?? string.Empty
        //     };
        // }

        // public static BarCodeRequestDto ToMostepsMocomponentBarCodeRequestDto(this VwApiMostepsMocomponent mostepsMocomponentModel)
        // {
        //     return new BarCodeRequestDto
        //     {
        //         Job = mostepsMocomponentModel.Job ?? string.Empty,
        //         Mono = mostepsMocomponentModel.Mono ?? string.Empty,
        //         CreationDate = mostepsMocomponentModel.CreationDate ?? DateTime.MinValue,
        //         Operation = mostepsMocomponentModel.Operation ?? string.Empty,
        //         BarCode = mostepsMocomponentModel.BarCode ?? string.Empty
        //     };
        // }
    }
    
}