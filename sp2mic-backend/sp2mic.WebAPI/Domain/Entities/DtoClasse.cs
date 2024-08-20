namespace sp2mic.WebAPI.Domain.Entities;

public class DtoClasse
{
  public DtoClasse()
  {
    Atributos = new HashSet<Atributo>();
    EndpointsQueRetornamEssaClasse = new HashSet<Endpoint>();
    StoredProceduresQueRetornamEssaClasse = new HashSet<StoredProcedure>();
  }

  public DtoClasse (string noDtoClasse)
  {
    NoDtoClasse = noDtoClasse;
    Atributos = new HashSet<Atributo>();
    EndpointsQueRetornamEssaClasse = new HashSet<Endpoint>();
    StoredProceduresQueRetornamEssaClasse = new HashSet<StoredProcedure>();
  }

  public int Id {get; set;}
  public string NoDtoClasse {get; set;} = null!;
  //public int? IdMicrosservico {get; set;}
  //public virtual Microsservico? IdMicrosservicoNavigation {get; set;}
  public int IdStoredProcedure {get; set;}
  public virtual StoredProcedure IdStoredProcedureNavigation {get; set;} = null!;

  public virtual ICollection<Atributo> Atributos {get; set;}
  public virtual ICollection<Endpoint> EndpointsQueRetornamEssaClasse {get; set;}
  public virtual ICollection<StoredProcedure> StoredProceduresQueRetornamEssaClasse {get; set;}
}
