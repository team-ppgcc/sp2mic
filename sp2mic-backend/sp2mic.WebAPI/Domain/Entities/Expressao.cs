using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Domain.Entities;

public class Expressao
{
  public Expressao()
  {
    Comandos = new HashSet<Comando>();
    Operandos = new HashSet<Operando>();
  }

  public int Id {get; set;}
  public TipoDadoEnum CoTipoDadoRetorno {get; set;}
  public int NuOrdemExecucao {get; set;}
  public int? IdOperandoEsquerda {get; set;}
  public virtual Operando? IdOperandoDireitaNavigation {get; set;}
  public TipoOperadorEnum? CoOperador {get; set;}
  public int? IdOperandoDireita {get; set;}
  public virtual Operando? IdOperandoEsquerdaNavigation {get; set;}

  public virtual ICollection<Comando> Comandos {get; set;}
  public virtual ICollection<Operando> Operandos {get; set;}
}
