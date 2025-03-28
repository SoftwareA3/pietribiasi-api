using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Mappers.Filter
{
    public class JobFiltersMapper : Profile
    {
        public JobFiltersMapper()
        {
            //Mappa automaticamente i Dto nei Filters
            CreateMap<VwApiMoRequestDto, VwApiMoRequestFilter>();
            CreateMap<VwApiMostepRequestDto, VwApiMostepRequestFilter>();
            CreateMap<VwApiMocomponentRequestDto, VwApiMocomponentRequestFilter>();
            CreateMap<VwApiMoStepsComponentRequestDto, VwApiMoStepsComponentRequestFilter>();
        }
    }
}