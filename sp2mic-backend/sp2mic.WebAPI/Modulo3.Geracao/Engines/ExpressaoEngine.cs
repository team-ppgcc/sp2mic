using System.Text;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class ExpressaoEngine : IExpressaoEngine
{
  private readonly IOperandoEngine _operandoEngine;
  private readonly IComandoService _comandoService;
  private readonly IOperandoService _operandoService;
  private readonly ILogger<VariavelEngine> _logger;

  public ExpressaoEngine(IOperandoEngine operandoEngine, IComandoService comandoService,
    IOperandoService operandoService, ILogger<VariavelEngine> logger)
  {
    _operandoEngine = operandoEngine;
    _comandoService = comandoService;
    _operandoService = operandoService;
    _logger = logger ?? throw new ArgumentNullException(nameof (logger));
  }

  public string Print(Expressao ex)
  {
    _logger.LogInformation("ExpressaoEngine => Print ExpressaoId = {Id} ######", ex?.Id);
    var texto = new StringBuilder("");
    if (ex.IdOperandoEsquerda != 0)
    {
      ex.IdOperandoEsquerdaNavigation = _operandoService.FindById(ex.IdOperandoEsquerda.Value);
    }
    var operandoEsquerda = _operandoEngine.Print(ex.IdOperandoEsquerdaNavigation!);
    if (ex.CoOperador is null or 0 && ex.IdOperandoDireita is null or 0)
    {
      if (ex.IdOperandoEsquerda is null or 0)
      {
        texto.Append("// Comando não mapeado\n");
        texto.Append($"//{_comandoService.FindByIdExpressao(ex.Id).TxComandoTratado}");
      }
      texto.Append(operandoEsquerda);
      return texto.ToString();
    }
    var operador = ex.CoOperador!.GetDescricao();
    if (!operador.Any())
    {
      return texto.ToString();
    }
    if (ex.IdOperandoDireita != 0)
    {
      ex.IdOperandoDireitaNavigation = _operandoService.FindById(ex.IdOperandoDireita.Value);
    }
    var operandoDireita = _operandoEngine.Print(ex.IdOperandoDireitaNavigation!);

    if (ex.IdOperandoDireitaNavigation!.CoTipoOperando == TipoOperandoEnum.CONSTANTE_STRING &&
      operador.Equals("=="))
    {
      texto.Append($"Objects.equals({operandoEsquerda}, {operandoDireita})");
      return texto.ToString();
    }
    texto.Append($"{operandoEsquerda} {operador} {operandoDireita}");
    return texto.ToString();
  }
}
