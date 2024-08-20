using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class AtributoEngine : IEngine<Atributo, AtributoInfo>
{
  private readonly IDtoClasseService _classeDTOService;

  public AtributoEngine(IDtoClasseService classeDTOService) => _classeDTOService = classeDTOService;

  public HashSet<AtributoInfo> ConvertDados(HashSet<Atributo>? atributos)
  {
    if (atributos is null)
    {
      throw new ArgumentNullException(nameof (atributos));
    }
    return atributos.Select(ConvertDados).ToHashSet();
  }

  public AtributoInfo ConvertDados(Atributo? a)
  {
    if (a is null)
    {
      throw new ArgumentNullException(nameof (a));
    }
    var info = new AtributoInfo
    {
      NomeAtributo = a.NoAtributo, TipoDadoNome = GetNomeTipo(a), Import = GetImport(a)
    };
    return info;
  }

  private static string GetImport(Atributo a)
  {
    var nomeImport = a.CoTipoDado.GetPacoteImport();
    return nomeImport is null or "" ? "" : nomeImport;
  }

  private string GetNomeTipo(Atributo a)
  {
    var classe = _classeDTOService.FindById(a.IdDtoClasse);
    if (classe is null)
    {
      throw new BadHttpRequestException("Class Not found");
    }
    return a.CoTipoDado == TipoDadoEnum.DTO ? classe.NoDtoClasse.InicialMaiuscula() :
      a.CoTipoDado.GetNome();
  }
}
