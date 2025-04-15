using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Dto.Models;
using apiPB.Models;

namespace apiPB.Mappers.Filter
{
    public class JobMapperFilters : Profile
    {
        public JobMapperFilters()
        {
            //Mappa automaticamente i Dto nei Filters
        }
    }
}