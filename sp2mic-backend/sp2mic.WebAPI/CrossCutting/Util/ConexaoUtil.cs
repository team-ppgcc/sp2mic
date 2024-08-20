using System.Data;
using Microsoft.SqlServer.Management.Smo;
using sp2mic.WebAPI.Modulo1.Carga.Dtos;

namespace sp2mic.WebAPI.CrossCutting.Util;

public static class ConexaoUtil
{

  public static List<ParDto> IdentificarTabelasProcedures(CargaDto dto, string nomeProcedure)
  {
    var query
      = $"SELECT NAME as Tabelas FROM SYSOBJECTS WHERE ID IN (SELECT SD.DEPID FROM SYSOBJECTS SO, SYSDEPENDS SD WHERE SO.NAME = \'{nomeProcedure}\' AND SD.ID = SO.ID)";
    var ds = RecuperarDadosBanco(dto, query);
    var retorno = new List<ParDto>();
    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
    {
      var par = new ParDto(i, ds.Tables[0].Rows[i]["Tabelas"].ToString()!);
      retorno.Add(par);
    }
    return retorno;
  }

  public static List<ParDto> ListarNomesProcedures(CargaDto dto)
  {
    var query
      = "select o.name from sys.objects o join sys.schemas s on s.schema_id = o.schema_id where o.type = 'P' and s.name = '" +
      dto.Schema + "'";
    var ds = RecuperarDadosBanco(dto, query);
    var retorno = new List<ParDto>();
    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
    {
      var par = new ParDto(i, ds.Tables[0].Rows[i]["name"].ToString()!);
      retorno.Add(par);
    }
    return retorno;
  }

  private static DataSet RecuperarDadosBanco(CargaDto dto, string query)
  {
    var svr = new Server(dto.DadosConexao!.Host);
    var db = svr.Databases[dto.DadosConexao.DatabaseName];
    return db.ExecuteWithResults(query);
  }

  public static List<ParDto> RecuperarTodasProcedures(CargaDto dto)
  {
    var query
      = "select o.name from sys.objects o join sys.schemas s on s.schema_id = o.schema_id where o.type = 'P' and s.name = '" +
      dto.Schema + "'";
    var ds = RecuperarDadosBanco(dto, query);
    var retorno = new List<ParDto>();
    for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
    {
      var par = new ParDto(i, ds.Tables[0].Rows[i]["name"].ToString()!);
      retorno.Add(par);
    }
    return retorno;
  }
}

  /*
   --Listar todas as stored procedures do banco
select * from sys.objects where type = 'P'


--Listar os campos e tipos da procedure
select p.name variavel, t.name tipo, t.max_length
from sys.objects o
join sys.parameters p on o.object_id = p.object_id
join sys.types t on p.system_type_id = t.system_type_id
where o.name = 'sps_PACAutorizacaoGetMaiorNumeroAutorizacaoOrgao'

--Listar tableas usadas pela procedure
SELECT NAME as Tabelas
FROM SYSOBJECTS
WHERE ID IN (   SELECT SD.DEPID
              FROM SYSOBJECTS SO,
              SYSDEPENDS SD
              WHERE SO.NAME = 'sps_PACAutorizacaoGetMaiorNumeroAutorizacaoOrgao'  ----name of stored procedures
              AND SD.ID = SO.ID
          )

--Listar corpo da procedure (texto interno)
sp_helptext 'sps_PACAutorizacaoGetMaiorNumeroAutorizacaoOrgao'

   */
  /*
private static final Logger
      LOG = LoggerFactory.getLogger(ConexaoUtil.class);

  public static Connection getConnection(String dbURL, String user, String pass) throws SQLException
{
  Connection conn;
  conn = DriverManager.getConnection(dbURL, user, pass);
      if (nonNull(conn)) {
          DatabaseMetaData dm = conn.getMetaData();
LOG.info("Driver name: {}", dm.getDriverName());
          LOG.info("Driver version: {}", dm.getDriverVersion());
          LOG.info("Product name: {}", dm.getDatabaseProductName());
          LOG.info("Product version: {}", dm.getDatabaseProductVersion());
          LOG.info("dbURL: {}", dbURL);
          LOG.info("user: {}", user);
          LOG.info("pass: {}", pass);
      }
      return conn;
  }

  public static String getSelectStoredProcedures(String schema)
{

return "SELECT schema_name(obj.schema_id) AS schema_name, "
    + "       obj.name as procedure_name, "
    + "        substring(par.parameters, 0, len(par.parameters)) AS parameters, "
    + "        mod.definition "
    + "FROM sys.objects obj "
    + "JOIN sys.sql_modules mod "
    + "     ON mod.object_id = obj.object_id "
    + "CROSS apply (SELECT p.name + ' ' + TYPE_NAME(p.user_type_id) + ', ' "
    + "             FROM sys.parameters p "
    + "             WHERE p.object_id = obj.object_id "
    + "                   AND p.parameter_id != 0 "
    + "             FOR XML PATH ('') ) par (parameters) "
    + "WHERE obj.type IN ('P', 'X') and schema_name(obj.schema_id) = '" + schema + "'"
    + "ORDER BY schema_name, procedure_name ";

}

public static String getSelectTabelas(String schema)
{

return "SELECT DISTINCT "
    + " schema_name(p.[schema_id]) as schema_procedure, p.name AS procedure_name, "
    + " schema_tabela = COALESCE(d.referenced_schema_name, s.name), d.referenced_entity_name AS tabela "
    + " FROM sys.sql_expression_dependencies AS d "
    + " INNER JOIN sys.procedures AS p "
    + " ON p.[object_id] = d.referencing_id "
    + " INNER JOIN sys.schemas AS s "
    + " ON p.[schema_id] = s.[schema_id] "
    + " WHERE schema_name(p.[schema_id]) = '" + schema + "'"
    + " ORDER BY p.name, schema_tabela ";
}

public static String getSelectColunas()
{

return "SELECT DISTINCT "
    + "    sys.tables.name AS no_table, "
    + "    sys.columns.name AS no_column, "
    + "    sys.types.name AS no_data_type, "
    + "    sys.columns.is_nullable AS sn_is_nullable, "
    + "    (   SELECT "
    + "            COUNT(column_name) "
    + "        FROM "
    + "            INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE "
    + "        WHERE "
    + "            TABLE_NAME = sys.tables.name AND "
    + "            CONSTRAINT_NAME = "
    + "                (   SELECT "
    + "                    constraint_name "
    + "                    FROM "
    + "                        INFORMATION_SCHEMA.TABLE_CONSTRAINTS "
    + "                    WHERE "
    + "                        TABLE_NAME = sys.tables.name AND "
    + "                        constraint_type = 'PRIMARY KEY' AND "
    + "                        COLUMN_NAME = sys.columns.name "
    + "                ) "
    + "    ) AS sn_is_primary_key, "
    + "    sys.columns.max_length AS nu_char_max_length "
    + "FROM "
    + "    sys.columns, sys.types, sys.tables, sys.schemas "
    + "WHERE "
    + "    sys.tables.object_id = sys.columns.object_id AND "
    + "    sys.types.system_type_id = sys.columns.system_type_id AND "
    + "    sys.types.user_type_id = sys.columns.user_type_id ";
  */
  /*return "SELECT  "
      + "        s.[name]            'schema', "
      + "        t.[name]            'table', "
      + "        c.[name]            'column', "
      + "        d.[name]            'data_type', "
      + "        c.[max_length]      'length', "
      + "        c.[is_identity]     'is_id', "
      + "        c.[is_nullable]     'is_nullable' "
      + "FROM        sys.schemas s "
      + "INNER JOIN  sys.tables  t "
      + "ON s.schema_id = t.schema_id "
      + "INNER JOIN  sys.columns c "
      + "on t.object_id = c.object_id "
      + "INNER JOIN  sys.types   d "
      + "ON c.user_type_id = d.user_type_id ";*/
  //}
