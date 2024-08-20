using AutoMapper;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;

namespace sp2mic.WebAPI.Modulo2.Analise.MapperProfiles;

public class MicrosservicoProfile : Profile
{
  public MicrosservicoProfile()
  {
    CreateMap<Microsservico, MicrosservicoDto>()
     .ForMember(dest
          => dest.QtdEndpoints,
        opt
          => opt.MapFrom(src => src.Endpoints.Count))
     .ReverseMap();
    CreateMap<Microsservico, MicrosservicoAddUpdateDto>().ReverseMap();
  }
}
