using AutoMapper;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using static System.String;

namespace sp2mic.WebAPI.Modulo2.Analise.MapperProfiles;

public class StoredProcedureProfile : Profile
{
  public StoredProcedureProfile()
  {
    CreateMap<StoredProcedure, StoredProcedureViewDto>();

    CreateMap<StoredProcedure, StoredProcedureDto>()
     .ForMember(dest => dest.TabelasAssociadas, opt => opt.MapFrom(src => ConverterListaEmString(src)))
     .ReverseMap();

    CreateMap<StoredProcedure, StoredProcedureListagemDto>()
     .ForMember(dest => dest.NoTipoDadoRetorno,
        opt => opt.MapFrom(src => GetNomeTipoDadoRetorno(src)))
     .ForMember(dest => dest.QtdEndpoints, opt => opt.MapFrom(src => src.Endpoints.Count))
     .ForMember(dest => dest.NoSucessoParser,
        opt => opt.MapFrom(src => src.SnSucessoParser ? "Success" : "Unhandled"))
     .ForMember(dest => dest.TabelasAssociadas,
        opt => opt.MapFrom(src => ConverterListaEmString(src)))
     .ForMember(dest => dest.TabelasAssociadas, opt => opt.MapFrom(src => ConverterListaEmString(src)))
     .ReverseMap();

    CreateMap<StoredProcedure, StoredProcedureUpdateDto>().ReverseMap();
  }

  private static string ConverterListaEmString(StoredProcedure src)
  {
    var tabelasAssociadas = src.TabelasAssociadas;
    if (tabelasAssociadas == null || tabelasAssociadas.Count == 0)
    {
      return "";
    }
    var nomesTabelasAssociadas = tabelasAssociadas.Select(t => t.NoTabela).Aggregate(((a, b) => a + ", " + b));
    return nomesTabelasAssociadas;
  }

  private static string GetNomeTipoDadoRetorno(StoredProcedure src)
  {
    if (src.CoTipoDadoRetorno == TipoDadoEnum.TIPO_NAO_MAPEADO)
    {
      return "";
    }
    var nomeRetorno = RecuperarNomeRetorno(src.CoTipoDadoRetorno, src.IdDtoClasseNavigation);
    return src.SnRetornoLista == true ? $"List<{nomeRetorno}>" : nomeRetorno;
  }

  private static string RecuperarNomeRetorno(TipoDadoEnum? coTipoDadoRetorno,
    DtoClasse? classeDTONavigation)
  {
    return coTipoDadoRetorno switch
    {
      null => "",
      TipoDadoEnum.DTO => IsNullOrEmpty(classeDTONavigation?.NoDtoClasse) ?
        coTipoDadoRetorno.GetDescricao() : classeDTONavigation.NoDtoClasse,
      _ => coTipoDadoRetorno.GetNome()
    };
  }
}
