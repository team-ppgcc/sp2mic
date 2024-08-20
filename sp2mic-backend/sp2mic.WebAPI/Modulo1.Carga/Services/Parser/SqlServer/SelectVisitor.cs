using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class SelectVisitor : TSqlFragmentVisitor
{
  public readonly List<SelectStatement> SelectStatements = new();

  public override void ExplicitVisit (SelectStatement node)
  {
    SelectStatements.Add(node);

    base.ExplicitVisit(node);

    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
