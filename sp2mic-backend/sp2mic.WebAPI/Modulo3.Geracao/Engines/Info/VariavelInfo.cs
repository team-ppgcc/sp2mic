using DotLiquid;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;

public class VariavelInfo : ILiquidizable
{
  public string? NomeVariavel {get; init;}
  public string? NomeVariavelUpperCase {get; init;}
  public string? NomeComInicialMinuscula {get; init;}
  public string? NomeTipoDado {get; init;}
  public string? TipoEscopo {get; init;}
  public int? Tamanho {get; init;}

  public object ToLiquid()
    => new
    {
      NomeVariavel,
      NomeVariavelUpperCase,
      NomeComInicialMinuscula,
      NomeTipoDado,
      TipoEscopo,
      Tamanho
    };
}
