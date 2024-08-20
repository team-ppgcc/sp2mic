namespace sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

public class MicrosservicoFilterDto
{
  public int? Id {get; set;}
  public string? NoMicrosservico {get; set;} = null!;
  public bool? SnProntoParaGerar {get; set;}
}
