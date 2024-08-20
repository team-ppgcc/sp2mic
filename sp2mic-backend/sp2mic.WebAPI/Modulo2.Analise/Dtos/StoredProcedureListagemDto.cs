namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class StoredProcedureListagemDto
{
  public int? Id {get; set;}
  public string? NoSchema {get; set;}
  public string? NoStoredProcedure {get; set;}
  public string? NoTipoDadoRetorno {get; set;}
  public string? NoSucessoParser {get; set;}
  public bool? SnAnalisada {get; set;}
  public int? QtdEndpoints {get; set;}

  public string? TabelasAssociadas {get; set;}
}
