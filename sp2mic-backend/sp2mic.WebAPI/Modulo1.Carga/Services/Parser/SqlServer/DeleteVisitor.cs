using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class DeleteVisitor : TSqlFragmentVisitor
{
  public readonly List<DeleteStatement> DeleteStatements = new();

  public override void ExplicitVisit (DeleteStatement node)
  {
    DeleteStatements.Add(node);

    base.ExplicitVisit(node);

    //   Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
