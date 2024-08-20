using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IMicrosservicoService : IApplicationService
{
  Microsservico FindById(int id);
  Task<Microsservico> FindByIdAsync(int id);
  IEnumerable<Microsservico> FindAll();
  IEnumerable<ComboBoxDto> FindAllForCombo();
  Task<IEnumerable<Microsservico>> FindAllAsync();
  IEnumerable<Microsservico> FindByFilter(MicrosservicoFilterDto? filter);
  Task<IEnumerable<Microsservico>> FindByFilterAsync(MicrosservicoFilterDto? filter);
  Task<Microsservico> AddAsync(Microsservico? obj);
  Task UpdateAsync(int id, Microsservico? obj);
  Task DeleteAsync(int id);

  public IEnumerable<Microsservico> FindProntosParaGerar();
}
