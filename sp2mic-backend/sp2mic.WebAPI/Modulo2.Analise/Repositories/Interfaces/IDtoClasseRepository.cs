using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IDtoClasseRepository : IApplicationRepository
{
  DtoClasse? FindById(int id);
  Task<DtoClasse?> FindByIdAsync(int id);
  IEnumerable<DtoClasse> FindAll();
  IEnumerable<DtoClasse>? FindByIdProcedure(int idStoredProcedure);
  Task<IEnumerable<DtoClasse>> FindAllAsync();
  IEnumerable<DtoClasse> FindByFilter(DtoClasseFilterDto filter);
  Task<IEnumerable<DtoClasse>> FindByFilterAsync(DtoClasseFilterDto filter);
  void Add(DtoClasse obj);
  void Update(DtoClasse obj);
  void Delete(DtoClasse obj);
  //bool JaExiste(DtoClasse obj); não precisa mais

  // IEnumerable<DtoClasse> FindByIdMicrosservico(int idMicrosservico); agora é RecuperarClassesDeUmMicrosservico
  string? GetNoDtoClasseById(int idDtoClasse);
  void DeleteRange(IEnumerable<DtoClasse> classes);
  IEnumerable<DtoClasse> RecuperarClassesDeUmMicrosservico(int idMicrosservico);
  IEnumerable<DtoClasse> RecuperarClassesDeUmaListaDeMicrosservicos(ISet<int> idsMicrosservicos);
  void AjustarNomesClasses();
}
