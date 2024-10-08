USE [dbp_54808_sig2000]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sps_SelectCodigoEmpreendimento]
  @EMPCod INT,
  @EMPAno INT,
  @MOMCod INT,
  @UNICod char(5)

AS

DECLARE @Sigla VARCHAR(4)
DECLARE @Num INT
DECLARE @CodEMP VARCHAR(10)
DECLARE @UNITpoCod CHAR(1)

SET @UNITpoCod = 'U'


IF (SELECT len(EMPCodigoEmpreend) 
    FROM Empreendimento
    WHERE EMPCod = @EMPCod AND
          EMPAno = @EMPAno AND
          MOMCod = @MOMCod) < 2
  BEGIN
    SET @Sigla = (SELECT RTRIM(SOGSigla)
                  FROM SiglaOrgao
                  WHERE SOGCodOrgao = (SELECT DISTINCT ORGCod
                                       FROM Unidade
                                       WHERE UNICod = @UNICod AND
                                             UNIAno = @EMPAno AND
                                             UNITpoCod = @UNITpoCod))
    
    SET @Num = (SELECT RTRIM(EMPCodigoEmpreend)
                FROM Empreendimento
                WHERE EMPCodigoEmpreend = @Sigla AND
                      EMPAno = @EMPAno)
    
    SET @CodEMP = @Sigla + '.' + @Num
    
    UPDATE Empreendimento 
    SET EMPCodigoEmpreend = @CodEMP
    WHERE EMPCod = @EMPCod AND
          EMPAno = @EMPAno AND
          MOMCod = @MOMCod

  END
GO