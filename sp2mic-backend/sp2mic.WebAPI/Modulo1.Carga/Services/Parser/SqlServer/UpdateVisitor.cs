using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class UpdateVisitor : TSqlFragmentVisitor
{
  public readonly List<UpdateStatement> UpdateStatements = new();

  public override void ExplicitVisit (UpdateStatement node)
  {
    UpdateStatements.Add(node);

    base.ExplicitVisit(node);

    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
