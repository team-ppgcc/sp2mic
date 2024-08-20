using Newtonsoft.Json;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.Mock;

public class SpsPacCadastroAnoVigente
{
  private StoredProcedure Procedure {get; set;}

  public SpsPacCadastroAnoVigente ()
  {
    const string txDefinicao = @"USE [dbp_54808_sig2000] GO
      /****** Object:  StoredProcedure [dbo].[sps_PACCadastroAnoVigente]    Script Date: 12/02/2021 20:41:37 ******/
      SET ANSI_NULLS ON
    GO
      SET QUOTED_IDENTIFIER ON
    GO

      CREATE PROCEDURE [dbo].[sps_PACCadastroAnoVigente]
    /*
      Criado por Lucianno Luis em 23/10/2007
      Retorna o Ano Vigente para o Ciclo do PAC
    */
    AS
      SELECT
    ANEAnoVigente AS anoVigente
      FROM
    PACAnoExercicio AS pacanoexercicio
      FOR XML AUTO, ELEMENTS
    GO";
    Procedure = new("dbo", "sps_PACCadastroAnoVigente", txDefinicao)
    {
      TxDefinicaoTratada = @"CREATE PROCEDURE [dbo].[sps_PACCadastroAnoVigente] AS SELECT ANEAnoVigente AS anoVigente FROM PACAnoExercicio AS pacanoexercicio FOR XML AUTO, ELEMENTS GO",
      CoTipoDadoRetorno = TipoDadoEnum.INTEGER,
      SnRetornoLista = false,
      SnSucessoParser = true,
      IdDtoClasse = 0,
      Comandos = new List<Comando>(),
      Endpoints = new List<Endpoint>(),
      Variaveis = new List<Variavel>()
    };

    Endpoint endPoint1 = new Endpoint
    {
      NoMetodoEndpoint = "",
      TxEndpoint = @"    SELECT
        ANEAnoVigente AS anoVigente
    FROM
        PACAnoExercicio AS pacanoexercicio",
      TxEndpointTratado = @"    SELECT
        ANEAnoVigente AS anoVigente
    FROM
        PACAnoExercicio AS pacanoexercicio",
      CoTipoSqlDml = TipoEndpointEnum.SELECT,
      CoTipoDadoRetorno = TipoDadoEnum.DTO,
      SnRetornoLista = true,
      IdMicrosservicoNavigation = null,
      IdStoredProcedureNavigation = Procedure,
      IdDtoClasseNavigation = null,
      Parametros = new List<Variavel>()
    };

    Comando comando1 = new Comando
    {
      TxComando = @"    SELECT
        ANEAnoVigente AS anoVigente
    FROM
        PACAnoExercicio AS pacanoexercicio",
      TxComandoTratado = @"    SELECT
        ANEAnoVigente AS anoVigente
    FROM
        PACAnoExercicio AS pacanoexercicio",
      CoTipoComando = TipoComandoEnum.ENDPOINT,
      NuOrdemExecucao = 1,
      VlAtribuidoVariavel = null,
      IdStoredProcedureNavigation = Procedure,
      IdComandoOrigemNavigation = null,
      IdExpressaoNavigation = null,
      //IdVariavelNavigation = null,
      SnCondicaoOrigem = false,
      AsVariaveisDesseComando = new List<ComandoVariavel>(),
      ComandosInternos = new List<Comando>(),
      IdEndpointNavigation = endPoint1
    };

    Procedure.Comandos.Add(comando1);
  }

  public override string ToString () => JsonConvert.SerializeObject(this);
}
