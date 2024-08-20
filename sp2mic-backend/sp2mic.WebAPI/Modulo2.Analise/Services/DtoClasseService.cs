
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class DtoClasseService : IDtoClasseService
{
  private readonly IUnitOfWork _uow;

  public DtoClasseService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public DtoClasse FindById(int id)
  {
    var obj = _uow.DtoClasseRepository.FindById(id);
    return obj ?? throw new BadHttpRequestException("DTO Class not found.");
  }

  public async Task<DtoClasse> FindByIdAsync(int id)
  {
    var obj = await _uow.DtoClasseRepository.FindByIdAsync(id);
    return obj ?? throw new BadHttpRequestException("DTO Class not found.");
  }

  public IEnumerable<DtoClasse> FindAll() => _uow.DtoClasseRepository.FindAll();

  public IEnumerable<ComboBoxDto> FindByIdProcedureForCombo(int idStoredProcedure)
  {
    var list = _uow.DtoClasseRepository.FindByIdProcedure(idStoredProcedure);
    return list != null ?
      list.Select(e => new ComboBoxDto(e.Id, e.NoDtoClasse)) :
      new List<ComboBoxDto>();
  }

  public async Task<IEnumerable<DtoClasse>> FindAllAsync()
    => await _uow.DtoClasseRepository.FindAllAsync();

  public IEnumerable<DtoClasse> FindByFilter(DtoClasseFilterDto? filter)
    => filter is null ? FindAll() : _uow.DtoClasseRepository.FindByFilter(filter);

  public async Task<IEnumerable<DtoClasse>> FindByFilterAsync(DtoClasseFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.DtoClasseRepository.FindByFilterAsync(filter);

  public async Task<DtoClasse> AddAsync(DtoClasse? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("DTO Class must be informed.");
    }
    _uow.DtoClasseRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, DtoClasse? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("DTO Class must be informed.");
    }
    var existente = await _uow.DtoClasseRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("DTO Class not found.");
    }
    // classe nao tem microsservico
    // if (existente.IdMicrosservico != null && _uow.DtoClasseRepository.JaExiste(existente))
    // {
    //   throw new BadHttpRequestException(
    //     "DTO Class already exists in this microservice/stored procedure. Please choose another name.");
    // }
    var atualizado = Sincronizar(atual, existente);
    _uow.DtoClasseRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static DtoClasse Sincronizar(DtoClasse atual, DtoClasse existente)
  {
    //var atualizado = new DtoClasse();
    if (!Equals(atual.NoDtoClasse, existente.NoDtoClasse))
    {
      existente.NoDtoClasse = atual.NoDtoClasse;
    }
    //não tem mais mic na classe
    // if (!Equals(atual.IdMicrosservico, existente.IdMicrosservico))
    // {
    //   existente.IdMicrosservico = atual.IdMicrosservico;
    // }
    return existente;
  }

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.DtoClasseRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("DTO Class not found.");
    }
    _uow.DtoClasseRepository.Delete(existente);
    await _uow.SaveAsync();
  }

  // public IEnumerable<DtoClasse> FindByIdMicrosservico(int idMicrosservico)
  // {
  //   var objs = _uow.DtoClasseRepository.FindByIdMicrosservico(idMicrosservico).ToHashSet();
  //   if (!objs.Any())
  //   {
  //     throw new BadHttpRequestException("DTO Class not found.");
  //   }
  //   return objs;
  // }

  public string? GetNoDtoClasseById(int idDtoClasse)
    => _uow.DtoClasseRepository.GetNoDtoClasseById(idDtoClasse);

  public IEnumerable<DtoClasse> RecuperarClassesDeUmMicrosservico(int idMicrosservico)
    => _uow.DtoClasseRepository.RecuperarClassesDeUmMicrosservico(idMicrosservico);

  public IEnumerable<DtoClasse> RecuperarClassesDeUmaListaDeMicrosservicos(ISet<int> idsMicrosservicos)
    => _uow.DtoClasseRepository.RecuperarClassesDeUmaListaDeMicrosservicos(idsMicrosservicos);


  public void AjustarNomesClasses()
  {
    _uow.DtoClasseRepository.AjustarNomesClasses();
  }
}
