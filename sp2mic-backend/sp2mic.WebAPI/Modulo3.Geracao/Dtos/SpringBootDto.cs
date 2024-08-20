
namespace sp2mic.WebAPI.Modulo3.Geracao.Dtos;


public class SpringBootDto
{
  public string DiretorioDestinoClasses {get; set;}
    = Path.Combine("Resources", "generated-microservices");
  public string SpringBootVersion {get; set;} = "3.2.6";
  public string ProjectMetadataGroupId {get; set;} = "com.example";
  public string ProjectMetadataArtifactId {get; set;} = "demo";
  public string ProjectMetadataPackageName {get; set;} = "com.example.demo";
  public string ProjectMetadataJavaVersion {get; set;} = "22";
}
