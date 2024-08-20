namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class DtoClasseDto
{
  public DtoClasseDto() { }

  public int? Id {get; set;}
  public string? NoDtoClasse {get; set;}
  //public string? NoMicrosservico {get; set;} não tem mais mic na classe
  //public int? IdMicrosservico {get; set;} não tem mais mic na classe
  public int? IdStoredProcedure {get; set;}
  public string? NoStoredProcedure {get; set;}
  public string? TxDtoClasse {get; set;}
  //public MicrosservicoDto? IdMicrosservicoNavigation {get; set;}
  // public ICollection<AtributoDto>? Atributos {get; set;}
}
