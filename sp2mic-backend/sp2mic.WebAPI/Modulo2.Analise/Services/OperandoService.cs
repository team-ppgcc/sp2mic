using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class OperandoService : IOperandoService
{
  private readonly IUnitOfWork _uow;

  public OperandoService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public Operando FindById(int id)
    => _uow.OperandoRepository.FindById(id)
   ?? throw new BadHttpRequestException("Operating not found.");

  public async Task<Operando> FindByIdAsync(int id)
    => await _uow.OperandoRepository.FindByIdAsync(id)
   ?? throw new BadHttpRequestException("Operating not found.");

  public IEnumerable<Operando> FindAll() => _uow.OperandoRepository.FindAll();

  public async Task<IEnumerable<Operando>> FindAllAsync()
    => await _uow.OperandoRepository.FindAllAsync();

  public IEnumerable<Operando> FindByFilter(OperandoFilterDto? filter)
    => filter is null ? FindAll() : _uow.OperandoRepository.FindByFilter(filter);

  public async Task<IEnumerable<Operando>> FindByFilterAsync(OperandoFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.OperandoRepository.FindByFilterAsync(filter);

  public async Task<Operando> AddAsync(Operando? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Operating must be informed.");
    }
    _uow.OperandoRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, Operando? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("Operating must be informed.");
    }
    var existente = await _uow.OperandoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Operating not found.");
    }
    if (_uow.OperandoRepository.JaExiste(existente))
    {
      throw new BadHttpRequestException("Already existing attribute.");
    }
    var atualizado = Sincronizar(atual, existente);
    _uow.OperandoRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static Operando Sincronizar(Operando atual, Operando existente)
  {
    //var atualizado = new Operando();
    if (!Equals(atual.CoTipoOperando, existente.CoTipoOperando))
    {
      existente.CoTipoOperando = atual.CoTipoOperando;
    }
    if (!Equals(atual.TxValor, existente.TxValor))
    {
      existente.TxValor = atual.TxValor;
    }
    if (!Equals(atual.SnNegacao, existente.SnNegacao))
    {
      existente.SnNegacao = atual.SnNegacao;
    }
    if (!Equals(atual.IdVariavel, existente.IdVariavel))
    {
      existente.IdVariavel = atual.IdVariavel;
    }
    if (!Equals(atual.IdExpressao, existente.IdExpressao))
    {
      existente.IdExpressao = atual.IdExpressao;
    }
    if (!Equals(atual.IdEndpoint, existente.IdEndpoint))
    {
      existente.IdEndpoint = atual.IdEndpoint;
    }
    return existente;
  }

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.OperandoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Operating not found.");
    }
    _uow.OperandoRepository.Delete(existente);
    await _uow.SaveAsync();
  }

  // public List<Variavel> GetVariaveisByOperandoId(int idOperando)
  //   => _dbContext.OperandoVariaveis.AsNoTracking()
  //    .Include(c => c.IdVariavelNavigation)
  //    .Where(x => x.IdOperando == idOperando)
  //    .OrderBy(x => x.NuOrdem)
  //    .Select(x => x.IdVariavelNavigation)
  //    .ToList()!;
}
