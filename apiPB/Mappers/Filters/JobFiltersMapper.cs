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
            CreateMap<MostepRequestDto, MostepRequestFilter>();
            CreateMap<MostepsMocomponentRequestDto, MostepsMocomponentRequestFilter>();
            CreateMap<MostepLavorazioniRequestDto, MostepLavorazioniRequestFilter>();
            CreateMap<MostepOdpRequestDto, MostepOdpRequestFilter>();
        }
    }
}