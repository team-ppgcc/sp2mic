using AutoMapper;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;

namespace sp2mic.WebAPI.Modulo2.Analise.MapperProfiles;

public class VariavelProfile : Profile
{
  public VariavelProfile()
  {
    CreateMap<Variavel, VariavelDto>().ReverseMap();
    CreateMap<Variavel, VariavelAddUpdateDto>().ReverseMap();
  }
}
