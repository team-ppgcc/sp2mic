using sp2mic.WebAPI.CrossCutting.Extensions;

namespace sp2mic.WebAPI.Domain.Enumerations;

public enum TipoOperadorEnum
{
  [EnumInformation(0, "", "", "")]
  SEM_OPERADOR = 0,

  [EnumInformation(1, "", "", "")]
  TIPO_NAO_MAPEADO = 1,

  [EnumInformation(2, "Adição", "", "+")]
  ADICAO = 2,

  [EnumInformation(3, "Subtração", "", "-")]
  SUBTRACAO = 3,

  [EnumInformation(4, "Divisão", "", "/")]
  DIVISAO = 4,

  [EnumInformation(5, "Multiplicação", "", "*")]
  MULTIPLICACAO = 5,

  [EnumInformation(6, "Maior que", "", ">")]
  MAIOR_QUE = 6,

  [EnumInformation(7, "Menor que", "", "<")]
  MENOR_QUE = 7,

  [EnumInformation(8, "Maior ou igual", "", ">=")]
  MAIOR_IGUAL = 8,

  [EnumInformation(9, "Menor ou igual", "", "<=")]
  MENOR_IGUAL = 9,

  [EnumInformation(10, "Igual", "", "==")]
  IGUAL = 10,

  [EnumInformation(11, "Diferente", "", "!=")]
  DIFERENTE = 11,

  [EnumInformation(12, "E", "", "&&")]
  E = 12,

  [EnumInformation(13, "Ou", "", "||")]
  OU = 13,

  [EnumInformation(14, "Exists", "", "exists")]
  EXISTS = 14,

  [EnumInformation(15, "Atribuição", "", "=")]
  ATRIBUICAO = 15
}
