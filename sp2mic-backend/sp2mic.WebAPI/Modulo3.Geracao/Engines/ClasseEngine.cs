using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class ClasseEngine : IEngine<DtoClasse, ClasseInfo>
{
  private readonly IEngine<Atributo, AtributoInfo> _atributoEngine;

  public ClasseEngine(IEngine<Atributo, AtributoInfo> atributoEngine)
    => _atributoEngine = atributoEngine;

  public HashSet<ClasseInfo> ConvertDados(HashSet<DtoClasse>? classes)
  {
    if (classes is null)
    {
      throw new ArgumentNullException(nameof (classes));
    }
    return classes.Select(ConvertDados).ToHashSet();
  }

  public ClasseInfo ConvertDados(DtoClasse? c)
  {
    if (c is null)
    {
      throw new ArgumentNullException(nameof (c));
    }
    var info = new ClasseInfo
    {
      NomeClasse = c.NoDtoClasse,
      //IdMicrosservico = c.IdMicrosservico ?? 0, não tem mais mic na classe
      NomeComInicialMaiuscula = c.NoDtoClasse.InicialMaiuscula(),
      Atributos = _atributoEngine.ConvertDados(c.Atributos.ToHashSet())
    };
    return info;
  }
}
