using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Models;

namespace apiPB.Mappers.Dto
{
    public static class InventarioMapperDto
    {
        public static InventarioDto ToInventarioDto(this A3AppInventario inventarioModel)
        {
            return new InventarioDto
            {
                InvId = inventarioModel.InvId,
                WorkerId = inventarioModel.WorkerId,
                SavedDate = inventarioModel.SavedDate,
                Item = inventarioModel.Item ?? string.Empty,
                Description = inventarioModel.Description ?? string.Empty,
                BarCode = inventarioModel.BarCode ?? string.Empty,
                FiscalYear = inventarioModel.FiscalYear,
                Storage = inventarioModel.Storage ?? string.Empty,
                BookInv = inventarioModel.BookInv,
                Imported = inventarioModel.Imported,
                UserImp = inventarioModel.UserImp ?? string.Empty,
                DataImp = inventarioModel.DataImp,
            };
        }
    }
}