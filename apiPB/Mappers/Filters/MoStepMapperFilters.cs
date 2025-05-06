using AutoMapper;
using apiPB.Filters;
using apiPB.Dto.Request;

namespace apiPB.Mappers.Filters
{
    public class MoStepMapperFilters : Profile
    {
        public MoStepMapperFilters() 
        {
            CreateMap<JobRequestDto, JobFilter>();
            CreateMap<OperationRequestDto, OperationFilter>();
            CreateMap<MonoRequestDto, MonoFilter>();
        }
    }
}