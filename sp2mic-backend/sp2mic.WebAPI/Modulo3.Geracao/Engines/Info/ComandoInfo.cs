using DotLiquid;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;

public class ComandoInfo : ILiquidizable
{
  public string? ConteudoComando {get; set;}
  public bool IsComandoOrigem {get; set;}
  public string? NomeVariavelRetornada {get; set;}

  public object ToLiquid() => new {ConteudoComando,IsComandoOrigem, NomeVariavelRetornada};
}
