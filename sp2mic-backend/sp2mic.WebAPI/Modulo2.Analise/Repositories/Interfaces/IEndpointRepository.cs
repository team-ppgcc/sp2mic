using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IEndpointRepository : IApplicationRepository
{
  Endpoint? FindById(int id);
  Task<Endpoint?> FindByIdAsync(int id);
  IEnumerable<Endpoint> FindAll();
  Task<IEnumerable<Endpoint>> FindAllAsync();
  IEnumerable<Endpoint> FindByFilter(EndpointFilterDto filter);
  Task<IEnumerable<Endpoint>> FindByFilterAsync(EndpointFilterDto filter);
  void Add(Endpoint obj);
  void Update(Endpoint obj);
  void Delete(Endpoint obj);
  bool JaExiste(Endpoint obj);

  void UpdateRange (IEnumerable<Endpoint> obj);
  IEnumerable<Endpoint> FindByIdMicrosservico(int idMicrosservico);
  IEnumerable<Endpoint> FindByIdDtoClasse(int? idDtoClasse);
  IEnumerable<Endpoint> FindByIdStoredProcedure(int idStoredProcedure);
  IEnumerable<Endpoint>? FindByIdProcedure(int idStoredProcedure);
  void DeleteRange(IEnumerable<Endpoint> endpoints);
  void AjustarRetornoEndpoints();
  void AjustarNomesEndpoints();
  void AjustarPathsEndpoints();
}

//
// public void SalvarOuAtualizar(Endpoint obj)
// {
//   //_logger.LogInformation(" EndpointService -> SalvarOuAtualizar obj.Id = {ObjId} #",obj.Id);
//
//   if (obj == null)
//   {
//     throw new BadHttpRequestException("Empty Endpoint.");
//   }
//
//   VerificarExistencia(obj);
//   //_classeDTOService.VerificarExistencia(obj.IdDtoClasseNavigation);
//   // um ou mais endpoint pode retornar a mesma classeDTO
//
//
//
//   AtualizarVariavel(obj);
//
//   if (obj.Id == 0)
//   {
//     _dbContext.Add(obj);
//   }
//   else
//   {
//     _dbContext.Update(obj);
//   }
//
//   _dbContext.SaveChanges();
//
//   AjustarClasseRetornada(obj.Id);
// }
//
