using DotLiquid;
using sp2mic.WebAPI.Modulo3.Geracao.Dtos;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Services.Interfaces;
using static sp2mic.WebAPI.CrossCutting.Util.FileUtil;
using static System.String;

namespace sp2mic.WebAPI.Modulo3.Geracao.Services;

public class ApiGatewayService : IApiGatewayService<GeracaoDto>
{
  private readonly ILogger<ApiGatewayService> _logger;

  public ApiGatewayService(ILogger<ApiGatewayService> logger)
    => _logger = logger ?? throw new ArgumentNullException(nameof (logger));

  public void GerarProjeto(GeracaoDto dto, HashSet<MicrosservicoInfo> msInfos)
  {
    //_logger.LogInformation("ApiGatewayService -> GerarProjeto -> {GeracaoDto}", dto);

    ApiGatewayConstants.ValorDefault(dto);
    dto.ProjectMetadataPackageName = Concat(dto.ProjectMetadataGroupId, ".gateway");
    var diretorioApiGateway = CriarDiretorioProjeto(Path.Combine("Resources", "generated-microservices"), "gateway");
    var diretorioSrcMainJavaPacote
      = CriarDiretorioSrcMainJavaPacote(diretorioApiGateway, dto.ProjectMetadataPackageName);
    var diretorioSrcTestJavaPacote
      = CriarDiretorioSrcTestJavaPacote(diretorioApiGateway, dto.ProjectMetadataPackageName);

    CriarArquivoPom(dto, diretorioApiGateway);
    CriarArquivoPropriedades(dto, CriarDiretorioResources(diretorioApiGateway));
    CriarClasseApplication(dto.ProjectMetadataPackageName, diretorioSrcMainJavaPacote);
    CriarClasseApplicationTests( dto.ProjectMetadataPackageName,
      diretorioSrcTestJavaPacote);
  }

  private void CriarArquivoPom(GeracaoDto dto, string diretorioApiGateway)
  {
    // _logger.LogInformation("================ ApiGatewayService -> CriarArquivoPom ==========" +
    //   "== \n ApiGatewayDto: {ApiGatewayDto}, \n diretorioApiGateway: {DiretorioApiGateway} == ",
    //   dto, diretorioApiGateway);

    var map = new Hash
    {
      {"projectMetadataGroupId", dto.ProjectMetadataGroupId},
      {"projectMetadataJavaVersion", dto.ProjectMetadataJavaVersion},
      {"springBootVersion", dto.SpringBootVersion}
    };

    var template = Template.Parse(File.ReadAllText(ApiGatewayConstants.TEMPLATE_PATH_FILE_POM));
    var content = template.Render(map);
    SalvarArquivo(diretorioApiGateway, "pom.xml", content);
  }

  private void CriarArquivoPropriedades(GeracaoDto dto, string diretorioResources)
  {
    // _logger.LogInformation(
    //   "================ ApiGatewayService -> CriarArquivoPropriedades ==============");
    //
    // _logger.LogInformation(
    //   "\n=== consulHost: {ConsulHost} diretorioResources: {DiretorioResources}", dto.ConsulHost,
    //   diretorioResources);

    var map = new Hash
    {
      {"gatewayPort", dto.GatewayPort},
      {"consulHost", dto.ConsulHost},
      {"consulPort", dto.ConsulPort}
    };

    var template
      = Template.Parse(
        File.ReadAllText(ApiGatewayConstants.TEMPLATE_PATH_FILE_ARQUIVO_PROPRIEDADES));

    var content = template.Render(map);
    SalvarArquivo(diretorioResources, "application.properties", content);

    template
      = Template.Parse(
        File.ReadAllText(ApiGatewayConstants.TEMPLATE_PATH_FILE_ARQUIVO_PROPRIEDADES_YML));

    content = template.Render();
    SalvarArquivo(diretorioResources, "application.yml", content);
  }

  private void CriarClasseApplication(string nomePacote, string diretorioSrcMainJavaPacote)
  {
    // _logger.LogInformation("================ ApiGatewayService -> CriarClasseApplication ====" +
    //   "\n==== {NomePacote} \n {DiretorioPacote}", nomePacote, diretorioSrcMainJavaPacote);

    var map = new Hash {{"nomePacote", nomePacote}};

    var template
      = Template.Parse(File.ReadAllText(ApiGatewayConstants.TEMPLATE_PATH_FILE_APPLICATION));

    var content = template.Render(map);
    SalvarArquivo(diretorioSrcMainJavaPacote, "GatewayApplication.java", content);
  }

  private void CriarClasseApplicationTests(string nomePacote, string diretorioSrcTestJavaPacote)
  {
    // _logger.LogInformation(
    //   "============= ApiGatewayService -> CriarClasseApplicationTests =========" +
    //   "\n={NomePacote}== {DiretorioSrcTestJavaPacote} \n ====", nomePacote,
    //   diretorioSrcTestJavaPacote);

    var map = new Hash {{"nomePacote", nomePacote}};

    var template
      = Template.Parse(File.ReadAllText(ApiGatewayConstants.TEMPLATE_PATH_FILE_APPLICATION_TEST));

    var content = template.Render(map);
    SalvarArquivo(diretorioSrcTestJavaPacote, "GatewayApplicationTests.java", content);
  }
}
