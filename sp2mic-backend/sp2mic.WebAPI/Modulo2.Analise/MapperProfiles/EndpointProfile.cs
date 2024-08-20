using AutoMapper;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using static System.String;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo2.Analise.MapperProfiles;

public class EndpointProfile : Profile
{
  public EndpointProfile()
  {
    CreateMap<EndpointUpdateDto, Endpoint>();

    CreateMap<Endpoint, EndpointDto>()
     .ForMember(dest => dest.NoMicrosservico, opt => opt.MapFrom(src => GetNomeMicrosservico(src)))
     .ForMember(dest => dest.NoMetodoEndpoint, opt => opt.MapFrom(src => GetNomeMetodo(src)))
     .ForMember(dest => dest.NoPath, opt => opt.MapFrom(src => GetNomePath(src)))
     .ForMember(dest => dest.NoStoredProcedure,
        opt => opt.MapFrom(src => src.IdStoredProcedureNavigation.NoStoredProcedure))
     .ForMember(dest => dest.NoVariavelRetornda,
        opt => opt.MapFrom(src => GetNomeVariavelRetornada(src)))
     .ForMember(dest => dest.TabelasAssociadas, opt => opt.MapFrom(src => ConverterListaEmString(src)))
     .ReverseMap();

    CreateMap<Endpoint, EndpointListagemDto>()
     .ForMember(dest => dest.NoMetodoEndpoint, opt => opt.MapFrom(src => GetNomeMetodo(src)))
     .ForMember(dest => dest.NoPath, opt => opt.MapFrom(src => GetNomePath(src)))
     .ForMember(dest => dest.NoMicrosservico, opt => opt.MapFrom(src => GetNomeMicrosservico(src)))
     .ForMember(dest => dest.NoTipoSqlDml, opt => opt.MapFrom(src => src.CoTipoSqlDml.GetNome()))
     .ForMember(dest => dest.NoTipoDadoRetorno,
        opt => opt.MapFrom(src => GetNomeTipoDadoRetornoEp(src)))
     .ForMember(dest => dest.NoStoredProcedure,
        opt => opt.MapFrom(src => GetNomeStoredProcedure(src)))
     .ForMember(dest => dest.TabelasAssociadas, opt => opt.MapFrom(src => ConverterListaEmString(src)));
  }

  private static string ConverterListaEmString(Endpoint src)
  {
    var tabelasAssociadas = src.TabelasAssociadas;
    if (tabelasAssociadas == null || tabelasAssociadas.Count == 0)
    {
      return "";
    }
    var nomesTabelasAssociadas = tabelasAssociadas.Select(t => t.NoTabela).Aggregate((a, b) => a + ", " + b);
    return nomesTabelasAssociadas;
  }

  private static string? GetNomeStoredProcedure(Endpoint src)
    => src.IdStoredProcedureNavigation?.NoStoredProcedure;

  private static string? GetNomeMicrosservico(Endpoint src)
    => src.IdMicrosservico is null ? "" : src.IdMicrosservicoNavigation?.NoMicrosservico;

  private static string GetNomeMetodo(Endpoint src)
    => src.NoMetodoEndpoint is null or "nomeAindaNaoDefinido" ? "" : src.NoMetodoEndpoint;

  private static string GetNomePath(Endpoint src)
    => src.NoPath is null or "/path-ainda-nao-definido" ? "" : src.NoPath;

  private static string? GetNomeVariavelRetornada(Endpoint src)
    => src.IdVariavelRetornada is null ? "" : src.IdVariavelRetornadaNavigation?.NoVariavel;

  private static string GetNomeTipoDadoRetornoEp(Endpoint src)
  {
    if (src.CoTipoDadoRetorno == TipoDadoEnum.TIPO_NAO_MAPEADO)
    {
      return "";
    }
    var nomeRetorno = RecuperarNomeRetorno(src.CoTipoDadoRetorno, src.IdDtoClasseNavigation);
    return src.SnRetornoLista == true ? $"List<{nomeRetorno}>" : nomeRetorno;
  }

  private static string
    RecuperarNomeRetorno(TipoDadoEnum? coTipoDadoRetorno, DtoClasse? classeDTONavigation)
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
