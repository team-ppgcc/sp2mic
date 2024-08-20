using System.Reflection;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public static class Parser
{
  private static readonly Assembly ScriptDomCode
    = Assembly.Load("Microsoft.SqlServer.TransactSql.ScriptDom");

  public static TSqlFragment GetFragment(string script)
  {
    /*
     var parser = new TSql150Parser(true);
IList<ParseError> parseErrors = new List<ParseError>();
TextReader stringReader = new StringReader("COMMIT TRANSACTION WITH (DELAYED_DURABILITY = ON);");
parser.Parse(stringReader, out parseErrors);
Console.WriteLine($"{parseErrors.Count} parse errors");
    */
    var parser = new TSql120Parser(false);
    var tSqlFragment = parser.Parse(new StringReader(script), out IList<ParseError> parseErrors);
    //Console.WriteLine($"{parseErrors.Count} parse errors");
    return tSqlFragment;
  }

  public static List<CreateProcedureStatement> GetProcedures(TSqlFragment fragment)
  {
    var visitor = new ProcedureVisitor();
    fragment.Accept(visitor);
    return visitor.Statements;
  }

  public static List<SelectStatement> GetSelectsStatements(TSqlFragment fragment)
  {
    var visitor = new SelectVisitor();
    fragment.Accept(visitor);
    return visitor.SelectStatements;
  }

  public static List<IfStatement> GetIfStatements(TSqlFragment fragment)
  {
    var visitor = new IfVisitor();
    fragment.Accept(visitor);
    return visitor.IfStatements;
  }

  public static List<DeclareVariableStatement> GetDeclareVariableStatements(TSqlFragment fragment)
  {
    var visitor = new DeclareVariableVisitor();
    fragment.Accept(visitor);
    return visitor.DeclareVariableStatements;
  }

  public static List<SetVariableStatement> GetSetVariableStatements(TSqlFragment fragment)
  {
    var visitor = new SetVariableVisitor();
    fragment.Accept(visitor);
    return visitor.SetVariableStatements;
  }

  public static List<InsertStatement> GetInsertStatements(TSqlFragment fragment)
  {
    var visitor = new InsertVisitor();
    fragment.Accept(visitor);
    return visitor.InsertStatements;
  }

  public static List<UpdateStatement> GetUpdateStatements(TSqlFragment fragment)
  {
    var visitor = new UpdateVisitor();
    fragment.Accept(visitor);
    return visitor.UpdateStatements;
  }

  public static List<DeleteStatement> GetDeleteStatements(TSqlFragment fragment)
  {
    var visitor = new DeleteVisitor();
    fragment.Accept(visitor);
    return visitor.DeleteStatements;
  }

  public static List<DeleteSpecification> GetDeleteSpecification(TSqlFragment fragment)
  {
    var deletes = new List<DeleteSpecification>();
    var statements = GetStatements(fragment);
    foreach (var statement in statements)
    {
      if (statement is DeleteStatement delete)
      {
        deletes.Add(delete.DeleteSpecification);
      }
    }
    return deletes;
  }

  private static List<TSqlStatement> GetStatements(TSqlFragment fragment)
  {
    var visitor = new StatementVisitor();
    fragment.Accept(visitor);
    return visitor.Statements;
  }

  public static string GetFragmentSql(TSqlFragment fragment)
  {
    var sql = new StringBuilder();
    if (fragment.FirstTokenIndex == -1)
    {
      return sql.ToString().Trim();
    }
    for (var counter = fragment.FirstTokenIndex; counter <= fragment.LastTokenIndex; counter++)
    {
      sql.Append(fragment.ScriptTokenStream[counter].Text);
    }
    //sql.Append(" ");
    return sql.ToString().Trim();
  }

  public static string GetFragmentSqlTratado(TSqlFragment fragment)
  {
    var sql = new StringBuilder();
    if (fragment.FirstTokenIndex == -1)
    {
      return sql.ToString().Trim();
    }
    for (var counter = fragment.FirstTokenIndex; counter <= fragment.LastTokenIndex; counter++)
    {
      var token = fragment.ScriptTokenStream[counter];
      if (token.TokenType != TSqlTokenType.SingleLineComment &&
        token.TokenType != TSqlTokenType.MultilineComment)
      {
        sql.Append(token.Text);
      }
    }
    return sql.ToString().Trim();
  }

  public static List<TableReference> GetTableList(TSqlFragment fragment)
  {
    var tables = new List<TableReference>();
    var visitor = new EnumeratorVisitor();
    fragment.Accept(visitor);
    foreach (var statement in visitor.Nodes)
    {
      tables.AddRange(FindTableReferences(statement));
    }
    return tables;
  }

  private static IEnumerable<TableReference> FindTableReferences(TSqlFragment? statement)
  {
    var nodeType = statement!.ToString()!.Split(' ')[0];
    var type = ScriptDomCode.GetType(nodeType, false, true);
    var tables = new List<TableReference>();
    foreach (var propertyInfo in type!.GetProperties())
    {
      var value = TryGetValue(propertyInfo, statement);
      switch (value)
      {
        case null:
          continue;
        case List<TableReference> list:
          tables.AddRange(list);
          continue;
        case TableReference reference:
          tables.Add(reference);
          continue;
        //don't move this before is List<TableReference> as they are also TSqlFragments which causes hilarity
        case IEnumerable<TSqlFragment> fragments:
        {
          foreach (var fragment in fragments)
          {
            tables.AddRange(FindTableReferences(fragment));
          }
          continue;
        }
        case TSqlFragment sqlFragment:
          tables.AddRange(FindTableReferences(sqlFragment));
          break;
      }
    }
    return tables;
  }

  private static object? TryGetValue(PropertyInfo propertyInfo, object? node)
  {
    try
    {
      if (propertyInfo.GetIndexParameters().Length == 0)
      {
        return propertyInfo.GetValue(node) ?? null;
      }
    }
    catch (Exception)
    {
      return "";
    }
    return null;
  }

  public static List<QuerySpecification> GetQuerySpecifications(TSqlFragment fragment)
  {
    var statements = GetStatements(fragment);
    var querySpecifications = new List<QuerySpecification>();
    foreach (var s in statements)
    {
      querySpecifications.AddRange(FlattenTreeGetQuerySpecifications(s));
    }
    return querySpecifications.ToList();
  }

  private static List<QuerySpecification> FlattenTreeGetQuerySpecifications(TSqlStatement statement)
  {
    var specifications = new List<QuerySpecification>();
    if (statement is SelectStatement selectStatement)
    {
      specifications.Add((selectStatement.QueryExpression as QuerySpecification)!);
    }
    specifications.AddRange(SearchChildren(statement));
    return specifications;
  }

  private static IEnumerable<QuerySpecification> SearchChildren(TSqlFragment fragment)
  {
    switch (fragment)
    {
      case InsertStatement statement:
        return SearchChildren(statement.InsertSpecification.InsertSource);
      case SelectInsertSource source:
        return SearchChildren(source.Select);
    }
    var children = new List<QuerySpecification>();
    switch (fragment)
    {
      case BinaryQueryExpression queryExpression:
        children.AddRange(SearchChildren(queryExpression.FirstQueryExpression));
        children.AddRange(SearchChildren(queryExpression.SecondQueryExpression));
        break;
      case QueryParenthesisExpression parenthesisExpression:
        children.AddRange(SearchChildren(parenthesisExpression.QueryExpression));
        break;
      case BooleanBinaryExpression binaryExpression:
        children.AddRange(SearchChildren(binaryExpression.FirstExpression));
        children.AddRange(SearchChildren(binaryExpression.SecondExpression));
        break;
      case BooleanComparisonExpression comparisonExpression:
        children.AddRange(SearchChildren(comparisonExpression.FirstExpression));
        children.AddRange(SearchChildren(comparisonExpression.SecondExpression));
        break;
      case ScalarSubquery query:
        children.AddRange(SearchChildren(query.QueryExpression));
        break;
      case QuerySpecification spec:
      {
        children.Add(spec);
        foreach (var select in spec.SelectElements)
        {
          children.AddRange(SearchChildren(select));
        }
        if (spec.WhereClause != null)
        {
          children.AddRange(SearchChildren(spec.WhereClause.SearchCondition));
        }
        if (spec.FromClause != null)
        {
          foreach (var table in spec.FromClause.TableReferences)
          {
            children.AddRange(SearchChildren(table));
          }
        }
        break;
      }
      case SelectStatement selectStatement:
      {
        switch (selectStatement.QueryExpression)
        {
          case BinaryQueryExpression expression1:
            children.AddRange(SearchChildren(expression1.FirstQueryExpression));
            children.AddRange(SearchChildren(expression1.SecondQueryExpression));
            break;
          case QuerySpecification expression:
            children.Add(expression);
            break;
        }
        break;
      }
    }
    return children;
  }
}
