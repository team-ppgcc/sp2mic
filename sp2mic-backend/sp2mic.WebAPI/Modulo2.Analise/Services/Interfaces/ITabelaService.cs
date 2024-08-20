using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface ITabelaService : IApplicationService
{
  Tabela FindById(int id);
  Task<Tabela> FindByIdAsync(int id);
  ISet<Tabela> FindAll();
  Task<IEnumerable<Tabela>> FindAllAsync();
  IEnumerable<Tabela> FindByFilter(TabelaFilterDto? filter);
  Task<IEnumerable<Tabela>> FindByFilterAsync(TabelaFilterDto? filter);
  Task<Tabela> AddAsync(Tabela? obj);
  Task UpdateAsync(int id, Tabela? obj);
  Task DeleteAsync(int id);

  Tabela? FindByName(string noTabela);

  // Tabela? ObterPorId(int? id);
  // HashSet<Tabela> FindByFilter(TabelaFilterDto filter);

}
