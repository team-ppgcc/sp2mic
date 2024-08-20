namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class MicrosservicoDto
{
  public int? Id {get; set;}
  public string? NoMicrosservico {get; set;}
  public bool? SnProntoParaGerar {get; set;}
  public int? QtdEndpoints {get; set;}

  public ICollection<EndpointListagemDto>? Endpoints {get; set;}

  public MicrosservicoDto () { }

  // public MicrosservicoDto (Microsservico? obj)
  // {
  //   if (obj is null)
  //   {
  //     return;
  //   }
  //
  //   Id = obj.Id;
  //   NoMicrosservico = obj.NoMicrosservico;
  //   NoPacote = obj.NoPacote;
  //   NoUrlBase = obj.NoUrlBase;
  //   CoNumeroPorta = obj.CoNumeroPorta;
  //   SnGerado = obj.SnGerado;
  // }
}
