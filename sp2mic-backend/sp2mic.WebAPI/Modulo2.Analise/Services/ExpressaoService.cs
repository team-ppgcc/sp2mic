using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class ExpressaoService : IExpressaoService
{
  private readonly IUnitOfWork _uow;

  public ExpressaoService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public Expressao FindById(int id)
    => _uow.ExpressaoRepository.FindById(id) ??
      throw new BadHttpRequestException("Expression not found.");

  public async Task<Expressao> FindByIdAsync(int id)
    => await _uow.ExpressaoRepository.FindByIdAsync(id) ??
      throw new BadHttpRequestException("Expression not found.");

  public IEnumerable<Expressao> FindAll() => _uow.ExpressaoRepository.FindAll();

  public async Task<IEnumerable<Expressao>> FindAllAsync()
    => await _uow.ExpressaoRepository.FindAllAsync();

  public IEnumerable<Expressao> FindByFilter(ExpressaoFilterDto? filter)
    => filter is null ? FindAll() : _uow.ExpressaoRepository.FindByFilter(filter);

  public async Task<IEnumerable<Expressao>> FindByFilterAsync(ExpressaoFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.ExpressaoRepository.FindByFilterAsync(filter);

  public async Task<Expressao> AddAsync(Expressao? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Expression must be informed.");
    }
    _uow.ExpressaoRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, Expressao? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("Expression must be informed.");
    }
    var existente = await _uow.ExpressaoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Expression not found.");
    }
    if (_uow.ExpressaoRepository.JaExiste(existente))
    {
      throw new BadHttpRequestException("Already existing attribute.");
    }
    var atualizado = Sincronizar(atual, existente);
    _uow.ExpressaoRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static Expressao Sincronizar(Expressao atual, Expressao existente)
  {
    if (!Equals(atual.CoTipoDadoRetorno, existente.CoTipoDadoRetorno))
    {
      existente.CoTipoDadoRetorno = atual.CoTipoDadoRetorno;
    }
    if (!Equals(atual.NuOrdemExecucao, existente.NuOrdemExecucao))
    {
      existente.NuOrdemExecucao = atual.NuOrdemExecucao;
    }
    if (!Equals(atual.IdOperandoEsquerda, existente.IdOperandoEsquerda))
    {
      existente.IdOperandoEsquerda = atual.IdOperandoEsquerda;
    }
    if (!Equals(atual.CoOperador, existente.CoOperador))
    {
      existente.CoOperador = atual.CoOperador;
    }
    if (!Equals(atual.IdOperandoDireita, existente.IdOperandoDireita))
    {
      existente.IdOperandoDireita = atual.IdOperandoDireita;
    }
    return existente;
  }

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.ExpressaoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Expression not found.");
    }
    _uow.ExpressaoRepository.Delete(existente);
    await _uow.SaveAsync();
  }
}
