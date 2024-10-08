USE [dbp_54808_sig2000]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spAnaliseDaExecucao2003] @USUCodCOORD char(5),
                                                 @SNCOORDENADOR BIT,
                                                 @PRGCod Char(4),
                                                 @PRGAno Char(4),
                                                 @USUCod int = null
As

    SET NOCOUNT ON


DECLARE @SNTEMFILTRO INT
DECLARE @FTVCOD VARCHAR(20)
DECLARE @STRSQL VARCHAR(8000)
    IF @USUCOD IS NOT NULL
        BEGIN
            SET @FTVCOD = 'TV-' + CONVERT(VARCHAR(10), @USUCOD)
            SET @SNTEMFILTRO = (SELECT COUNT(*)
                                FROM FILTROTREEVIEW
                                WHERE FTVCOD = 'TV-' + CONVERT(VARCHAR(10), @USUCOD))
        END
    ELSE
        BEGIN
            SET @SNTEMFILTRO = 0
        END
    IF @USUCOD IS NULL OR @SNTEMFILTRO = 0
        BEGIN


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 as CANCELADOANO
            INTO #TEMPORARIO
            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         CASE WHEN ACA.SACCOD = '0000' THEN '' ELSE ACA.SACCOD END AS SACCOD,
                         ACA.LOCCOD,
                         ACA.ACADSC,
                         ISNULL(ACA.SACDSC, ACA.ACADSC)                            AS SACDSC,
                         ACA.ACASNRAP,
                         1                                                         AS ORDEM,
                         TAC.TACDSC,
                         PRO.PRODSC,
                         UNM.UNMDSC,
                         ACA.ORGCOD,
                         3                                                         AS NUMCASAS,
                         'Físico'                                                  AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0))                           AS ATUALANO,
                         CASE
                             WHEN ACA.ACASNMETANAOCUMULATIVA = 0 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                                                   AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           LEFT JOIN TIPOACAO TAC (NOLOCK) ON ACA.TACCOD = TAC.TACCOD
                           LEFT JOIN PRODUTO PRO (NOLOCK) ON ACA.PROCOD = PRO.PROCOD
                           LEFT JOIN UNIDADEMEDIDA UNM (NOLOCK) ON ACA.UNMCOD = UNM.UNMCOD
                           INNER JOIN DADOFISICO FIS (NOLOCK) ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                                                 ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.SACCOD, ACA.LOCCOD,
                           ACA.ACADSC, ACA.SACDSC, ACA.ACASNRAP, ACA.ACASNMETANAOCUMULATIVA,
                           TAC.TACDSC, PRO.PRODSC, UNM.UNMDSC, ACA.ORGCOD) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP, ORDEM, TACDSC,
                     PRODSC, UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   CASE WHEN ACA.SACCOD = '0000' THEN '' ELSE ACA.SACCOD END AS SACCOD,
                   ACA.LOCCOD,
                   ACA.ACADSC,
                   ISNULL(ACA.SACDSC, ACA.ACADSC)                            AS SACDSC,
                   ACA.ACASNRAP,
                   2                                                         AS ORDEM,
                   TAC.TACDSC,
                   PRO.PRODSC,
                   UNM.UNMDSC,
                   ACA.ORGCOD,
                   0                                                         AS NUMCASAS,
                   'Financeiro'                                              AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))                            AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADO1, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO2, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO3, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO4, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO5, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO6, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO7, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO8, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO9, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO10, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO11, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO12, 0))
                                                                             AS REALIZADOANO,
                   0                                                         as CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     LEFT JOIN TIPOACAO TAC (NOLOCK) ON ACA.TACCOD = TAC.TACCOD
                     LEFT JOIN PRODUTO PRO (NOLOCK) ON ACA.PROCOD = PRO.PROCOD
                     LEFT JOIN UNIDADEMEDIDA UNM (NOLOCK) ON ACA.UNMCOD = UNM.UNMCOD
                     INNER JOIN DADOFINANCEIRO FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.SACCOD, ACA.LOCCOD,
                     ACA.ACADSC, ACA.SACDSC, ACA.ACASNRAP,
                     TAC.TACDSC, PRO.PRODSC, UNM.UNMDSC,
                     ACA.ORGCOD

            UNION


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 as CANCELADOANO
            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         CASE WHEN ACA.SACCOD = '0000' THEN '' ELSE ACA.SACCOD END AS SACCOD,
                         ACA.LOCCOD,
                         ACA.ACADSC,
                         ISNULL(ACA.SACDSC, ACA.ACADSC)                            AS SACDSC,
                         ACA.ACASNRAP,
                         1                                                         AS ORDEM,
                         TAC.TACDSC,
                         PRO.PRODSC,
                         UNM.UNMDSC,
                         ACA.ORGCOD,
                         3                                                         AS NUMCASAS,
                         'Físico (Restos a Pagar)'                                 AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0))                           AS ATUALANO,
                         CASE
                             WHEN 1 = 1 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                                                   AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           LEFT JOIN TIPOACAO TAC (NOLOCK) ON ACA.TACCOD = TAC.TACCOD
                           LEFT JOIN PRODUTO PRO (NOLOCK) ON ACA.PROCOD = PRO.PROCOD
                           LEFT JOIN UNIDADEMEDIDA UNM (NOLOCK) ON ACA.UNMCOD = UNM.UNMCOD
                           INNER JOIN DADOFISICORAP FIS (NOLOCK)
                                      ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                         ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC, ACA.ACACOD, ACA.SACCOD, ACA.LOCCOD,
                           ACA.ACADSC, ACA.SACDSC, ACA.ACASNRAP,
                           ACA.ACASNMETANAOCUMULATIVA, TAC.TACDSC,
                           PRO.PRODSC, UNM.UNMDSC, ACA.ORGCOD) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP, ORDEM, TACDSC,
                     PRODSC, UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   CASE WHEN ACA.SACCOD = '0000' THEN '' ELSE ACA.SACCOD END AS SACCOD,
                   ACA.LOCCOD,
                   ACA.ACADSC,
                   ISNULL(ACA.SACDSC, ACA.ACADSC)                            AS SACDSC,
                   ACA.ACASNRAP,
                   4                                                         AS ORDEM,
                   TAC.TACDSC,
                   PRO.PRODSC,
                   UNM.UNMDSC,
                   ACA.ORGCOD,
                   0                                                         AS NUMCASAS,
                   'Financeiro (Restos a Pagar)'                             AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))                            AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADOANO, 0))                        AS REALIZADOANO,
                   SUM(ISNULL(FINVLRCANCELADO1, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO2, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO3, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO4, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO5, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO6, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO7, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO8, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO9, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO10, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO11, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO12, 0))
                                                                             AS CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     LEFT JOIN TIPOACAO TAC (NOLOCK) ON ACA.TACCOD = TAC.TACCOD
                     LEFT JOIN PRODUTO PRO (NOLOCK) ON ACA.PROCOD = PRO.PROCOD
                     LEFT JOIN UNIDADEMEDIDA UNM (NOLOCK) ON ACA.UNMCOD = UNM.UNMCOD
                     INNER JOIN DADOFINANCEIRORAP FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.SACCOD, ACA.LOCCOD,
                     ACA.ACADSC, ACA.SACDSC, ACA.ACASNRAP,
                     ACA.ORGCOD, TAC.TACDSC, PRO.PRODSC, UNM.UNMDSC

            UNION


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 as CANCELADOANO
            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         '0000'                          AS SACCOD,
                         'XXXX'                          AS LOCCOD,
                         ACA.ACADSC,
                         ' '                             AS SACDSC,
                         ACA.ACASNRAP,
                         1                               AS ORDEM,
                         ' '                             AS TACDSC,
                         ' '                             AS PRODSC,
                         ' '                             AS UNMDSC,
                         ACA.ORGCOD,
                         3                               AS NUMCASAS,
                         'Físico'                        AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0)) AS ATUALANO,
                         CASE
                             WHEN ACA.ACASNMETANAOCUMULATIVA = 0 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                         AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           INNER JOIN DADOFISICO FIS (NOLOCK) ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                                                 ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.ACADSC, ACA.ORGCOD,
                           ACA.ACASNRAP, ACA.ACASNMETANAOCUMULATIVA) DADOS

            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP,
                     ORDEM, TACDSC, PRODSC,
                     UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 as CANCELADOANO
            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         '0000'                          AS SACCOD,
                         'XXXX'                          AS LOCCOD,
                         ACA.ACADSC,
                         ' '                             AS SACDSC,
                         ACA.ACASNRAP,
                         1                               AS ORDEM,
                         ' '                             AS TACDSC,
                         ' '                             AS PRODSC,
                         ' '                             AS UNMDSC,
                         '-----'                            ORGCOD,
                         3                               AS NUMCASAS,
                         'Físico'                        AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0)) AS ATUALANO,
                         CASE
                             WHEN ACA.ACASNMETANAOCUMULATIVA = 0 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                         AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           INNER JOIN DADOFISICO FIS (NOLOCK) ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                                                 ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.ACADSC, ACA.ORGCOD,
                           ACA.ACASNRAP, ACA.ACASNMETANAOCUMULATIVA) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP,
                     ORDEM, TACDSC, PRODSC,
                     UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   '0000'                            AS SACCOD,
                   'XXXX'                            AS LOCCOD,
                   ACA.ACADSC,
                   ''                                AS SACDSC,
                   ACA.ACASNRAP,
                   2                                 AS ORDEM,
                   ''                                AS TACDSC,
                   ''                                AS PRODSC,
                   ''                                AS UNMDSC,
                   ACA.ORGCOD,
                   0                                 AS NUMCASAS,
                   'Financeiro'                      AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))    AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADO1, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO2, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO3, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO4, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO5, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO6, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO7, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO8, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO9, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO10, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO11, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO12, 0)) AS REALIZADOANO,
                   0                                 as CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     INNER JOIN DADOFINANCEIRO FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.ACADSC, ACA.ORGCOD, ACA.ACASNRAP

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   '0000'                            AS SACCOD,
                   'XXXX'                            AS LOCCOD,
                   ACA.ACADSC,
                   ''                                AS SACDSC,
                   ACA.ACASNRAP,
                   2                                 AS ORDEM,
                   ''                                AS TACDSC,
                   ''                                AS PRODSC,
                   ''                                AS UNMDSC,
                   '-----'                              ORGCOD,
                   0                                 AS NUMCASAS,
                   'Financeiro'                      AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))    AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADO1, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO2, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO3, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO4, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO5, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO6, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO7, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO8, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO9, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO10, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO11, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO12, 0)) AS REALIZADOANO,
                   0                                 as CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     INNER JOIN DADOFINANCEIRO FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.ACADSC, ACA.ACASNRAP

            UNION


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 AS CANCELADOANO
            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         '0000'                          AS SACCOD,
                         'XXXX'                          AS LOCCOD,
                         ACA.ACADSC,
                         ''                              AS SACDSC,
                         ACA.ACASNRAP,
                         3                               AS ORDEM,
                         ''                              AS TACDSC,
                         ''                              AS PRODSC,
                         ''                              AS UNMDSC,
                         '-----'                         as ORGCOD,
                         3                               AS NUMCASAS,
                         'Físico (Restos a Pagar)'       AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0)) AS ATUALANO,
                         CASE
                             WHEN 1 = 1 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                         AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           INNER JOIN DADOFISICORAP FIS (NOLOCK)
                                      ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                         ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.ACADSC,
                           ACA.ACASNRAP, ACA.ACASNMETANAOCUMULATIVA) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP, ORDEM, TACDSC,
                     PRODSC, UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   '0000'                             AS SACCOD,
                   'XXXX'                             AS LOCCOD,
                   ACA.ACADSC,
                   ''                                 AS SACDSC,
                   ACA.ACASNRAP,
                   4                                  AS ORDEM,
                   ''                                 AS TACDSC,
                   ''                                 AS PRODSC,
                   ''                                 AS UNMDSC,
                   '-----'                            as ORGCOD,
                   0                                  AS NUMCASAS,
                   'Financeiro (Restos a Pagar)'      AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))     AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADOANO, 0)) AS REALIZADOANO,
                   SUM(ISNULL(FINVLRCANCELADO1, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO2, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO3, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO4, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO5, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO6, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO7, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO8, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO9, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO10, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO11, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO12, 0))
                                                      AS CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     INNER JOIN DADOFINANCEIRORAP FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.ACADSC,
                     ACA.ACASNRAP
            ORDER BY ACADSC, SACDSC, ORDEM


            SET @STRSQL = '
    SELECT
        TMP.*,
            UNI.UNIDSC
    FROM
        #TEMPORARIO TMP
    LEFT JOIN UNIDADE UNI
    ON
        UNI.UNIANO = TMP.PRGANO AND
        UNI.UNICOD = TMP.ORGCOD AND
        UNI.UNITPOCOD = ''U'' '
            IF @SNCOORDENADOR = 1
                SET @STRSQL = @STRSQL + ' INNER JOIN UsuarioNivel UsuarioNivel ON
            UsuarioNivel.USNChave1 = TMP.PRGCOD
            AND UsuarioNivel.USNChave2 = TMP.ACACOD
            AND UsuarioNivel.USNChave3 = TMP.ORGCOD
            AND UsuarioNivel.USNFim IS NULL
            AND UsuarioNivel.USUCod = ' + @USUCodCOORD

            SET @STRSQL = @STRSQL + 'ORDER BY
        TMP.ACASNRAP, TMP.ACADSC, TMP.SACDSC,
        TMP.ORDEM, UNI.UNIDSC'

            EXEC (@STRSQL)


        END


    ELSE

        BEGIN

            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 as CANCELADOANO
            INTO #TEMPORARIO1
            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         CASE WHEN ACA.SACCOD = '0000' THEN '' ELSE ACA.SACCOD END AS SACCOD,
                         ACA.LOCCOD,
                         ACA.ACADSC,
                         ISNULL(ACA.SACDSC, ACA.ACADSC)                            AS SACDSC,
                         ACA.ACASNRAP,
                         1                                                         AS ORDEM,
                         TAC.TACDSC,
                         PRO.PRODSC,
                         UNM.UNMDSC,
                         ACA.ORGCOD,
                         3                                                         AS NUMCASAS,
                         'Físico'                                                  AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0))                           AS ATUALANO,
                         CASE
                             WHEN ACA.ACASNMETANAOCUMULATIVA = 0 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                                                   AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           LEFT JOIN TIPOACAO TAC (NOLOCK) ON ACA.TACCOD = TAC.TACCOD
                           LEFT JOIN PRODUTO PRO (NOLOCK) ON ACA.PROCOD = PRO.PROCOD
                           LEFT JOIN UNIDADEMEDIDA UNM (NOLOCK) ON ACA.UNMCOD = UNM.UNMCOD
                           INNER JOIN FILTROTREEVIEWDETALHE FTV
                                      ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND
                                         ACA.SACCOD = FTV.SACCOD AND FTV.FTVCOD = @FTVCOD
                           INNER JOIN DADOFISICO FIS (NOLOCK) ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                                                 ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.SACCOD, ACA.LOCCOD,
                           ACA.ACADSC, ACA.SACDSC, ACA.ACASNRAP,
                           ACA.ACASNMETANAOCUMULATIVA, ACA.ORGCOD,
                           TAC.TACDSC, PRO.PRODSC, UNM.UNMDSC) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP, ORDEM, TACDSC,
                     PRODSC, UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   CASE WHEN ACA.SACCOD = '0000' THEN '' ELSE ACA.SACCOD END AS SACCOD,
                   ACA.LOCCOD,
                   ACA.ACADSC,
                   ISNULL(ACA.SACDSC, ACA.ACADSC)                            AS SACDSC,
                   ACA.ACASNRAP,
                   2                                                         AS ORDEM,
                   TAC.TACDSC,
                   PRO.PRODSC,
                   UNM.UNMDSC,
                   ACA.ORGCOD,
                   0                                                         AS NUMCASAS,
                   'Financeiro'                                              AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))                            AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADO1, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO2, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO3, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO4, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO5, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO6, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO7, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO8, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO9, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO10, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO11, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO12, 0))
                                                                             AS REALIZADOANO,
                   0                                                         AS CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     LEFT JOIN TIPOACAO TAC (NOLOCK) ON ACA.TACCOD = TAC.TACCOD
                     LEFT JOIN PRODUTO PRO (NOLOCK) ON ACA.PROCOD = PRO.PROCOD
                     LEFT JOIN UNIDADEMEDIDA UNM (NOLOCK) ON ACA.UNMCOD = UNM.UNMCOD
                     INNER JOIN FILTROTREEVIEWDETALHE FTV
                                ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND ACA.SACCOD = FTV.SACCOD AND
                                   FTV.FTVCOD = @FTVCOD
                     INNER JOIN DADOFINANCEIRO FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.SACCOD, ACA.LOCCOD,
                     ACA.ACADSC, ACA.SACDSC, ACA.ACASNRAP,
                     TAC.TACDSC, PRO.PRODSC, UNM.UNMDSC, ACA.ORGCOD

            UNION


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 AS CANCELADOANO

            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         CASE WHEN ACA.SACCOD = '0000' THEN '' ELSE ACA.SACCOD END AS SACCOD,
                         ACA.LOCCOD,
                         ACA.ACADSC,
                         ISNULL(ACA.SACDSC, ACA.ACADSC)                            AS SACDSC,
                         ACA.ACASNRAP,
                         1                                                         AS ORDEM,
                         TAC.TACDSC,
                         PRO.PRODSC,
                         UNM.UNMDSC,
                         ACA.ORGCOD,
                         3                                                         AS NUMCASAS,
                         'Físico (Restos a Pagar)'                                 AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0))                           AS ATUALANO,
                         CASE
                             WHEN 1 = 1 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                                                   AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           LEFT JOIN TIPOACAO TAC (NOLOCK) ON ACA.TACCOD = TAC.TACCOD
                           LEFT JOIN PRODUTO PRO (NOLOCK) ON ACA.PROCOD = PRO.PROCOD
                           LEFT JOIN UNIDADEMEDIDA UNM (NOLOCK) ON ACA.UNMCOD = UNM.UNMCOD
                           INNER JOIN FILTROTREEVIEWDETALHE FTV
                                      ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND
                                         ACA.SACCOD = FTV.SACCOD AND FTV.FTVCOD = @FTVCOD
                           INNER JOIN DADOFISICORAP FIS (NOLOCK)
                                      ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                         ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.SACCOD, ACA.LOCCOD, ACA.ACADSC,
                           ACA.SACDSC, ACA.ACASNRAP, ACA.ACASNMETANAOCUMULATIVA,
                           TAC.TACDSC, PRO.PRODSC, UNM.UNMDSC, ACA.ORGCOD) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP, ORDEM, TACDSC,
                     PRODSC, UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   CASE WHEN ACA.SACCOD = '0000' THEN '' ELSE ACA.SACCOD END AS SACCOD,
                   ACA.LOCCOD,
                   ACA.ACADSC,
                   ISNULL(ACA.SACDSC, ACA.ACADSC)                            AS SACDSC,
                   ACA.ACASNRAP,
                   4                                                         AS ORDEM,
                   TAC.TACDSC,
                   PRO.PRODSC,
                   UNM.UNMDSC,
                   ACA.ORGCOD,
                   0                                                         AS NUMCASAS,
                   'Financeiro (Restos a Pagar)'                             AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))                            AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADOANO, 0))                        AS REALIZADOANO,
                   SUM(ISNULL(FINVLRCANCELADO1, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO2, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO3, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO4, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO5, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO6, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO7, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO8, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO9, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO10, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO11, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO12, 0))
                                                                             AS CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     LEFT JOIN TIPOACAO TAC (NOLOCK) ON ACA.TACCOD = TAC.TACCOD
                     LEFT JOIN PRODUTO PRO (NOLOCK) ON ACA.PROCOD = PRO.PROCOD
                     LEFT JOIN UNIDADEMEDIDA UNM (NOLOCK) ON ACA.UNMCOD = UNM.UNMCOD
                     INNER JOIN FILTROTREEVIEWDETALHE FTV
                                ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND ACA.SACCOD = FTV.SACCOD AND
                                   FTV.FTVCOD = @FTVCOD
                     INNER JOIN DADOFINANCEIRORAP FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.SACCOD, ACA.LOCCOD,
                     ACA.ACADSC, ACA.SACDSC, ACA.ACASNRAP,
                     TAC.TACDSC, PRO.PRODSC, UNM.UNMDSC, ACA.ORGCOD

            UNION


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 AS CANCELADOANO

            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         '0000'                          AS SACCOD,
                         'XXXX'                          AS LOCCOD,
                         ACA.ACADSC,
                         ' '                             AS SACDSC,
                         ACA.ACASNRAP,
                         1                               AS ORDEM,
                         ' '                             AS TACDSC,
                         ' '                             AS PRODSC,
                         ' '                             AS UNMDSC,
                         ACA.ORGCOD,
                         3                               AS NUMCASAS,
                         'Físico'                        AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0)) AS ATUALANO,
                         CASE
                             WHEN ACA.ACASNMETANAOCUMULATIVA = 0 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                         AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           INNER JOIN FILTROTREEVIEWDETALHE FTV
                                      ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND
                                         ACA.SACCOD = FTV.SACCOD AND FTV.FTVCOD = @FTVCOD
                           INNER JOIN DADOFISICO FIS (NOLOCK) ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                                                 ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.ACADSC, ACA.ORGCOD,
                           ACA.ACASNRAP, ACA.ACASNMETANAOCUMULATIVA) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP, ORDEM, TACDSC,
                     PRODSC, UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 AS CANCELADOANO

            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         '0000'                          AS SACCOD,
                         'XXXX'                          AS LOCCOD,
                         ACA.ACADSC,
                         ' '                             AS SACDSC,
                         ACA.ACASNRAP,
                         1                               AS ORDEM,
                         ' '                             AS TACDSC,
                         ' '                             AS PRODSC,
                         ' '                             AS UNMDSC,
                         '-----'                            ORGCOD,
                         3                               AS NUMCASAS,
                         'Físico'                        AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0)) AS ATUALANO,
                         CASE
                             WHEN ACA.ACASNMETANAOCUMULATIVA = 0 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                         AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           INNER JOIN FILTROTREEVIEWDETALHE FTV
                                      ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND
                                         ACA.SACCOD = FTV.SACCOD AND FTV.FTVCOD = @FTVCOD
                           INNER JOIN DADOFISICO FIS (NOLOCK) ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                                                 ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.ACADSC, ACA.ORGCOD,
                           ACA.ACASNRAP, ACA.ACASNMETANAOCUMULATIVA) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP, ORDEM, TACDSC,
                     PRODSC, UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   '0000'                            AS SACCOD,
                   'XXXX'                            AS LOCCOD,
                   ACA.ACADSC,
                   ''                                AS SACDSC,
                   ACA.ACASNRAP,
                   2                                 AS ORDEM,
                   ''                                AS TACDSC,
                   ''                                AS PRODSC,
                   ''                                AS UNMDSC,
                   ACA.ORGCOD,
                   0                                 AS NUMCASAS,
                   'Financeiro'                      AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))    AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADO1, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO2, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO3, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO4, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO5, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO6, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO7, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO8, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO9, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO10, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO11, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO12, 0)) AS REALIZADOANO,
                   0                                 AS CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     INNER JOIN FILTROTREEVIEWDETALHE FTV
                                ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND ACA.SACCOD = FTV.SACCOD AND
                                   FTV.FTVCOD = @FTVCOD
                     INNER JOIN DADOFINANCEIRO FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.ACADSC, ACA.ORGCOD,
                     ACA.ACASNRAP

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   '0000'                            AS SACCOD,
                   'XXXX'                            AS LOCCOD,
                   ACA.ACADSC,
                   ''                                AS SACDSC,
                   ACA.ACASNRAP,
                   2                                 AS ORDEM,
                   ''                                AS TACDSC,
                   ''                                AS PRODSC,
                   ''                                AS UNMDSC,
                   '-----'                              ORGCOD,
                   0                                 AS NUMCASAS,
                   'Financeiro'                      AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))    AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADO1, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO2, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO3, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO4, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO5, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO6, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO7, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO8, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO9, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO10, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO11, 0)) +
                   SUM(ISNULL(FINVLRREALIZADO12, 0)) AS REALIZADOANO,
                   0                                 AS CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     INNER JOIN FILTROTREEVIEWDETALHE FTV
                                ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND ACA.SACCOD = FTV.SACCOD AND
                                   FTV.FTVCOD = @FTVCOD
                     INNER JOIN DADOFINANCEIRO FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.ACADSC, ACA.ACASNRAP

            UNION


            SELECT PRGANO,
                   PRGCOD,
                   PRGDSC,
                   ACACOD,
                   SACCOD,
                   LOCCOD,
                   ACADSC,
                   SACDSC,
                   ACASNRAP,
                   ORDEM,
                   TACDSC,
                   PRODSC,
                   UNMDSC,
                   ORGCOD,
                   NUMCASAS,
                   TIPO,
                   SUM(ATUALANO)     AS ATUALANO,
                   SUM(REALIZADOANO) AS REALIZADOANO,
                   0                 AS CANCELADOANO
            FROM (SELECT PRG.PRGANO,
                         PRG.PRGCOD,
                         PRG.PRGDSC,
                         ACA.ACACOD,
                         '0000'                          AS SACCOD,
                         'XXXX'                          AS LOCCOD,
                         ACA.ACADSC,
                         ''                              AS SACDSC,
                         ACA.ACASNRAP,
                         3                               AS ORDEM,
                         ''                              AS TACDSC,
                         ''                              AS PRODSC,
                         ''                              AS UNMDSC,
                         '-----'                         as ORGCOD,
                         3                               AS NUMCASAS,
                         'Físico (Restos a Pagar)'       AS TIPO,
                         SUM(ISNULL(FISQTDEATUALANO, 0)) AS ATUALANO,
                         CASE
                             WHEN 1 = 1 THEN
                                 SUM(ISNULL(FISQTDEREALIZADO1, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO2, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO3, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO4, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO5, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO6, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO7, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO8, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO9, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO10, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO11, 0)) +
                                 SUM(ISNULL(FISQTDEREALIZADO12, 0))
                             ELSE
                                 SUM(ISNULL(FISQTDEREALIZADOANO, 0))
                             END                         AS REALIZADOANO
                  FROM PROGRAMA PRG (NOLOCK)
                           INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                           INNER JOIN FILTROTREEVIEWDETALHE FTV
                                      ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND
                                         ACA.SACCOD = FTV.SACCOD AND FTV.FTVCOD = @FTVCOD
                           INNER JOIN DADOFISICORAP FIS (NOLOCK)
                                      ON ACA.PRGANO = FIS.PRGANO AND ACA.PRGCOD = FIS.PRGCOD AND
                                         ACA.ACACOD = FIS.ACACOD AND ACA.SACCOD = FIS.SACCOD
                  WHERE PRG.PRGANO = @PRGANO
                    AND PRG.PRGCOD = @PRGCOD
                    AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
                  GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                           ACA.ACACOD, ACA.ACADSC,
                           ACA.ACASNRAP, ACA.ACASNMETANAOCUMULATIVA) DADOS
            GROUP BY PRGANO, PRGCOD, PRGDSC,
                     ACACOD, SACCOD, LOCCOD,
                     ACADSC, SACDSC, ACASNRAP, ORDEM, TACDSC,
                     PRODSC, UNMDSC, ORGCOD, NUMCASAS, TIPO

            UNION


            SELECT PRG.PRGANO,
                   PRG.PRGCOD,
                   PRG.PRGDSC,
                   ACA.ACACOD,
                   '0000'                             AS SACCOD,
                   'XXXX'                             AS LOCCOD,
                   ACA.ACADSC,
                   ''                                 AS SACDSC,
                   ACA.ACASNRAP,
                   4                                  AS ORDEM,
                   ''                                 AS TACDSC,
                   ''                                 AS PRODSC,
                   ''                                 AS UNMDSC,
                   '-----'                            as ORGCOD,
                   0                                  AS NUMCASAS,
                   'Financeiro (Restos a Pagar)'      AS TIPO,
                   SUM(ISNULL(FINVLRATUALANO, 0))     AS ATUALANO,
                   SUM(ISNULL(FINVLRREALIZADOANO, 0)) AS REALIZADOANO,
                   SUM(ISNULL(FINVLRCANCELADO1, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO2, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO3, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO4, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO5, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO6, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO7, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO8, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO9, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO10, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO11, 0)) +
                   SUM(ISNULL(FINVLRCANCELADO12, 0))
                                                      AS CANCELADOANO
            FROM PROGRAMA PRG (NOLOCK)
                     INNER JOIN ACAO ACA (NOLOCK) ON PRG.PRGANO = ACA.PRGANO AND PRG.PRGCOD = ACA.PRGCOD
                     INNER JOIN FILTROTREEVIEWDETALHE FTV
                                ON ACA.PRGCOD = FTV.PRGCOD AND ACA.ACACOD = FTV.ACACOD AND ACA.SACCOD = FTV.SACCOD AND
                                   FTV.FTVCOD = @FTVCOD
                     INNER JOIN DADOFINANCEIRORAP FIN (NOLOCK)
                                ON ACA.PRGANO = FIN.PRGANO AND ACA.PRGCOD = FIN.PRGCOD AND ACA.ACACOD = FIN.ACACOD AND
                                   ACA.SACCOD = FIN.SACCOD
            WHERE PRG.PRGANO = @PRGANO
              AND PRG.PRGCOD = @PRGCOD
              AND (ACA.SACCOD <> '0000' OR ACA.TACCOD = '3')
            GROUP BY PRG.PRGANO, PRG.PRGCOD, PRG.PRGDSC,
                     ACA.ACACOD, ACA.ACADSC, ACA.ACASNRAP
            ORDER BY ACADSC, SACDSC, ORDEM


            SELECT TMP.*,
                   UNI.UNIDSC
            FROM #TEMPORARIO1 TMP
                     LEFT JOIN UNIDADE UNI
                               ON
                                   UNI.UNIANO = TMP.PRGANO AND
                                   UNI.UNICOD = TMP.ORGCOD AND
                                   UNI.UNITPOCOD = 'U'
            ORDER BY TMP.ACASNRAP, TMP.ACADSC,
                     TMP.SACDSC, TMP.ORDEM, UNI.UNIDSC

        END

    SET NOCOUNT OFF
