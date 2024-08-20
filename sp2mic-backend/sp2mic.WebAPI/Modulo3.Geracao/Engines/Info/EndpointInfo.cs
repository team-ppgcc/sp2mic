using DotLiquid;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;

public class EndpointInfo : ILiquidizable
{
  public string? NomeMetodo {get; init;}
  public string? NomeMetodoParaOrquestrador {get; init;}
  public string? NomeMetodoComInicialMinuscula {get; init;}
  public string? NomeMetodoComInicialMaiuscula {get; init;}
  public string? NomeMetodoTodoMinusculo {get; set;}
  public int IdMicrosservico {get; init;}
  public string? NomeMicrosservico {get; init;}
  public string? NomeMicrosservicoInicialMinuscula {get; init;}
  //public bool? IsRetornoVoid {get; set;}
  public bool? PossuiRetorno {get; init;}
  public bool? IsRetornoLista {get; init;}
  public bool? IsRetornoDTO {get; init;}
  public string Path {get; init;} = null!;
  public string ParametrosPath {get; init;} = null!;
  public string? RetornoMetodo {get; init;}
  public string? TipoNomeParametros {get; init;}
  public string? TipoQueryMethod {get; init;}
  public string? TipoHttpMapping {get; init;}
  public string? UrlParametros {get; init;}
  public string? NomeClasseRetorno {get; init;}
  public string? Texto {get; init;}
  public string? ParametrosChamada {get; init;}
  public string? ParametrosController {get; init;}
  public string? AtributosCast {get; init;}
  public HashSet<string>? ImportsController {get; init;}
  public HashSet<string>? ImportsRepository {get; init;}
  public MicrosservicoInfo? Microsservico {get; set;}
  public HashSet<VariavelInfo> Parametros {get; init;} = new();

  public object ToLiquid()
    => new
    {
      NomeMetodo,
      NomeMetodoParaOrquestrador,
      NomeMetodoComInicialMinuscula,
      NomeMetodoComInicialMaiuscula,
      NomeMetodoTodoMinusculo,
      NomeMicrosservico,
      NomeMicrosservicoInicialMinuscula,
      PossuiRetorno,
      IsRetornoLista,
      Path,
      ParametrosPath,
      RetornoMetodo,
      TipoNomeParametros,
      TipoQueryMethod,
      TipoHttpMapping,
      UrlParametros,
      NomeClasseRetorno,
      IsRetornoDTO,
      Texto,
      ParametrosChamada,
      AtributosCast,
      ParametrosController,
      ImportsController,
      ImportsRepository,
      Microsservico,
      Parametros
    };
}
