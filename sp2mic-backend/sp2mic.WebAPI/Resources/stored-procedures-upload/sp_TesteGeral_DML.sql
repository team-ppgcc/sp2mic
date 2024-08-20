USE [dbp]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[sp_TesteGeral_DML]
        @USUCod int,
        @NIVCod int,
        @ORGAno int = 0,
        @EMPAno int,
        @EMPCod int,
        @AGRCod CHAR (4),
        @PRGAno CHAR (4),
        @ORGCod CHAR (5),
        @MOMCod int,
        @CEPCod int,
        @EMPSNOrc int,
        @UNICod char(5),
        @UNIAno int,
        @EMPDsc char(200),
        @SEGCod int,
        @EMPValor2007 money,
        @EMPValorPACTotal money,
        @EMPValorPAC2 money,
        @EMPCodigoEmpreend char(10),
        @STSCod int,
        @EMPSNExcluido int,
        @ESECod int,
        @EMPCusto money,
        @JustIFicativa char(800),
        @EMPUsuarioAlt int,
        @EMPSisSiglaAlt char(15),
        @EMPDataEnvioEmail char(20),
        @EMPSiglaAlt  char(4)
AS
  BEGIN
    DECLARE @Sigla varchar(4)
    DECLARE @Num varchar(5)
    DECLARE @CodEMP varchar(10)
    DECLARE @UNITpoCod CHAR(1)
    DECLARE @HABCod int
    DECLARE @cod int
    SET @UNITpoCod = 'U'
------------------------------------------------------------------------------------------------------------------------
    IF (@ORGAno = 0)
      BEGIN
        SELECT TOP 1 @ORGAno = ANEAnoCorrente
        FROM PACAnoExercicio
      END
------------------------------------------------------------------------------------------------------------------------
    SELECT @HABCod = HABCod
      FROM nivelacesso nivelacesso
      WHERE nivelacesso.nivcod = @NIVCod
------------------------------------------------------------------------------------------------------------------------
    IF @HABCod = 1
      BEGIN
        SELECT distinct
            orgaos.ORGcod AS codigo,
            orgaos.ORGdsc AS descricao,
            orgaos.TPOCod AS tipo,
            orgaos.ORGCod +' - ' + orgaos.ORGdsc AS descricaoCompleta,
            orgaos.ORGAno AS ano
        FROM
            Orgao AS orgaos
                INNER JOIN  USUARIONIVEL UN ON
                        UN.USNChave1 = orgaos.ORGCod AND
                        UN.USNChave2 = orgaos.TPOCod
                INNER JOIN Unidade AS unidade ON
                        unidade.UNIAno = orgaos.ORGAno
                    AND unidade.ORGCod = orgaos.ORGCod
                    AND unidade.TPOCod = orgaos.TPOCod
                INNER JOIN Acao AS acao ON
                    unidade.UNIAno = acao.PRGAno AND
                    unidade.UNICod = acao.ORGCod AND
                    unidade.UNITpoCod = acao.TPOCod
                INNER JOIN AgrupamentoProgramaAcao agp ON
                        agp.PRGAno = acao.PRGAno AND
                        agp.PRGCod = acao.PRGCod AND
                        agp.ACACod = acao.ACACod AND
                        ((agp.SACCod = '0000' AND agp.ORGCod=unidade.ORGCod AND agp.TPOCod=unidade.TPOCod) OR
                         (agp.SACCod<>'0000' AND agp.UNICod=acao.ORGCod AND agp.UNITPOCod=acao.TPOCod))
        WHERE
              orgaos.ORGAno = @ORGAno AND
              USNFim is null AND
              UN.USUCOD=@USUCod AND
              UN.NIVCOD = @NIVCod AND
              (orgaos.orgcod IN (SELECT distinct usuarionivel.USNChave1
                                 FROM usuarionivel usuarionivel
                                 WHERE usuarionivel.nivcod = @NIVCod)) AND
            cast(agp.AGRCod AS int) IN (12, 16)
        ORDER BY ORGDsc
      END
    ELSE
      BEGIN
        SELECT DISTINCT
          ORGcod AS codigo,
          ORGdsc AS descricao,
          TPOCod AS tipo,
          ORGCod +' - ' + ORGdsc AS descricaoCompleta, ORGAno AS ano
        FROM USUARIONIVEL UN
             INNER JOIN ORGAO orgaos ON UN.USNChave1 = Orgaos.ORGCod AND
                                               UN.USNChave2 = Orgaos.TPOCod
        WHERE UN.USUCOD=@USUCod AND
              UN.NIVCOD = @NIVCod AND
              Orgaos.ORGAno = @ORGAno AND
              USNFim is null
              ORDER BY ORGDsc
      END

    IF (SELECT len(EMPCodigoEmpreend) FROM PACEmpreendimento
                                      WHERE EMPCod = @EMPCod AND
                                            EMPAno = @EMPAno AND
                                            MOMCod = @MOMCod) < 2
      BEGIN
        SET @Sigla = (SELECT RTRIM(SOGSigla)
                      FROM PACSiglaOrgao
                      WHERE SOGCodOrgao = (SELECT DISTINCT ORGCod
                                           FROM Unidade
                                           WHERE UNICod = @UNICod AND
                                                 UNIAno = @EMPAno AND
                                                 UNITpoCod = @UNITpoCod))
        SET @Num = (SELECT RTRIM(EMPCodigoEmpreend)
                    FROM PACEmpreendimento
                    WHERE EMPCodigoEmpreend = @Sigla AND
                          EMPAno = @EMPAno)
        SET @CodEMP = @Sigla + '.' + @Num
        UPDATE PACEmpreendimento SET EMPCodigoEmpreend = @CodEMP
                                 WHERE EMPCod = @EMPCod AND
                                       EMPAno = @EMPAno AND
                                       MOMCod = @MOMCod
      END
------------------------------------------------------------------------------------------------------------------------
    DELETE FROM AgrupamentoProgramaAcao
           WHERE AGRCod = @AGRCOD AND
                 PRGAno = @PRGAno AND
                 ORGCod = @ORGCod
------------------------------------------------------------------------------------------------------------------------
    UPDATE PACFuncional SET FALSNExcluido  = 0, FALUsuarioAlt  = @EMPUsuarioAlt, FALSisSiglaAlt = @EMPSiglaAlt
                        WHERE EMPCod = @EMPCod AND
                              EMPAno = @EMPAno
------------------------------------------------------------------------------------------------------------------------
    UPDATE PACContrato SET CONSNExcluido  = 0, CONUsuarioAlt  = @EMPUsuarioAlt, CONSisSiglaAlt = @EMPSiglaAlt
                       WHERE EMPCod = @EMPCod AND
                             EMPAno = @EMPAno
------------------------------------------------------------------------------------------------------------------------
    UPDATE PACEmpreendimento SET EMPSNExcluido  = 0, EMPUsuarioAlt  = @EMPUsuarioAlt, EMPSisSiglaAlt = @EMPSiglaAlt
                             WHERE EMPCod = @EMPCod AND
                                   EMPAno = @EMPAno
------------------------------------------------------------------------------------------------------------------------
    UPDATE PACEmpreendimentoEspelho SET EMPSNExcluido  = 0, EMPUsuarioAlt  = @EMPUsuarioAlt, EMPSisSiglaAlt = @EMPSiglaAlt
                                    WHERE EMPCod = @EMPCod AND
                                          EMPAno = @EMPAno
------------------------------------------------------------------------------------------------------------------------
    IF @cod = 0
      BEGIN
        SET @cod = (SELECT max(empcod) + 1 FROM pacempreendimento)
------------------------------------------------------------------------------------------------------------------------
        INSERT INTO pacempreendimento (empcod, empano, momcod, cepcod, EMPSNOrcamentaria, UNICod, UNITpoCod, UNIAno,
                                       EMPDsc, SEGCod, EMPValor2007, EMPValorPACTotal, EMPValorPAC2, EMPCodigoEmpreend,
                                       STSCod, EMPSNExcluido, ESECod, EMPCusto, EMPJustIFicativaDevolucao, EMPUsuarioAlt,
                                       EMPSisSiglaAlt, EMPDataAlt, EMPAnoCriacao, EMPDataEnvioEmail)
        VALUES (@cod, @EMPAno, @MOMCod, @CEPCod, @EMPSNOrc, @UNICod, @UNITpoCod, @UNIAno, @EMPDsc, @SEGCod,
                @EMPValor2007, @EMPValorPACTotal, @EMPValorPAC2, @EMPCodigoEmpreend, @STSCod, @EMPSNExcluido, @ESECod,
                @EMPCusto, @JustIFicativa, @EMPUsuarioAlt,
                @EMPSisSiglaAlt, getdate(), YEAR(getdate()),  CONVERT(SMALLDATETIME, @EMPDataEnvioEmail,103))
      END
    ELSE
      BEGIN
        UPDATE pacempreendimento
          SET CEPCod        = (CASE WHEN @CEPCod IS NULL THEN CEPCod ELSE @CEPCod END),
              EMPSNOrcamentaria = (CASE WHEN @EMPSNOrc IS NULL THEN EMPSNOrcamentaria ELSE @EMPSNOrc END),
              UNICod            = (CASE WHEN @UNICod IS NULL THEN UNICod ELSE @UNICod END),
              UNITpoCod         = (CASE WHEN @UNITpoCod IS NULL THEN UNITpoCod ELSE @UNITpoCod END),
              UNIAno            = (CASE WHEN @UNIAno IS NULL THEN UNIAno ELSE @UNIAno END),
              EMPDsc            = (CASE WHEN @EMPDsc IS NULL THEN EMPDsc ELSE @EMPDsc END),
              SEGCod            = (CASE WHEN @SEGCod IS NULL THEN SEGCod ELSE @SEGCod END),
              EMPValor2007      = @EMPValor2007,
              EMPValorPACTotal  = @EMPValorPACTotal,
              EMPValorPAC2      = @EMPValorPAC2,
              EMPCodigoEmpreend = (CASE WHEN @EMPCodigoEmpreend IS NULL THEN EMPCodigoEmpreend ELSE @EMPCodigoEmpreend END),
              STSCod            = (CASE WHEN @STSCod IS NULL THEN STSCod ELSE @STSCod END),
              EMPSNExcluido     = (CASE WHEN @EMPSNExcluido IS NULL THEN EMPSNExcluido ELSE @EMPSNExcluido END),
              ESECod            = (CASE WHEN @ESECod IS NULL THEN ESECod ELSE @ESECod END),
              EMPCusto          = (CASE WHEN @EMPCusto IS NULL THEN EMPCusto ELSE @EMPCusto END),
              EMPSisSiglaAlt    = (CASE WHEN @EMPSisSiglaAlt IS NULL THEN EMPSisSiglaAlt ELSE @EMPSisSiglaAlt END),
              EMPUsuarioAlt     = @EMPUsuarioAlt,
              EMPDataAlt        = getdate(),
              EMPJustIFicativaDevolucao = (CASE WHEN @JustIFicativa IS NULL THEN EMPJustIFicativaDevolucao ELSE @JustIFicativa END),
              EMPDataEnvioEmail = (CASE WHEN @EMPDataEnvioEmail IS NULL THEN EMPDataEnvioEmail ELSE  CONVERT(SMALLDATETIME,@EMPDataEnvioEmail,103) END)
          WHERE
              EMPCod = @EMPCod AND
              EMPAno = @EMPAno AND
              MOMCod = @MOMCod
      END
------------------------------------------------------------------------------------------------------------------------
    SELECT empcod AS empcod, empano AS empano, momcod AS momcod, rtrim(empcodigoempreend) AS sigla,
           empreendimento.stscod AS stscod, rtrim(s.stsdsc) AS descricaostatus
    FROM PACEmpreendimento AS empreendimento
    INNER JOIN PACStatus AS s ON empreendimento.stscod = s.stscod
    WHERE @empcod = empcod AND
          @empano = empano AND
          @momcod = momcod
  END
GO
