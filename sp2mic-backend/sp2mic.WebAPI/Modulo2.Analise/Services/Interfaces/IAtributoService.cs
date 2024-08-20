using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IAtributoService : IApplicationService
{
  Atributo FindById(int id);
  Task<Atributo> FindByIdAsync(int id);
  IEnumerable<Atributo> FindAll();
  Task<IEnumerable<Atributo>> FindAllAsync();
  IEnumerable<Atributo> FindByFilter(AtributoFilterDto? filter);
  Task<IEnumerable<Atributo>> FindByFilterAsync(AtributoFilterDto? filter);
  Task<Atributo> AddAsync(Atributo? obj);
  Task UpdateAsync(int id, Atributo? obj);
  Task DeleteAsync(int id);

  IEnumerable<Atributo> GetAtributosByIdDtoClasse(int idDtoClasse);
}
