using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace sp2mic.WebAPI.CrossCutting;

public static class Application
{
  private static string Prefix {get;} = Assembly.GetEntryAssembly()?.FullName?[..6] ??
    throw new InvalidOperationException();

  public static IEnumerable<Assembly> Assemblies {get;} = DependencyContext.Default!
   .RuntimeLibraries.Where(library => library.Name.Contains(Prefix))
   .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
   .Select(Assembly.Load);
}
