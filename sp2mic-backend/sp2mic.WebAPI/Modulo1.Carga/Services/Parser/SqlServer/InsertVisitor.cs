using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class InsertVisitor : TSqlFragmentVisitor
{
  public readonly List<InsertStatement> InsertStatements = new();

  public override void ExplicitVisit (InsertStatement node)
  {
    InsertStatements.Add(node);

    base.ExplicitVisit(node);

    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
