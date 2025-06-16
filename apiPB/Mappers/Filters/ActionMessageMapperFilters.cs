using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Models;

namespace apiPB.Mappers.Filters
{
    public class ActionMessageMapperFilters : Profile
    {
        public ActionMessageMapperFilters()
        {
            CreateMap<ImportedLogMessageDto, ImportedLogMessageFilter>();
        }
    }
}