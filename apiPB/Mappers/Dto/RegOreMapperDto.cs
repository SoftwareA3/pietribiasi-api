using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Mappers.Dto
{
    public static class RegOreMapperDto
    {
        // Da Models a Dto
        public static RegOreDto ToA3AppRegOreDto(this A3AppRegOre a3AppRegOreModel)
        {
            return new RegOreDto
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


        // Da Dto a Request
        public static RegOreRequestDto ToA3AppRegOreRequestDto(this A3AppRegOre a3AppRegOreModel)
        {
            return new RegOreRequestDto
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

        // Da Request a Dto
        public static RegOreDto ToA3AppRegOreDtoFromRequest(this RegOreRequestDto a3AppRegOreRequestDto)
        {
            return new RegOreDto
            {
                WorkerId = a3AppRegOreRequestDto.WorkerId ?? string.Empty,
                Job = a3AppRegOreRequestDto.Job ?? string.Empty,
                RtgStep = a3AppRegOreRequestDto.RtgStep ?? 0,
                Alternate = a3AppRegOreRequestDto.Alternate ?? string.Empty,
                AltRtgStep = a3AppRegOreRequestDto.AltRtgStep ?? 0,
                Operation = a3AppRegOreRequestDto.Operation ?? string.Empty,
                OperDesc = a3AppRegOreRequestDto.OperDesc ?? string.Empty,
                Bom = a3AppRegOreRequestDto.Bom ?? string.Empty,
                Variant = a3AppRegOreRequestDto.Variant ?? string.Empty,
                ItemDesc = a3AppRegOreRequestDto.ItemDesc ?? string.Empty,
                Moid = a3AppRegOreRequestDto.Moid ?? 0,
                Uom = a3AppRegOreRequestDto.Uom ?? string.Empty,
                ProductionQty = a3AppRegOreRequestDto.ProductionQty ?? 0,
                ProducedQty = a3AppRegOreRequestDto.ProducedQty ?? 0,
                ResQty = a3AppRegOreRequestDto.ResQty ?? 0,
                Storage = a3AppRegOreRequestDto.Storage ?? string.Empty,
                Wc = a3AppRegOreRequestDto.Wc ?? string.Empty,
                WorkingTime = a3AppRegOreRequestDto.WorkingTime ?? 0,
                Imported = a3AppRegOreRequestDto.Imported ?? false,
                UserImp = a3AppRegOreRequestDto.UserImp ?? string.Empty,
                DataImp = a3AppRegOreRequestDto.DataImp ?? DateTime.Now
            };
        }
    }
}