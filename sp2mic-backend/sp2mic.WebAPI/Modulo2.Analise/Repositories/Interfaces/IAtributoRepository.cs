using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IAtributoRepository : IApplicationRepository
{
  Atributo? FindById(int id);
  Task<Atributo?> FindByIdAsync(int id);
  IEnumerable<Atributo> FindAll();
  Task<IEnumerable<Atributo>> FindAllAsync();
  IEnumerable<Atributo> FindByFilter(AtributoFilterDto filter);
  Task<IEnumerable<Atributo>> FindByFilterAsync(AtributoFilterDto filter);
  void Add(Atributo obj);
  void Update(Atributo obj);
  void Delete(Atributo obj);
  bool JaExiste(Atributo obj);

  IEnumerable<Atributo> GetAtributosByIdDtoClasse(int idDtoClasse);
}
