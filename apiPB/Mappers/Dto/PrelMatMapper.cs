using apiPB.Dto.Models;
using apiPB.Dto.Request;
using apiPB.Models;

namespace apiPB.Mappers.Dto
{
    public static class PrelMatMapper
    {
        public static PrelMatDto ToDtoPrelMatDto(this A3AppPrelMat request)
        {
            return new PrelMatDto
            {
                PrelMatId = request.PrelMatId,
                WorkerId = request.WorkerId,
                SavedDate = request.SavedDate,
                Job = request.Job ?? string.Empty,
                RtgStep = request.RtgStep,
                Alternate = request.Alternate ?? string.Empty,
                AltRtgStep = request.AltRtgStep,
                Operation = request.Operation ?? string.Empty,
                OperDesc = request.OperDesc ?? string.Empty,
                Position = request.Position,
                Component = request.Component ?? string.Empty,
                Bom = request.Bom ?? string.Empty,
                Variant = request.Variant ?? string.Empty,
                ItemDesc = request.ItemDesc ?? string.Empty,
                Moid = request.Moid,
                Mono = request.Mono ?? string.Empty,
                CreationDate = request.CreationDate,
                UoM = request.UoM ?? string.Empty,
                ProductionQty = request.ProductionQty,
                ProducedQty = request.ProducedQty,
                ResQty = request.ResQty,
                Storage = request.Storage ?? string.Empty,
                BarCode = request.BarCode ?? string.Empty,
                Wc = request.Wc ?? string.Empty,
                PrelQty = request.PrelQty,
                Imported = request.Imported,
                UserImp = request.UserImp ?? string.Empty,
                DataImp = request.DataImp
            };
        }
    }
}