using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IVariavelService : IApplicationService
{
  //VariavelDto? FindById (int? id);
  Variavel? FindById(int id);

  //Variavel? ObterPorId (int? id);
  //Task<VariavelDto?> FindByIdAsync (int id);
  Task<Variavel> FindByIdAsync(int id);

  ISet<Variavel> FindAll();
  Task<IEnumerable<Variavel>> FindAllAsync();



  //List<Variavel> FindByEndpointId (int epId);
  //List<Variavel> FindByFilter (VariavelFilterDto variavel);
  IEnumerable<Variavel> FindByFilter(VariavelFilterDto? filter);

  //Task<IEnumerable<VariavelDto>> FindByFilterAsync (VariavelFilterDto? filter);
  Task<IEnumerable<Variavel>> FindByFilterAsync(VariavelFilterDto? filter);



  //Task<VariavelDto> AddAsync (VariavelAddUpdateDto addDto);
  Task<Variavel> AddAsync(Variavel? obj);

  Task UpdateAsync (int id, Variavel obj);
  //Task UpdateAsync(int id, Variavel? obj);

  Task DeleteAsync (int id); // ok


  IEnumerable<Variavel?> FindByIdStoredProcedure (int idStoredProcedure);
  //void TratarNomesDasVariaveis();

}
