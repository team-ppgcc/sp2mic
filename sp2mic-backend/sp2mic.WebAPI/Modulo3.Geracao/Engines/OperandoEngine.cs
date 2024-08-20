using System.Text;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class OperandoEngine : IOperandoEngine
{
  private readonly IExpressaoService _expressaoService;
  private readonly IComandoService _comandoService;
  private readonly IOperandoService _operandoService;
  private readonly ILogger<VariavelEngine> _logger;

  public OperandoEngine(IExpressaoService expressaoService, IComandoService comandoService,
    IOperandoService operandoService, ILogger<VariavelEngine> logger)
  {
    _expressaoService = expressaoService;
    _comandoService = comandoService;
    _operandoService = operandoService;
    _logger = logger ?? throw new ArgumentNullException(nameof (logger));
  }

  public string Print(Operando? op)
  {
    if (op is null)
    {
      return "";
    }
    _logger.LogInformation("OperandoEngine =>  Print OperandoId = {Id} ######", op?.Id);
    var texto = new StringBuilder("");
    switch (op.CoTipoOperando)
    {
      case TipoOperandoEnum.CONSTANTE:
        texto.Append(op.TxValor);
        break;
      case TipoOperandoEnum.CONSTANTE_STRING:
        texto.Append($"\"{op.TxValor}\"");
        break;
      case TipoOperandoEnum.VARIAVEL:
        if (op.SnNegacao)
        {
          texto.Append('!');
        }
        texto.Append(op.IdVariavelNavigation!.NoVariavel);
        break;
      case TipoOperandoEnum.EXPRESSAO:
        // vem com os operandos e suas variáveis e endpoints
        //var expressao = _expressaoService.ObterPorId(op.IdExpressao);
        //texto.Append($"({Print(expressao)})");
        op.IdExpressaoNavigation = _expressaoService.FindById(op.IdExpressao!.Value);
        texto.Append($"({Print(op.IdExpressaoNavigation)})");
        break;
      case TipoOperandoEnum.ENDPOINT:
        var parametrosLista = op.IdEndpointNavigation!.Parametros.OrderBy(v => v.NoVariavel)
         .Select(ov => ov.NoVariavel).ToList();
        var parametros = parametrosLista.Count == 0 ? "" :
          parametrosLista.Aggregate((a, b) => a + ", " + b);
        var nomeMetodo = op.IdEndpointNavigation is null ? "endPointWithoutName" :
          op.IdEndpointNavigation.NoMetodoEndpoint;
        var mic = op.IdEndpointNavigation!.IdMicrosservicoNavigation;
        var nomeMicrosservico = mic != null ? mic.NoMicrosservico.InicialMaiuscula() :
          "MicrosservicoNaoEncontrado";
        texto.Append($"{nomeMetodo}{nomeMicrosservico}({parametros})");
        break;
    }

    return texto.ToString();
  }

  private string Print(Expressao ex)
  {
    _logger.LogInformation("OperandoEngine =>  Print ExpressaoId = {Id} ######", ex?.Id);
    var texto = new StringBuilder("");
    if (ex.IdOperandoEsquerda != 0)
    {
      ex.IdOperandoEsquerdaNavigation = _operandoService.FindById(ex.IdOperandoEsquerda.Value);
    }
    var operandoEsquerda = Print(ex.IdOperandoEsquerdaNavigation!);
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
    var operandoDireita = Print(ex.IdOperandoDireitaNavigation!);
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
