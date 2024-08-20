using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using static System.String;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class EndpointService : IEndpointService
{
  private readonly IUnitOfWork _uow;
  private readonly IVariavelService _variavelService;
  private readonly IDtoClasseService _dtoClasseService;

  public EndpointService(IUnitOfWork uow, IVariavelService variavelService,
    IDtoClasseService dtoClasseService)
  {
    _uow = uow ?? throw new ArgumentNullException(nameof (uow));
    _variavelService = variavelService ?? throw new ArgumentNullException(nameof (variavelService));
    _dtoClasseService
      = dtoClasseService ?? throw new ArgumentNullException(nameof (dtoClasseService));
  }

  public Endpoint FindById(int id)
  {
    var obj = _uow.EndpointRepository.FindById(id);
    return obj ?? throw new BadHttpRequestException("Endpoint not found.");
  }

  public async Task<Endpoint> FindByIdAsync(int id)
  {
    var obj = await _uow.EndpointRepository.FindByIdAsync(id);
    return obj ?? throw new BadHttpRequestException("Endpoint not found.");
  }

  public IEnumerable<Endpoint> FindAll() => _uow.EndpointRepository.FindAll();


  public async Task<IEnumerable<Endpoint>> FindAllAsync()
    => await _uow.EndpointRepository.FindAllAsync();

  public IEnumerable<Endpoint> FindByFilter(EndpointFilterDto? filter)
    => filter is null ? FindAll() : _uow.EndpointRepository.FindByFilter(filter);

  public async Task<IEnumerable<Endpoint>> FindByFilterAsync(EndpointFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.EndpointRepository.FindByFilterAsync(filter);

  public async Task<Endpoint> AddAsync(Endpoint? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Endpoint must be informed.");
    }
    _uow.EndpointRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, Endpoint? novo)
  {
    if (novo is null)
    {
      throw new BadHttpRequestException("Endpoint must be informed.");
    }
    var existente = await _uow.EndpointRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Endpoint not found.");
    }
    var atualizado = Sincronizar(novo, existente);
    if (_uow.EndpointRepository.JaExiste(atualizado))
    {
      throw new BadHttpRequestException(
        "Method already existing in this microservice. Please choose another name.");
    }
    AjustarEntrada(atualizado);
    _uow.EndpointRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static void AjustarEntrada(Endpoint obj)
  {
    obj.NoMetodoEndpoint = IsNullOrEmpty(obj.NoMetodoEndpoint) ? null : obj.NoMetodoEndpoint.TrimAndRemoveWhiteSpaces();
    obj.NoPath = IsNullOrEmpty(obj.NoPath) ? null : obj.NoPath.TrimAndRemoveWhiteSpaces();
    if (!IsNullOrEmpty(obj.NoPath) && !obj.NoPath[..1].Equals("/"))
    {
      obj.NoPath = Concat("/", obj.NoPath);
    }
  }

  private Endpoint Sincronizar(Endpoint novoEndpoint, Endpoint existente)
  {
    if (!Equals(novoEndpoint.NoMetodoEndpoint, existente.NoMetodoEndpoint))
    {
      existente.NoMetodoEndpoint = novoEndpoint.NoMetodoEndpoint;
    }
    if (!Equals(novoEndpoint.NoPath, existente.NoPath))
    {
      existente.NoPath = novoEndpoint.NoPath;
    }
    if (!Equals(novoEndpoint.TxEndpointTratado, existente.TxEndpointTratado))
    {
      existente.TxEndpointTratado = novoEndpoint.TxEndpointTratado;
    }
    if (!Equals(novoEndpoint.IdDtoClasse, existente.IdDtoClasse))
    {
      existente.IdDtoClasse = novoEndpoint.IdDtoClasse;
    //   if (novo.IdDtoClasse is not null)
    //   {
    //     var classeNova
    //       = RecuperarDtoClasse(novo.IdDtoClasse
    //        .Value); //_classeDTOService.FindById(dto.IdDtoClasse);
    //     classeNova.IdMicrosservico = novo.IdMicrosservico;
    //     existente.IdDtoClasseNavigation = classeNova;
    //   }
    // }
    // if (novo.IdDtoClasseNavigation?.Id is not null &&
    //   novo.IdDtoClasseNavigation.Id != novo.IdDtoClasse)
    // {
    //   var enpointsQueUtilizamClasseAnterior
    //     = FindByIdDtoClasse(novo.IdDtoClasseNavigation.Id).ToHashSet();
    //   if (enpointsQueUtilizamClasseAnterior.Any() &&
    //     enpointsQueUtilizamClasseAnterior.Count == 1)
    //   {
    //     var classeAnterior = RecuperarDtoClasse(novo.IdDtoClasseNavigation.Id);
    //     classeAnterior.IdMicrosservico = null;
    //     // retira a associação da classe anterior com esse microsserviço já que nenhum outro endpoint desse microsserviço a utiliza
    //     _dtoClasseService.UpdateAsync(classeAnterior.Id, classeAnterior);
    //   }
    }
    if (!Equals(novoEndpoint.CoTipoDadoRetorno, existente.CoTipoDadoRetorno))
    {
      existente.CoTipoDadoRetorno = novoEndpoint.CoTipoDadoRetorno;
      if (novoEndpoint.CoTipoDadoRetorno != TipoDadoEnum.DTO)
      {
        existente.IdDtoClasse = null;
        existente.IdDtoClasseNavigation = null;
      }
    }
    if (!Equals(novoEndpoint.SnRetornoLista, existente.SnRetornoLista))
    {
      existente.SnRetornoLista = novoEndpoint.SnRetornoLista;
    }
    if (!Equals(novoEndpoint.SnAnalisado, existente.SnAnalisado))
    {
      existente.SnAnalisado = novoEndpoint.SnAnalisado;
    }
    if (!Equals(novoEndpoint.IdMicrosservico, existente.IdMicrosservico))
    {
      existente.IdMicrosservico = novoEndpoint.IdMicrosservico;
    }
    // if (Equals(novo.IdVariavelRetornada, existente.IdVariavelRetornada))
    // {
    //   return existente;
    // }
    // existente.IdVariavelRetornada = novo.IdVariavelRetornada;
    // existente.IdVariavelRetornadaNavigation = novo.IdVariavelRetornada is null ? null :
    //   RecuperarVariavel(novo.IdVariavelRetornada.Value);
    return existente;
  }

  private Variavel? RecuperarVariavel(int idVariavel) => _variavelService.FindById(idVariavel);

  private DtoClasse RecuperarDtoClasse(int idDtoClasse) => _dtoClasseService.FindById(idDtoClasse);

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.EndpointRepository.FindByIdAsync(id) ??
      throw new BadHttpRequestException("Endpoint not found.");
    _uow.EndpointRepository.Delete(existente);
    await _uow.SaveAsync();
  }

  private IEnumerable<Endpoint> FindByIdDtoClasse(int? idDtoClasse)
    => idDtoClasse is null ? new HashSet<Endpoint>() :
      _uow.EndpointRepository.FindByIdDtoClasse(idDtoClasse);

  public IEnumerable<Endpoint> FindByIdMicrosservico(int idMicrosservico)
    => _uow.EndpointRepository.FindByIdMicrosservico(idMicrosservico);

  public IEnumerable<Endpoint> FindByIdStoredProcedure(int idStoredProcedure)
    => _uow.EndpointRepository.FindByIdStoredProcedure(idStoredProcedure);

  public void TratarTextoEndpoints()
  {
    var todosOsEndpoints = _uow.EndpointRepository.FindAll().ToList();
    foreach (var ep in todosOsEndpoints.Where(ep => !IsNullOrEmpty(ep.TxEndpoint)))
    {
      ep.TxEndpointTratado = ep.TxEndpoint.TratarEndpoint();
    }
    _uow.EndpointRepository.UpdateRange(todosOsEndpoints);
    _uow.SaveAsync();
  }

  public void AjustarRetornoEndpoints() => _uow.EndpointRepository.AjustarRetornoEndpoints();

  public void AjustarNomesEndpoints() => _uow.EndpointRepository.AjustarNomesEndpoints();

  public void AjustarPathsEndpoints() => _uow.EndpointRepository.AjustarPathsEndpoints();
}
