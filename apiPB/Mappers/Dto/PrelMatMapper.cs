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
                Job = request.Job,
                RtgStep = request.RtgStep,
                Alternate = request.Alternate,
                AltRtgStep = request.AltRtgStep,
                Operation = request.Operation,
                OperDesc = request.OperDesc,
                Position = request.Position,
                Component = request.Component,
                Bom = request.Bom,
                Variant = request.Variant,
                ItemDesc = request.ItemDesc,
                Moid = request.Moid,
                Mono = request.Mono,
                CreationDate = request.CreationDate,
                UoM = request.UoM,
                ProductionQty = request.ProductionQty,
                ProducedQty = request.ProducedQty,
                ResQty = request.ResQty,
                Storage = request.Storage,
                BarCode = request.BarCode,
                Wc = request.Wc,
                PrelQty = request.PrelQty,
                Imported = request.Imported,
                UserImp = request.UserImp,
                DataImp = request.DataImp
            };
        }
    }
}