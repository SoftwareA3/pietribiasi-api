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
                Storage = mostepModel.Storage ?? string.Empty,
                Wc = mostepModel.Wc ?? string.Empty
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
                ProducedQty = mostepsMocomponentModel.ProducedQty,
                ResQty = mostepsMocomponentModel.ResQty,
                Storage = mostepsMocomponentModel.Storage ?? string.Empty
            };
        }

        public static A3AppRegOreDto ToA3AppRegOreDto(this A3AppRegOre a3AppRegOreModel)
        {
            return new A3AppRegOreDto
            {
                RegOreId = a3AppRegOreModel.RegOreId,
                WorkerId = a3AppRegOreModel.WorkerId ?? string.Empty,
                SavedDate = a3AppRegOreModel.SavedDate,
                Job = a3AppRegOreModel.Job ?? string.Empty,
                RtgStep = a3AppRegOreModel.RtgStep,
                Alternate = a3AppRegOreModel.Alternate ?? string.Empty,
                AltRtgStep = a3AppRegOreModel.AltRtgStep,
                Operation = a3AppRegOreModel.Operation ?? string.Empty,
                OperDesc = a3AppRegOreModel.OperDesc ?? string.Empty,
                Bom = a3AppRegOreModel.Bom ?? string.Empty,
                Variant = a3AppRegOreModel.Variant ?? string.Empty,
                ItemDesc = a3AppRegOreModel.ItemDesc ?? string.Empty,
                Moid = a3AppRegOreModel.Moid,
                Mono = a3AppRegOreModel.Mono ?? string.Empty,
                CreationDate = a3AppRegOreModel.CreationDate,
                Uom = a3AppRegOreModel.Uom ?? string.Empty,
                ProductionQty = a3AppRegOreModel.ProductionQty,
                ProducedQty = a3AppRegOreModel.ProducedQty,
                ResQty = a3AppRegOreModel.ResQty,
                Storage = a3AppRegOreModel.Storage ?? string.Empty,
                Wc = a3AppRegOreModel.Wc ?? string.Empty,
                WorkingTime = a3AppRegOreModel.WorkingTime,
                Imported = a3AppRegOreModel.Imported,
                UserImp = a3AppRegOreModel.UserImp ?? string.Empty,
                DataImp = a3AppRegOreModel.DataImp
            };
        }

        public static IEnumerable<A3AppRegOreDto> ToA3AppRegOreListDto(this IEnumerable<A3AppRegOre> a3AppRegOreModelList)
        {
            foreach (var a3AppRegOreModel in a3AppRegOreModelList)
            {
                yield return a3AppRegOreModel.ToA3AppRegOreDto();
            }
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

        public static A3AppRegOreRequestDto ToA3AppRegOreRequestDto(this A3AppRegOre a3AppRegOreModel)
        {
            return new A3AppRegOreRequestDto
            {
                WorkerId = a3AppRegOreModel.WorkerId ?? string.Empty,
                Job = a3AppRegOreModel.Job ?? string.Empty,
                RtgStep = a3AppRegOreModel.RtgStep,
                Alternate = a3AppRegOreModel.Alternate ?? string.Empty,
                AltRtgStep = a3AppRegOreModel.AltRtgStep,
                Operation = a3AppRegOreModel.Operation ?? string.Empty,
                OperDesc = a3AppRegOreModel.OperDesc ?? string.Empty,
                Bom = a3AppRegOreModel.Bom ?? string.Empty,
                Variant = a3AppRegOreModel.Variant ?? string.Empty,
                ItemDesc = a3AppRegOreModel.ItemDesc ?? string.Empty,
                Moid = a3AppRegOreModel.Moid,
                Uom = a3AppRegOreModel.Uom ?? string.Empty,
                ProductionQty = a3AppRegOreModel.ProductionQty,
                ProducedQty = a3AppRegOreModel.ProducedQty,
                ResQty = a3AppRegOreModel.ResQty,
                Storage = a3AppRegOreModel.Storage ?? string.Empty,
                Wc = a3AppRegOreModel.Wc ?? string.Empty,
                WorkingTime = a3AppRegOreModel.WorkingTime
            };
        }
    }
}