using DotLiquid.Util;
using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class DtoClasseRepository : IDtoClasseRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public DtoClasseRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public DtoClasse? FindById(int id)
    => _dbContext.DtoClasses
     .Include(x => x.Atributos)
     .SingleOrDefault(x => x.Id == id);

  public async Task<DtoClasse?> FindByIdAsync(int id)
    => await _dbContext.DtoClasses
     //.Include(x => x.IdMicrosservicoNavigation) Classe não tem mais Microsservico
     .Include(x => x.IdStoredProcedureNavigation)
     .Include(x => x.Atributos)
     .SingleOrDefaultAsync(x => x.Id == id);

  public IEnumerable<DtoClasse> FindAll()
    => _dbContext.DtoClasses
     //.Include(x => x.IdMicrosservicoNavigation) Classe não tem mais Microsservico
     .Include(x => x.IdStoredProcedureNavigation)
     .Include(x => x.Atributos)
     .OrderBy(x => x.NoDtoClasse).ToHashSet();

  public IEnumerable<DtoClasse> FindByIdProcedure(int idStoredProcedure)
    => _dbContext.DtoClasses
     .Where(x=>x.IdStoredProcedure == idStoredProcedure)
     .OrderBy(x => x.NoDtoClasse).ToHashSet();

  public async Task<IEnumerable<DtoClasse>> FindAllAsync()
    => await _dbContext.DtoClasses
     .Include(x => x.Atributos)
     //.Include(x => x.IdMicrosservicoNavigation) classe nao tem mais mic
     .Include(x => x.IdStoredProcedureNavigation)
     .OrderBy(x => x.NoDtoClasse)
     .ToListAsync();

  public IEnumerable<DtoClasse> FindByFilter(DtoClasseFilterDto filter)
    => _dbContext.DtoClasses.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoDtoClasse == null ||
        x.NoDtoClasse.ToLower().Contains(filter.NoDtoClasse.ToLower()))
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     //.Where(x => filter.IdMicrosservico == null || x.IdMicrosservico == filter.IdMicrosservico)
     //.Include(x => x.IdMicrosservicoNavigation) classe nao tem mais mic
     .Include(x => x.IdStoredProcedureNavigation)
      // .Include(x => x.Atributos)
     .OrderBy(x => x.NoDtoClasse).ToHashSet();

  public async Task<IEnumerable<DtoClasse>> FindByFilterAsync(DtoClasseFilterDto filter)
    => await _dbContext.DtoClasses.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoDtoClasse == null ||
        x.NoDtoClasse.ToLower().Contains(filter.NoDtoClasse.ToLower()))
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     //.Where(x => filter.IdMicrosservico == null || x.IdMicrosservico == filter.IdMicrosservico)
     //.Include(x => x.IdMicrosservicoNavigation) classe nao tem mais mic
     .Include(x => x.IdStoredProcedureNavigation)
      .Include(x => x.Atributos)
     .OrderBy(x => x.NoDtoClasse)
     .ToListAsync();

  public void Add(DtoClasse obj) => _dbContext.DtoClasses.Add(obj);

  public void Update(DtoClasse obj) => _dbContext.DtoClasses.Update(obj);

  public void Delete(DtoClasse obj) => _dbContext.DtoClasses.Remove(obj);

  public void DeleteRange(IEnumerable<DtoClasse> range) => _dbContext.DtoClasses.RemoveRange(range);

  // public bool JaExiste(DtoClasse obj)
  // {
  //
  //   var commandText
  //     = Concat(
  //       $"SELECT * FROM {Schema}.\"DtoClasse\" WHERE  \"No_DtoClasse\" = \'{obj.NoDtoClasse}\'",
  //       $" and \"Id_Microsservico\" = {obj.IdMicrosservico} and \"Id\" != {obj.Id}");
  //   var result = _dbContext.DtoClasses.FromSqlRaw(commandText).ToHashSet();
  //   return result.Count == 1;
  // }

  // public IEnumerable<DtoClasse> FindByIdMicrosservico(int idMicrosservico)
  // {
  //   return _dbContext.DtoClasses
  //    .Where(x => x.IdMicrosservico == idMicrosservico)
  //    .OrderBy(x => x.NoDtoClasse).ToHashSet();
  // }

  public string? GetNoDtoClasseById(int idDtoClasse)
    => _dbContext.DtoClasses.FirstOrDefault(x => x.Id == idDtoClasse)?.NoDtoClasse;

  public async Task<IEnumerable<DtoClasse>> FindByFilterAsync(DtoClasseDto filter)
    => await _dbContext.DtoClasses.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoDtoClasse == null ||
        x.NoDtoClasse.ToLower().Contains(filter.NoDtoClasse.ToLower()))
     //.Where(x => filter.IdMicrosservico == null || x.IdMicrosservico == filter.IdMicrosservico) Classe não tem mais Microsservico
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     //.Include(x => x.IdMicrosservicoNavigation) Classe não tem mais Microsservico
     .Include(x => x.IdStoredProcedureNavigation)
     .OrderBy(x => x.NoDtoClasse)
     .ToListAsync();

  public IEnumerable<DtoClasse> RecuperarClassesDeUmMicrosservico(int idMicrosservico)
  {
    var query = $"SELECT * FROM {Schema}.\"DtoClasse\" " +
                $"WHERE \"Id\" IN (SELECT DISTINCT \"Id_DtoClasse\" FROM {Schema}.\"Endpoint\" " +
                $"WHERE \"Id_DtoClasse\" IS NOT NULL AND " +
                $"\"Id_Microsservico\" = {idMicrosservico}) ";
    return _dbContext.DtoClasses.FromSqlRaw(query).ToList();
  }

  public IEnumerable<DtoClasse> RecuperarClassesDeUmaListaDeMicrosservicos(ISet<int> idsMicrosservicos)
  {
    var idsString = idsMicrosservicos.Select(i => i.ToString()).Aggregate((a, b) => a + ", " + b);
    var query = $"SELECT * FROM {Schema}.\"DtoClasse\" " +
      $"WHERE \"Id\" IN (SELECT DISTINCT \"Id_DtoClasse\" FROM {Schema}.\"Endpoint\" " +
      $"WHERE \"Id_DtoClasse\" IS NOT NULL AND " +
      $"\"Id_Microsservico\" in ({idsString})) ";
    return _dbContext.DtoClasses.FromSqlRaw(query).ToList();;
  }

  public void AjustarNomesClasses()
  {
    const string query =
      $"UPDATE {Schema}.\"DtoClasse\" SET \"No_DtoClasse\" = " +
      $"concat(\"No_DtoClasse\", cast(\"Id\" as varchar));";
    _dbContext.Database.ExecuteSqlRaw(query);
  }
}
