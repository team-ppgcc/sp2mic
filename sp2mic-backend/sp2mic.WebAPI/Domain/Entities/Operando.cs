using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Domain.Entities;


public class Operando
{
  public Operando()
  {
    ExpressaoIdOperandoDireitaNavigations = new HashSet<Expressao>();
    ExpressaoIdOperandoEsquerdaNavigations = new HashSet<Expressao>();
  }
  public int Id {get; set;}
  public TipoOperandoEnum CoTipoOperando {get; set;}
  public string? TxValor {get; set;}
  public bool SnNegacao {get; set;}
  public int? IdVariavel {get; set;}
  public virtual Variavel? IdVariavelNavigation {get; set;}
  public int? IdExpressao {get; set;}
  public virtual Expressao? IdExpressaoNavigation {get; set;}
  public int? IdEndpoint {get; set;}
  public virtual Endpoint? IdEndpointNavigation {get; set;}

  public virtual ICollection<Expressao> ExpressaoIdOperandoDireitaNavigations {get; set;}
  public virtual ICollection<Expressao> ExpressaoIdOperandoEsquerdaNavigations {get; set;}
}
