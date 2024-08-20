using DotLiquid;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Dtos;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Services.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Util.FileUtil;

namespace sp2mic.WebAPI.Modulo3.Geracao.Services;

public class GeracaoMicrosservicosService : IGeracaoMicrosservicos<GeracaoDto>
{
  private readonly ILogger<GeracaoMicrosservicosService> _logger;
  private readonly IMicrosservicoService _microsservicoService;
  private readonly IEngine<Microsservico, MicrosservicoInfo> _microsservicoEngine;
  private readonly IApiGatewayService<GeracaoDto> _apiGatewayService;
  private readonly IGeracaoService<GeracaoDto> _orquestradorService;

  public GeracaoMicrosservicosService(IMicrosservicoService microsservicoService,
    IEngine<Microsservico, MicrosservicoInfo> microsservicoEngine,
    IApiGatewayService<GeracaoDto> apiGatewayService,
    IGeracaoService<GeracaoDto> orquestradorService,
    ILogger<GeracaoMicrosservicosService> logger)
  {
    _microsservicoService = microsservicoService ??
      throw new ArgumentNullException(nameof (microsservicoService));
    _microsservicoEngine = microsservicoEngine ??
      throw new ArgumentNullException(nameof (microsservicoEngine));
    _apiGatewayService = apiGatewayService ??
      throw new ArgumentNullException(nameof (apiGatewayService));
    _orquestradorService = orquestradorService ??
      throw new ArgumentNullException(nameof (orquestradorService));
    _logger = logger ??
      throw new ArgumentNullException(nameof (logger));
  }

  public string GerarTodosProjetos(GeracaoDto dto)
  {
    var microsservicosProntosParaGerar = _microsservicoService.FindProntosParaGerar().ToHashSet();

    if (microsservicosProntosParaGerar.Count == 0)
    {
      return "No microservices pending generation were found.";
    }

    var microsservicosProntosParaGerarInfo = microsservicosProntosParaGerar
     .Select(mic => _microsservicoEngine.ConvertDados(mic)).ToHashSet();

    var microsservicosGerados = GerarMicrosservicos(dto, microsservicosProntosParaGerarInfo);

    _apiGatewayService.GerarProjeto(dto, microsservicosProntosParaGerarInfo);
    _orquestradorService.GerarProjeto(dto, microsservicosProntosParaGerarInfo);

    return $"Microservices Projects Successfully generated. {microsservicosGerados}";
  }

  private string GerarMicrosservicos(GeracaoDto dto,
    HashSet<MicrosservicoInfo> microsservicosProntosParaGerar)
  {
    _logger.LogInformation(@"MicrosservicosService -> GerarMicrosservicos =========== \n {Dir}", Path.Combine("Resources", "generated-microservices"));

    var dir = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "generated-microservices");
    _logger.LogInformation(@"dir = {Dir}", dir);
    if(Directory.Exists(dir))
    {
      Directory.Delete(dir, true);
      Directory.CreateDirectory(dir);
    }
    GeracaoMicrosservicosConstants.ValorDefault(dto);

    var microsservicosGerados = new HashSet<string>();

    foreach (var micInfo in microsservicosProntosParaGerar)
    {
      microsservicosGerados.Add(micInfo.NomeMicrosservico);
      GerarClassesMicrosservicos(micInfo, dto);
    }
    _logger.LogInformation(@"MicrosservicosService -> GerarProjeto =========== FIM \n");
    return microsservicosGerados.Select(s => s).Aggregate((a, b) => a + ", " + b);
  }

  private void GerarClassesMicrosservicos(MicrosservicoInfo msInfo, GeracaoDto dto)
  {
    dto.ProjectMetadataArtifactId = msInfo.NomeMicrosservico.Replace(" ", "-");
    dto.ProjectMetadataPackageName
      = Concat(dto.ProjectMetadataGroupId, '.', dto.ProjectMetadataArtifactId.Replace("-", ""));

    var diretorioMicrosservico
      = CriarDiretorioProjeto(Path.Combine("Resources", "generated-microservices"), msInfo.NomeMicrosservico);

    var diretorioSrcMainJavaPacote
      = CriarDiretorioSrcMainJavaPacote(diretorioMicrosservico, dto.ProjectMetadataPackageName);

    var diretorioSrcTestJavaPacote
      = CriarDiretorioSrcTestJavaPacote(diretorioMicrosservico, dto.ProjectMetadataPackageName);

    CriarArquivoPom(dto, diretorioMicrosservico);
    CriarArquivoPropriedades(dto, CriarDiretorioResources(diretorioMicrosservico));

    CriarClasseApplicationTests(dto.ProjectMetadataPackageName, msInfo.NomeMicrosservico,
      diretorioSrcTestJavaPacote);

    // CriarClassesPacoteUtil(nomePacote, CriarDiretorioUtil(diretorioSrcMainJavaPacote));
    CriarClasseApplication(dto.ProjectMetadataPackageName, msInfo.NomeMicrosservico,
      diretorioSrcMainJavaPacote);

    CriarClasseRepository(msInfo, dto.ProjectMetadataPackageName,
      CriarDiretorioRepository(diretorioSrcMainJavaPacote));

    CriarClasseController(msInfo, dto.ProjectMetadataPackageName,
      CriarDiretorioController(diretorioSrcMainJavaPacote));

    CriarClassesDto(msInfo, dto.ProjectMetadataPackageName,
      CriarDiretorioDto(diretorioSrcMainJavaPacote));

    CriarClasseResourceExceptionHandler(dto.ProjectMetadataPackageName,
      CriarDiretorioException(diretorioSrcMainJavaPacote));
  }

  /* ******************************************************************************************** */
  private void CriarArquivoPom(GeracaoDto dto, string diretorioMicrosservico)
  {
    // _logger.LogInformation("================ MicrosservicosService -> CriarArquivoPom =========" +
    //   "== \n {GeracaoDto} \n {DiretorioMicrosservico}",
    //   dto, diretorioMicrosservico);

    var map = new Hash
    {
      {"projectMetadataGroupId", dto.ProjectMetadataGroupId},
      {"projectMetadataArtifactId", dto.ProjectMetadataArtifactId},
      {"projectMetadataJavaVersion", dto.ProjectMetadataJavaVersion},
      {"springBootVersion", dto.SpringBootVersion}
    };

    var template
      = Template.Parse(File.ReadAllText(GeracaoMicrosservicosConstants.TEMPLATE_PATH_FILE_POM));

    var content = template.Render(map);
    SalvarArquivo(diretorioMicrosservico, "pom.xml", content);
  }

  /* ******************************************************************************************** */
  private static void CriarArquivoPropriedades(GeracaoDto dto, string diretorioResources)
  {
    // _logger.LogInformation(
    //   "================ MicrosservicosService -> CriarArquivoPropriedades ==============" +
    //   "\n {NumeroPorta} \n {DiretorioResources}", nomeMicrosservico, diretorioResources);

    var map = new Hash
    {
      {"projectMetadataArtifactId", dto.ProjectMetadataArtifactId},
      {"consulHost", dto.ConsulHost},
      {"consulPort", dto.ConsulPort},
      {"databaseHost", dto.DatabaseHost},
      {"databasePort", dto.DatabasePort},
      {"databaseName", dto.DatabaseName},
      {"databaseUserName", dto.DatabaseUserName},
      {"databasePassword", dto.DatabasePassword}
    };
    var template
      = Template.Parse(File.ReadAllText(GeracaoMicrosservicosConstants
       .TEMPLATE_PATH_FILE_ARQUIVO_PROPRIEDADES));
    var content = template.Render(map);
    SalvarArquivo(diretorioResources, "application.properties", content);
  }

  private void CriarClasseApplication(string nomePacote, string nomeMicrosservico,
    string diretorioSrcMainJavaPacote)
  {
    // _logger.LogInformation(
    //   "================ MicrosservicosService -> CriarClasseApplication =========" +
    //   "\n=== {NomePacote} \n {NomeMicrosservico} " +
    //   "\n ===== {DiretorioPacote}", nomePacote, nomeMicrosservico, diretorioSrcMainJavaPacote);

    var nomeMicrosservicoComInicialMaiuscula = nomeMicrosservico.InicialMaiuscula();
    var map = new Hash
    {
      {"nomePacote", nomePacote},
      {"nomeMicrosservicoComInicialMaiuscula", nomeMicrosservicoComInicialMaiuscula}
    };
    var template
      = Template.Parse(
        File.ReadAllText(GeracaoMicrosservicosConstants.TEMPLATE_PATH_FILE_APPLICATION));
    var content = template.Render(map);
    SalvarArquivo(diretorioSrcMainJavaPacote,
      Concat(nomeMicrosservicoComInicialMaiuscula, "Application.java"), content);
  }

  private void CriarClasseApplicationTests(string nomePacote, string nomeMicrosservico,
    string diretorioSrcTestJavaPacote)
  {
    // _logger.LogInformation(
    //   "================ MicrosservicosService -> CriarClasseApplicationTests =========" +
    //   "\n={NomePacote}= \n= {NomeMicrosservico} \n  {DiretorioSrcTestJavaPacote}\n ====",
    //   nomePacote, nomeMicrosservico, diretorioSrcTestJavaPacote);

    var nomeMicrosservicoComInicialMaiuscula = nomeMicrosservico.InicialMaiuscula();
    var map = new Hash
    {
      {"nomePacote", nomePacote},
      {"nomeMicrosservicoComInicialMaiuscula", nomeMicrosservicoComInicialMaiuscula}
    };
    var template
      = Template.Parse(File.ReadAllText(GeracaoMicrosservicosConstants
       .TEMPLATE_PATH_FILE_APPLICATION_TEST));
    var content = template.Render(map);
    SalvarArquivo(diretorioSrcTestJavaPacote,
      Concat(nomeMicrosservicoComInicialMaiuscula, "ApplicationTests.java"),
      content);
  }

  private static bool PossuiDto(MicrosservicoInfo info)
  {
    if (info == null)
    {
      throw new ArgumentNullException(nameof (info));
    }
    return info.Classes != null && info.Classes.Any();
  }

  private void CriarClassesDto(MicrosservicoInfo info, string nomePacote,
    string diretorioDto)
  {
    // _logger.LogInformation(
    //   "================ MicrosservicosService -> CriarClassesDto ==============" +
    //   "\n {Info} \n {DiretorioDto}", SerializeObject(info), diretorioDto);

    if (!PossuiDto(info))
    {
      return;
    }

    foreach (var classe in info.Classes!)
    {
      var nomeClasseDto = classe.NomeClasse.InicialMaiuscula();
      //The result of the expression is always 'false' because a value of type 'int' is never equal to 'null' of type 'int?'
      // if (classe.IdMicrosservico == null)
      // {
      //   throw new BadHttpRequestException(
      //     "Operation Denied. There is(are) DTO Class(es) not associated with Microservice.");
      // }

      var map = RecuperarParametrosDto(nomePacote, nomeClasseDto, classe.Atributos!,
        GetImportsDasDtoClasses(classe.Atributos));
      var template
        = Template.Parse(File.ReadAllText(GeracaoMicrosservicosConstants.TEMPLATE_PATH_FILE_DTO));
      var content = template.Render(map);
      SalvarArquivo(diretorioDto, Concat(nomeClasseDto, ".java"),
        content);
    }
  }

  private static HashSet<string> GetImportsDasDtoClasses(IEnumerable<AtributoInfo>? atributoInfos)
  {
    var imports = new HashSet<string>();

    if (atributoInfos is null)
    {
      return imports;
    }
    foreach (var a in atributoInfos)
    {
      if (a.Import is null or "")
      {
        continue;
      }

      imports.Add(a.Import);
    }
    return imports;
  }

  private static HashSet<string> GetImportsController(string nomePacote,
    HashSet<EndpointInfo> endPointInfo)
  {
    var imports = new HashSet<string>();
    if (!endPointInfo.Any())
    {
      return imports;
    }
    foreach (var ep in endPointInfo.Where(ep => ep.ImportsController is not null && ep.ImportsController.Any()))
    {
      imports.UnionWith(ep.ImportsController!);
      if (ep.IsRetornoDTO == true)
      {
        imports.Add($"{nomePacote}.dto.{ep.NomeClasseRetorno?.InicialMaiuscula()}");
      }
    }
    return imports;
  }

  private static HashSet<string> GetImportsRepository(string nomePacote,
    HashSet<EndpointInfo> endPointInfo)
  {
    var imports = new HashSet<string>();
    if (!endPointInfo.Any())
    {
      return imports;
    }
    foreach (var ep in endPointInfo.Where(ep => ep.ImportsRepository is not null && ep.ImportsRepository.Any()))
    {
      //imports.UnionWith(ep.ImportsRepository!);
      foreach (var e in ep.ImportsRepository!.Where(e => e != ""))
      {
        imports.Add(e);
      }
      if (ep.IsRetornoDTO == true)
      {
        imports.Add($"{nomePacote}.dto.{ep.NomeClasseRetorno?.InicialMaiuscula()}");
      }
    }
    return imports;
  }

  private static void CriarClasseResourceExceptionHandler(string nomePacote,
    string diretorioException)
  {
    var map = new Hash {{"nomePacote", nomePacote}};

    var template
      = Template.Parse(File.ReadAllText(GeracaoMicrosservicosConstants
       .TEMPLATE_PATH_FILE_RESOURCE_EXCEPTION_HANDLER));

    var content = template.Render(map);
    SalvarArquivo(diretorioException, "ResourceExceptionHandler.java", content);
  }

  private static Hash RecuperarParametrosDto(string nomePacote, string nomeDtoClasse,
    IEnumerable<AtributoInfo> atributosInfo, HashSet<string> imports)
    => new()
    {
      {"nomePacote", nomePacote},
      {"imports", imports},
      {"nomeDtoClasse", nomeDtoClasse},
      {"atributos", atributosInfo},
    };

  private void CriarClasseRepository(MicrosservicoInfo micInfo, string nomePacote,
    string diretorioRepository)
  {
    // _logger.LogInformation(
    //   "================ MicrosservicosService -> CriarClasseRepository ==============" +
    //   "\n {Info} \n {DiretorioRepository}", SerializeObject(micInfo), diretorioRepository);

    var nomeMicrosservicoInicialMaiuscula = micInfo.NomeMicrosservico.InicialMaiuscula();
    var map = RecuperarParametrosRepository(nomePacote, nomeMicrosservicoInicialMaiuscula,
      micInfo.Endpoints, GetImportsRepository(nomePacote, micInfo.Endpoints));

    var template
      = Template.Parse(
        File.ReadAllText(GeracaoMicrosservicosConstants.TEMPLATE_PATH_FILE_REPOSITORY));

    var content = template.Render(map);

    SalvarArquivo(diretorioRepository, Concat(nomeMicrosservicoInicialMaiuscula, "Repository.java"),
      content);
  }

  private static Hash RecuperarParametrosRepository(string nomePacote, string nomeMicrosservico,
    IEnumerable<EndpointInfo> endPointsInfo, HashSet<string> imports)
    => new()
    {
      {"nomePacote", nomePacote},
      {"imports", imports},
      {"nomeMicrosservicoComInicialMaiuscula", nomeMicrosservico},
      {"endpoints", endPointsInfo}
    };

  /* ******************************************************************************************** */
  private void CriarClasseController(MicrosservicoInfo info, string nomePacote,
    string diretorioController)
  {
    // _logger.LogInformation(
    //   "================ MicrosservicosService -> CriarClasseController ==============" +
    //   "\n {Info} \n {DiretorioController}", SerializeObject(info), diretorioController);

    CriarClasseIndexController(nomePacote, info.NomeMicrosservico, diretorioController);
    var nomeMicrosservicoInicialMaiuscula = info.NomeMicrosservico.InicialMaiuscula();
    var map = RecuperarParametrosController(nomePacote,
      nomeMicrosservicoInicialMaiuscula, info.Endpoints,
      GetImportsController(nomePacote, info.Endpoints));

    var template
      = Template.Parse(
        File.ReadAllText(GeracaoMicrosservicosConstants.TEMPLATE_PATH_FILE_CONTROLLER));

    var content = template.Render(map);

    SalvarArquivo(diretorioController, Concat(nomeMicrosservicoInicialMaiuscula, "Controller.java"),
      content);
  }

  private static Hash RecuperarParametrosController(string nomePacote, string nomeMicrosservico,
    IEnumerable<EndpointInfo> endPointsInfo, HashSet<string> imports)
    => new()
    {
      {"nomePacote", nomePacote},
      {"imports", imports},
      {"nomeMicrosservicoComInicialMaiuscula", nomeMicrosservico},
      {"endpoints", endPointsInfo}
    };

  private static void CriarClasseIndexController(string nomePacote, string nomeMicrosservico,
    string diretorioController)
  {
    var map = new Hash {{"nomePacote", nomePacote}, {"nomeMicrosservico", nomeMicrosservico}};

    var template
      = Template.Parse(File.ReadAllText(GeracaoMicrosservicosConstants
       .TEMPLATE_PATH_FILE_INDEX_CONTROLLER));

    var content = template.Render(map);
    SalvarArquivo(diretorioController, "IndexController.java", content);
  }
}
