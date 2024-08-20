using sp2mic.WebAPI.Modulo3.Geracao.Dtos;

namespace sp2mic.WebAPI.Modulo3.Geracao.Services;

public sealed class ApiGatewayConstants
{
  public static string TEMPLATE_PATH_FILE_POM
    = Path.Combine("Modulo3.Geracao", "Templates", "gateway", "pom.liquid");

  public static string TEMPLATE_PATH_FILE_APPLICATION
    = Path.Combine("Modulo3.Geracao", "Templates", "gateway", "Application.liquid");

  public static string TEMPLATE_PATH_FILE_APPLICATION_TEST
    = Path.Combine("Modulo3.Geracao", "Templates", "gateway", "GatewayControllerTest.liquid");

  public static string TEMPLATE_PATH_FILE_ARQUIVO_PROPRIEDADES_YML
    = Path.Combine("Modulo3.Geracao", "Templates", "gateway", "application.yml.liquid");

  public static string TEMPLATE_PATH_FILE_ARQUIVO_PROPRIEDADES
    = Path.Combine("Modulo3.Geracao", "Templates", "gateway", "application.properties.liquid");

  public ApiGatewayConstants ()
    => throw new AccessViolationException("This class cannot be instantiated");

  public static void ValorDefault (GeracaoDto dto)
  {
    dto.DiretorioDestinoClasses ??= Path.Combine("Resources", "generated-microservices", "gateway");

    dto.SpringBootVersion ??= "3.2.0";

    dto.ProjectMetadataGroupId ??= "com.example";
    dto.ProjectMetadataArtifactId ??= "gateway";
    //dto.ProjectMetadataName ??= "ApiGateway";
    //dto.ProjectMetadataDescription ??= "Projeto do ApiGateway";
    dto.ProjectMetadataPackageName
      ??= string.Concat(dto.ProjectMetadataGroupId, '.', "gateway");

    dto.ProjectMetadataJavaVersion ??= "21";

    dto.ConsulPort ??= "8500";
    dto.ConsulHost ??= "localhost";
    dto.GatewayPort ??= "8090";
  }
}
