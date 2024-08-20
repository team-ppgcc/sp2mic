using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IVariavelRepository: IApplicationRepository
{
  Variavel? FindById(int id);
  Task<Variavel?> FindByIdAsync(int id);
  IEnumerable<Variavel> FindAll();
  Task<IEnumerable<Variavel>> FindAllAsync();
  IEnumerable<Variavel> FindByFilter(VariavelFilterDto filter);
  Task<IEnumerable<Variavel>> FindByFilterAsync(VariavelFilterDto filter);
  void Add(Variavel obj);
  void Update(Variavel obj);
  void Delete(Variavel obj);
  bool JaExiste(Variavel obj);

  IEnumerable<Variavel?> FindByIdStoredProcedure (int idStoredProcedure);
  void UpdateRange(IEnumerable<Variavel> obj);
  IEnumerable<Variavel> FindByIdProcedure(int idStoredProcedure);
  void DeleteRange(IEnumerable<Variavel> variaveis);
}
