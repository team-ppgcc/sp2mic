using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class OperandoRepository : IOperandoRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public OperandoRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public Operando? FindById(int id)
    => _dbContext.Operandos
     // .Include(x => x.ExpressaoIdOperandoDireitaNavigations)
     // .Include(x => x.ExpressaoIdOperandoEsquerdaNavigations)
     .SingleOrDefault(x => x.Id == id);

  public async Task<Operando?> FindByIdAsync(int id)
    => await _dbContext.Operandos
     // .Include(x => x.ExpressaoIdOperandoDireitaNavigations)
     // .Include(x => x.ExpressaoIdOperandoEsquerdaNavigations)
     .SingleOrDefaultAsync(x => x.Id == id);

  public IEnumerable<Operando> FindAll()
    => _dbContext.Operandos
     // .Include(x => x.ExpressaoIdOperandoDireitaNavigations)
     // .Include(x => x.ExpressaoIdOperandoEsquerdaNavigations)
     .OrderBy(x => x.CoTipoOperando)
     .ToHashSet();

  public async Task<IEnumerable<Operando>> FindAllAsync()
    => await _dbContext.Operandos
     // .Include(x => x.ExpressaoIdOperandoDireitaNavigations)
     // .Include(x => x.ExpressaoIdOperandoEsquerdaNavigations)
     .OrderBy(x => x.CoTipoOperando)
     .ToListAsync();

  public IEnumerable<Operando> FindByFilter(OperandoFilterDto filter)
    => _dbContext.Operandos
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.CoTipo == null || x.CoTipoOperando == filter.CoTipo)
     .Where(x => filter.TxValor == null || x.TxValor == filter.TxValor)
     .Where(x => filter.SnNegacao == null || x.SnNegacao == filter.SnNegacao)
     .Where(x => filter.IdVariavel == null || x.IdVariavel == filter.IdVariavel)
     .Where(x => filter.IdExpressao == null || x.IdExpressao == filter.IdExpressao)
     .Where(x => filter.IdEndpoint == null || x.IdEndpoint == filter.IdEndpoint)
     // .Include(x => x.ExpressaoIdOperandoDireitaNavigations)
     // .Include(x => x.ExpressaoIdOperandoEsquerdaNavigations)
     .OrderBy(x => x.CoTipoOperando)
     .ToHashSet();

  public async Task<IEnumerable<Operando>> FindByFilterAsync(OperandoFilterDto filter)
    => await _dbContext.Operandos
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.CoTipo == null || x.CoTipoOperando == filter.CoTipo)
     .Where(x => filter.TxValor == null || x.TxValor == filter.TxValor)
     .Where(x => filter.SnNegacao == null || x.SnNegacao == filter.SnNegacao)
     .Where(x => filter.IdVariavel == null || x.IdVariavel == filter.IdVariavel)
     .Where(x => filter.IdExpressao == null || x.IdExpressao == filter.IdExpressao)
     .Where(x => filter.IdEndpoint == null || x.IdEndpoint == filter.IdEndpoint)
     // .Include(x => x.ExpressaoIdOperandoDireitaNavigations)
     // .Include(x => x.ExpressaoIdOperandoEsquerdaNavigations)
     .OrderBy(x => x.CoTipoOperando)
     .ToListAsync();

  public void Add(Operando obj) => _dbContext.Operandos.Add(obj);

  public void Update(Operando obj) => _dbContext.Operandos.Update(obj);

  public void Delete(Operando obj) => _dbContext.Operandos.Remove(obj);

  public void Delete(int id) => _dbContext.Operandos.Remove(FindById(id));

  public void DeleteRange(IEnumerable<Operando> range) => _dbContext.Operandos.RemoveRange(range);

  public bool JaExiste(Operando obj)
  {
    var commandText
      = Concat($"SELECT * FROM {Schema}.\"Operando\" WHERE  \"Co_Tipo\" = {obj.CoTipoOperando}",
        $" and \"Id_Expressao\" = {obj.IdExpressao} and \"Id_Variavel\" = {obj.IdVariavel}",
        $" and \"Id_Endpoint\" = {obj.IdEndpoint} and \"Tx_Valor\" = {obj.TxValor}",
        $" and \"Id\" != {obj.Id}");
    var result = _dbContext.Operandos.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }
}
