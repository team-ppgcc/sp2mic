using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class StatementVisitor : TSqlFragmentVisitor
{
  public readonly List<TSqlStatement> Statements = new();

  public override void Visit (TSqlStatement node)
  {
    Statements.Add(node);

    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
