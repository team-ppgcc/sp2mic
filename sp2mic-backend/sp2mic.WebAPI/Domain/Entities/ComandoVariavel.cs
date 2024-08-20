namespace sp2mic.WebAPI.Domain.Entities;

public class ComandoVariavel
{
  public int Id {get; set;}
  public int IdComando {get; set;}
  public int IdVariavel {get; set;}
  public int NuOrdem {get; set;}

  public virtual Comando IdComandoNavigation {get; set;} = null!;
  public virtual Variavel IdVariavelNavigation {get; set;} = null!;
}
