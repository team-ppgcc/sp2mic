using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Domain.Entities;

public class Comando
{
  public Comando()
  {
    AsVariaveisDesseComando = new HashSet<ComandoVariavel>();
    ComandosInternos = new HashSet<Comando>();
  }

  public int Id {get; set;}
  public string TxComando {get; set;} = null!;
  public string TxComandoTratado {get; set;} = null!;
  public TipoComandoEnum CoTipoComando {get; set;}
  public int NuOrdemExecucao {get; set;}
  public string? VlAtribuidoVariavel {get; set;}
  public int IdStoredProcedure {get; set;}
  public virtual StoredProcedure IdStoredProcedureNavigation {get; set;} = null!;
  public int? IdComandoOrigem {get; set;}
  public virtual Comando? IdComandoOrigemNavigation {get; set;}
  public int? IdEndpoint {get; set;}
  public virtual Endpoint? IdEndpointNavigation {get; set;}
  public int? IdExpressao {get; set;}
  public virtual Expressao? IdExpressaoNavigation {get; set;}
  public bool? SnCondicaoOrigem {get; set;}

  public virtual ICollection<ComandoVariavel> AsVariaveisDesseComando {get; set;}
  public virtual ICollection<Comando> ComandosInternos {get; set;}
}
