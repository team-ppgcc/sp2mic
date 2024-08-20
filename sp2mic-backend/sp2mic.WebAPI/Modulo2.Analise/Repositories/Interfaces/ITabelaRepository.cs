using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface ITabelaRepository : IApplicationRepository
{
  Tabela? FindById(int id);
  Task<Tabela?> FindByIdAsync(int id);
  IEnumerable<Tabela> FindAll();
  Task<IEnumerable<Tabela>> FindAllAsync();
  IEnumerable<Tabela> FindByFilter(TabelaFilterDto filter);
  Task<IEnumerable<Tabela>> FindByFilterAsync(TabelaFilterDto filter);
  void Add(Tabela obj);
  void Update(Tabela obj);
  void Delete(Tabela obj);
  bool JaExiste(Tabela obj);

  Tabela? FindByName(string noTabela);
  void DeleteRange(IEnumerable<Tabela> tabelas);
  //IEnumerable<Tabela> FindByIdProcedure(int idStoredProcedure);
}
