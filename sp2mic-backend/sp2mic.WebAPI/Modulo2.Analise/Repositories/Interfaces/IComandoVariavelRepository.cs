using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;

public interface IComandoVariavelRepository : IApplicationRepository
{
  IEnumerable<ComandoVariavel> GetByIdComando (int idComando);
}
