namespace sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

public class DtoClasseFilterDto
{
  public int? Id {get; set;}
  public string? NoDtoClasse {get; set;} = null!;
  public int? IdMicrosservico {get; set;}
  public int? IdStoredProcedure {get; set;}
}
