using sp2mic.WebAPI.CrossCutting.Extensions;

namespace sp2mic.WebAPI.Domain.Enumerations;

public enum TipoEscopoEnum
{
  [EnumInformation(1, "Parâmetro da Stored Procedure")]
  PARAMETRO_STORED_PROCEDURE = 1,

  [EnumInformation(2, "Local")]
  LOCAL = 2,

  [EnumInformation(3, "Parâmetro do Endpoint")]
  PARAMETRO_ENDPOINT = 3
}
