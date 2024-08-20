// using System.Text.RegularExpressions;
// using sp2mic.WebAPI.Domain.Enumerations;
// using sp2mic.WebAPI.Domain.Enumerations.Extensions;
//
// namespace sp2mic.WebAPI.CrossCutting.Util;
//
// public static partial class StringUtil
// {
//   // public static string ComInicialMaiuscula(string nome)
//   // {
//   //   var clean = Regex.Replace(nome, @"\s+", "");
//   //   return string.Concat(clean[..1].ToUpper(), clean.AsSpan(1));
//   // }
//   //
//   // public static string ComInicialMinuscula(string nome)
//   // {
//   //   var clean = Regex.Replace(nome, @"\s+", "");
//   //   return string.Concat(clean[..1].ToLower(), clean.AsSpan(1));
//   // }
//
//   public static string TodoMinusculo (string nome) => MyRegex().Replace(nome, "").ToLower();
//
//   public static string? TrataNullTipoDado (TipoDadoEnum? tipo)
//     => tipo?.GetNome();
//   // string shadeName = ((Shade) 1).ToString();
//
//   public static string? TrataNullTipoEscopo (TipoEscopoEnum? tipo)
//     => tipo?.GetNome();
//   [GeneratedRegex("\\s+")]
//   private static partial Regex MyRegex();
// }
