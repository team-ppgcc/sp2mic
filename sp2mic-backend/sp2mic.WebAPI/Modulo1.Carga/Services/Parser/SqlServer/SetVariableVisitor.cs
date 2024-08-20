using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class SetVariableVisitor : TSqlFragmentVisitor
{
  public readonly List<SetVariableStatement> SetVariableStatements = new();

  public override void ExplicitVisit (SetVariableStatement node)
  {
    SetVariableStatements.Add(node);

    base.ExplicitVisit(node);

    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
