using DotLiquid;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;

public class ClasseInfo : ILiquidizable
{
  public string NomeClasse {get; init;} = null!;

  //public int IdMicrosservico {get; init;} não tem mais mic na classe

  public string? NomeComInicialMaiuscula {get; init;}

  public HashSet<AtributoInfo>? Atributos {get; init;} = new();

  public object ToLiquid() => new {NomeClasse, NomeComInicialMaiuscula, Atributos};
}
