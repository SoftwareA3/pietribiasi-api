using AutoMapper;
using apiPB.Dto.Request;
using apiPB.Filters;
using apiPB.Models;

namespace apiPB.Mappers.Filters
{
    public class RegOreMapperFilters : Profile
    {
        public RegOreMapperFilters()
        {
            CreateMap<RegOreRequestDto, RegOreFilter>();
            CreateMap<RegOreFilter, A3AppRegOre>()
                .ForMember(dest => dest.RegOreId, opt => opt.Ignore())
                .ForMember(dest => dest.SavedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionQty, opt => opt.Ignore())
                .ForMember(dest => dest.ProducedQty, opt => opt.Ignore())
                .ForMember(dest => dest.ResQty, opt => opt.Ignore())
                .ForMember(dest => dest.WorkingTime, opt => opt.Ignore())
                .ForMember(dest => dest.Imported, opt => opt.Ignore())
                .ForMember(dest => dest.UserImp, opt => opt.Ignore())
                .ForMember(dest => dest.DataImp, opt => opt.Ignore())
                .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.WorkerId))
                .ForMember(dest => dest.Job, opt => opt.MapFrom(src => src.Job))
                .ForMember(dest => dest.RtgStep, opt => opt.MapFrom(src => src.RtgStep))
                .ForMember(dest => dest.Alternate, opt => opt.MapFrom(src => src.Alternate))
                .ForMember(dest => dest.AltRtgStep, opt => opt.MapFrom(src => src.AltRtgStep))
                .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation))
                .ForMember(dest => dest.OperDesc, opt => opt.MapFrom(src => src.OperDesc))
                .ForMember(dest => dest.Bom, opt => opt.MapFrom(src => src.Bom))
                .ForMember(dest => dest.Variant, opt => opt.MapFrom(src => src.Variant))
                .ForMember(dest => dest.ItemDesc, opt => opt.MapFrom(src => src.ItemDesc))
                .ForMember(dest => dest.Moid, opt => opt.MapFrom(src => src.Moid))
                .ForMember(dest => dest.Mono, opt => opt.MapFrom(src => src.Mono))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.Uom, opt => opt.MapFrom(src => src.Uom))
                .ForMember(dest => dest.Storage, opt => opt.MapFrom(src => src.Storage))
                .ForMember(dest => dest.Wc, opt => opt.MapFrom(src => src.Wc));
            CreateMap<ViewOreRequestDto, ViewOreRequestFilter>();
            CreateMap<ViewOrePutRequestDto, ViewOrePutFilter>();
            CreateMap<ViewOreDeleteRequestDto, ViewOreDeleteRequestFilter>();
        }
    }
}