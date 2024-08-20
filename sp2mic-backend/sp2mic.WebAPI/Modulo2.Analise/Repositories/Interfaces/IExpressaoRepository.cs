using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IExpressaoRepository : IApplicationRepository
{
  Expressao? FindById(int id);
  Task<Expressao?> FindByIdAsync(int id);
  IEnumerable<Expressao> FindAll();
  Task<IEnumerable<Expressao>> FindAllAsync();
  IEnumerable<Expressao> FindByFilter(ExpressaoFilterDto filter);
  Task<IEnumerable<Expressao>> FindByFilterAsync(ExpressaoFilterDto filter);
  void Add(Expressao obj);
  void Update(Expressao obj);
  void Delete(Expressao obj);
  bool JaExiste(Expressao obj);
  //IEnumerable<Expressao> FindByIdProcedure(int idStoredProcedure);
  void DeleteRange(IEnumerable<Expressao> expressoes);
  IEnumerable<Expressao> FindByComandos(IEnumerable<Comando> comandosDaSp);
}
