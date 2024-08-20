using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;

public interface IEngine<TEntity, TInfo> : IApplicationService
{
  HashSet<TInfo> ConvertDados (HashSet<TEntity>? list);
  TInfo ConvertDados (TEntity? entity);
}
