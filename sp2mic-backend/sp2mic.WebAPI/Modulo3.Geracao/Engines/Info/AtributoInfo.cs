using DotLiquid;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;

public class AtributoInfo : ILiquidizable
{
  public string? TipoDadoNome {get; init;}

  public string? NomeAtributo {get; init;}

  public string? Import {get; init;}

  public object ToLiquid() => new {TipoDadoNome, NomeAtributo, Import};
}
