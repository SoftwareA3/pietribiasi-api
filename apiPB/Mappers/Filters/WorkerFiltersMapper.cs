using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Models;

namespace apiPB.Mappers.Filters
{
    public class WorkerFiltersMapper : Profile
    {
        public WorkerFiltersMapper()
        {
            CreateMap<PasswordWorkersRequestDto, PasswordWorkersRequestFilter>();
            CreateMap<WorkerDto, PasswordWorkersRequestFilter>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
            CreateMap<WorkerDto, WorkerIdRequestFilter>()
                .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.WorkerId))
                .ForMember(dest => dest.FieldValue, opt => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            CreateMap<WorkersFieldRequestDto, WorkerIdRequestFilter>();
            CreateMap<PasswordWorkersRequestFilter, WorkerDto>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
            CreateMap<WorkerDto, WorkerIdRequestFilter>()
                .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.WorkerId))
                .ForMember(dest => dest.FieldValue, opt => opt.MapFrom(src => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}