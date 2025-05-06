using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Dto.Models;

namespace apiPB.Mappers.Filters
{
    public class WorkerMapperFilters : Profile
    {
        public WorkerMapperFilters()
        {
            CreateMap<PasswordWorkersRequestDto, PasswordWorkersRequestFilter>();
            CreateMap<WorkerDto, PasswordWorkersRequestFilter>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
            CreateMap<WorkerDto, WorkerIdAndValueRequestFilter>()
                .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.WorkerId))
                .ForMember(dest => dest.FieldValue, opt => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            CreateMap<WorkersFieldRequestDto, WorkerIdAndValueRequestFilter>();
            CreateMap<PasswordWorkersRequestFilter, WorkerDto>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
            CreateMap<WorkerDto, WorkerIdAndValueRequestFilter>()
                .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.WorkerId))
                .ForMember(dest => dest.FieldValue, opt => opt.MapFrom(src => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            CreateMap<WorkerIdAndPasswordRequestDto, WorkerIdAndPasswordFilter>()
                .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.WorkerId))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
        }
    }
}