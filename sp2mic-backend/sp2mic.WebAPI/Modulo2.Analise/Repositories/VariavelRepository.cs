using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class VariavelRepository : IVariavelRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public VariavelRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public Variavel? FindById(int id) => _dbContext.Variaveis.Find(id);

  public async Task<Variavel?> FindByIdAsync(int id) => await _dbContext.Variaveis.FindAsync(id);

  public IEnumerable<Variavel> FindAll()
    => _dbContext.Variaveis
     // .Include(x => x.IdStoredProcedureNavigation)
     // .Include(x => x.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoVariavel)
     .ToHashSet();

  public async Task<IEnumerable<Variavel>> FindAllAsync()
    => await _dbContext.Variaveis
     // .Include(x => x.IdStoredProcedureNavigation)
     // .Include(x => x.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoVariavel)
     .ToListAsync();

  public IEnumerable<Variavel> FindByFilter(VariavelFilterDto filter)
    => _dbContext.Variaveis
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoVariavel == null ||
        x.NoVariavel.ToLower().Contains(filter.NoVariavel.ToLower()))
     .Where(x => filter.CoTipoDado == null || x.CoTipoDado == filter.CoTipoDado)
     .Where(x => filter.CoTipoDado == null || x.CoTipoDado == filter.CoTipoDado)
     .Where(x => filter.CoTipoEscopo == null || x.CoTipoEscopo == filter.CoTipoEscopo)
     .Where(x => filter.NuTamanho == null || x.NuTamanho == filter.NuTamanho)
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     // .Include(x => x.IdStoredProcedureNavigation)
     // .Include(x => x.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoVariavel)
     .ToHashSet();

  public async Task<IEnumerable<Variavel>> FindByFilterAsync(VariavelFilterDto filter)
    => await _dbContext.Variaveis
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoVariavel == null ||
        x.NoVariavel.ToLower().Contains(filter.NoVariavel.ToLower()))
     .Where(x => filter.CoTipoDado == null || x.CoTipoDado == filter.CoTipoDado)
     .Where(x => filter.CoTipoEscopo == null || x.CoTipoEscopo == filter.CoTipoEscopo)
     .Where(x => filter.NuTamanho == null || x.NuTamanho == filter.NuTamanho)
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     // .Include(x => x.IdStoredProcedureNavigation)
     // .Include(x => x.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoVariavel)
     .ToListAsync();

  public void Add(Variavel obj) => _dbContext.Variaveis.Add(obj);

  public void Update(Variavel obj) => _dbContext.Variaveis.Update(obj);

  public void Delete(Variavel obj) => _dbContext.Variaveis.Remove(obj);

  public void DeleteRange(IEnumerable<Variavel> range) => _dbContext.Variaveis.RemoveRange(range);

  public bool JaExiste(Variavel obj)
  {
    var commandText
      = Concat(
        $"SELECT * FROM {Schema}.\"Variavel\" WHERE  \"No_Variavel\" = \'{obj.NoVariavel}\'",
        $" and \"Id_StoredProcedure\" = {obj.IdStoredProcedure} and \"Id\" != {obj.Id}");
    var result = _dbContext.Variaveis.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }

  // public async Task<IEnumerable<Variavel>> FindByFilterAsync(VariavelFilterDto? filter)
  // {
  //   if (filter is null ||
  //     (filter.NoVariavel is null && filter.CoTipoDado is null && filter.CoTipoEscopo is null &&
  //       filter.NuTamanho is null
  //    && filter.IdStoredProcedure is null))
  //   {
  //     return await _dbContext.Variaveis
  //      .OrderBy(x => x.NoVariavel)
  //      .ToListAsync();
  //   }
  //
  //   return await _dbContext.Variaveis
  //    .Where(x => filter.NoVariavel == null ||
  //       x.NoVariavel.ToLower().Contains(filter.NoVariavel.ToLower()))
  //    .Where(x => filter.CoTipoDado == null ||
  //       x.CoTipoDado == filter.CoTipoDado)
  //    .Where(x => filter.CoTipoEscopo == null ||
  //       x.CoTipoEscopo == filter.CoTipoEscopo)
  //    .Where(x => filter.NuTamanho == null ||
  //       x.NuTamanho == filter.NuTamanho)
  //    .Where(x => filter.IdStoredProcedure == null ||
  //       x.IdStoredProcedure == filter.IdStoredProcedure)
  //     // .Where(x => filter.IdEndpoint == null ||
  //     //    x.IdEndpoint == filter.IdEndpoint)
  //    .OrderBy(x => x.NoVariavel)
  //    .ToListAsync();
  // }
  //
  // public async Task<Variavel?> FindByIdAsync(int id)
  // {
  //   return await _dbContext.Variaveis
  //    .SingleOrDefaultAsync(x => x.Id == id);
  // }
  //
  // public Variavel? FindById(int id) => _dbContext.Variaveis.Find(id);
  //
  // public void Add(Variavel obj) => _dbContext.Variaveis.Add(obj);
  //
  // public void Update(Variavel obj) => _dbContext.Variaveis.Update(obj);
  //
  // public void Delete(Variavel obj) => _dbContext.Variaveis.Remove(obj);
  //
  // public IEnumerable<Variavel> FindAll() => _dbContext.Variaveis.ToList();

  public void UpdateRange(IEnumerable<Variavel> obj) => _dbContext.Variaveis.UpdateRange(obj);

  public IEnumerable<Variavel?> FindByIdStoredProcedure(int idStoredProcedure)
    => _dbContext.Variaveis
     .Where(x => x.IdStoredProcedure == idStoredProcedure)
     .OrderBy(x => x.NoVariavel)
     .ToList();

  public IEnumerable<Variavel>? FindByIdProcedure(int idStoredProcedure)
    => _dbContext.Variaveis
     .Where(x => x.IdStoredProcedure == idStoredProcedure)
     .ToList();
}
