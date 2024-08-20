namespace sp2mic.WebAPI.CrossCutting;

public static class Constantes
{
  //public const string SCHEMA = "public";
  public const string Schema = "sp2mic";
  //public const string SCHEMA = "sp2mic1";

  public const string UploadDirecotroy = "stored-procedures-upload";
  public const string SufixoArquivo = ".sql";

  public const int IdProcedureAnalisada = 1614;

  public const string RegexPrimeiraLetra = @"(^\w{1})|(\s+\w{1})";
  public const string RegexEspacosNoInicio = "^\\s+";
  public const string RegexEspacosNoFinal = "\\s+$";
  public const string RegexQuebraDeLinha = "\n";
  public const string RegexEspacosTabulacoesQuebraDeLinhas = "\\s+";
  public const string RegexComentarioUmaLinha = "--[\\s\\S]*?\n";
  public const string RegexComentarioVariasLinhas = @"\/\*[\s\S]*?\*\/";

  //public const string RegexEspacoNoFinalDaLinha = @"\h+$";
  public const string RegexEspacoNoInicioDaLinha = @"^\h+";
  // remove empty lines
  //public const string RegexEmptyLines = "(?m)^[;\\s]*$[\r\n]";
  //public const string RegexEmptyLines = "(?m)^[;\\h]*$[\r\n]";

  public const string RegexEmptyLinesEspacoNoFinalDaLinha = @"(?m)^[\s]*$[\r\n]|\h+$";
  public const string RegexEmptyLines = @"(?m)^[\s]*$[\r\n]";
  public const string RegexEspacoNoFinalDaLinha = @"(?m)[\s]+$";

  // find all regular comments if the comment is inside quotation marks, just ignore
  public const string RegexComentarios
    = "[\"'][^'\"]*?['\"]|(?m)--.*$|(?s)/\\*.*?\\*/[\r\n]?";
  // these chars will be removed ;+[]'
  public const string RegexChars = "\\(\\d+\\)|;|'|\\+|\\[|]";
  // this is to replace all multiple blank spaces, character '=' and '.' as ONE blank space
  public const string RegexMultipleBlankSpaces = "\t{1,}|\r\n|\\s{2,}|=|\\.|,|\\(|\\)";
}
