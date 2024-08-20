
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class ComandoVariavelService : IComandoVariavelService
{
  private readonly IUnitOfWork _uow;

  public ComandoVariavelService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public IEnumerable<ComandoVariavel> GetByIdComando(int idComando)
  {
    var objs = _uow.ComandoVariavelRepository.GetByIdComando(idComando).ToHashSet();
    if (!objs.Any())
    {
      throw new BadHttpRequestException("Variable not found.");
    }
    return objs;
  }
}
