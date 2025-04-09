using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Dto.Models;

namespace apiPB.Mappers.Filter
{
    public class JobFiltersMapper : Profile
    {
        public JobFiltersMapper()
        {
            //Mappa automaticamente i Dto nei Filters
            CreateMap<MoRequestDto, MoRequestFilter>();
            CreateMap<MostepRequestDto, MostepRequestFilter>();
            CreateMap<MocomponentRequestDto, MocomponentRequestFilter>();
            CreateMap<MoStepsComponentRequestDto, MoStepsComponentRequestFilter>();
            CreateMap<MostepDto, MoStepsComponentRequestDto>();
        }
    }
}