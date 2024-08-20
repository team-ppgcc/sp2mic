using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo2.Analise.Dtos;

public class StoredProcedureDto
{
  public int? Id {get; set;}
  public string? NoSchema {get; set;}
  public string? NoStoredProcedure {get; set;}
  public bool? SnRetornoLista {get; set;}
  public bool? SnAnalisada {get; set;}
  public bool? SnSucessoParser {get; set;}
  public TipoDadoEnum? CoTipoDadoRetorno {get; set;}
  public int? IdDtoClasse {get; set;}

  public string? TxResultadoParser {get; set;} = null!;
  public string? TabelasAssociadas {get; set;}

  public ICollection<EndpointListagemDto>? Endpoints {get; set;} = new List<EndpointListagemDto>();


  // public StoredProcedureDto (StoredProcedure? obj)
  // {
  //   if (obj is null)
  //   {
  //     return;
  //   }
  //
  //   Id = obj.Id;
  //   NoStoredProcedure = obj.NoStoredProcedure;
  //   NoSchema = obj.NoSchema;
  //   TxDefinicao = obj.TxDefinicao;
  //   CoTipoDadoRetorno = obj.CoTipoDadoRetorno;
  //   SnRetornoLista = obj.SnRetornoLista;
  //   SnAnalisada = obj.SnAnalisada;
  //   IdClasseRetornada = obj.IdDtoClasse;
  //   TxDefinicaoTratada = obj.TxDefinicaoTratada;
  //   IdDtoClasseNavigation = new DtoClasseDto(obj.IdDtoClasseNavigation);
  //   Endpoints = obj.Endpoints?.Select(e => new EndpointDto(e)).ToList();
  // }
}
