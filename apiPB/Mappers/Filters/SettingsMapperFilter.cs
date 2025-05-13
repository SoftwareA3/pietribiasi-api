using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Models;
using apiPB.Dto.Models;

namespace apiPB.Mappers.Filters
{
    public class SettingsMapperFilter : Profile
    {
        public SettingsMapperFilter()
        {
            CreateMap<SettingsDto, SettingsFilter>();
                
        }
    }
}