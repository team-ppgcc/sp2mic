using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class DeclareVariableVisitor : TSqlFragmentVisitor
{
  public readonly List<DeclareVariableStatement> DeclareVariableStatements = new();

  public override void ExplicitVisit (DeclareVariableStatement node)
  {
    DeclareVariableStatements.Add(node);

    base.ExplicitVisit(node);

    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
