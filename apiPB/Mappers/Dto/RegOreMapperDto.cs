using apiPB.Models;
using apiPB.Dto.Request;

namespace apiPB.Mappers.Dto
{
    public static class RegOreMapperDto
    {
        // Dto Richieste
        public static RegOreRequestDto ToA3AppRegOreRequestDto(this A3AppRegOre a3AppRegOreModel)
        {
            return new RegOreRequestDto
            {
                WorkerId = a3AppRegOreModel.WorkerId,
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