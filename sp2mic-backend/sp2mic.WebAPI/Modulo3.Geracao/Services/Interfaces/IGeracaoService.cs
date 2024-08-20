using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;

namespace sp2mic.WebAPI.Modulo3.Geracao.Services.Interfaces;

public interface IGeracaoService<in TDto> : IApplicationService
{
  public void GerarProjeto (TDto dto, HashSet<MicrosservicoInfo> microsservicosInfo);
}
