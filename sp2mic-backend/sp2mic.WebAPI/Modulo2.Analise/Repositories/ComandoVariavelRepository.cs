using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class ComandoVariavelRepository : IComandoVariavelRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public ComandoVariavelRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public IEnumerable<ComandoVariavel> GetByIdComando(int idComando)
    => _dbContext.ComandoVariaveis
     // .Include(c => c.IdVariavelNavigation)
     // .Include(c => c.IdComandoNavigation)
     .Where(x => x.IdComando == idComando)
     .OrderBy(x => x.NuOrdem)
     .ToHashSet();

  // public List<Variavel> GetVariaveisByComandoId (int idComando)
  //   => GetComandoVariaveisByComandoId(idComando)
  //    .Select(x => x.IdVariavelNavigation)
  //    .ToList();

  // public List<ComandoVariavel> GetComandoVariaveisByComandoId (int idComando)
  //   => _dbEntity.AsNoTracking()
  //    .Include(c => c.IdVariavelNavigation)
  //    .Where(x => x.IdComando == idComando)
  //    .OrderBy(x => x.NuOrdem)
  //    .ToList();
}
