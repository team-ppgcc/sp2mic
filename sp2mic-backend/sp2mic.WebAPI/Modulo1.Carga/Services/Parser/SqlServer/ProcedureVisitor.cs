using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class ProcedureVisitor : TSqlFragmentVisitor
{
  public readonly List<CreateProcedureStatement> Statements = new();

  public override void Visit (CreateProcedureStatement node)
  {
    Statements.Add(node);

    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
