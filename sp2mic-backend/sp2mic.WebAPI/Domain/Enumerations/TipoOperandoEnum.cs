using sp2mic.WebAPI.CrossCutting.Extensions;

namespace sp2mic.WebAPI.Domain.Enumerations;

public enum TipoOperandoEnum
{
  [EnumInformation(1, "")]
  TIPO_NAO_MAPEADO = 1,

  [EnumInformation(2, "Constante")]
  CONSTANTE = 2,

  [EnumInformation(3, "Constante String")]
  CONSTANTE_STRING = 3,

  [EnumInformation(4, "Variável")]
  VARIAVEL = 4,

  [EnumInformation(5, "Expressão")]
  EXPRESSAO = 5,

  [EnumInformation(6, "Endpoint")]
  ENDPOINT = 6,

  [EnumInformation(7, "Método")]
  METODO = 7,

  [EnumInformation(8, "Constante null")]
  CONSTANTE_NULL = 8,
}
