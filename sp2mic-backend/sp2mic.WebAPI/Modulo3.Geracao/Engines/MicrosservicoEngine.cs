using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class MicrosservicoEngine : IEngine<Microsservico, MicrosservicoInfo>
{
  private readonly IEngine<DtoClasse, ClasseInfo> _classeEngine;
  private readonly IEngine<Endpoint, EndpointInfo> _endPointEngine;
  private readonly IDtoClasseService _dtoClasseService;

  public MicrosservicoEngine(IEngine<DtoClasse, ClasseInfo> classeEngine,
    IEngine<Endpoint, EndpointInfo> endPointEngine, IDtoClasseService dtoClasseService)
  {
    _classeEngine = classeEngine;
    _endPointEngine = endPointEngine;
    _dtoClasseService = dtoClasseService;
  }

  public HashSet<MicrosservicoInfo> ConvertDados(HashSet<Microsservico>? microsservicos)
  {
    if (microsservicos is null)
    {
      throw new ArgumentNullException(nameof (microsservicos));
    }
    return microsservicos.Select(ConvertDados).ToHashSet();
  }

  public MicrosservicoInfo ConvertDados(Microsservico? m)
  {
    if (m is null)
    {
      throw new ArgumentNullException(nameof (m));
    }
    var micInfo = new MicrosservicoInfo
    {
      Id = m.Id,
      NomeMicrosservico = m.NoMicrosservico,
      NomeTodoMinusculo = m.NoMicrosservico.TodoMinusculo(),
      NomeComInicialMaiuscula = m.NoMicrosservico.InicialMaiuscula(),
      NomeComInicialMinuscula = m.NoMicrosservico.InicialMinuscula(),
      PathMicrosservico = m.NoMicrosservico.ToLower()
    };
    var classesDoMicrosservico = _dtoClasseService.RecuperarClassesDeUmMicrosservico(m.Id);
    micInfo.Classes = _classeEngine.ConvertDados(classesDoMicrosservico.ToHashSet());

    var info2 = new MicrosservicoInfo
    {
      Id = m.Id,
      NomeMicrosservico = m.NoMicrosservico,
      NomeTodoMinusculo = m.NoMicrosservico.TodoMinusculo(),
      NomeComInicialMaiuscula = m.NoMicrosservico.InicialMaiuscula(),
      NomeComInicialMinuscula = m.NoMicrosservico.InicialMinuscula(),
      PathMicrosservico = m.NoMicrosservico.ToLower(),
      //Classes = _classeEngine.ConvertDados(m.DtoClasses.ToHashSet()) microsservico nao tem classes
    };
    var classesDoMicrosservicoDoInfo2 = _dtoClasseService.RecuperarClassesDeUmMicrosservico(m.Id);
    info2.Classes = _classeEngine.ConvertDados(classesDoMicrosservicoDoInfo2.ToHashSet());

    //info.WithEndpoints(_endPointEngine.ConvertDados(m.Endpoints));
    var endPointInfos = new HashSet<EndpointInfo>();

    foreach (var ep in m.Endpoints.Where(ep => ep.SnAnalisado))
    {
      var epInfo = _endPointEngine.ConvertDados(ep);
      epInfo.Microsservico = info2;
      endPointInfos.Add(epInfo);
    }

    micInfo.Endpoints = endPointInfos;
    return micInfo;
  }

  public MicrosservicoInfo ConvertDadosSemEp(Microsservico m)
  {
    var info = new MicrosservicoInfo
    {
      Id = m.Id,
      NomeMicrosservico = m.NoMicrosservico,
      NomeComInicialMaiuscula = m.NoMicrosservico.InicialMaiuscula(),
      NomeComInicialMinuscula = m.NoMicrosservico.InicialMinuscula(),
      PathMicrosservico = m.NoMicrosservico.ToLower(),
      //Classes = _classeEngine.ConvertDados(m.DtoClasses.ToHashSet())
    };
    var classesDoMicrosservico = _dtoClasseService.RecuperarClassesDeUmMicrosservico(m.Id);
    info.Classes = _classeEngine.ConvertDados(classesDoMicrosservico.ToHashSet());

    return info;
  }
}
