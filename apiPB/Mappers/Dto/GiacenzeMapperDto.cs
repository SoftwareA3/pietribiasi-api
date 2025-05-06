using apiPB.Dto.Models;
using apiPB.Models;

namespace apiPB.Mappers.Dto
{
    public static class GiacenzeMapperDto
    {
        // Dto Modello
        public static GiacenzeDto ToGiacenzeDto(this VwApiGiacenze giacenzeModel)
        {
            return new GiacenzeDto
            {
                Item = giacenzeModel.Item ?? string.Empty,
                Description = giacenzeModel.Description ?? string.Empty,
                BarCode = giacenzeModel.BarCode ?? string.Empty,
                FiscalYear = giacenzeModel.FiscalYear,
                Storage = giacenzeModel.Storage ?? string.Empty,
                BookInv = giacenzeModel.BookInv
            };
        }
    }
}