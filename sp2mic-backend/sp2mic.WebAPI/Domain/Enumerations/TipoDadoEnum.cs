using sp2mic.WebAPI.CrossCutting.Extensions;

namespace sp2mic.WebAPI.Domain.Enumerations;

public enum TipoDadoEnum
{

  // 1-Tipo nao mapeado, 2-void, 3-DTO Classe, 4-String, 5-Integer, 6-Long, 7-Double, 8-Float,
  // 9-Boolean, 10-LocalDate, 11-LocalDateTime, 12-BigDecimal.
  [EnumInformation(1, "","", "")]
  TIPO_NAO_MAPEADO = 1,

  [EnumInformation(2, "void")]
  VOID = 2,

  [EnumInformation(3, "DTO", "","DTO Class")]
  DTO = 3,

  [EnumInformation(4, "String")]
  STRING = 4,

  [EnumInformation(5, "Integer")]
  INTEGER = 5,

  [EnumInformation(6, "Long")]
  LONG = 6,

  [EnumInformation(7, "Double")]
  DOUBLE = 7,

  [EnumInformation(8, "Float")]
  FLOAT = 8,

  [EnumInformation(9, "Boolean")]
  BOOLEAN = 9,

  [EnumInformation(10, "LocalDate", "java.time.LocalDate")]
  LOCAL_DATE = 10,

  [EnumInformation(11, "LocalDateTime", "java.time.LocalDateTime")]
  LOCAL_DATE_TIME = 11,

  [EnumInformation(12, "BigDecimal", "java.math.BigDecimal")]
  BIG_DECIMAL = 12,

  [EnumInformation(13, "Object")]
  OBJECT = 13
}
