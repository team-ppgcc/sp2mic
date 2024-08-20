
using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Domain.Entities;

public class Endpoint
{
  public Endpoint()
  {
    Comandos = new HashSet<Comando>();
    Operandos = new HashSet<Operando>();
    TabelasAssociadas = new HashSet<Tabela>();
    Parametros = new HashSet<Variavel>();
  }

  public int Id {get; set;}
  public string? NoMetodoEndpoint {get; set;}
  public string? NoPath {get; set;}
  public string TxEndpoint {get; set;} = null!;
  public string TxEndpointTratado {get; set;} = null!;
  public TipoEndpointEnum CoTipoSqlDml {get; set;}
  public TipoDadoEnum CoTipoDadoRetorno {get; set;}
  public bool? SnRetornoLista {get; set;}
  public bool SnAnalisado {get; set;}
  public int? IdMicrosservico {get; set;}
  public virtual Microsservico? IdMicrosservicoNavigation {get; set;}
  public int IdStoredProcedure {get; set;}
  public virtual StoredProcedure IdStoredProcedureNavigation {get; set;} = null!;
  public int? IdDtoClasse {get; set;}
  public virtual DtoClasse? IdDtoClasseNavigation {get; set;}
  public int? IdVariavelRetornada {get; set;}
  public virtual Variavel? IdVariavelRetornadaNavigation {get; set;}

  public virtual ICollection<Comando> Comandos {get; set;}
  public virtual ICollection<Operando> Operandos {get; set;}
  public virtual ICollection<Tabela> TabelasAssociadas {get; set;}
  public virtual ICollection<Variavel> Parametros {get; set;}

  public List<Variavel> GetParametros()
  {
    return !Parametros.Any() ? new List<Variavel>() :
      Parametros.OrderBy(v => v.NoVariavel).ToList();
  }
}
