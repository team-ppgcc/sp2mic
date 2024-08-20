using System.Text;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class ComandoEngine : IEngine<Comando, ComandoInfo>
{
  private readonly IExpressaoService _expressaoService;
  private readonly IExpressaoEngine _expressaoEngine;
  private readonly IComandoService _comandoService;

  public ComandoEngine(IExpressaoService expressaoService, IExpressaoEngine expressaoEngine, IComandoService comandoService)
  {
    _expressaoService = expressaoService;
    _expressaoEngine = expressaoEngine;
    _comandoService = comandoService;
  }

  public HashSet<ComandoInfo> ConvertDados(HashSet<Comando>? comandos)
  {
    if (comandos is null)
    {
      throw new ArgumentNullException(nameof (comandos));
    }
    //var comandosOrdenados = comandos.
    var qtdTotalComandos
      = comandos.Count(x => x.CoTipoComando != TipoComandoEnum.TIPO_NAO_MAPEADO);
    return comandos.Where(c => c.IdComandoOrigem == null)
     .OrderBy(c => c.NuOrdemExecucao)
     .Select(c => ConvertDados(c, qtdTotalComandos))
     .ToHashSet();
  }

  //public ComandoInfo ConvertDados(Comando? c) => null!;

  public ComandoInfo ConvertDados(Comando? c)
  {
    if (c is null)
    {
      throw new ArgumentNullException(nameof (c));
    }
    return null!;
  }

  private ComandoInfo ConvertDados(Comando c, int qtdTotalComandos)
  {
    var info = new ComandoInfo();

    //if (c.IdExpressao != null && c.IdExpressaoNavigation == null)
    if (c is {IdExpressao: not null, IdExpressaoNavigation: null})
    {
      c.IdExpressaoNavigation = _expressaoService.FindById(c.IdExpressao!.Value);
    }
    //   if (c.IdExpressaoNavigation.IdOperandoEsquerda != null &&
    //     c.IdExpressaoNavigation.IdOperandoEsquerdaNavigation == null)
    //   {
    //     c.IdExpressaoNavigation.IdOperandoEsquerdaNavigation
    //       = _operandoService.ObterPorId(c.IdExpressaoNavigation.IdOperandoEsquerda);
    //
    //     if (c.IdExpressaoNavigation.IdOperandoEsquerdaNavigation.IdVariavel != null &&
    //       c.IdExpressaoNavigation.IdOperandoEsquerdaNavigation.IdVariavelNavigation == null)
    //     {
    //       c.IdExpressaoNavigation.IdOperandoEsquerdaNavigation.IdVariavelNavigation
    //         = _variavelService.ObterPorId(c.IdExpressaoNavigation.IdOperandoEsquerdaNavigation
    //          .IdVariavel);
    //     }
    //   }
    //
    //   if (c.IdExpressaoNavigation.IdOperandoDireita != null &&
    //     c.IdExpressaoNavigation.IdOperandoDireitaNavigation == null)
    //   {
    //     c.IdExpressaoNavigation.IdOperandoDireitaNavigation
    //       = _operandoService.ObterPorId(c.IdExpressaoNavigation.IdOperandoDireita);
    //
    //     if (c.IdExpressaoNavigation.IdOperandoDireitaNavigation.IdVariavel != null &&
    //       c.IdExpressaoNavigation.IdOperandoDireitaNavigation.IdVariavelNavigation == null)
    //     {
    //       c.IdExpressaoNavigation.IdOperandoDireitaNavigation.IdVariavelNavigation
    //         = _variavelService.ObterPorId(c.IdExpressaoNavigation.IdOperandoDireitaNavigation
    //          .IdVariavel);
    //     }
    //   }
    // }
    info.IsComandoOrigem = c.IdComandoOrigem is null;
    info.ConteudoComando = ToImpressao(c, qtdTotalComandos);
    info.NomeVariavelRetornada = "";//GetNomeVariavelRetornada(c);
    return info;
  }

  // private static string GetNomeVariavelRetornada(Comando comando)
  // {
  //   // if (comando.IdEndpoint is not null &&
  //   //   comando.IdEndpointNavigation.CoTipoDadoRetorno != TipoDadoEnum.VOID.GetCodigo())
  //   // {
  //   //   return comando.IdEndpointNavigation?.IdVariavelRetornadaNavigation?.NoVariavel is not null ?
  //   //     $"{comando.IdEndpointNavigation.IdVariavelRetornadaNavigation.NoVariavel} = " :
  //   //     "retorno = ";
  //   // }
  //   return "";
  // }
  // private List<Variavel> RecuperarVariaveis (int comandoId)
  //   => _comandovariavelService.GetComandoVariaveisByComandoId(comandoId);

  // private HashSet<ComandoVariavel> RecuperarComandoVariaveis(int comandoId)
  //   => _comandovariavelService.GetByIdComando(comandoId).ToHashSet();

  private string PrintDeclaracao(Comando c, Variavel v)
  {
    var texto = new StringBuilder("");
    texto.Append($"    {v.CoTipoDado.GetNome()}");
    texto.Append(' ');
    texto.Append(PrintAtribuicao(c, v));
    texto.Append(";\n");
    return texto.ToString();
  }

  private string PrintAtribuicao(Comando c, Variavel v)
  {
    var texto = new StringBuilder("");
    texto.Append(v.NoVariavel);
    if (c.IdExpressao is null or 0)
    {
      return texto.Append(" = null").ToString();
    }
    var expressao = _expressaoService.FindById(c.IdExpressao.Value);
    // vem com os operandos e suas variáveis e endpoints
    texto.Append(" = ");
    texto.Append(_expressaoEngine.Print(expressao));
    texto.Append(";\n");
    return texto.ToString();
  }

  // private bool EhUltimoComando(int? ordemExecucao, int qtdTotalComandos)
  // {
  //   if (ordemExecucao is null or 0)
  //   {
  //     return true;
  //   }
  //   return qtdTotalComandos <= ordemExecucao;
  // }

  // private string PrintEndpoint(Endpoint e)
  // {
  //   var texto = new StringBuilder("");
  //   //if (e.IdStoredProcedureNavigation.CoTipoDadoRetorno != TipoDadoEnum.VOID.GetCodigo())
  //   if (e.IdStoredProcedureNavigation.CoTipoDadoRetorno != TipoDadoEnum.VOID)
  //   {
  //     if (e.CoTipoDadoRetorno != TipoDadoEnum.VOID)
  //     {
  //       texto.Append("    return ");
  //     }
  //   }
  //   var nomeMicrosservico = _microsservicoService.FindById(e.IdMicrosservico!.Value).NoMicrosservico
  //    .InicialMaiuscula();
  //   texto.Append(e.NoMetodoEndpoint);
  //   texto.Append(nomeMicrosservico);
  //   texto.Append('(');
  //   var parametros = e.Parametros.OrderBy(v => v.NoVariavel).ToHashSet();
  //   if (parametros.Any())
  //   {
  //     texto.Append(parametros.Select(p => p.NoVariavel)
  //      .Aggregate((a, b) => a + ", " + b));
  //   }
  //   texto.Append(");\n");
  //   return texto.ToString();
  // }

  private string ToImpressao(Comando c, int qtdTotalComandos)
  {
    //var comandoVariaveis = RecuperarComandoVariaveis(c.Id);
    var comandoVariaveis = c.AsVariaveisDesseComando;
    var variaveis = comandoVariaveis.Select(x => x.IdVariavelNavigation);
    var texto = new StringBuilder("");
    switch (c.CoTipoComando)
    {
      case TipoComandoEnum.ENDPOINT:

        if (c.IdEndpoint is 0 or null)
        {
          throw new BadHttpRequestException(
            "Command of type ENDPOINT must have an associated endpoint.");
        }
        // Se o comando é do tipo ENDPOINT, mas é interno provavelmente já foi impresso.
        // if (c.IdComandoOrigem == null)
        // {
        //var ep = _endPointService.ObterPorId(c.IdEndpoint);
        //texto.Append(PrintEndpointCompleto(ep, c.NuOrdemExecucao, qtdTotalComandos));
        //texto.Append(PrintComandoEndpoint(c.IdEndpointNavigation!, c.NuOrdemExecucao, qtdTotalComandos));
        if (c.IdEndpointNavigation!.IdMicrosservicoNavigation is not null &&
          c.IdEndpointNavigation!.IdMicrosservicoNavigation.SnProntoParaGerar)
        {
          texto.Append(PrintComandoEndpoint(c.IdEndpointNavigation!));
        }
        // }
        break;
      case TipoComandoEnum.DECLARACAO:
        foreach (var v in variaveis)
        {
          //var variavel = _variavelService.ObterPorId(v.Id);
          //if (v is not null)
          //{
            // texto.Append(PrintDeclaracao(c, _variavelService.ObterPorId(v.Id)));
            texto.Append(PrintDeclaracao(c, v));
          //}
        }

        break;
      case TipoComandoEnum.ATRIBUICAO:
        if (c.IdExpressao is 0 or null)
        {
          throw new BadHttpRequestException(
            "Command of type ATRIBUICAO must have an associated expression.");
        }
        //texto.Append(PrintAtribuicao(c));
        foreach (var v in variaveis)
        {
          //if (v is not null)
          //{
            // texto.Append(PrintAtribuicao(c, _variavelService.ObterPorId(v.Id)));
            texto.Append(PrintAtribuicao(c, v));
          //}
        }
        break;
      case TipoComandoEnum.IF:
        if (c.IdExpressao is 0 or null)
        {
          throw new BadHttpRequestException(
            "Command of type IF must have an associated expression.");
        }
        texto.Append(PrintIf(c, qtdTotalComandos));
        break;
      case TipoComandoEnum.WHILE:
        if (c.IdExpressao is 0 or null)
        {
          throw new BadHttpRequestException(
            "Command of type WHILE must have an associated expression.");
        }
        texto.Append(PrintWhile(c, qtdTotalComandos));
        break;
      //case 7: EXEC
      // StoredProcedureQuery query = em.createStoredProcedureQuery(proc.getNomeProcedure());
      // query.registerStoredProcedureParameter(parametro.getNomeParametro(), obj.getClass() , ParameterMode.IN);
      // query.execute();
      //return query.getResultList().toString();
    }

    return texto.ToString();
  }

  //private static string PrintComandoEndpoint(Endpoint ep, int? cNuOrdemExecucao, int qtdTotalComandos)
  private static string PrintComandoEndpoint(Endpoint ep)
  {
    var texto = new StringBuilder("");
    var mic = ep.IdMicrosservicoNavigation;
    var nomeMicrosservico = mic != null ? mic.NoMicrosservico.InicialMaiuscula() :
      "MicrosservicoNaoEncontrado";
    var parametrosPassados = "";
    if (ep.Parametros.Any())
    {
      var parametros = ep.Parametros.OrderBy(v => v.NoVariavel);
      parametrosPassados = parametros.Select(p => p.NoVariavel).Aggregate((a, b) => a + ", " + b);
    }
    var variavelRetornada = ep.IdVariavelRetornadaNavigation;
    // se possui retorno
    if (ep.CoTipoDadoRetorno != TipoDadoEnum.VOID && ep.CoTipoDadoRetorno != TipoDadoEnum.TIPO_NAO_MAPEADO)
    {
      if (variavelRetornada is not null)
      {
        texto.Append($"        {variavelRetornada.NoVariavel} = ");
      }
      else
      {
        texto.Append("        retorno = ");
      }
    }
    texto.Append($"{ep.NoMetodoEndpoint}{nomeMicrosservico}({parametrosPassados});\n");
    return texto.ToString();
  }

  private string PrintIf(Comando c, int qtdTotalComandos)
  {
    var comandosIf = _comandoService.RecuperarComandosIf(c);
    var comandosElse = _comandoService.RecuperarComandosElse(c).ToHashSet();
    //c.IdExpressaoNavigation = _expressaoService.ObterPorId(c.IdExpressao);
    var texto = new StringBuilder("");
    texto.Append($"        if ({_expressaoEngine.Print(c.IdExpressaoNavigation!)}) ");
    texto.Append("{\n");

    // foreach (var ci in c.ComandosInternos)
    foreach (var ci in comandosIf)
    {
      texto.Append("            " + ToImpressao(ci, qtdTotalComandos));
    }
    texto.Append("        }\n");
    if (!comandosElse.Any())
    {
      return texto.ToString();
    }
    texto.Append("        else {\n");
    foreach (var ci in comandosElse)
    {
      texto.Append("            " + ToImpressao(ci, qtdTotalComandos));
    }
    texto.Append("        }\n");
    return texto.ToString();
  }

  private string PrintWhile(Comando c, int qtdTotalComandos)
  {
    var texto = new StringBuilder("");
    texto.Append($"        while ({_expressaoEngine.Print(c.IdExpressaoNavigation!)}) ");
    texto.Append("{\n");
    foreach (var ci in c.ComandosInternos)
    {
      texto.Append("            " + ToImpressao(ci, qtdTotalComandos));
    }
    texto.Append("        }\n");
    return texto.ToString();
  }
}
