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
                Storage = mostepModel.Storage ?? string.Empty
            };
        }

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
                ProducedQty = mostepsMocomponentModel.ProducedQty
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