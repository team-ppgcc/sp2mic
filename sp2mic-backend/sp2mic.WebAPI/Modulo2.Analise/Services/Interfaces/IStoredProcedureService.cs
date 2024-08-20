using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IStoredProcedureService : IApplicationService
{
  StoredProcedure FindById(int id);
  Task<StoredProcedure> FindByIdAsync(int id);
  IEnumerable<StoredProcedure> FindAll();
  Task<IEnumerable<StoredProcedure>> FindAllAsync();
  IEnumerable<StoredProcedure> FindByFilter(StoredProcedureFilterDto? filter);
  Task<IEnumerable<StoredProcedure>> FindByFilterAsync(StoredProcedureFilterDto? filter);
  Task<StoredProcedure> AddAsync(StoredProcedure? obj);
  Task UpdateAsync(int id, StoredProcedure? obj);
  void Delete(int idStoredProcedure);
  void DeleteAll();
  void DeleteRange();

  //StoredProcedure GetDefinicaoById(int idStoredProcedure);
  IEnumerable<StoredProcedure> RecuperarAnalisadas();
  StoredProcedure? FindBySchemaNome(string spSchema, string spName);

  void SaveOne(StoredProcedure obj);
  void SaveAll (IEnumerable<StoredProcedure> entityRange);
}
