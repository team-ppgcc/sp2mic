namespace sp2mic.WebAPI.CrossCutting.Extensions;

[AttributeUsage(AttributeTargets.All)]
public class EnumInformationAttribute : Attribute
{
  public EnumInformationAttribute (int codigo, string nome)
  {
    Codigo = codigo;
    Nome = nome;
    PacoteImport = string.Empty;
    Descricao = string.Empty;
    DescricaoComplementar = string.Empty;
  }

  public EnumInformationAttribute (int codigo, string nome, string pacoteImport)
  {
    Codigo = codigo;
    Nome = nome;
    PacoteImport = pacoteImport;
    Descricao = string.Empty;
    DescricaoComplementar = string.Empty;
  }

  public EnumInformationAttribute (int codigo, string nome, string pacoteImport, string descricao)
  {
    Codigo = codigo;
    Nome = nome;
    PacoteImport = pacoteImport;
    Descricao = descricao;
    DescricaoComplementar = string.Empty;
  }

  public EnumInformationAttribute (int codigo, string nome, string pacoteImport, string descricao,
    string descricaoComplementar)
  {
    Codigo = codigo;
    Nome = nome;
    PacoteImport = pacoteImport;
    Descricao = descricao;
    DescricaoComplementar = descricaoComplementar;
  }

  public int Codigo {get; set;}
  public string Nome {get; set;}
  public string PacoteImport {get; set;}
  public string Descricao {get; set;}
  public string DescricaoComplementar {get; set;}
}
