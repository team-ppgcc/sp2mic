using DotLiquid;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;

public class MicrosservicoInfo : ILiquidizable
{
  public int Id {get; init;}
  public string NomeMicrosservico {get; init;} = null!;
  public string? NomeTodoMinusculo {get; init;}
  public string NomeComInicialMaiuscula {get; init;} = null!;
  public string? NomeComInicialMinuscula {get; init;}
  public string? PathMicrosservico {get; init;}
  public HashSet<ClasseInfo>? Classes {get; set;} = new();
  public HashSet<EndpointInfo> Endpoints {get; set;} = new();

  public object ToLiquid()
    => new
    {
      Id,
      NomeMicrosservico,
      NomeTodoMinusculo,
      NomeComInicialMaiuscula,
      NomeComInicialMinuscula,
      PathMicrosservico,
      Classes,
      Endpoints
    };
}
