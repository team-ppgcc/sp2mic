using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IEndpointService : IApplicationRepository
{
  Endpoint FindById(int id);
  Task<Endpoint> FindByIdAsync(int id);
  IEnumerable<Endpoint> FindAll();
  Task<IEnumerable<Endpoint>> FindAllAsync();
  IEnumerable<Endpoint> FindByFilter(EndpointFilterDto? filter);
  Task<IEnumerable<Endpoint>> FindByFilterAsync(EndpointFilterDto? filter);
  Task<Endpoint> AddAsync(Endpoint? obj);
  Task UpdateAsync(int id, Endpoint? novo);
  Task DeleteAsync(int id);

  IEnumerable<Endpoint> FindByIdMicrosservico(int idMicrosservico);
  IEnumerable<Endpoint> FindByIdStoredProcedure(int idStoredProcedure);
  void TratarTextoEndpoints();
  void AjustarRetornoEndpoints();
  void AjustarNomesEndpoints();
  void AjustarPathsEndpoints();
}
