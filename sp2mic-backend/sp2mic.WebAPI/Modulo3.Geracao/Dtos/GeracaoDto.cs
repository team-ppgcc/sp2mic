

namespace sp2mic.WebAPI.Modulo3.Geracao.Dtos;


public class GeracaoDto : SpringBootDto
{
  public string OrchestratorPort {get; set;} = "8081";
  public string GatewayPort {get; set;} = "8090";
  public string ConsulHost {get; set;} = "localhost";
  public string ConsulPort {get; set;} = "8500";
  public string DatabaseHost {get; set;} = "localhost";
  public string DatabasePort {get; set;} = "1433";
  public string DatabaseName {get; set;} = "dbp_54808_sig2000";
  public string DatabaseUserName {get; set;} = "sp2mic";
  public string DatabasePassword {get; set;} = "admin";
}
