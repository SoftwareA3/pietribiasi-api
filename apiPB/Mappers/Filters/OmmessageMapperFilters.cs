using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;

namespace apiPB.Mappers.Filters 
{
    public class OmmessageMapperFilters : Profile
    {
        public OmmessageMapperFilters()
        { 
            CreateMap<MoIdRequestDto, MoIdFilter>();
        }
    }
}