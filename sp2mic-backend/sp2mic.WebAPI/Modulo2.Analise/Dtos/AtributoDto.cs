using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class AtributoDto
{
  public int? Id {get; set;}
  public string? NoAtributo {get; set;}
  public string? NoTipoDado {get; set;}
  public TipoDadoEnum? CoTipoDado {get; set;}
  public int? IdDtoClasse {get; set;}
}
