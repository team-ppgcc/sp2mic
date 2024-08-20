namespace sp2mic.WebAPI.Domain.Entities;

public class Tabela
{
  public Tabela()
  {
    EndpointsAssociados = new HashSet<Endpoint>();
    StoredProceduresAssociadas = new HashSet<StoredProcedure>();
  }

  public Tabela(string noTabela, StoredProcedure storedProcedure)
  {
    NoTabela = noTabela;
    StoredProceduresAssociadas.Add(storedProcedure);
  }

  public int Id {get; set;}
  public string NoTabela {get; set;} = null!;

  public virtual ICollection<Endpoint> EndpointsAssociados {get; set;}  = new HashSet<Endpoint>();
  public virtual ICollection<StoredProcedure> StoredProceduresAssociadas {get; set;} = new HashSet<StoredProcedure>();

  protected bool Equals(Tabela other) => NoTabela == other.NoTabela;

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
    return Equals((Tabela) obj);
  }

  public override int GetHashCode() => NoTabela.GetHashCode();

  public static bool operator ==(Tabela? left, Tabela? right) => Equals(left, right);
  public static bool operator !=(Tabela? left, Tabela? right) => !Equals(left, right);
}
