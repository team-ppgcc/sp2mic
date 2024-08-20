using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;


public class StoredProcedureFilterDto
{
  public int? Id {get; set;}
  public string? NoStoredProcedure {get; set;} = null!;
  public string? NoSchema {get; set;} = null!;
  //public string? TxDefinicao {get; set;} = null!;
  //public string? TxDefinicaoTratada {get; set;} = null!;
  public TipoDadoEnum? CoTipoDadoRetorno {get; set;}
  public bool? SnRetornoLista {get; set;}
  public int? IdDtoClasse {get; set;}
  public string? TxResultadoParser {get; set;} = null!;
  public bool? SnSucessoParser {get; set;}
  public bool? SnAnalisada {get; set;}
}
