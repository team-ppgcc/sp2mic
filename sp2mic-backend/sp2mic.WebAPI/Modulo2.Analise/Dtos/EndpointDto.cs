using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class EndpointDto
{
  public int? Id {get; set;}
  public string? NoMetodoEndpoint {get; set;}
  public string? NoPath {get; set;}
  public bool? SnRetornoLista {get; set;}
  public bool? SnAnalisado {get; set;}
  public string? TxEndpointTratado {get; set;}
  public TipoDadoEnum? CoTipoDadoRetorno {get; set;}
  public int? IdDtoClasse {get; set;}
  public string? NoVariavelRetornda {get; set;}
  public string? NoStoredProcedure {get; set;}
  public int? IdMicrosservico {get; set;}
  public string? NoMicrosservico {get; set;}
  public int? IdStoredProcedure {get; set;}

  public string? TabelasAssociadas {get; set;}
}
