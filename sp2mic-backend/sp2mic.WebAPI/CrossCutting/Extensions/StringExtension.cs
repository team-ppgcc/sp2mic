//using System.Text;

using System.Text.RegularExpressions;
using sp2mic.WebAPI.Domain.Enumerations;
using static System.String;

namespace sp2mic.WebAPI.CrossCutting.Extensions;

public static partial class StringExtension
{
  [GeneratedRegex(@"\t")]
  private static partial Regex RegexTabulacao();
  [GeneratedRegex(@"\s+")]
  private static partial Regex RegexEspacosTabulacoesQuebraDeLinhas();
  [GeneratedRegex(@"--[\s\S]*?\n")]
  private static partial Regex RegexComentarioUmaLinha();
  [GeneratedRegex(@"/\*[\s\S]*?\*/")]
  private static partial Regex RegexComentarioVariasLinhas();
  [GeneratedRegex(@"(?m)^\s$[\r\n]")]
  private static partial Regex RegexEmptyLines();
  [GeneratedRegex("[\"'][^'\"]*?['\"]|(?m)--.*$|(?s)/\\*.*?\\*/[\r\n]?")]
  private static partial Regex RegexComentarios();
  [GeneratedRegex(@"\n")]
  private static partial Regex RegexQuebraDeLinha();
  [GeneratedRegex(@"(?m)\s+$")]
  private static partial Regex RegexEspacoNoFinalDaLinha();

  // public static string RegexPrimeiraLetra = "(^\\w{1})|(\\s+\\w{1})";
  // public static string RegexEspacosNoInicio = "^\\s+";
  // public static string RegexEspacosNoFinal = "\\s+$";
  // public static string RegexQuebraDeLinha = "\n";
  // public static string RegexEspacosTabulacoesQuebraDeLinhas = "\\s+";
  // public static string RegexComentarioUmaLinha = "--[\\s\\S]*?\n";
  // public static string RegexComentarioVariasLinhas = "\\/\\*[\\s\\S]*?\\*\\/";

  // // remove empty lines
  // public static string RegexEmptyLines = "(?m)^[;\\s]*$[\r\n]";
  //
  // // find all regular comments if the comment is inside quotation marks, just ignore
  // public static string RegexComentarios = "[\"'][^'\"]*?['\"]|(?m)--.*$|(?s)/\\*.*?\\*/[\r\n]?";
  //
  // // these chars will be removed ;+[]'
  // public static string RegexChars = "\\(\\d+\\)|;|'|\\+|\\[|]";
  //
  // // this is to replace all multiple blank spaces, character '=' and '.' as ONE blank space
  // public static string RegexMultipleBlankSpaces = "\t{1,}|\r\n|\\s{2,}|=|\\.|,|\\(|\\)";

  public static string TrimAndReduce(this string str)
    => str.ConvertWhitespacesToSingleSpaces().Trim();

  private static string ConvertWhitespacesToSingleSpaces(this string value)
    => RegexEspacosTabulacoesQuebraDeLinhas().Replace(value, " ");

  //private static string RemoveWhiteSpaces(this string nome) => Regex.Replace(nome, @"\s+", "");
  private static string RemoveWhiteSpaces(this string nome) => RegexEspacosTabulacoesQuebraDeLinhas().Replace(nome, "");

  public static string TrimAndRemoveWhiteSpaces(this string str) => str.RemoveWhiteSpaces().Trim();

  public static string InicialMaiuscula(this string value)
  {
    //var str = Regex.Replace(value,@"\s+", "");
    var str = RegexEspacosTabulacoesQuebraDeLinhas().Replace(value, "");
    return Concat(str[..1].ToUpper(), str.AsSpan(1));
  }

  //public static string TrocarArrobaPorDoisPontos(this string text) => text.Replace("@", ":");

  public static string SemArroba(this string nome) => nome[..1].Equals("@") ? nome[1..] : nome;
  //private static string retirarArroba (string nome) => nome[..1].Equals("@") ? nome[1..] : nome;

  public static string InicialMinuscula(this string value)
  {
    //var str = Regex.Replace(value, @"\s+", "");
    var str = RegexEspacosTabulacoesQuebraDeLinhas().Replace(value, "");
    return Concat(str[..1].ToLower(), str.AsSpan(1));
  }

  public static string TratarComando(this string comando)
  {
    comando = RegexEspacosTabulacoesQuebraDeLinhas().Replace(comando, " ");
    comando = RegexComentarioUmaLinha().Replace(comando, " ");
    comando = RegexComentarioVariasLinhas().Replace(comando, " ");
    comando = RegexEmptyLines().Replace(comando, " ");
    comando = RegexComentarios().Replace(comando, " ");
    comando = RegexQuebraDeLinha().Replace(comando, " ");
    comando = RegexEspacoNoFinalDaLinha().Replace(comando, "");
    return comando.ToLower().Trim();
  }

  public static string TratarEndpoint(this string endpoint)
  {
    endpoint = endpoint.Replace("@", ":");
    endpoint = endpoint.Replace("FOR XML AUTO, ELEMENTS", "");
    endpoint = endpoint.Replace("for xml auto, elements", "");
    endpoint = endpoint.Replace("	", "    ");
    endpoint = RegexTabulacao().Replace(endpoint, "    ");
    endpoint = RegexEspacoNoFinalDaLinha().Replace(endpoint, "");
    return endpoint.ToLower().Trim();
  }

  public static string TratarProcedure(this string procedure)
  {
    procedure = procedure.Replace("	", "    ");
    procedure = Regex.Replace(procedure, @"\t", "    ");
    procedure = RegexEspacoNoFinalDaLinha().Replace(procedure, "");
    return procedure.Trim();
  }
  // public static string TratarEndpoint (this string endpoint)
  // {
  //   endpoint = endpoint.Replace(Constantes.RegexEspacosTabulacoesQuebraDeLinhas, " ");
  //   endpoint = endpoint.Replace(Constantes.RegexComentarioUmaLinha, " ");
  //   endpoint = endpoint.Replace(Constantes.RegexComentarioVariasLinhas, " ");
  //   endpoint = endpoint.Replace(Constantes.RegexEmptyLines, " ");
  //   endpoint = endpoint.Replace(Constantes.RegexComentarios, " ");
  //   endpoint = endpoint.Replace(Constantes.RegexQuebraDeLinha, " ");
  //   endpoint = endpoint.Replace("@", ":");
  //   endpoint = Regex.Replace(endpoint, @"\s+", " ");
  //   return endpoint.Trim().ToUpper();
  // }

  // public static string ToEncodedString(this Stream stream, Encoding? enc = null)
  // {
  //   enc ??= Encoding.UTF8;
  //
  //   var bytes = new byte[stream.Length];
  //   stream.Position = 0;
  //   stream.Read(bytes, 0, (int) stream.Length);
  //
  //   return enc.GetString(bytes);
  // }



  // public static string ComInicialMaiuscula(this string nome)
  // {
  //   var clean = Regex.Replace(nome, @"\s+", "");
  //   return string.Concat(clean[..1].ToUpper(), clean.AsSpan(1));
  // }
  //
  // public static string ComInicialMinuscula(string nome)
  // {
  //   var clean = Regex.Replace(nome, @"\s+", "");
  //   return string.Concat(clean[..1].ToLower(), clean.AsSpan(1));
  // }

  public static string TodoMinusculo (this string nome)
    => RegexEspacosTabulacoesQuebraDeLinhas().Replace(nome, "").ToLower();

  public static string? TrataNullTipoDado (this TipoDadoEnum? tipo)
    => tipo?.GetNome();
  // string shadeName = ((Shade) 1).ToString();

  public static string? TrataNullTipoEscopo (this TipoEscopoEnum? tipo)
    => tipo?.GetNome();

}
