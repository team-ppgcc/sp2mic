
using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

public class ExpressaoFilterDto
{
  public int? Id {get; set;}
  public int? IdComando {get; set;}
  public TipoDadoEnum? CoTipoDadoRetorno {get; set;}
  public int? NuOrdemExecucao {get; set;}
  public int? IdOperandoEsquerda {get; set;}
  public TipoOperadorEnum? CoOperador {get; set;}
  public int? IdOperandoDireita {get; set;}
}
