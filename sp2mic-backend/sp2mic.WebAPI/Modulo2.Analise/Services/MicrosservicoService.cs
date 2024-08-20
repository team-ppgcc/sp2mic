using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class MicrosservicoService : IMicrosservicoService
{
  private readonly IUnitOfWork _uow;

  public MicrosservicoService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public Microsservico FindById(int id)
    => _uow.MicrosservicoRepository.FindById(id) ??
      throw new BadHttpRequestException("Microservice not found.");

  public async Task<Microsservico> FindByIdAsync(int id)
    => await _uow.MicrosservicoRepository.FindByIdAsync(id) ??
      throw new BadHttpRequestException("Microservice not found.");

  public IEnumerable<Microsservico> FindAll() => _uow.MicrosservicoRepository.FindAll();

  public IEnumerable<ComboBoxDto> FindAllForCombo()
    => _uow.MicrosservicoRepository.FindAll().Select(e => new ComboBoxDto(e.Id, e.NoMicrosservico));

  public async Task<IEnumerable<Microsservico>> FindAllAsync()
    => await _uow.MicrosservicoRepository.FindAllAsync();

  public IEnumerable<Microsservico> FindByFilter(MicrosservicoFilterDto? filter)
    => filter is null ? FindAll() : _uow.MicrosservicoRepository.FindByFilter(filter);

  public async Task<IEnumerable<Microsservico>> FindByFilterAsync(MicrosservicoFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.MicrosservicoRepository.FindByFilterAsync(filter);

  public async Task<Microsservico> AddAsync(Microsservico? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Microservice must be informed.");
    }
    _uow.MicrosservicoRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, Microsservico? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("Microservice must be informed.");
    }
    var existente = await _uow.MicrosservicoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Microservice not found.");
    }
    if (_uow.MicrosservicoRepository.JaExiste(existente))
    {
      throw new BadHttpRequestException("Already existing attribute.");
    }
    var atualizado = Sincronizar(atual, existente);
    _uow.MicrosservicoRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static Microsservico Sincronizar(Microsservico atual, Microsservico existente)
  {
    //var atualizado = new Microsservico();
    if (!Equals(atual.NoMicrosservico, existente.NoMicrosservico))
    {
      existente.NoMicrosservico = atual.NoMicrosservico.TrimAndRemoveWhiteSpaces();
    }
    if (!Equals(atual.SnProntoParaGerar, existente.SnProntoParaGerar))
    {
      existente.SnProntoParaGerar = atual.SnProntoParaGerar;
    }
    return existente;
  }

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.MicrosservicoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Microservice not found.");
    }
    _uow.MicrosservicoRepository.Delete(existente);
    await _uow.SaveAsync();
  }

  public IEnumerable<Microsservico> FindProntosParaGerar()
     => _uow.MicrosservicoRepository.FindProntosParaGerar();
}
// private void CompletarObjeto(List<Microsservico> microsservicos)
// {
//   foreach (var mic in microsservicos)
//   {
//     mic.DtoClasses = _classeDTOService.FindByMicrosservico(mic.Id); // ok
//     mic.Endpoints = _endPointService.FindByMicrosservico(mic.Id);
//   }
// }
//
