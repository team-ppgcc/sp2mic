using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class AtributoUpdateDto
{
  public AtributoUpdateDto() { }

  public string? NoAtributo {get; set;}
  public TipoDadoEnum? CoTipoDado {get; set;}
  public int? IdDtoClasse {get; set;}
}
