
CREATE PROCEDURE  [dbo].[sp_GetOrgaoUsuario]
	@USUCod int,
	@NIVCod int,
	@ORGAno int = 0
AS

	declare @HABCod int

	SELECT @HABCod = HABCod
	  FROM TBNivelacesso na
     WHERE na.nivcod = @NIVCod

	IF @HABCod = 1
		BEGIN
			SELECT distinct
				org.ORGcod as codigo,
				org.ORGdsc as descricao,
				org.TPOCod as tipo,
				org.ORGAno as ano
			FROM
				TBOrgao as org
				INNER JOIN  TBUsuarioNivel un ON
					un.USNChave1 = org.ORGCod
					AND	un.USNChave2 = org.TPOCod
			 WHERE
				org.ORGAno = @ORGAno
				AND un.USUCOD = @USUCod
				AND un.NIVCOD = @NIVCod
			ORDER BY ORGDsc
		END
	ELSE
		BEGIN
			SELECT distinct
				ORGcod as codigo,
				ORGdsc as descricao,
				TPOCod as tipo,
				ORGAno as ano
			FROM
				TBUsuarioNivel un
				INNER JOIN TBOrgao org ON
					un.USNChave1 = org.ORGCod AND
					un.USNChave2 = org.TPOCod
			WHERE
				un.USUCOD = @USUCod AND
				un.NIVCOD = @NIVCod AND
				org.ORGAno = @ORGAno
			ORDER BY ORGDsc
		END

GO