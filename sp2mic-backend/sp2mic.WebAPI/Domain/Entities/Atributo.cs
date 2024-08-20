using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Domain.Entities;

public class Atributo
{
  public Atributo() { }

  public Atributo(string noAtributo, TipoDadoEnum coTipoDado, int idDtoClasse)
  {
    NoAtributo = noAtributo;
    CoTipoDado = coTipoDado;
    IdDtoClasse = idDtoClasse;
  }

  public int Id {get; set;}
  public string NoAtributo {get; set;} = null!;
  public TipoDadoEnum CoTipoDado {get; set;}
  public int IdDtoClasse {get; set;}
  public virtual DtoClasse IdDtoClasseNavigation {get; set;} = null!;
}
