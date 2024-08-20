using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IComandoService : IApplicationService
{
  Comando FindById(int id);
  Task<Comando> FindByIdAsync(int id);
  IEnumerable<Comando> FindAll();
  Task<IEnumerable<Comando>> FindAllAsync();
  IEnumerable<Comando> FindByFilter(ComandoFilterDto? filter);
  Task<IEnumerable<Comando>> FindByFilterAsync(ComandoFilterDto? filter);
  Task<Comando> AddAsync(Comando? obj);
  Task UpdateAsync(int id, Comando? obj);
  Task DeleteAsync(int id);

  IEnumerable<Comando> FindByIdStoredProcedure (int idStoredProcedure, bool isInterno);
  Comando FindByIdExpressao (int idExpressao);
  IEnumerable<Comando> RecuperarComandosIf (Comando comando);
  IEnumerable<Comando> RecuperarComandosElse (Comando comando);
}
