namespace sp2mic.WebAPI.Modulo1.Carga.Dtos;


public class DadosConexaoDto
{
  public string Host {get; set;} = "localhost";
  public string Port {get; set;} = "1433";
  public string DatabaseName {get; set;} = "dbp_54808_sig2000";
  public string UserName {get; set;} = "sp2mic";
  public string Password {get; set;} = "admin";
}
