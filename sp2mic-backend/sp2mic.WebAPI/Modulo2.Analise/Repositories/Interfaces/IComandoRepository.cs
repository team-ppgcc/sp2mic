using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IComandoRepository : IApplicationRepository
{
  Comando? FindById(int id);
  Task<Comando?> FindByIdAsync(int id);
  IEnumerable<Comando> FindAll();
  Task<IEnumerable<Comando>> FindAllAsync();
  IEnumerable<Comando> FindByFilter(ComandoFilterDto filter);
  Task<IEnumerable<Comando>> FindByFilterAsync(ComandoFilterDto filter);
  void Add(Comando obj);
  void Update(Comando obj);
  void Delete(Comando obj);
  bool JaExiste(Comando obj);

  IEnumerable<Comando> FindByIdStoredProcedure (int idStoredProcedure, bool isInterno);
  Comando? FindByIdExpressao (int idExpressao);
  IEnumerable<Comando> RecuperarComandosIf (Comando comando);
  IEnumerable<Comando> RecuperarComandosElse (Comando comando);
  IEnumerable<Comando>? FindByIdProcedure(int idStoredProcedure);
  void DeleteRange(IEnumerable<Comando> comandos);
}
