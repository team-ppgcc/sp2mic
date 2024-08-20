using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class IfVisitor : TSqlFragmentVisitor
{
  public readonly List<IfStatement> IfStatements = new();

  public override void ExplicitVisit (IfStatement node)
  {
    IfStatements.Add(node);

    base.ExplicitVisit(node);

    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
