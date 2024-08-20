
using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

public class EndpointFilterDto
{
  public int? Id {get; set;}
  public string? NoMetodoEndpoint {get; set;}
  public string? NoPath {get; set;}
  public string? TxEndpoint {get; set;}
  public string? TxEndpointTratado {get; set;}
  public TipoEndpointEnum? CoTipoSqlDml {get; set;}
  public TipoDadoEnum? CoTipoDadoRetorno {get; set;}
  public bool? SnRetornoLista {get; set;}
  public bool? SnAnalisado {get; set;}
  public int? IdMicrosservico {get; set;}
  public int? IdStoredProcedure {get; set;}
  public int? IdDtoClasse {get; set;}
  public int? IdVariavelRetornada {get; set;}
}
