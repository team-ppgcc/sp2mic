using sp2mic.WebAPI.Modulo3.Geracao.Dtos;

namespace sp2mic.WebAPI.Modulo3.Geracao.Services;

public sealed class GeracaoMicrosservicosConstants
{
  public static string RESOURCES_PATH
    = Path.Combine("src", "main", "resources");

  public static string SRC_MAIN_JAVA
    = Path.Combine("src", "main", "java");

  public static string SRC_TEST_JAVA
    = Path.Combine("src", "test", "java");

  public static string TEMPLATE_PATH_FILE_POM
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "pom.liquid");

  public static readonly string TEMPLATE_PATH_FILE_PARENT_POM
    = Path.Combine("Modulo3.Geracao", "Templates", "parentPom.liquid");

  public static string TEMPLATE_PATH_FILE_CONTROLLER
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "Controller.liquid");

  public static string TEMPLATE_PATH_FILE_INDEX_CONTROLLER
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "IndexController.liquid");

  public static string TEMPLATE_PATH_FILE_REPOSITORY
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "Repository.liquid");

  public static string TEMPLATE_PATH_FILE_DTO
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "DTO.liquid");

  public static string TEMPLATE_PATH_FILE_RESOURCE_EXCEPTION_HANDLER
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "ResourceExceptionHandler.liquid");

  public static string TEMPLATE_PATH_FILE_APPLICATION
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "Application.liquid");

  public static string TEMPLATE_PATH_FILE_APPLICATION_TEST
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "ApplicationTests.liquid");

  public static string TEMPLATE_PATH_FILE_ARQUIVO_PROPRIEDADES
    = Path.Combine("Modulo3.Geracao", "Templates", "microservices", "application.properties.liquid");


  public GeracaoMicrosservicosConstants ()
    => throw new AccessViolationException("This class cannot be instantiated");

  public static void ValorDefault (GeracaoDto dto)
  {
    dto.DiretorioDestinoClasses ??= Path.Combine("Resources", "generated-microservices");

    dto.SpringBootVersion ??= "3.2.0";

    dto.ProjectMetadataGroupId ??= "com.example";
    dto.ProjectMetadataArtifactId ??= "microsservico";
    //dto.ProjectMetadataName ??= "Microsservico";
    //dto.ProjectMetadataDescription ??= "Projeto do Microsservico";
    dto.ProjectMetadataPackageName
      ??= string.Concat(dto.ProjectMetadataGroupId, '.', dto.ProjectMetadataArtifactId);

    dto.ProjectMetadataJavaVersion ??= "21";

    //dto.EurekaClientVersion ??= "3.1.3";
    //dto.MssqlJdbcVersion = "11.2.1.jre17";
    dto.ConsulHost ??= "localhost";
    dto.ConsulPort ??= "8500";
    dto.DatabaseHost ??= "localhost";
    dto.DatabasePort ??= "1433";
    dto.DatabaseName ??= "dbp_54808_sig2000";
    dto.DatabaseUserName ??= "sp2mic";
    dto.DatabasePassword ??= "admin";
  }
}
