using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class AtributoService : IAtributoService
{
  private readonly IUnitOfWork _uow;

  public AtributoService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public Atributo FindById(int id)
    => _uow.AtributoRepository.FindById(id)
   ?? throw new BadHttpRequestException("Attribute not found.");

  public async Task<Atributo> FindByIdAsync(int id)
    => await _uow.AtributoRepository.FindByIdAsync(id)
   ?? throw new BadHttpRequestException("Attribute not found.");

  public IEnumerable<Atributo> FindAll() => _uow.AtributoRepository.FindAll();

  public async Task<IEnumerable<Atributo>> FindAllAsync()
    => await _uow.AtributoRepository.FindAllAsync();

  public IEnumerable<Atributo> FindByFilter(AtributoFilterDto? filter)
    => filter is null ? FindAll() : _uow.AtributoRepository.FindByFilter(filter);

  public async Task<IEnumerable<Atributo>> FindByFilterAsync(AtributoFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.AtributoRepository.FindByFilterAsync(filter);

  public async Task<Atributo> AddAsync(Atributo? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Attribute must be informed.");
    }
    _uow.AtributoRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, Atributo? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("Attribute must be informed.");
    }
    var existente = await _uow.AtributoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Attribute not found.");
    }
    if (_uow.AtributoRepository.JaExiste(existente))
    {
      throw new BadHttpRequestException("Already existing attribute.");
    }
    var atualizado = Sincronizar(atual, existente);
    _uow.AtributoRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static Atributo Sincronizar(Atributo atual, Atributo existente)
  {
    //var atualizado = new Atributo();
    if (!Equals(atual.NoAtributo, existente.NoAtributo))
    {
      existente.NoAtributo = atual.NoAtributo;
    }
    if (!Equals(atual.CoTipoDado, existente.CoTipoDado))
    {
      existente.CoTipoDado = atual.CoTipoDado;
    }
    if (!Equals(atual.IdDtoClasse, existente.IdDtoClasse))
    {
      existente.IdDtoClasse = atual.IdDtoClasse;
    }
    return existente;
  }

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.AtributoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Attribute not found.");
    }
    _uow.AtributoRepository.Delete(existente);
    await _uow.SaveAsync();
  }

  public IEnumerable<Atributo> GetAtributosByIdDtoClasse(int idDtoClasse)
    => _uow.AtributoRepository.GetAtributosByIdDtoClasse(idDtoClasse)
   ?? throw new BadHttpRequestException("Attributes not found.");
}
