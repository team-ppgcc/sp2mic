using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;
using static System.String;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class VariavelEngine : IEngine<Variavel, VariavelInfo>
{
  private readonly ILogger<VariavelEngine> _logger;

  public VariavelEngine(ILogger<VariavelEngine> logger)
    => _logger = logger ?? throw new ArgumentNullException(nameof (logger));

  public HashSet<VariavelInfo> ConvertDados(HashSet<Variavel>? variaveis)
  {
    if (variaveis is null)
    {
      throw new ArgumentNullException(nameof (variaveis));
    }
    return variaveis.Select(ConvertDados).ToHashSet().OrderBy(v => v.NomeVariavel).ToHashSet();
  }

  public VariavelInfo ConvertDados(Variavel? v)
  {
    _logger.LogInformation("VariavelEngine => ConvertDados");

    if (v is null)
    {
      throw new ArgumentNullException(nameof (v));
    }
    var info = new VariavelInfo
    {
      NomeVariavel = IsNullOrEmpty(v.NoVariavel) ? "" : v.NoVariavel,
      NomeVariavelUpperCase = IsNullOrEmpty(v.NoVariavel) ? "" : v.NoVariavel.ToUpper(),
      NomeComInicialMinuscula = v.NoVariavel,
      NomeTipoDado = v.CoTipoDado == 0 ? "" : v.CoTipoDado.GetNome(),
      TipoEscopo = v.CoTipoEscopo == 0 ? "" : v.CoTipoEscopo.GetNome(),
      Tamanho = v.NuTamanho
    };
    return info;
  }
}
