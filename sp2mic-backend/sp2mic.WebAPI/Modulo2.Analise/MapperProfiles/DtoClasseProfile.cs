using System.Text;
using AutoMapper;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;

namespace sp2mic.WebAPI.Modulo2.Analise.MapperProfiles;

public class DtoClasseProfile : Profile
{
  public DtoClasseProfile()
  {
    CreateMap<DtoClasse, DtoClasseDto>()
     .ForMember(dest
          => dest.TxDtoClasse,
        opt
          => opt.MapFrom(src => MontarDtoClasse(src)))
     // .ForMember(dest
     //      => dest.NoMicrosservico,
     //    opt
     //      => opt.MapFrom(src => src.IdMicrosservicoNavigation != null
     //        ? src.IdMicrosservicoNavigation.NoMicrosservico : ""))
     .ForMember(dest
          => dest.NoStoredProcedure,
        opt
          => opt.MapFrom(src => src.IdStoredProcedureNavigation.NoStoredProcedure))
     .ReverseMap()
     .ForMember(dest
          => dest.NoDtoClasse,
        opt =>
          opt.MapFrom(src => src.NoDtoClasse != null
            ? src.NoDtoClasse.TrimAndRemoveWhiteSpaces().InicialMaiuscula() : null));

    CreateMap<DtoClasse, DtoClasseUpdateDto>()
     .ReverseMap()
     .ForMember(dest
          => dest.NoDtoClasse,
        opt =>
          opt.MapFrom(src => src.NoDtoClasse != null
            ? src.NoDtoClasse.TrimAndRemoveWhiteSpaces().InicialMaiuscula() : null));
  }

  private static string MontarDtoClasse(DtoClasse classe)
  {
    var texto = new StringBuilder("");
    texto.Append($"public class {classe.NoDtoClasse.InicialMaiuscula()}");
    texto.Append(" {\n");
    foreach (var atributo in classe.Atributos)
    {
      texto.Append($"    private {atributo.CoTipoDado.GetNome()} {atributo.NoAtributo};\n");
    }
    texto.Append('}');
    return texto.ToString();
  }
}
