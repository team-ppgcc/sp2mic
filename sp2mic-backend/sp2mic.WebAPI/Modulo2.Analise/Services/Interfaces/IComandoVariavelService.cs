using sp2mic.WebAPI.Domain.Entities;

namespace sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

public interface IComandoVariavelService : IApplicationService
{
  IEnumerable<ComandoVariavel> GetByIdComando(int idComando);
}
