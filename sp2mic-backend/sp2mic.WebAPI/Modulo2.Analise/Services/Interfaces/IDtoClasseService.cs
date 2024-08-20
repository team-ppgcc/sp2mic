using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IDtoClasseService : IApplicationService
{
  DtoClasse FindById(int id);
  Task<DtoClasse> FindByIdAsync(int id);
  IEnumerable<DtoClasse> FindAll();
  IEnumerable<ComboBoxDto> FindByIdProcedureForCombo(int idStoredProcedure);
  Task<IEnumerable<DtoClasse>> FindAllAsync();
  IEnumerable<DtoClasse> FindByFilter(DtoClasseFilterDto? filter);
  Task<IEnumerable<DtoClasse>> FindByFilterAsync(DtoClasseFilterDto? filter);
  Task<DtoClasse> AddAsync(DtoClasse? obj);
  Task UpdateAsync(int id, DtoClasse? obj);
  Task DeleteAsync(int id);

  //IEnumerable<DtoClasse> FindByIdMicrosservico (int idMicrosservico); agora é RecuperarClassesDeUmMicrosservico
  string? GetNoDtoClasseById(int idDtoClasse);
  IEnumerable<DtoClasse> RecuperarClassesDeUmMicrosservico(int idMicrosservico);
  IEnumerable<DtoClasse> RecuperarClassesDeUmaListaDeMicrosservicos(ISet<int> idsMicrosservicos);
  void AjustarNomesClasses();
}
