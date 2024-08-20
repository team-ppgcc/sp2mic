using DotLiquid;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;

public class StoredProcedureInfo : ILiquidizable
{
  public int Id {get; init;}
  public string? NomeStoredProcedure {get; init;}
  public string? NomeClasseRetorno {get; init;}

  public string? RetornoMetodo {get; init;}

  //public string? TipoDadoRetorno {get; set;}
  //public bool? TemRetorno { get; set; }
  public bool? PossuiRetorno {get; init;}
  public string? UrlParametros {get; init;}
  public string? TipoNomeParametros {get; init;}
  public string? ParametrosPath {get; init;}
  public string? ParametrosController {get; init;}
  public string? ParametrosChamada {get; init;}
  public string? TxResultadoParser {get; init;}
  public string? ConteudoComandos {get; init;}

  //public HashSet<string>? EndpointsParaImpressao {get; set;}
  public HashSet<string>? ImportsController {get; init;}
  public HashSet<string>? ImportsService {get; init;}
  public HashSet<EndpointInfo>? Endpoints {get; init;}
  public HashSet<EndpointInfo>? EndpointsDosMicrosservicosProntosParaGerar {get; set;}
  public HashSet<ComandoInfo>? ComandoInfos {get; init;} = new();
  public HashSet<VariavelInfo>? VariavelInfos {get; init;} = new();
  public HashSet<ClasseInfo>? ClasseInfos {get; init;} = new();

  public object ToLiquid()
    => new
    {
      NomeStoredProcedure,
      NomeClasseRetorno,
      RetornoMetodo,
      PossuiRetorno,
      UrlParametros,
      TipoNomeParametros,
      ParametrosPath,
      ParametrosController,
      ParametrosChamada,
      TxResultadoParser,
      ConteudoComandos,
      Endpoints,
      EndpointsDosMicrosservicosProntosParaGerar,
      ImportsController,
      ImportsService,
      ComandoInfos,
      VariavelInfos,
      ClasseInfos
    };
}
