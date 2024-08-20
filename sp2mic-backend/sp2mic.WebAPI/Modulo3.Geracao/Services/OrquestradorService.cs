using System.Drawing.Drawing2D;
using DotLiquid;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Dtos;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Services.Interfaces;
using static sp2mic.WebAPI.CrossCutting.Util.FileUtil;
using static System.String;

namespace sp2mic.WebAPI.Modulo3.Geracao.Services;

public class OrquestradorService : IGeracaoService<GeracaoDto>
{
  private readonly IStoredProcedureService _storedProcedureService;
  private readonly IEngine<StoredProcedure, StoredProcedureInfo> _storedProcedureEngine;
  private readonly IDtoClasseService _dtoClasseService;
  private readonly ILogger<OrquestradorService> _logger;

  public OrquestradorService(IStoredProcedureService storedProcedureService,
    IEngine<StoredProcedure, StoredProcedureInfo> storedProcedureEngine,
    IDtoClasseService dtoClasseService, ILogger<OrquestradorService> logger)
  {
    _storedProcedureService = storedProcedureService ??
      throw new ArgumentNullException(nameof (storedProcedureService));
    _storedProcedureEngine = storedProcedureEngine;
    _dtoClasseService = dtoClasseService;
    _logger = logger ?? throw new ArgumentNullException(nameof (logger));
  }

  public void GerarProjeto(GeracaoDto dto, HashSet<MicrosservicoInfo> msInfo)
  {
    _logger.LogInformation("GerarProjeto \n OrquestradorDto: {OrquestradorDto}", dto.ToString());
    var spsAnalisadas = _storedProcedureService.RecuperarAnalisadas().ToHashSet();
    if (spsAnalisadas.Count == 0)
    {
      throw new BadHttpRequestException("No analyzed Stored Procedure was found.");
    }
    OrquestradorConstants.ValorDefault(dto);
    var idsMicrosservicosProntosParaGerar = msInfo.Select(m => m.Id).ToHashSet();
    var spsAnalisadasInfo = _storedProcedureEngine.ConvertDados(spsAnalisadas).ToHashSet();
    spsAnalisadasInfo = RemoverSpsSemEps(spsAnalisadasInfo);
    dto.ProjectMetadataPackageName = Concat(dto.ProjectMetadataGroupId, ".orchestrator");
    var diretorioOrquestrador = CriarDiretorioProjeto(Path.Combine("Resources", "generated-microservices"), "orchestrator");

    var diretorioSrcMainJavaPacote
      = CriarDiretorioSrcMainJavaPacote(diretorioOrquestrador,
        dto.ProjectMetadataPackageName);
    var diretorioSrcTestJavaPacote
      = CriarDiretorioSrcTestJavaPacote(diretorioOrquestrador,
        dto.ProjectMetadataPackageName);
    CriarArquivoPom(dto, diretorioOrquestrador);
    CriarArquivoPropriedades(CriarDiretorioResources(diretorioOrquestrador), dto);
    CriarClasseApplicationTests(dto.ProjectMetadataPackageName, diretorioSrcTestJavaPacote);
    CriarClasseApplication(dto.ProjectMetadataPackageName, diretorioSrcMainJavaPacote);
    CriarClasseRestClientServices(dto.ProjectMetadataPackageName, msInfo, spsAnalisadasInfo,
      CriarDiretorioRestClientServices(diretorioSrcMainJavaPacote));
    CriarClasseController(dto.ProjectMetadataPackageName, spsAnalisadasInfo, msInfo,
      idsMicrosservicosProntosParaGerar, CriarDiretorioController(diretorioSrcMainJavaPacote));
    CriarClassesDto(dto.ProjectMetadataPackageName, msInfo,
      CriarDiretorioDto(diretorioSrcMainJavaPacote));
  }

  private static HashSet<StoredProcedureInfo> RemoverSpsSemEps(HashSet<StoredProcedureInfo> spsAnalisadas)
  {
    var spsAnalisadasSemAsVazias = new HashSet<StoredProcedureInfo>();
    foreach (var sp in spsAnalisadas.Where(sp => sp.EndpointsDosMicrosservicosProntosParaGerar.Any()))
    {
      spsAnalisadasSemAsVazias.Add(sp);
    }
    return spsAnalisadasSemAsVazias;
  }

  /* ******************************************************************************************** */
  private static void CriarArquivoPom(GeracaoDto dto, string diretorioOrquestrador)
  {
    // _logger.LogInformation("OrquestradorService -> CriarArquivoPom \n {DiretorioOrquestrador}",
    //   diretorioOrquestrador);
    var map = new Hash
    {
      {"projectMetadataGroupId", dto.ProjectMetadataGroupId},
      {"projectMetadataJavaVersion", dto.ProjectMetadataJavaVersion},
      {"springBootVersion", dto.SpringBootVersion}
    };
    var template = Template.Parse(File.ReadAllText(OrquestradorConstants.TEMPLATE_PATH_FILE_POM));
    var content = template.Render(map);
    SalvarArquivo(diretorioOrquestrador, "pom.xml", content);
  }

  /* ******************************************************************************************** */
  private void CriarArquivoPropriedades(string diretorioResources, GeracaoDto dto)
  {
    // _logger.LogInformation(
    //   "OrquestradorService -> CriarArquivoPropriedades -> \n {DiretorioResources} \n {OrquestradorDto}",
    //   diretorioResources, dto);

    var map = new Hash
    {
      {"orchestratorPort", dto.OrchestratorPort},
      {"consulHost", dto.ConsulHost},
      {"consulPort", dto.ConsulPort}
    };

    var template
      = Template.Parse(
        File.ReadAllText(OrquestradorConstants.TEMPLATE_PATH_FILE_ARQUIVO_PROPRIEDADES));

    var content = template.Render(map);
    SalvarArquivo(diretorioResources, "application.properties", content);
  }

  private void CriarClasseApplication(string nomePacote, string diretorioPacote)
  {
    // _logger.LogInformation(
    //   "OrquestradorService -> CriarClasseApplication {NomePacote} \n {DiretorioPacote}", nomePacote,
    //   diretorioPacote);

    var map = new Hash {{"nomePacote", nomePacote}};
    var template
      = Template.Parse(File.ReadAllText(OrquestradorConstants.TEMPLATE_PATH_FILE_APPLICATION));

    var content = template.Render(map);
    SalvarArquivo(diretorioPacote, "OrchestratorApplication.java", content);
  }

  private void CriarClasseApplicationTests(string nomePacote, string diretorioPacoteTest)
  {
    // _logger.LogInformation(
    //   "OrquestradorService -> CriarClasseApplicationTests \n {NomePacote} \n {DiretorioPacote}",
    //   nomePacote, diretorioPacoteTest);

    var map = new Hash {{"nomePacote", nomePacote}};
    var template
      = Template.Parse(File.ReadAllText(OrquestradorConstants.TEMPLATE_PATH_FILE_APPLICATION_TEST));

    var content = template.Render(map);
    SalvarArquivo(diretorioPacoteTest, "OrchestratorApplicationTests.java", content);
  }

  /* ******************************************************************************************** */
  private void CriarClassesDto(string nomePacote, IEnumerable<MicrosservicoInfo> mInfo,
    string diretorioDto)
  {
    // _logger.LogInformation(
    //   "OrquestradorService -> CriarClassesDto -> \n {DiretorioDto} \n {NomePacote}", diretorioDto,
    //   nomePacote);

    foreach (var m in mInfo)
    {
      foreach (var c in m.Classes!)
      {
        var imports = GetImportsDasDtoClasses(c.Atributos!);
        var map = new Hash
        {
          {"nomePacote", nomePacote},
          {"imports", imports},
          {"nomeDtoClasse", c.NomeComInicialMaiuscula},
          {"atributos", c.Atributos},
        };
        var template
          = Template.Parse(File.ReadAllText(OrquestradorConstants.TEMPLATE_PATH_FILE_DTO));
        var content = template.Render(map);
        SalvarArquivo(diretorioDto, Concat(c.NomeComInicialMaiuscula, ".java"), content);
      }
    }
  }

  private static HashSet<string> GetImportsDasDtoClasses(HashSet<AtributoInfo> atributoInfos)
  {
    var imports = new HashSet<string>();
    if (!atributoInfos.Any())
    {
      return imports;
    }
    foreach (var a in atributoInfos.Where(a => !IsNullOrEmpty(a.Import)))
    {
      imports.Add(a.Import!);
    }
    return imports;
  }

  /* ******************************************************************************************** */
  private void CriarClasseRestClientServices(string nomePacote, HashSet<MicrosservicoInfo> msInfo,
    HashSet<StoredProcedureInfo> spsAnalisadasInfo, string diretorioService)
  {
    // _logger.LogInformation(
    //   "OrquestradorService -> CriarClasseRestClientServices -> \n  {DiretorioService}  \n {NomePacote} \n",
    //   diretorioService, nomePacote);

    foreach (var mInfo in msInfo)
    {
      // foreach (var sp in spsAnalisadasInfo)
      // {
      //   sp.EndpointsDosMicrosservicosProntosParaGerar
      //     = sp.Endpoints?.Where(ep => ep.IdMicrosservico == m.Id).ToHashSet();
      // }
      // var todosEps = mInfos.Select(m => m.Endpoints).SelectMany(e => e).ToHashSet();
      var imports = GetImportsRestClientService(nomePacote, mInfo.Endpoints);
      var map = RecuperarParametrosRestClientServices(nomePacote, mInfo.NomeMicrosservico,
        mInfo.NomeComInicialMaiuscula, mInfo.Endpoints, imports);

      var template
        = Template.Parse(File.ReadAllText(OrquestradorConstants.TEMPLATE_PATH_FILE_SERVICES));

      var content = template.Render(map);

      SalvarArquivo(diretorioService, Concat(mInfo.NomeComInicialMaiuscula, "RestClientService.java"),
        content);
    }
  }

  // private static IEnumerable<string> GetImports (HashSet<StoredProcedure> storedProcedures)
  // {
  //   var imports = new HashSet<string>();
  //
  //   foreach (var s in storedProcedures)
  //   {
  //     AdicionarImportsDasDeclaracoes(imports, s);
  //     AdicionarImportsDasAtribuicoes(imports, s);
  //   }
  //
  //   return imports;
  // }

  // private static void AdicionarImportsDasAtribuicoes (HashSet<string> imports, StoredProcedure s)
  // {
  //   var atribuicoes = s.Comandos.Where(c => c.CoTipoComando == 3).ToHashSet();
  //
  //   foreach (var c in atribuicoes)
  //   {
  //     foreach (var cv in c.VariaveisComando)
  //     {
  //       var pacoteImport = ((TipoDadoEnum) cv.IdVariavelNavigation.CoTipoDado).GetDescricao();
  //
  //       if (!pacoteImport.Equals(""))
  //       {
  //         imports.Add(pacoteImport);
  //       }
  //     }
  //   }
  //
  //   /* imports.AddRange(atribuicoes
  //     .Select(a => ((TipoDadoEnum) a.IdVariavelNavigation.CoTipoDado).GetDescricao())
  //     .Where(pacoteImport => !pacoteImport.Equals("")));*/
  // }

  // private static void AdicionarImportsDasDeclaracoes (HashSet<string> imports, StoredProcedure s)
  // {
  //   var declaracoes = s.Comandos.Where(c => c.CoTipoComando == 2).ToHashSet();
  //
  //   foreach (var c in declaracoes)
  //   {
  //     foreach (var cv in c.VariaveisComando)
  //     {
  //       var pacoteImport = ((TipoDadoEnum) cv.IdVariavelNavigation.CoTipoDado).GetDescricao();
  //
  //       if (!pacoteImport.Equals(""))
  //       {
  //         imports.Add(pacoteImport);
  //       }
  //     }
  //   }
  //
  //   /* imports.AddRange(declaracoes
  //     .Select(d => ((TipoDadoEnum) d.IdVariavelNavigation.CoTipoDado).GetDescricao())
  //     .Where(pacoteImport => !pacoteImport.Equals("")));*/
  // }

  private static Hash RecuperarParametrosRestClientServices(string nomePacote,
    string nomeMicrosservico, string nomeMicrosservicoComInicialMaiuscula,
    HashSet<EndpointInfo> endpoints, HashSet<string> imports)
    => new()
    {
      {"nomePacote", nomePacote},
      {"imports", imports},
      {"nomeMicrosservico", nomeMicrosservico},
      {"nomeMicrosservicoComInicialMaiuscula", nomeMicrosservicoComInicialMaiuscula},
      {"endpoints", endpoints}
    };

  /* ******************************************************************************************** */
  private void CriarClasseController(string nomePacote,
    HashSet<StoredProcedureInfo> spsAnalisadasInfo, HashSet<MicrosservicoInfo> mInfos,
    ISet<int> idsMicrosservicosProntosParaGerar, string diretorioController)
  {
    // _logger.LogInformation(
    //   "OrquestradorService -> CriarClasseController -> \n {DiretorioController} \n {NomePacote}",
    //   diretorioController, nomePacote);
    // CriarClasseIndexController(nomePacote, diretorioController);

    //var todosEps = mInfos.Select(m => m.Endpoints).SelectMany(e => e).ToHashSet();
    //IEnumerable<ClasseInfo> todosEps = new HashSet<ClasseInfo>();
    // foreach (var sp in spsAnalisadasInfo)
    // {
    //   sp.EndpointsDosMicrosservicosProntosParaGerar = sp.Endpoints
    //   ?.Where(ep => idsMicrosservicosProntosParaGerar.Contains(ep.IdMicrosservico)).ToHashSet();
    // }

    var dtosDosMicrosservicosProntosParaGerar = _dtoClasseService
     .RecuperarClassesDeUmaListaDeMicrosservicos(idsMicrosservicosProntosParaGerar).ToList();
    var map = RecuperarParametrosController(nomePacote,
      GetImportsController(spsAnalisadasInfo, dtosDosMicrosservicosProntosParaGerar, mInfos,
        nomePacote), spsAnalisadasInfo, mInfos);

    var template
      = Template.Parse(File.ReadAllText(OrquestradorConstants.TEMPLATE_PATH_FILE_CONTROLLER));

    var content = template.Render(map);

    SalvarArquivo(diretorioController, "OrchestratorController.java", content);
  }

  private static Hash RecuperarParametrosController(string nomePacote, HashSet<string> imports,
    IEnumerable<StoredProcedureInfo> spInfos, IEnumerable<MicrosservicoInfo> mInfos)
  {
    var parametrosConstrutorOrquestrador = MontarParametrosConstrutorOrquestrador(mInfos);
    return new Hash
    {
      {"nomePacote", nomePacote},
      {"imports", imports},
      {"microsservicos", mInfos},
      {"parametrosConstrutorOrquestrador", parametrosConstrutorOrquestrador},
      {"storedProcedures", spInfos}
    };
  }

  private static string MontarParametrosConstrutorOrquestrador(
    IEnumerable<MicrosservicoInfo> mInfos)
  {
    var nomesClasseNomeVariavel = new HashSet<string>();
    foreach (var m in mInfos)
    {
      nomesClasseNomeVariavel.Add(
        $"{m.NomeComInicialMaiuscula}RestClientService {m.NomeComInicialMinuscula}RestClientService");
    }
    return nomesClasseNomeVariavel.Aggregate((a, b) => a + ", " + b);
  }

  private static HashSet<string> GetImportsController(
    HashSet<StoredProcedureInfo> spsAnalisadasInfo,
    List<DtoClasse> dtosDosMicrosservicosProntosParaGerar, HashSet<MicrosservicoInfo> mInfos,
    string nomePacote)
  {
    var imports = new HashSet<string>();
    if (spsAnalisadasInfo.Any())
    {
      foreach (var sp in spsAnalisadasInfo.Where(sp => sp.ImportsController!.Any()))
      {
        imports.UnionWith(sp.ImportsController!);
      }
    }
    foreach (var m in mInfos)
    {
      imports.Add($"{nomePacote}.services.{m.NomeComInicialMaiuscula}RestClientService");
    }
    if (!dtosDosMicrosservicosProntosParaGerar.Any())
    {
      return imports;
    }
    foreach (var dto in dtosDosMicrosservicosProntosParaGerar)
    {
      imports.Add($"{nomePacote}.dto.{dto.NoDtoClasse.InicialMaiuscula()}");
    }
    return imports;
  }

  private static HashSet<string> GetImportsRestClientService(string nomePacote,
    HashSet<EndpointInfo> endPointInfo)
  {
    var imports = new HashSet<string>();
    if (!endPointInfo.Any())
    {
      return imports;
    }
    foreach (var ep in endPointInfo.Where(ep => ep.ImportsController!.Any()))
    {
      imports.UnionWith(ep.ImportsController!);
      if (ep.IsRetornoDTO == true)
      {
        imports.Add($"{nomePacote}.dto.{ep.NomeClasseRetorno?.InicialMaiuscula()}");
      }
    }
    return imports;
  }
  // private static HashSet<string> GetImportsService(HashSet<StoredProcedureInfo> spsAnalisadasInfo,
  //   HashSet<ClasseInfo> todosDtos, string nomePacote)
  // var imports = new HashSet<string>();
  // if (spsAnalisadasInfo.Any())
  // {
  //   foreach (var sp in spsAnalisadasInfo.Where(sp => sp.ImportsService.Any()))
  //   {
  //     imports.UnionWith(sp.ImportsService!);
  //   }
  // }
  // if (todosDtos.Any())
  // {
  //   return imports;
  // }
  // foreach (var dto in todosDtos)
  // {
  //   imports.Add($"{nomePacote}.dto.{dto.NomeComInicialMaiuscula}");
  // }
  // return imports;
  //}

  // private static void CriarClasseIndexController(string nomePacote, string diretorioController)
  // {
  //   var map = new Hash {{"nomePacote", nomePacote}};
  //
  //   var template
  //     = Template.Parse(File.ReadAllText(OrquestradorConstants.TEMPLATE_PATH_FILE_INDEX_CONTROLLER));
  //
  //   var content = template.Render(map);
  //   SalvarArquivo(diretorioController, "IndexController.java", content);
  // }
}
