using Microsoft.SqlServer.TransactSql.ScriptDom;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using static System.String;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public static class Converter
{
  public static TipoDadoEnum TipoDado(StoredProcedure procedure, SqlDataTypeOption input)
  {
    switch (input)
    {
      case SqlDataTypeOption.Char:
        return TipoDadoEnum.STRING;
      case SqlDataTypeOption.VarChar:
        return TipoDadoEnum.STRING;
      case SqlDataTypeOption.NChar:
        return TipoDadoEnum.STRING;
      case SqlDataTypeOption.NText:
        return TipoDadoEnum.STRING;
      case SqlDataTypeOption.Text:
        return TipoDadoEnum.STRING;
      case SqlDataTypeOption.NVarChar:
        return TipoDadoEnum.STRING;
      case SqlDataTypeOption.Int:
        return TipoDadoEnum.INTEGER;
      case SqlDataTypeOption.Numeric:
        return TipoDadoEnum.INTEGER;
      case SqlDataTypeOption.TinyInt:
        return TipoDadoEnum.INTEGER;
      case SqlDataTypeOption.SmallInt:
        return TipoDadoEnum.INTEGER;
      case SqlDataTypeOption.BigInt:
        return TipoDadoEnum.INTEGER;
      case SqlDataTypeOption.Bit:
        return TipoDadoEnum.INTEGER;
      case SqlDataTypeOption.SmallDateTime:
        return TipoDadoEnum.LOCAL_DATE_TIME;
      case SqlDataTypeOption.DateTime:
        return TipoDadoEnum.LOCAL_DATE_TIME;
      case SqlDataTypeOption.Money:
        return TipoDadoEnum.BIG_DECIMAL;
      case SqlDataTypeOption.Decimal:
        return TipoDadoEnum.BIG_DECIMAL;
      case SqlDataTypeOption.Float:
        return TipoDadoEnum.BIG_DECIMAL;
      case SqlDataTypeOption.Real:
        return TipoDadoEnum.BIG_DECIMAL;
      default: // SqlDataTypeOption.None:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Data Type \"{input}\" not mapped yet.\n");
        return TipoDadoEnum.TIPO_NAO_MAPEADO;
      //SqlDataTypeOption.Image =>
      //_ => throw new ApplicationException("Data Type not yet mapped: " + input)
    }
  }

  public static TipoComandoEnum TipoComando(StoredProcedure procedure, TSqlStatement input)
  {
    switch (input)
    {
      case DeleteStatement:
      case UpdateStatement:
      case InsertStatement:
        return TipoComandoEnum.ENDPOINT;
      case DeclareVariableStatement:
        return TipoComandoEnum.DECLARACAO;
      case SetVariableStatement:
        return TipoComandoEnum.ATRIBUICAO;
      case IfStatement:
        return TipoComandoEnum.IF;
      case PredicateSetStatement:
        return TipoComandoEnum.TIPO_NAO_MAPEADO;
      case BeginEndBlockStatement:
        return TipoComandoEnum.BLOCO;
      case ExecuteStatement:
        return TipoComandoEnum.EXEC;
      case BeginTransactionStatement:
        return TipoComandoEnum.BEGIN_TRANSACTION;
    }
    procedure.SnSucessoParser = false;
    procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
      $"Command Type \"{input}\" not mapped yet.\n");
    return TipoComandoEnum.TIPO_NAO_MAPEADO;
    //throw new ApplicationException("Command Type not yet mapped: " + input.ToString());
  }

  public static TipoOperadorEnum TipoOperadorBooleano(StoredProcedure procedure,
    BooleanComparisonType input)
  {
    switch (input)
    {
      case BooleanComparisonType.Equals:
        return TipoOperadorEnum.IGUAL;
      case BooleanComparisonType.GreaterThan:
        return TipoOperadorEnum.MAIOR_QUE;
      case BooleanComparisonType.LessThan:
        return TipoOperadorEnum.MENOR_QUE;
      case BooleanComparisonType.GreaterThanOrEqualTo:
        return TipoOperadorEnum.MAIOR_IGUAL;
      case BooleanComparisonType.LessThanOrEqualTo:
        return TipoOperadorEnum.MENOR_IGUAL;
      case BooleanComparisonType.NotEqualToExclamation:
        return TipoOperadorEnum.DIFERENTE;
      default:
        //throw new ApplicationException("Operator Type not yet mapped: " + input)
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Command Type \"{input}\" not mapped yet.\n");
        return TipoOperadorEnum.TIPO_NAO_MAPEADO;
    }
  }

  public static TipoOperadorEnum TipoOperadorBinario(StoredProcedure procedure,
    BinaryExpressionType input)
  {
    switch (input)
    {
      case BinaryExpressionType.Add:
        return TipoOperadorEnum.ADICAO;
      case BinaryExpressionType.Subtract:
        return TipoOperadorEnum.SUBTRACAO;
      case BinaryExpressionType.Multiply:
        return TipoOperadorEnum.MULTIPLICACAO;
      case BinaryExpressionType.Divide:
        return TipoOperadorEnum.DIVISAO;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Operator Type \"{input}\" not mapped yet.\n");
        return TipoOperadorEnum.TIPO_NAO_MAPEADO;
      //_ => throw new ApplicationException("Operator Type not yet mapped: " + input)
    }
  }
}
