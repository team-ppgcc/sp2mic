using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IExpressaoService : IApplicationService
{
  Expressao FindById(int id);
  Task<Expressao> FindByIdAsync(int id);
  IEnumerable<Expressao> FindAll();
  Task<IEnumerable<Expressao>> FindAllAsync();
  IEnumerable<Expressao> FindByFilter(ExpressaoFilterDto? filter);
  Task<IEnumerable<Expressao>> FindByFilterAsync(ExpressaoFilterDto? filter);
  Task<Expressao> AddAsync(Expressao? obj);
  Task UpdateAsync(int id, Expressao? obj);
  Task DeleteAsync(int id);
}
