using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class StoredProcedureUpdateDto
{
  public string NoStoredProcedure {get; set;} = null!;
  public string NoSchema {get; set;} = null!;
  public TipoDadoEnum CoTipoDadoRetorno {get; set;}
  public bool SnRetornoLista {get; set;}
  public bool SnAnalisada {get; set;}
  public int? IdDtoClasse {get; set;}
}
