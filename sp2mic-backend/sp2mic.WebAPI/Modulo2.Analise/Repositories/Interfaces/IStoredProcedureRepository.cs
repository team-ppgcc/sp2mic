using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IStoredProcedureRepository : IApplicationRepository
{
  StoredProcedure? FindById(int id);
  Task<StoredProcedure?> FindByIdAsync(int id);
  IEnumerable<StoredProcedure> FindAll();
  Task<IEnumerable<StoredProcedure>> FindAllAsync();
  IEnumerable<StoredProcedure> FindByFilter(StoredProcedureFilterDto filter);
  Task<IEnumerable<StoredProcedure>> FindByFilterAsync(StoredProcedureFilterDto filter);
  void Add(StoredProcedure obj);
  void Update(StoredProcedure obj);
  void Delete(StoredProcedure obj);
  void Delete(int idStoredProcedure);
  void DeleteRange(IEnumerable<StoredProcedure> storedProcedures);
  void DeleteAll();
  bool JaExiste(StoredProcedure obj);

  //StoredProcedure? GetDefinicaoById(int id);
  IEnumerable<StoredProcedure> RecuperarAnalisadas();
  void UpdateRange(IEnumerable<StoredProcedure> obj);
  StoredProcedure? FindBySchemaNome(string spSchema, string spName);

  void SaveOne(StoredProcedure obj);
  void SaveAll (IEnumerable<StoredProcedure> entityRange);
}
