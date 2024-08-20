using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class EnumeratorVisitor : TSqlFragmentVisitor
{
  public readonly List<TSqlStatement?> Nodes = new();

  public override void Visit (TSqlStatement? node)
  {
    base.Visit(node);
    if (!Nodes.Any(p
      => p != null &&
      node != null &&
      p.StartOffset <= node.StartOffset &&
      p.StartOffset + p.FragmentLength >= node.StartOffset + node.FragmentLength))
    {
      Nodes.Add(node);
    }
    // Console.WriteLine(node.GetType().Name + " found at line " + node.StartLine + ", column " +
    //                   node.StartColumn + ", length " + node.FragmentLength);
  }
}
