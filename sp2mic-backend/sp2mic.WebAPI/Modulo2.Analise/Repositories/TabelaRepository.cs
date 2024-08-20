using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class TabelaRepository : ITabelaRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public TabelaRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public Tabela? FindById(int id)
    => _dbContext.Tabelas
     // .Include(x => x.StoredProceduresAssociadas)
     // .Include(x => x.EndpointsAssociados)
     .SingleOrDefault(x => x.Id == id);

  public async Task<Tabela?> FindByIdAsync(int id)
    => await _dbContext.Tabelas
     // .Include(x => x.StoredProceduresAssociadas)
     // .Include(x => x.EndpointsAssociados)
     .SingleOrDefaultAsync(x => x.Id == id);

  public IEnumerable<Tabela> FindAll()
    => _dbContext.Tabelas
     .OrderBy(x => x.NoTabela)
     .ToHashSet();

  public async Task<IEnumerable<Tabela>> FindAllAsync()
    => await _dbContext.Tabelas
     .OrderBy(x => x.NoTabela)
     .ToListAsync();

  public IEnumerable<Tabela> FindByFilter(TabelaFilterDto filter)
    => _dbContext.Tabelas
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoTabela == null ||
        x.NoTabela.ToLower().Contains(filter.NoTabela.ToLower()))
     .OrderBy(x => x.NoTabela)
     .ToHashSet();

  public async Task<IEnumerable<Tabela>> FindByFilterAsync(TabelaFilterDto filter)
    => await _dbContext.Tabelas
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoTabela == null ||
        x.NoTabela.ToLower().Contains(filter.NoTabela.ToLower()))
     .OrderBy(x => x.NoTabela)
     .ToListAsync();

  public void Add(Tabela obj) => _dbContext.Tabelas.Add(obj);

  public void Update(Tabela obj) => _dbContext.Tabelas.Update(obj);

  public void Delete(Tabela obj) => _dbContext.Tabelas.Remove(obj);

  public void DeleteRange(IEnumerable<Tabela> range) => _dbContext.Tabelas.RemoveRange(range);

  public bool JaExiste(Tabela obj)
  {
    var commandText
      = Concat($"SELECT * FROM {Schema}.\"Tabela\" WHERE  \"No_Tabela\" = \'{obj.NoTabela}\'",
        $" and \"Id\" != {obj.Id}");
    var result = _dbContext.Tabelas.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }

  public Tabela? FindByName(string noTabela)
    => _dbContext.Tabelas
     .SingleOrDefault(x => x.NoTabela == noTabela);
}
