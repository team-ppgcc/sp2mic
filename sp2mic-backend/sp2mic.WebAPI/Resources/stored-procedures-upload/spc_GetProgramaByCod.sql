USE [dbp_54808_sig2000]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE proc [dbo].[spc_GetProgramaByCod]
    @PRGCod VARCHAR(4)
AS
    SELECT
        PRGCod AS code,
        PRGSisSiglaAlt as sigla,
        PRGDsc AS description        
    FROM
        Programa as prog
    WHERE
        PRGCod = @PRGCod
    ORDER BY
        prog.PRGCod

    FOR XML AUTO, ELEMENTS

GO