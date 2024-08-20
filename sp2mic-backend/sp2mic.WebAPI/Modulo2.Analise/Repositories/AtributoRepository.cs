using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class AtributoRepository : IAtributoRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public AtributoRepository(DbContextSp2Mic dbContext)
    => _dbContext = dbContext;

  public Atributo? FindById(int id) => _dbContext.Atributos
   // .Include(x => x.IdDtoClasseNavigation)
   // .ThenInclude(c => c.IdMicrosservicoNavigation)
   .SingleOrDefault(x => x.Id == id);

  public async Task<Atributo?> FindByIdAsync(int id)
    => await _dbContext.Atributos
     // .Include(x => x.IdDtoClasseNavigation)
     // .ThenInclude(c => c.IdMicrosservicoNavigation)
     .SingleOrDefaultAsync(x => x.Id == id);

  public IEnumerable<Atributo> FindAll()
    => _dbContext.Atributos
     // .Include(x => x.IdDtoClasseNavigation)
     // .ThenInclude(c => c.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoAtributo)
     .ToHashSet();

  public async Task<IEnumerable<Atributo>> FindAllAsync()
    => await _dbContext.Atributos
     // .Include(x => x.IdDtoClasseNavigation)
     // .ThenInclude(c => c.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoAtributo)
     .ToListAsync();

  public IEnumerable<Atributo> FindByFilter(AtributoFilterDto filter)
    => _dbContext.Atributos
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoAtributo == null ||
        x.NoAtributo.ToLower().Contains(filter.NoAtributo.ToLower()))
     .Where(x => filter.CoTipoDado == null || x.CoTipoDado == filter.CoTipoDado)
     .Where(x => filter.IdDtoClasse == null || x.IdDtoClasse == filter.IdDtoClasse)
     // .Include(x => x.IdDtoClasseNavigation)
     // .ThenInclude(c => c.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoAtributo)
     .ToHashSet();

  public async Task<IEnumerable<Atributo>> FindByFilterAsync(AtributoFilterDto filter)
    => await _dbContext.Atributos
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoAtributo == null ||
        x.NoAtributo.ToLower().Contains(filter.NoAtributo.ToLower()))
     .Where(x => filter.CoTipoDado == null || x.CoTipoDado == filter.CoTipoDado)
     .Where(x => filter.IdDtoClasse == null || x.IdDtoClasse == filter.IdDtoClasse)
     // .Include(x => x.IdDtoClasseNavigation)
     // .ThenInclude(c => c.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoAtributo)
     .ToListAsync();

  public void Add(Atributo obj) => _dbContext.Atributos.Add(obj);

  public void Update(Atributo obj) => _dbContext.Atributos.Update(obj);

  public void Delete(Atributo obj) => _dbContext.Atributos.Remove(obj);

  public bool JaExiste(Atributo obj)
  {
    var commandText
      = Concat(
        $"SELECT * FROM {Schema}.\"Atributo\" WHERE  \"No_Atributo\" = \'{obj.NoAtributo}\'",
        $" and \"Id_DtoClasse\" = {obj.IdDtoClasse} and \"Id\" != {obj.Id}");
    var result = _dbContext.Atributos.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }

  public IEnumerable<Atributo> GetAtributosByIdDtoClasse(int idDtoClasse)
    => _dbContext.Atributos
     .Where(x => x.IdDtoClasse == idDtoClasse)
     // .Include(x => x.IdDtoClasseNavigation)
     // .ThenInclude(c => c.IdMicrosservicoNavigation)
     .OrderBy(x => x.NoAtributo).ToHashSet();
}
