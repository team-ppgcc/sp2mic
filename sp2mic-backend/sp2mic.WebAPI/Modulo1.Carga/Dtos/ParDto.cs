

namespace sp2mic.WebAPI.Modulo1.Carga.Dtos;


public class ParDto
{
  public ParDto (int id, string nome)
  {
    Id = id;
    Nome = nome;
  }

  public int Id {get; set;}
  public string Nome {get; set;}
  public bool? SnCarregada {get; set;}
}
