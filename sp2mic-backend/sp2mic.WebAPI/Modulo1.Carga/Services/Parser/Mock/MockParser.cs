using sp2mic.WebAPI.CrossCutting.Exceptions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo1.Carga.Dtos;
using sp2mic.WebAPI.Modulo1.Carga.Services.Interfaces;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.Mock;

public class MockParser : ITradutorBanco
{
  private static MockParser? _instance;
  //private readonly ILogger<MockParser> _logger;

  private readonly List<StoredProcedure> _procedures = new();

  private MockParser()
  {
    //var host = Host.CreateDefaultBuilder().Build();
    //var logger = host.Services.GetRequiredService<ILogger<MockParser>>();
    //_logger = logger ?? throw new ArgumentNullException(nameof (logger));

    _procedures.Add(CriarSPEXCLUIR_AGRUPAMENTO_PROGRAMA_ACAO());
    _procedures.Add(CriarSPspa_PACApoioAdicionaAtualizaFiltro());
    _procedures.Add(CriarSPspa_PACApoioSetGerarCodigoEmpreendimento());
    _procedures.Add(CriarSPspe_PACMonitoramentoInterSetOrgaoRestricao());
  }

  public StoredProcedure FetchProcedure(CargaDto dto, string nome)
  {
    var sp = _procedures.Where(sp => sp.NoStoredProcedure.Equals(nome)).ElementAtOrDefault(0);
    return sp ?? throw new EntidadeNaoEncontradaException("Stored Procedure not found.");
  }

  public List<StoredProcedure> FetchProcedures(CargaDto dto) => _procedures;

  public IEnumerable<ParDto> FetchNomesProcedures(CargaDto dto)
  {
    return _procedures.Select((sp, id) => new ParDto(id, sp.NoStoredProcedure)).ToList();
  }

  public IEnumerable<StoredProcedure> FetchProceduresSelecionadas(CargaDto dto)
  {
    return _procedures
     .Where(sp => dto.NomesProcedures.Exists(p => p.Nome.Equals(sp.NoStoredProcedure)))
     .ToList();
  }

  public static MockParser GetInstance() { return _instance ??= new MockParser(); }

  private static StoredProcedure CriarSPEXCLUIR_AGRUPAMENTO_PROGRAMA_ACAO()
  {
    StoredProcedure retorno
      = new("dbo", "EXCLUIR_AGRUPAMENTO_PROGRAMA_ACAO", "Jogar aqui o texto completo da procedure")
      {
        SnSucessoParser = true,
        TxDefinicaoTratada = "Jogar aki o texto completo sem espaços e sem comentários",
        SnRetornoLista = false,
        CoTipoDadoRetorno = TipoDadoEnum.VOID
      };
    //Definindo parâmetros
    Variavel pbuilder1 = new()
    {
      NoVariavel = "@AGRCod",
      CoTipoDado = TipoDadoEnum.STRING,
      NuTamanho = 4,
      IdStoredProcedureNavigation = retorno
    };
    Variavel pbuilder2 = new()
    {
      NoVariavel = "@PRGAno",
      CoTipoDado = TipoDadoEnum.STRING,
      NuTamanho = 4,
      IdStoredProcedureNavigation = retorno
    };
    Variavel pbuilder3 = new()
    {
      NoVariavel = "@ORGCod",
      CoTipoDado = TipoDadoEnum.STRING,
      NuTamanho = 5,
      IdStoredProcedureNavigation = retorno
    };
    retorno.Variaveis.Add(pbuilder1);
    retorno.Variaveis.Add(pbuilder2);
    retorno.Variaveis.Add(pbuilder3);
    //Definindo Comandos
    Endpoint eBuilder1 = new()
    {
      TxEndpoint = @"DELETE FROM
                              AgrupamentoProgramaAcao
                              WHERE
                                 AGRCod = @AGRCOD
                                 AND PRGAno = @PRGAno
                               AND ORGCod = @ORGCod",
      TxEndpointTratado
        = "DELETE FROM AgrupamentoProgramaAcao WHERE AGRCod = @AGRCOD AND PRGAno = @PRGAno AND ORGCod = @ORGCod",
      CoTipoSqlDml = TipoEndpointEnum.DELETE,
      CoTipoDadoRetorno = TipoDadoEnum.VOID,
      IdStoredProcedureNavigation = retorno
    };

    var ep = eBuilder1;
    Variavel vbuilder1 = new()
    {
      NoVariavel = "@AGRCod", CoTipoDado = TipoDadoEnum.STRING, NuTamanho = 4
    };
    vbuilder1.EndpointsQueContemEssaVariavelComoParametro.Add(ep);
    Variavel vbuilder2 = new()
    {
      NoVariavel = "@PRGAno", CoTipoDado = TipoDadoEnum.STRING, NuTamanho = 4
    };
    vbuilder2.EndpointsQueContemEssaVariavelComoParametro.Add(ep);
    Variavel vbuilder3 = new()
    {
      NoVariavel = "@ORGCod", CoTipoDado = TipoDadoEnum.STRING, NuTamanho = 5
    };
    vbuilder3.EndpointsQueContemEssaVariavelComoParametro.Add(ep);
    ep.Parametros.Add(vbuilder1);
    ep.Parametros.Add(vbuilder2);
    ep.Parametros.Add(vbuilder3);
    Comando cBuilder = new()
    {
      IdStoredProcedureNavigation = retorno,
      CoTipoComando = TipoComandoEnum.ENDPOINT,
      NuOrdemExecucao = 1,
      IdEndpointNavigation = ep
    };
    retorno.Comandos.Add(cBuilder);
    return retorno;
  }

  private static StoredProcedure CriarSPspa_PACApoioAdicionaAtualizaFiltro()
  {
    const string txDefinicao = @"CREATE PROCEDURE [dbo].[spa_PACApoioAdicionaAtualizaFiltro]
    @USUCod as int,
    @NIVCod as int,
    @Modulo as char(50),
    @Apelido as char(50),
    @Filtro as Ntext
AS
BEGIN
    DECLARE @DataAtual as smalldatetime
    set @DataAtual = GETDATE()
    IF exists (select 1 from PacFiltro
                where
                    USUCod = @USUCod and
                    NIVCod = @NIVCod and
                    FLTModulo = @Modulo and
                    FLTApelido = @Apelido)
        BEGIN
            UPDATE PACFiltro
            SET FLTXml = @Filtro,
                FLTDataHora = @DataAtual
            WHERE
                USUCod = @USUCod and
                NIVCod = @NIVCod and
                FLTModulo = @Modulo and
                FLTApelido = @Apelido
        END
    ELSE
        BEGIN
            INSERT INTO PACFiltro (USUCod,NIVCod,FLTModulo,FLTApelido,FLTXml,FLTDataHora)
            VALUES(@USUCod,@NIVCod,@Modulo,@Apelido,@Filtro,@DataAtual)
        END
END";
    StoredProcedure retorno = new("dbo", "spa_PACApoioAdicionaAtualizaFiltro", txDefinicao)
    {
      SnSucessoParser = true,
      TxDefinicaoTratada = "definição sem espaços e sem comentários",
      SnRetornoLista = false,
      CoTipoDadoRetorno = TipoDadoEnum.VOID
    };
    //Definindo parâmetros
    Variavel pbuilder1 = new()
    {
      NoVariavel = "@USUCod",
      CoTipoDado = TipoDadoEnum.INTEGER,
      IdStoredProcedureNavigation = retorno
    };
    Variavel pbuilder2 = new()
    {
      NoVariavel = "@NIVCod",
      CoTipoDado = TipoDadoEnum.INTEGER,
      IdStoredProcedureNavigation = retorno
    };
    Variavel pbuilder3 = new()
    {
      NoVariavel = "@Modulo",
      CoTipoDado = TipoDadoEnum.STRING,
      NuTamanho = 50,
      IdStoredProcedureNavigation = retorno
    };
    Variavel pbuilder4 = new()
    {
      NoVariavel = "@Apelido",
      CoTipoDado = TipoDadoEnum.STRING,
      NuTamanho = 50,
      IdStoredProcedureNavigation = retorno
    };
    Variavel pbuilder5 = new()
    {
      NoVariavel = "@Filtro", CoTipoDado = TipoDadoEnum.STRING, IdStoredProcedureNavigation = retorno
    };
    retorno.Variaveis.Add(pbuilder1);
    retorno.Variaveis.Add(pbuilder2);
    retorno.Variaveis.Add(pbuilder3);
    retorno.Variaveis.Add(pbuilder4);
    retorno.Variaveis.Add(pbuilder5);
    //Definindo variáveis
    Variavel vbuilder1 = new()
    {
      NoVariavel = "@DataAtual",
      CoTipoDado = TipoDadoEnum.LOCAL_DATE_TIME,
      IdStoredProcedureNavigation = retorno
    };
    ICollection<ComandoVariavel> cvList = new List<ComandoVariavel>();
    ComandoVariavel cv = new();
    cvList.Add(cv);
    Comando cBuilder = new()
    {
      IdStoredProcedureNavigation = retorno,
      CoTipoComando = TipoComandoEnum.DECLARACAO,
      NuOrdemExecucao = 1,
      TxComando = "DECLARE @DataAtual as smalldatetime",
      AsVariaveisDesseComando = cvList
    };
    retorno.Comandos.Add(cBuilder);
    cv.IdComandoNavigation = cBuilder;
    cv.IdVariavelNavigation = vbuilder1;
    cv.NuOrdem = 1;
    Endpoint eBuilder1 = new()
    {
      TxEndpoint = @"UPDATE PACFiltro
            SET FLTXml = @Filtro,
                FLTDataHora = @DataAtual
            WHERE
                USUCod = @USUCod and
                NIVCod = @NIVCod and
                FLTModulo = @Modulo and
                FLTApelido = @Apelido",
      TxEndpointTratado
        = @"UPDATE PACFiltro SET FLTXml = @Filtro, FLTDataHora = @DataAtual WHERE USUCod = @USUCod and NIVCod = @NIVCod and FLTModulo = @Modulo and FLTApelido = @Apelido",
      CoTipoSqlDml = TipoEndpointEnum.UPDATE,
      CoTipoDadoRetorno = TipoDadoEnum.VOID,
      IdStoredProcedureNavigation = retorno
    };
    Comando cBuilder1 = new()
    {
      IdStoredProcedureNavigation = retorno,
      CoTipoComando = TipoComandoEnum.ENDPOINT,
      NuOrdemExecucao = 1,
      IdEndpointNavigation = eBuilder1
    };
    retorno.Comandos.Add(cBuilder1);
    Endpoint eBuilder2 = new()
    {
      TxEndpoint = @"INSERT INTO PACFiltro (USUCod,NIVCod,FLTModulo,FLTApelido,FLTXml,FLTDataHora)
            VALUES(@USUCod,@NIVCod,@Modulo,@Apelido,@Filtro,@DataAtual)",
      TxEndpointTratado
        = @"INSERT INTO PACFiltro (USUCod,NIVCod,FLTModulo,FLTApelido,FLTXml,FLTDataHora VALUES(@USUCod,@NIVCod,@Modulo,@Apelido,@Filtro,@DataAtual)",
      CoTipoSqlDml = TipoEndpointEnum.INSERT,
      CoTipoDadoRetorno = TipoDadoEnum.VOID,
      IdStoredProcedureNavigation = retorno
    };
    Comando cBuilder2 = new()
    {
      IdStoredProcedureNavigation = retorno,
      CoTipoComando = TipoComandoEnum.ENDPOINT,
      NuOrdemExecucao = 1,
      IdEndpointNavigation = eBuilder2
    };
    retorno.Comandos.Add(cBuilder2);
    return retorno;
  }

  private static StoredProcedure CriarSPspa_PACApoioSetGerarCodigoEmpreendimento()
  {
    StoredProcedure retorno
      = new("dbo", "mock_spa_PACApoioSetGerarCodigoEmpreendimento",
        "Jogar aqui o texto completo da procedure")
      {
        SnSucessoParser = true,
        TxDefinicaoTratada = "Jogar aki o texto completo sem espaços e sem comentários",
        SnRetornoLista = false,
        CoTipoDadoRetorno = TipoDadoEnum.VOID
      };
    Endpoint eBuilder1 = new()
    {
      TxEndpoint
        = @"SELECT len(EMPCodigoEmpreend) FROM PACEmpreendimento WHERE EMPCod = @EMPCod and EMPAno = @EMPAno and MOMCod = @MOMCod",
      TxEndpointTratado
        = @"SELECT len(EMPCodigoEmpreend) FROM PACEmpreendimento WHERE EMPCod = @EMPCod and EMPAno = @EMPAno and MOMCod = @MOMCod",
      CoTipoSqlDml = TipoEndpointEnum.SELECT,
      CoTipoDadoRetorno = TipoDadoEnum.STRING,
      SnRetornoLista = false
    };
    retorno.Endpoints.Add(eBuilder1);
    return retorno;
  }

  private static StoredProcedure CriarSPspe_PACMonitoramentoInterSetOrgaoRestricao()
  {
    Console.WriteLine("CriarSPspe_PACMonitoramentoInterSetOrgaoRestricao");
    return new StoredProcedure("dbo", "spe_PACMonitoramentoInterSetOrgaoRestricao",
      "spe_PACMonitoramentoInterSetOrgaoRestricao");
  }
}
