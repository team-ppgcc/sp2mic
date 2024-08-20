using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IMicrosservicoRepository : IApplicationRepository
{
  Microsservico? FindById(int id);
  Task<Microsservico?> FindByIdAsync(int id);
  IEnumerable<Microsservico> FindAll();
  Task<IEnumerable<Microsservico>> FindAllAsync();
  IEnumerable<Microsservico> FindByFilter(MicrosservicoFilterDto filter);
  Task<IEnumerable<Microsservico>> FindByFilterAsync(MicrosservicoFilterDto filter);
  void Add(Microsservico obj);
  void Update(Microsservico obj);
  void Delete(Microsservico obj);
  void DeleteRange(IEnumerable<Microsservico> microsservicos);
  bool JaExiste(Microsservico obj);

  public IEnumerable<Microsservico> FindProntosParaGerar();
}
