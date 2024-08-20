using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class OperandoFilterDto
{
  public int? Id {get; set;}
  public TipoOperandoEnum? CoTipo {get; set;}
  public string? TxValor {get; set;}
  public bool? SnNegacao {get; set;}
  public int? IdVariavel {get; set;}
  public int? IdExpressao {get; set;}
  public int? IdEndpoint {get; set;}
}
