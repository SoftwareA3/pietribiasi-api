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

        // Dto Richieste
    }
}