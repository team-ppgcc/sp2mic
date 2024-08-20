using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class TabelaService : ITabelaService
{
  private readonly IUnitOfWork _uow;

  public TabelaService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public Tabela FindById(int id)
    => _uow.TabelaRepository.FindById(id)
   ?? throw new BadHttpRequestException("Table not found.");

  public async Task<Tabela> FindByIdAsync(int id)
    => await _uow.TabelaRepository.FindByIdAsync(id)
   ?? throw new BadHttpRequestException("Table not found.");

  public ISet<Tabela> FindAll()
    => _uow.TabelaRepository.FindAll().ToHashSet();

  public async Task<IEnumerable<Tabela>> FindAllAsync()
    => await _uow.TabelaRepository.FindAllAsync();

  public IEnumerable<Tabela> FindByFilter(TabelaFilterDto? filter)
    => filter is null ? FindAll() : _uow.TabelaRepository.FindByFilter(filter);

  public async Task<IEnumerable<Tabela>> FindByFilterAsync(TabelaFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.TabelaRepository.FindByFilterAsync(filter);

  public async Task<Tabela> AddAsync(Tabela? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Table must be informed.");
    }
    _uow.TabelaRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, Tabela? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("Table must be informed.");
    }
    var existente = await _uow.TabelaRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Table not found.");
    }
    if (_uow.TabelaRepository.JaExiste(existente))
    {
      throw new BadHttpRequestException("Already existing attribute.");
    }
    var atualizado = Sincronizar(atual, existente);
    _uow.TabelaRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static Tabela Sincronizar(Tabela atual, Tabela existente)
  {
    //var atualizado = new Tabela();
    if (!Equals(atual.NoTabela, existente.NoTabela))
    {
      existente.NoTabela = atual.NoTabela;
    }
    return existente;
  }

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.TabelaRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Table not found.");
    }
    _uow.TabelaRepository.Delete(existente);
    await _uow.SaveAsync();
  }

  public Tabela? FindByName(string noTabela)

    => _uow.TabelaRepository.FindByName(noTabela)
      ?? null;

}
