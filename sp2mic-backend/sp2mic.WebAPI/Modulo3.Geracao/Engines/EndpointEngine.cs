using System.Text;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Info;
using sp2mic.WebAPI.Modulo3.Geracao.Engines.Interfaces;
using static System.String;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo3.Geracao.Engines;

public class EndpointEngine : IEngine<Endpoint, EndpointInfo>
{
  private readonly IEngine<Variavel, VariavelInfo> _variavelEngine;

  public EndpointEngine(IEngine<Variavel, VariavelInfo> variavelEngine)
    => _variavelEngine = variavelEngine;

  public HashSet<EndpointInfo> ConvertDados(HashSet<Endpoint>? endpoints)
  {
    if (endpoints is null)
    {
      throw new ArgumentNullException(nameof (endpoints));
    }
    return endpoints.Select(ConvertDados).ToHashSet();
  }

  public EndpointInfo ConvertDados(Endpoint? ep)
  {
    if (ep is null)
    {
      throw new ArgumentNullException(nameof (ep));
    }
    //_logger.LogInformation(" ConvertDados EndpointId = {Id} ######", ep.Id);
    //ep.Parametros = RecuperarParametros(ep.Id);
    //ep.IdMicrosservicoNavigation = RecuperarMicrosservico(ep.IdMicrosservico);
    //ep.IdDtoClasseNavigation = RecuperarClasse(ep.IdDtoClasse);

    var info = new EndpointInfo
    {
      NomeMetodo = ep.NoMetodoEndpoint,
      NomeMetodoParaOrquestrador = ep.NoMetodoEndpoint +
        ep.IdMicrosservicoNavigation?.NoMicrosservico.InicialMaiuscula(),
      IdMicrosservico = ep.IdMicrosservico ?? 0,
      NomeMicrosservico = ep.IdMicrosservicoNavigation?.NoMicrosservico.InicialMaiuscula(),
      NomeMicrosservicoInicialMinuscula = ep.IdMicrosservicoNavigation?.NoMicrosservico
       .InicialMinuscula(),
      RetornoMetodo = RetornoMetodo(ep),
      //info.IsRetornoVoid = IsRetornoVoid(ep);
      PossuiRetorno = PossuiRetorno(ep),
      IsRetornoLista = ep.SnRetornoLista,
      IsRetornoDTO = GetIsRetornoDto(ep),
      TipoNomeParametros = TipoNomeParametros(ep),
      TipoQueryMethod = ep.CoTipoSqlDml.GetDescricaoComplementar(),
      TipoHttpMapping = ep.CoTipoSqlDml.GetDescricao(),
      NomeMetodoComInicialMinuscula = ep.NoMetodoEndpoint?.InicialMinuscula(),
      NomeMetodoComInicialMaiuscula = ep.NoMetodoEndpoint?.InicialMaiuscula(),
      Path = ep.NoPath ?? "path-endpoint",
      ParametrosPath = ParametrosPath(ep),
      //  info.NomeMetodoTodoMinusculo = ep.NoMetodoEndpoint.ToLower());
      UrlParametros = UrlParametros(ep),
      NomeClasseRetorno = NomeClasseRetorno(ep),
      Texto = GetTexto(ep),
      ParametrosChamada = ParametrosChamada(ep),
      ParametrosController = ParametrosController(ep),
      AtributosCast = GetAtributosCast(ep),
      ImportsController = GetImportsController(ep),
      ImportsRepository = GetImportsRepository(ep),
      Parametros = _variavelEngine.ConvertDados(ep.Parametros.ToHashSet())
    };
    //var mInfo = _microsservicoEngine.ConvertDados(ep.IdMicrosservicoNavigation);
    //mInfo.WithEndpoints(null);
    //info.Microsservico(mInfo);

    return info;
  }

  private static string GetAtributosCast(Endpoint ep)
  {
    if (!GetIsRetornoDto(ep))
    {
      return "";
    }
    var i = 0;
    var listaCasting = ep.IdDtoClasseNavigation?.Atributos
     .Select(atr => $"({atr.CoTipoDado.GetNome()}) row[{i++}]").ToList();
    //(String) row[0], (String) row[1], (String) row[2]
    // TODO verificar pq a classe 40 nao trouxeo os atributos
    if (listaCasting is null || !listaCasting.Any())
    {
      return "";
    }
    return listaCasting!.Aggregate((a, b) => a + ", " + b);
  }

  private static bool GetIsRetornoDto(Endpoint ep)
    => ep.CoTipoDadoRetorno == TipoDadoEnum.DTO;

  private static HashSet<string> GetImportsController(Endpoint ep)
  {
    var imports = new HashSet<string>();
    if (ep.SnRetornoLista == true)
    {
      imports.Add("java.util.List");
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
      if (param.CoTipoDado is 0 || ep.CoTipoDadoRetorno == TipoDadoEnum.DTO || ep.CoTipoDadoRetorno == TipoDadoEnum.TIPO_NAO_MAPEADO)
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
      return imports;
    }
    var pacote = ep.CoTipoDadoRetorno.GetPacoteImport();
    if (!IsNullOrEmpty(pacote))
    {
      imports.Add(pacote);
    }
    return imports;
  }

  private static HashSet<string> GetImportsRepository(Endpoint ep)
  {
    var imports = new HashSet<string>();
    if (ep.SnRetornoLista == true)
    {
      imports.Add("java.util.List");
      imports.Add("java.util.ArrayList");
    }
    if (ep.CoTipoDadoRetorno == TipoDadoEnum.TIPO_NAO_MAPEADO ||
      ep.CoTipoDadoRetorno == TipoDadoEnum.DTO)
    {
      imports.Add("");
    }
    var pacote = ep.CoTipoDadoRetorno.GetPacoteImport();
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
    return imports;
  }

  // private Microsservico RecuperarMicrosservico(int? idMicrosservico)
  //   => _microsservicoService.FindById(idMicrosservico!.Value);

  // private DtoClasse RecuperarClasse(int? idClasse) => _classeDTOService.FindById(idClasse!.Value);

  /*private ICollection<Variavel> RecuperarParametros (int idEndpoint)
  => _variavelService.FindByEndpointId(idEndpoint);*/

  // private static string GetTexto (Endpoint ep)
  //   => IsNullOrEmpty(ep.TxEndpoint) ? "Endpoint without information." : ep.TxEndpoint;
  private static string GetTexto(Endpoint ep)
  {
    if (IsNullOrEmpty(ep.TxEndpointTratado))
    {
      return IsNullOrEmpty(ep.TxEndpoint) ? "Endpoint sem informação." : ep.TxEndpoint;
    }
    return ep.TxEndpointTratado;
  }

  private static string RetornoMetodo(Endpoint ep)
  {
    if (!PossuiRetorno(ep))
    {
      return "void";
    }
    var tipo = ep.CoTipoDadoRetorno == TipoDadoEnum.DTO ?
      ep.IdDtoClasseNavigation?.NoDtoClasse.InicialMaiuscula() : ep.CoTipoDadoRetorno.GetNome();
    tipo = IsNullOrEmpty(tipo) ? "ErroNomeClasse" : tipo;
    return ep.SnRetornoLista == true ? Concat("List<", tipo, ">") : tipo;
  }

  private static string NomeClasseRetorno(Endpoint ep)
  {
    var nome = ep.CoTipoDadoRetorno == TipoDadoEnum.DTO ?
      ep.IdDtoClasseNavigation?.NoDtoClasse.InicialMaiuscula() : ep.CoTipoDadoRetorno.GetNome();

    return IsNullOrEmpty(nome) ? "ErroNomeClasse" : nome;
  }

  private string TipoNomeParametros(Endpoint ep)
  { // Integer empcod, Integer empano, Integer momcod
    var parametros = ep.GetParametros();

    if (!parametros.Any())
    {
      return Empty;
    }

    var texto = new StringBuilder();
    texto.Append(parametros.Select(p => FormatTipoNome(p.CoTipoDado, p.NoVariavel))
     .Aggregate((a, b) => a + ", " + b));

    return texto.ToString();
  }

  private static string FormatTipoNome(TipoDadoEnum? tipo, string nome)
    => Concat(tipo?.GetNome(), ' ', nome);

  //private bool IsRetornoVoid (Endpoint ep) => ep.CoTipoDadoRetorno == TipoDadoEnum.VOID;

  private static bool PossuiRetorno(Endpoint ep) => ep.CoTipoDadoRetorno != TipoDadoEnum.VOID;

  private static string ParametrosController(Endpoint ep)
  { // @PathVariable(value = "empcod") Integer empcod,
    // @PathVariable(value = "empano") Integer empano,
    // @PathVariable(value = "momcod") Integer momcod
    var parametros = ep.GetParametros();

    if (!parametros.Any())
    {
      return Empty;
    }

    var texto = new StringBuilder();
    texto.Append(parametros.Select(p => FormatParametrosController(p.CoTipoDado, p.NoVariavel))
     .Aggregate((a, b) => a + ", " + b));

    return texto.ToString();
  }

  private static string FormatParametrosController(TipoDadoEnum? tipo, string nome)
  {
    var texto = new StringBuilder();
    texto.Append("\n        @PathVariable(value = \"");
    texto.Append(nome);
    texto.Append("\") ");
    texto.Append(tipo?.GetNome());
    texto.Append(' ');
    texto.Append(nome);
    return texto.ToString();
  }

  private static string ParametrosChamada(Endpoint ep)
  { // empcod, empano, momcod
    var parametros = ep.GetParametros();

    if (!parametros.Any())
    {
      return Empty;
    }

    var texto = new StringBuilder();
    texto.Append(parametros.Select(p => p.NoVariavel)
     .Aggregate((a, b) => a + ", " + b));

    return texto.ToString();
  }

  private string UrlParametros(Endpoint ep)
  { //  + "/" + orgCod + "/" + prgAno + "/" + empAno
    var parametros = ep.GetParametros();

    if (!parametros.Any())
    {
      return "";
    }

    var texto = new StringBuilder();
    texto.Append(" + \"/\" + ");
    texto.Append(parametros.Select(p => p.NoVariavel)
     .Aggregate((a, b) => a + " + \"/\" + " + b));

    return texto.ToString();
  }

  private string ParametrosPath(Endpoint ep)
  { // /{empcod}/{empano}/{momcod}
    var parametros = ep.GetParametros();

    if (!parametros.Any())
    {
      return Empty;
    }

    var texto = new StringBuilder();
    texto.Append("/{");
    texto.Append(parametros.Select(p => p.NoVariavel)
     .Aggregate((a, b) => a + "}/{" + b));

    texto.Append('}');
    return texto.ToString();
  }
}
