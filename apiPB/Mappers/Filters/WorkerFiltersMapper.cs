using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Mappers.Filters
{
    public class WorkerFiltersMapper : Profile
    {
        public WorkerFiltersMapper()
        {
            CreateMap<PasswordWorkersRequestDto, PasswordWorkersRequestFilter>();
        }
    }
}