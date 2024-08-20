namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class ComboBoxDto
{
  public ComboBoxDto(int id, string nome)
  {
    Id = id;
    Nome = nome;
  }

  public int Id {get; set;}
  public string Nome {get; set;}
}
