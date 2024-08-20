using System.Text;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;
using static System.String;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class StoredProcedureEngine : IEngine<StoredProcedure, StoredProcedureInfo>
{
  private readonly IEngine<Comando, ComandoInfo> _comandoEngine;
  private readonly IEngine<Endpoint, EndpointInfo> _endPointEngine;
  private readonly IEngine<Variavel, VariavelInfo> _variavelEngine;
  private readonly IEngine<DtoClasse, ClasseInfo> _classeEngine;
  private readonly IMicrosservicoService _microsservicoService;
  private readonly IDtoClasseService _classeDTOService;

  public StoredProcedureEngine(IEngine<Comando, ComandoInfo> comandoEngine,
    IEngine<Endpoint, EndpointInfo> endPointEngine, IEngine<Variavel, VariavelInfo> variavelEngine,
    IEngine<DtoClasse, ClasseInfo> classeEngine, IDtoClasseService classeDTOService,
    IMicrosservicoService microsservicoService)
  {
    _comandoEngine = comandoEngine;
    _endPointEngine = endPointEngine;
    _variavelEngine = variavelEngine;
    _classeEngine = classeEngine;
    _classeDTOService = classeDTOService;
    _microsservicoService = microsservicoService;
  }

  public HashSet<StoredProcedureInfo> ConvertDados(HashSet<StoredProcedure>? storedProcedures)
  {
    if (storedProcedures is null)
    {
      throw new ArgumentNullException(nameof (storedProcedures));
    }
    return storedProcedures.Select(ConvertDados).ToHashSet();
  }

  public StoredProcedureInfo ConvertDados(StoredProcedure? sp)
  {
    if (sp is null)
    {
      throw new ArgumentNullException(nameof (sp));
    }
    //_logger.LogInformation(" ConvertDados StoredProcedureId = {Id} ######", sp.Id);
    var comandosInfos = _comandoEngine.ConvertDados(sp.Comandos.ToHashSet());
    var info = new StoredProcedureInfo
    {
      //sp.Comandos = RecuperarComandosNaoInternos(sp.Id);
      //sp.Variaveis = RecuperarVariaveis(sp.Id);
      Id = sp.Id,
      NomeStoredProcedure = sp.NoStoredProcedure,
      PossuiRetorno = PossuiRetorno(sp),
      RetornoMetodo = RetornoMetodo(sp),
      NomeClasseRetorno = NomeClasseRetorno(sp),
      TipoNomeParametros = TipoNomeParametros(sp),
      // info.TemRetorno = TemRetorno(sp);
      UrlParametros = UrlParametros(sp),
      ParametrosPath = ParametrosPath(sp),
      ParametrosController = ParametrosController(sp),
      ParametrosChamada = ParametrosChamada(sp),
      ConteudoComandos = GetConteudoComandos(comandosInfos),
      Endpoints = _endPointEngine.ConvertDados(sp.Endpoints.ToHashSet()),
      ImportsController = GetImportsController(sp),
      ImportsService = GetImportsService(sp),
      TxResultadoParser = GetTxResultadoParser(sp),
      ComandoInfos = comandosInfos,
      VariavelInfos = _variavelEngine.ConvertDados(sp.Variaveis.ToHashSet()),
      ClasseInfos = _classeEngine.ConvertDados(sp.DtoClasses.ToHashSet())
    };
    info.EndpointsDosMicrosservicosProntosParaGerar
      = GetEndpointsDosMicrosservicosProntosParaGerar(info.Endpoints);

    return info;
  }

  private HashSet<EndpointInfo> GetEndpointsDosMicrosservicosProntosParaGerar(IEnumerable<EndpointInfo> infoEndpoints)
  {
    var microsservicosProntosParaGerar = _microsservicoService.FindProntosParaGerar().ToHashSet();
    var idsMicrosservicosProntosParaGerar = microsservicosProntosParaGerar.Select(m => m.Id);
    var endpointsDosMicrosservicosProntosParaGerar = new HashSet<EndpointInfo>();

    foreach (var epInfo in infoEndpoints.Where(epInfo => idsMicrosservicosProntosParaGerar.Contains(epInfo.IdMicrosservico)))
    {
      endpointsDosMicrosservicosProntosParaGerar.Add(epInfo);
    }
    return endpointsDosMicrosservicosProntosParaGerar;
  }

  private static string GetTxResultadoParser(StoredProcedure sp)
    => IsNullOrEmpty(sp.TxResultadoParser) ? "" : Concat("/******  ", sp.TxResultadoParser, "  ******/");

  private static string GetConteudoComandos(HashSet<ComandoInfo> comandosInfos)
  {
    var texto = new StringBuilder();
    foreach (var comando in comandosInfos)
    {
      texto.Append(comando.ConteudoComando);
    }
    return texto.ToString();
  }

  private static HashSet<string> GetImportsController(StoredProcedure sp)
  {
    var imports = new HashSet<string>();
    if (sp.SnRetornoLista == true)
    {
      imports.Add("java.util.List");
    }
    if (sp.CoTipoDadoRetorno != TipoDadoEnum.DTO &&
      sp.CoTipoDadoRetorno != TipoDadoEnum.TIPO_NAO_MAPEADO)
    {
      var pacoteSp = sp.CoTipoDadoRetorno!.GetPacoteImport();
      if (!IsNullOrEmpty(pacoteSp))
      {
        imports.Add(pacoteSp);
      }
    }
    if (sp.Variaveis.Any())
    {
      if (sp.Variaveis.Select(v => v.CoTipoEscopo = TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE).Any())
      {
        imports.Add("org.springframework.web.bind.annotation.PathVariable");
      }

      foreach (var param in sp.Variaveis)
      {
        if (param.CoTipoDado is TipoDadoEnum.DTO or TipoDadoEnum.TIPO_NAO_MAPEADO)
        {
          continue;
        }
        var pacoteParam = param.CoTipoDado.GetPacoteImport();
        if (!IsNullOrEmpty(pacoteParam))
        {
          imports.Add(pacoteParam);
        }
      }
    }

    foreach (var ep in sp.Endpoints)
    {
      if (ep.SnRetornoLista == true)
      {
        imports.Add("java.util.List");
      }
      var retorno = ep.CoTipoDadoRetorno.GetPacoteImport();
      if (retorno != "") {
        imports.Add(retorno);
      }
      var pacoteEp = ep.CoTipoSqlDml.GetPacoteImport();
      if (!IsNullOrEmpty(pacoteEp))
      {
        imports.Add(pacoteEp);
      }
      if (!ep.Parametros.Any())
      {
        return imports;
      }
      imports.Add("org.springframework.web.bind.annotation.PathVariable");
      foreach (var param in ep.Parametros)
      {
        if (param.CoTipoDado is TipoDadoEnum.DTO or TipoDadoEnum.TIPO_NAO_MAPEADO)
        {
          continue;
        }
        var pacoteParam = param.CoTipoDado.GetPacoteImport();
        if (!IsNullOrEmpty(pacoteParam))
        {
          imports.Add(pacoteParam);
        }
      }
      if (ep.CoTipoDadoRetorno is TipoDadoEnum.DTO or TipoDadoEnum.TIPO_NAO_MAPEADO)
      {
        continue;
      }
      var pacote = (ep.CoTipoDadoRetorno).GetPacoteImport();
      if (!IsNullOrEmpty(pacote))
      {
        imports.Add(pacote);
      }

      // if (ep.CoTipoDadoRetorno != TipoDadoEnum.DTO)
      // {
      //   return imports;
      // }
      // no controller do orquestrador o pacote é o do parent e não o do microsserviço
      // var nomeClasse = ep.IdDtoClasseNavigation?.NoDtoClasse.InicialMaiuscula() ??
      //   "erroAoRecuperarNomeClasse";
      //
      // var nomePacote = ep.IdMicrosservicoNavigation?.NoPacote ?? "erroAoRecuperarNomePacote";
      //
      // imports.Add($"{nomePacote}.{ep.IdMicrosservicoNavigation?.NoMicrosservico}.dto.{nomeClasse}");
    }
    return imports;
  }

  private static HashSet<string> GetImportsService(StoredProcedure sp)
  {
    var imports = new HashSet<string>();
    if (sp.SnRetornoLista == true)
    {
      imports.Add("java.util.List");
    }
    if (sp.CoTipoDadoRetorno != TipoDadoEnum.DTO &&
      sp.CoTipoDadoRetorno != TipoDadoEnum.TIPO_NAO_MAPEADO)
    {
      var pacoteSp = sp.CoTipoDadoRetorno!.GetPacoteImport();
      if (!IsNullOrEmpty(pacoteSp))
      {
        imports.Add(pacoteSp);
      }
    }
    foreach (var ep in sp.Endpoints)
    {
      if (ep.SnRetornoLista == true)
      {
        imports.Add("java.util.List");
      }

      if (ep.CoTipoDadoRetorno is TipoDadoEnum.DTO or TipoDadoEnum.TIPO_NAO_MAPEADO)
      {
        continue;
      }

      var pacote = (ep.CoTipoDadoRetorno).GetPacoteImport();

      if (!IsNullOrEmpty(pacote))
      {
        imports.Add(pacote);
      }

      if (!ep.Parametros.Any())
      {
        return imports;
      }

      foreach (var param in ep.Parametros)
      {
        if (param.CoTipoDado is 0 or TipoDadoEnum.DTO or TipoDadoEnum.TIPO_NAO_MAPEADO)
        {
          continue;
        }
        var pacoteParam = (param.CoTipoDado).GetPacoteImport();
        if (!IsNullOrEmpty(pacoteParam))
        {
          imports.Add(pacoteParam);
        }
      }

      // if (ep.CoTipoDadoRetorno != TipoDadoEnum.DTO)
      // {
      //   return imports;
      // }
      // no service do orquestrador o pacote é o do parent e não o do microsserviço
      // var nomeClasse = ep.IdDtoClasseNavigation?.NoDtoClasse.InicialMaiuscula() ??
      //   "erroAoRecuperarNomeClasse";
      //
      // var nomePacote = ep.IdMicrosservicoNavigation?.NoPacote ?? "erroAoRecuperarNomePacote";
      //
      // imports.Add($"{nomePacote}.{ep.IdMicrosservicoNavigation?.NoMicrosservico}.dto.{nomeClasse}");
    }

    return imports;
  }

  // private ICollection<Comando> RecuperarComandosNaoInternos(int spId)
  //   => _comandoService.FindByStoredProcedureId(spId, false);

  // private ICollection<Variavel> RecuperarVariaveis(int spId)
  //   => _variavelService.FindByStoredProcedureId(spId);

  private static string RetornoMetodo(StoredProcedure sp)
  {
    if (!PossuiRetorno(sp))
    {
      return "void";
    }

    var tipo = sp.CoTipoDadoRetorno == TipoDadoEnum.DTO ?
      //var tipo = sp.CoTipoDadoRetorno == TipoDadoEnum.DTO.GetCodigo() ?
      sp.IdDtoClasseNavigation?.NoDtoClasse.InicialMaiuscula() :
      sp.CoTipoDadoRetorno!.GetNome();

    tipo = IsNullOrEmpty(tipo) ? "ErroNomeClasse" : tipo;

    return sp.SnRetornoLista == true ? Concat("List<", tipo, ">") : tipo;
  }

  private string NomeClasseRetorno(StoredProcedure sp)
  {
    //if (sp.CoTipoDadoRetorno == TipoDadoEnum.DTO.GetCodigo())
    if (sp.CoTipoDadoRetorno == TipoDadoEnum.DTO)
    {
      var nomeClasse = _classeDTOService.GetNoDtoClasseById(sp.IdDtoClasse!.Value);
      return nomeClasse!.InicialMaiuscula();
    }
    var nome = sp.CoTipoDadoRetorno!.GetNome();
    return IsNullOrEmpty(nome) ? "ErroNomeClasse" : nome;
  }

  private static string TipoNomeParametros(StoredProcedure sp)
  { // Integer empcod, Integer empano, Integer momcod
    var parametros = sp.GetParametrosStoredProcedure();
    if (!parametros.Any())
    {
      return "";
    }
    var texto = new StringBuilder();
    texto.Append(parametros.Select(p => FormatTipoNome(p.CoTipoDado, p.NoVariavel))
     .Aggregate((a, b) => a + ", " + b));
    return texto.ToString();
  }

  private static string FormatTipoNome(TipoDadoEnum tipo, string nome)
    => Concat(tipo.GetNome(), ' ', nome);

  private static bool PossuiRetorno(StoredProcedure sp)
    //=> sp.CoTipoDadoRetorno != TipoDadoEnum.VOID.GetCodigo();
    => sp.CoTipoDadoRetorno != TipoDadoEnum.VOID;

  private static string UrlParametros(StoredProcedure sp)
  { //    + "/" + orgCod + "/" + prgAno + "/" + empAno
    var parametros
      = sp.Variaveis.Where(v => v.CoTipoEscopo == TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE)
       .OrderBy(v => v.NoVariavel).ToList();

    if (!parametros.Any())
    {
      return "";
    }

    var texto = new StringBuilder();
    texto.Append(" + \"/\" + ");
    texto.Append(parametros.Select(p => p.NoVariavel.ToLower())
     .Aggregate((a, b) => a + " + \"/\" + " + b));

    return texto.ToString();
  }

  private static string ParametrosPath(StoredProcedure sp)
  { //    /{empcod}/{empano}/{momcod}
    var parametros = sp.GetParametrosStoredProcedure();
    if (!parametros.Any())
    {
      return "";
    }
    var texto = new StringBuilder();
    texto.Append("/{");
    texto.Append(parametros.Select(p => p.NoVariavel.ToLower())
     .Aggregate((a, b) => a + "}/{" + b));
    texto.Append('}');
    return texto.ToString();
  }

  private static string ParametrosController(StoredProcedure sp)
  { // @PathVariable(value = "empcod") Integer empcod,
    // @PathVariable(value = "empano") Integer empano,
    // @PathVariable(value = "momcod") Integer momcod
    var parametros = sp.GetParametrosStoredProcedure();
    if (!parametros.Any())
    {
      return "";
    }
    var texto = new StringBuilder();
    texto.Append(parametros.Select(p => FormatParametrosController(p.CoTipoDado, p.NoVariavel))
     .Aggregate((a, b) => a + ", " + b));
    return texto.ToString();
  }

  private static string FormatParametrosController(TipoDadoEnum tipo, string nome)
    => $"\n            @PathVariable {tipo.GetNome()} {nome}";

  // var texto = new StringBuilder();
  // //texto.Append("\n        @PathVariable(value = \"");
  // //texto.Append(nome.ToLower());
  // //texto.Append("\") ");
  // texto.Append(tipo?.GetNome());
  // texto.Append(' ');
  // texto.Append(nome);
  //return texto.ToString();
  private static string ParametrosChamada(StoredProcedure sp)
  { // empcod, empano, momcod
    var parametros = sp.GetParametrosStoredProcedure();
    if (!parametros.Any())
    {
      return "";
    }
    var texto = new StringBuilder();
    texto.Append(parametros.Select(p => p.NoVariavel)
     .Aggregate((a, b) => a + ", " + b));
    return texto.ToString();
  }
}
