using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class VariavelAddUpdateDto
{
  public string NoVariavel {get; set;} = null!;
  public TipoDadoEnum CoTipoDado {get; set;}
  public TipoEscopoEnum CoTipoEscopo {get; set;}
  public int NuTamanho {get; set;}
  public int? IdStoredProcedure {get; set;}
  public int? IdEndpoint {get; set;}

  // public VariavelDto (Variavel? obj)
  // {
  //   if (obj is null)
  //   {
  //     return;
  //   }
  //
  //   Id = obj.Id;
  //   NoVariavel = obj.NoVariavel;
  //   CoTipoDado = obj.CoTipoDado;
  //   CoTipoEscopo = obj.CoTipoEscopo;
  //   NuTamanho = obj.NuTamanho;
  //   IdStoredProcedure = obj.IdStoredProcedure;
  //   IdEndpoint = obj.IdEndpoint;
  // }
}
