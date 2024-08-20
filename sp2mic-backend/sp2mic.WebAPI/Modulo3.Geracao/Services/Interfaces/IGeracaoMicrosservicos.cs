using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo3.Geracao.Services.Interfaces;

public interface IGeracaoMicrosservicos<in TDto> : IApplicationService
{
  public string GerarTodosProjetos (TDto dto);
}
