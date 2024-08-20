namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class EndpointListagemDto
{
  public int? Id {get; set;}
  public string? NoMetodoEndpoint {get; set;}
  public string? NoPath {get; set;}
  public string? NoMicrosservico {get; set;}
  public string? NoTipoSqlDml {get; set;}
  public string? NoTipoDadoRetorno {get; set;}
  public bool? SnAnalisado {get; set;}

  public int? IdStoredProcedure {get; set;}
  public string? NoStoredProcedure {get; set;}

  public string? TabelasAssociadas {get; set;}
}
