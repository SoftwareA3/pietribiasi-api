using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Models;

namespace apiPB.Mappers.Filters
{
    public class WorkerMapperFilters : Profile
    {
        public WorkerMapperFilters()
        {
            CreateMap<WorkersFieldRequestDto, WorkersFieldFilter>();
            CreateMap<WorkersRequestDto, WorkersFilter>();
        }
    }
}