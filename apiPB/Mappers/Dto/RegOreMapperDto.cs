using apiPB.Models;
using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Mappers.Dto
{
    public static class RegOreMapperDto
    {
        // Dto Richieste
        public static RegOreDto ToA3AppRegOreDto(this A3AppRegOre a3AppRegOreModel)
        {
            return new RegOreDto
            {
                RegOreId = a3AppRegOreModel.RegOreId,
                WorkerId = a3AppRegOreModel.WorkerId,
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
                ProducedQty = a3AppRegOreModel.ProductionQty, // Impostato ProducedQty uguale a ProductionQty cosi al termine lavorazioni la da al 100%
                ResQty = a3AppRegOreModel.ResQty,
                Storage = a3AppRegOreModel.Storage ?? string.Empty,
                Wc = a3AppRegOreModel.Wc ?? string.Empty,
                WorkingTime = a3AppRegOreModel.WorkingTime,
                Imported = a3AppRegOreModel.Imported,
                UserImp = a3AppRegOreModel.UserImp ?? string.Empty,
                DataImp = a3AppRegOreModel.DataImp,
                Closed = a3AppRegOreModel.Closed
            };
        }

        // public static IEnumerable<RegOreDto> ToA3AppRegOreListDto(this IEnumerable<A3AppRegOre> a3AppRegOreModelList)
        // {
        //     foreach (var a3AppRegOreModel in a3AppRegOreModelList)
        //     {
        //         yield return a3AppRegOreModel.ToA3AppRegOreDto();
        //     }
        // }
    }
}