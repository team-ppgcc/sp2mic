using static Newtonsoft.Json.JsonConvert;

namespace sp2mic.WebAPI.Modulo1.Carga.Dtos;

public class TabelasProcedureDto
{
  public string NomeProcedure {get; set;}
  public List<ParDto> TabelasDaProcedure {get; set;}

  public TabelasProcedureDto (string nomeProcedure, List<ParDto> tabelasDaProcedure)
  {
    NomeProcedure = nomeProcedure;
    TabelasDaProcedure = tabelasDaProcedure;
  }

  public override string ToString () => SerializeObject(this);
}
