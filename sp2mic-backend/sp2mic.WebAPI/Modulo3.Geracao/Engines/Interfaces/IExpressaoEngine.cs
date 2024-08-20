using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;

public interface IExpressaoEngine : IApplicationService
{
  string Print (Expressao op);
}
