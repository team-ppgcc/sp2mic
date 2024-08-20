using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IOperandoRepository : IApplicationRepository
{
  Operando? FindById(int id);
  Task<Operando?> FindByIdAsync(int id);
  IEnumerable<Operando> FindAll();
  Task<IEnumerable<Operando>> FindAllAsync();
  IEnumerable<Operando> FindByFilter(OperandoFilterDto filter);
  Task<IEnumerable<Operando>> FindByFilterAsync(OperandoFilterDto filter);
  void Add(Operando obj);
  void Update(Operando obj);
  void Delete(Operando obj);
  void Delete(int obj);
  bool JaExiste(Operando obj);
  //IEnumerable<Operando> FindByIdProcedure(int idStoredProcedure);
  void DeleteRange(IEnumerable<Operando> operandos);
}
