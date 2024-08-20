using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class ExpressaoRepository : IExpressaoRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public ExpressaoRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public Expressao? FindById(int id)
    => _dbContext.Expressoes
      // .Include(x => x.IdOperandoEsquerdaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdVariavelNavigation)
      // .Include(x => x.IdOperandoEsquerdaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdEndpointNavigation)
      // .Include(x => x.IdOperandoDireitaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdVariavelNavigation)
      // .Include(x => x.IdOperandoDireitaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdEndpointNavigation)
     .SingleOrDefault(x => x.Id == id);

  public async Task<Expressao?> FindByIdAsync(int id)
    => await _dbContext.Expressoes
      // .Include(x => x.IdOperandoEsquerdaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdVariavelNavigation)
      // .Include(x => x.IdOperandoEsquerdaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdEndpointNavigation)
      // .Include(x => x.IdOperandoDireitaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdVariavelNavigation)
      // .Include(x => x.IdOperandoDireitaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdEndpointNavigation)
     .SingleOrDefaultAsync(x => x.Id == id);

  public IEnumerable<Expressao> FindAll()
    => _dbContext.Expressoes.OrderBy(x => x.NuOrdemExecucao)
      // .Include(x => x.IdOperandoEsquerdaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdVariavelNavigation)
      // .Include(x => x.IdOperandoEsquerdaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdEndpointNavigation)
      // .Include(x => x.IdOperandoDireitaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdVariavelNavigation)
      // .Include(x => x.IdOperandoDireitaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdEndpointNavigation)
     .ToHashSet();

  public async Task<IEnumerable<Expressao>> FindAllAsync()
    => await _dbContext.Expressoes
      // .Include(x => x.IdOperandoEsquerdaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdVariavelNavigation)
      // .Include(x => x.IdOperandoEsquerdaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdEndpointNavigation)
      // .Include(x => x.IdOperandoDireitaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdVariavelNavigation)
      // .Include(x => x.IdOperandoDireitaNavigation)
      // .ThenInclude(o => o == null ? null : o.IdEndpointNavigation)
     .OrderBy(x => x.NuOrdemExecucao).ToListAsync();

  public IEnumerable<Expressao> FindByFilter(ExpressaoFilterDto filter)
    => _dbContext.Expressoes.Where(x => filter.Id == null || x.Id == filter.Id)
     .OrderBy(x => x.NuOrdemExecucao).ToHashSet();

  public async Task<IEnumerable<Expressao>> FindByFilterAsync(ExpressaoFilterDto filter)
    => await _dbContext.Expressoes.Where(x => filter.Id == null || x.Id == filter.Id)
     .OrderBy(x => x.NuOrdemExecucao).ToListAsync();

  public void Add(Expressao obj) => _dbContext.Expressoes.Add(obj);

  public void Update(Expressao obj) => _dbContext.Expressoes.Update(obj);

  public void Delete(Expressao obj) => _dbContext.Expressoes.Remove(obj);

  public void DeleteRange(IEnumerable<Expressao> range) => _dbContext.Expressoes.RemoveRange(range);

  public IEnumerable<Expressao> FindByComandos(IEnumerable<Comando> comandosDaSp)
  {
    return comandosDaSp
     .Select(comando => _dbContext.Expressoes.SingleOrDefault(x => x.Comandos.Contains(comando)))
     .OfType<Expressao>().ToList();
  }

  public bool JaExiste(Expressao obj)
  {
    var commandText = Concat(
      $"SELECT * FROM {Schema}.\"Expressao\" WHERE  \"Id_OperandoEsquerda\" = {
        obj.IdOperandoEsquerda}",
      $" and \"Id_OperandoDireita\" = {obj.IdOperandoDireita} and \"Co_Operador\" = {obj.CoOperador
      } and \"Id\" != {obj.Id}");
    var result = _dbContext.Expressoes.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }
}
