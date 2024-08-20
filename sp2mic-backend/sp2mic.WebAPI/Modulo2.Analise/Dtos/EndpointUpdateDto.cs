using System.ComponentModel.DataAnnotations;
using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class EndpointUpdateDto
{
  public EndpointUpdateDto () { }

  public string NoMetodoEndpoint {get; set;}
  public string NoPath {get; set;}
  public string TxEndpointTratado {get; set;}
  public TipoDadoEnum CoTipoDadoRetorno {get; set;}
  public bool SnRetornoLista {get; set;}
  public bool SnAnalisado {get; set;}
  public int? IdDtoClasse {get; set;}
  [Required(ErrorMessage = "Microservice must be filled.")]
  public int IdMicrosservico {get; set;}

  public DtoClasseUpdateDto? IdDtoClasseNavigation {get; set;}

}
