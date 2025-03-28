using apiPB.Dto.Request;
using apiPB.Dto.Models;

namespace apiPB.Services.Request.Abstraction
{
    public interface IJobRequestService
    {
        IEnumerable<JobDto> GetJobs();
        IEnumerable<MocomponentDto> GetMocomponent(MocomponentRequestDto request);
        IEnumerable<MoDto> GetMo(MoRequestDto request);
        IEnumerable<MostepDto> GetMostepByMoId(MostepRequestDto request);
        IEnumerable<MoStepsComponentDto> GetMoStepsComponentByMoId(MoStepsComponentRequestDto request);
    }
}