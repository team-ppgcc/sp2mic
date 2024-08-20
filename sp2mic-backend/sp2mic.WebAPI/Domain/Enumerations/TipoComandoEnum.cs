using sp2mic.WebAPI.CrossCutting.Extensions;

namespace sp2mic.WebAPI.Domain.Enumerations;

public enum TipoComandoEnum
{
  [EnumInformation(1, "Tipo não mapeado")]
  TIPO_NAO_MAPEADO = 1,
  [EnumInformation(2, "endpoint")]
  ENDPOINT = 2,
  [EnumInformation(3, "declaração")]
  DECLARACAO = 3,
  [EnumInformation(4, "atribuição")]
  ATRIBUICAO = 4,
  [EnumInformation(5, "if")]
  IF = 5,
  [EnumInformation(6, "while")]
  WHILE = 6,
  [EnumInformation(7, "Bloco de comandos")]
  BLOCO = 7,
  [EnumInformation(8, "Execute Statement")]
  EXEC = 8,
  [EnumInformation(9, "Begin Transaction Statement")]
  BEGIN_TRANSACTION = 9
}
