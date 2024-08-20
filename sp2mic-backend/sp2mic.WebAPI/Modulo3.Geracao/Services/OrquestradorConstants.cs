using sp2mic.WebAPI.Modulo3.Geracao.Dtos;

namespace sp2mic.WebAPI.Modulo3.Geracao.Services;

public sealed class OrquestradorConstants
{
  public static string TEMPLATE_PATH_FILE_POM
    = Path.Combine("Modulo3.Geracao", "Templates", "orquestrador", "pom.liquid");

  public static string TEMPLATE_PATH_FILE_CONTROLLER
    = Path.Combine("Modulo3.Geracao", "Templates", "orquestrador", "Controller.liquid");

  public static string TEMPLATE_PATH_FILE_INDEX_CONTROLLER
    = Path.Combine("Modulo3.Geracao", "Templates", "orquestrador", "IndexController.liquid");

  public static string TEMPLATE_PATH_FILE_SERVICES
    = Path.Combine("Modulo3.Geracao", "Templates", "orquestrador", "RestClientServices.liquid");

  public static string TEMPLATE_PATH_FILE_DTO
    = Path.Combine("Modulo3.Geracao", "Templates", "orquestrador", "DTO.liquid");

  public static string TEMPLATE_PATH_FILE_APPLICATION
    = Path.Combine("Modulo3.Geracao", "Templates", "orquestrador", "Application.liquid");

  public static string TEMPLATE_PATH_FILE_APPLICATION_TEST
    = Path.Combine("Modulo3.Geracao", "Templates", "orquestrador", "ApplicationTests.liquid");

  public static string TEMPLATE_PATH_FILE_ARQUIVO_PROPRIEDADES
    = Path.Combine("Modulo3.Geracao", "Templates", "orquestrador", "application.properties.liquid");

  private OrquestradorConstants ()
    => throw new AccessViolationException("This class cannot be instantiated");

  public static void ValorDefault (GeracaoDto dto)
  {
    dto.SpringBootVersion ??= "3.2.0";

    dto.ProjectMetadataGroupId ??= "com.example";
    dto.ProjectMetadataArtifactId ??= "orchestrator";
    //dto.ProjectMetadataName ??= "Orchestrator";
    //dto.ProjectMetadataDescription ??= "Orchestrator Project";
    dto.ProjectMetadataPackageName
      ??= string.Concat(dto.ProjectMetadataGroupId, '.', dto.ProjectMetadataArtifactId);

    dto.ProjectMetadataJavaVersion ??= "21";

    dto.OrchestratorPort = "8080";
  }
}
