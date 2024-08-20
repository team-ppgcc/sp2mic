using AutoMapper;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;

namespace sp2mic.WebAPI.Modulo2.Analise.MapperProfiles;

public class AtributoProfile : Profile
{
  public AtributoProfile()
  {
    CreateMap<Atributo, AtributoDto>()
     .ForMember(dest
          => dest.NoTipoDado,
        opt
          => opt.MapFrom(src => src.CoTipoDado.GetNome()))
     .ReverseMap()
     .ForMember(dest
          => dest.NoAtributo,
        opt =>
          opt.MapFrom(src => src.NoAtributo != null
            ? src.NoAtributo.TrimAndRemoveWhiteSpaces() : null));

    CreateMap<Atributo, AtributoUpdateDto>().ReverseMap()
     .ForMember(dest
          => dest.NoAtributo,
        opt =>
          opt.MapFrom(src => src.NoAtributo != null
            ? src.NoAtributo.TrimAndRemoveWhiteSpaces() : null));
  }
}
