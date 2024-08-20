
using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

public class ComandoFilterDto
{
  public int? Id {get; set;}
  public string? TxComando {get; set;}
  public string? TxComandoTratado {get; set;}
  public TipoComandoEnum? CoTipoComando {get; set;}
  public int? NuOrdemExecucao {get; set;}
  public string? VlAtribuidoVariavel {get; set;}
  public int? IdStoredProcedure {get; set;}
  public int? IdComandoOrigem {get; set;}
  public int? IdEndpoint {get; set;}
  public int? IdExpressao {get; set;}
  public bool? SnCondicaoOrigem {get; set;}
}
