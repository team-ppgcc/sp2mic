using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

public class VariavelFilterDto
{
  public int? Id {get; set;}
  public string? NoVariavel {get; set;} = null!;
  public TipoDadoEnum? CoTipoDado {get; set;}
  public TipoEscopoEnum? CoTipoEscopo {get; set;}
  public int? NuTamanho {get; set;}
  public int? IdStoredProcedure {get; set;}
  public int? IdMicrosservico {get; set;}
}
