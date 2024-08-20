namespace sp2mic.WebAPI.Domain.Entities;

public class Microsservico
{
  public Microsservico()
  {
    //DtoClasses = new HashSet<DtoClasse>();
    Endpoints = new HashSet<Endpoint>();
  }

  public int Id {get; set;}
  public string NoMicrosservico {get; set;} = null!;
  public bool SnProntoParaGerar {get; set;}

  //public virtual ICollection<DtoClasse> DtoClasses {get; set;}
  public virtual ICollection<Endpoint> Endpoints {get; set;}
}
