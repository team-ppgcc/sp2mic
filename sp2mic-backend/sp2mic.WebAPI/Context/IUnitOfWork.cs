using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

namespace sp2mic.WebAPI.Context;

public interface IUnitOfWork : IApplicationRepository
{
  IAtributoRepository AtributoRepository {get;}
  IComandoRepository ComandoRepository {get;}
  IComandoVariavelRepository ComandoVariavelRepository {get;}
  IDtoClasseRepository DtoClasseRepository {get;}
  IEndpointRepository EndpointRepository {get;}
  IExpressaoRepository ExpressaoRepository {get;}
  IMicrosservicoRepository MicrosservicoRepository {get;}
  IOperandoRepository OperandoRepository {get;}
  IStoredProcedureRepository StoredProcedureRepository {get;}
  ITabelaRepository TabelaRepository {get;}
  IVariavelRepository VariavelRepository {get;}

  Task<bool> SaveAsync();
  bool Save();
}
