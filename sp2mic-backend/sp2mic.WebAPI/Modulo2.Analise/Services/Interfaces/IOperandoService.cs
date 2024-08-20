using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IOperandoService : IApplicationService
{
  Operando FindById(int id);
  Task<Operando> FindByIdAsync(int id);
  IEnumerable<Operando> FindAll();
  Task<IEnumerable<Operando>> FindAllAsync();
  IEnumerable<Operando> FindByFilter(OperandoFilterDto? filter);
  Task<IEnumerable<Operando>> FindByFilterAsync(OperandoFilterDto? filter);
  Task<Operando> AddAsync(Operando? obj);
  Task UpdateAsync(int id, Operando? obj);
  Task DeleteAsync(int id);

  //List<Variavel> GetVariaveisByOperandoId(int idComando);
}
