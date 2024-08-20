using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Domain.Entities;

public class Variavel
{
  public Variavel()
  {
    OsComandosQueContemEssaVariavel = new HashSet<ComandoVariavel>();
    EndpointsQueContemEssaVariavelComoParametro = new HashSet<Endpoint>();
    Operandos = new HashSet<Operando>();
    EndpointsQueRetornamEssaVariavel = new HashSet<Endpoint>();
  }
  public int Id {get; set;}
  public string NoVariavel {get; set;} = null!;
  public TipoDadoEnum CoTipoDado {get; set;}
  public TipoEscopoEnum CoTipoEscopo {get; set;}
  public int? NuTamanho {get; set;}
  public int IdStoredProcedure {get; set;}
  public virtual StoredProcedure IdStoredProcedureNavigation {get; set;} = null!;

  public virtual ICollection<ComandoVariavel> OsComandosQueContemEssaVariavel {get; set;}
  public virtual ICollection<Endpoint> EndpointsQueRetornamEssaVariavel {get; set;}
  public virtual ICollection<Operando> Operandos {get; set;}
  public virtual ICollection<Endpoint> EndpointsQueContemEssaVariavelComoParametro {get; set;}

  protected bool Equals(Variavel other)
    => NoVariavel == other.NoVariavel && CoTipoDado == other.CoTipoDado && IdStoredProcedure == other.IdStoredProcedure;

  public override bool Equals(object? obj)
  {
    if (ReferenceEquals(null, obj))
    {
      return false;
    }
    if (ReferenceEquals(this, obj))
    {
      return true;
    }
    if (obj.GetType() != this.GetType())
    {
      return false;
    }
    return Equals((Variavel) obj);
  }

  public override int GetHashCode() => HashCode.Combine(NoVariavel, (int) CoTipoDado);

  public static bool operator ==(Variavel? left, Variavel? right) => Equals(left, right);
  public static bool operator !=(Variavel? left, Variavel? right) => !Equals(left, right);
}
