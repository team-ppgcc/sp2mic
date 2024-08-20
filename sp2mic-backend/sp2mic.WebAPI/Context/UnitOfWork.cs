using sp2mic.WebAPI.Modulo2.Analise.Repositories;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

namespace sp2mic.WebAPI.Context;

public class UnitOfWork : IUnitOfWork
{
  private readonly DbContextSp2Mic _dbContext;

  public UnitOfWork(DbContextSp2Mic dbContext) => this._dbContext = dbContext;

  public IAtributoRepository AtributoRepository => new AtributoRepository(_dbContext);
  public IComandoRepository ComandoRepository => new ComandoRepository(_dbContext);
  public IComandoVariavelRepository ComandoVariavelRepository => new ComandoVariavelRepository(_dbContext);
  public IDtoClasseRepository DtoClasseRepository => new DtoClasseRepository(_dbContext);
  public IEndpointRepository EndpointRepository => new EndpointRepository(_dbContext);
  public IExpressaoRepository ExpressaoRepository => new ExpressaoRepository(_dbContext);
  public IMicrosservicoRepository MicrosservicoRepository => new MicrosservicoRepository(_dbContext);
  public IOperandoRepository OperandoRepository => new OperandoRepository(_dbContext);
  public IStoredProcedureRepository StoredProcedureRepository => new StoredProcedureRepository(_dbContext);
  public ITabelaRepository TabelaRepository => new TabelaRepository(_dbContext);
  public IVariavelRepository VariavelRepository => new VariavelRepository(_dbContext);

  public async Task<bool> SaveAsync() => await _dbContext.SaveChangesAsync() > 0;
  public bool Save() => _dbContext.SaveChanges() > 0;
}
