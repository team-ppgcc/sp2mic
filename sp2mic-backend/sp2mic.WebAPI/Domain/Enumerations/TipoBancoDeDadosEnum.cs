using sp2mic.WebAPI.CrossCutting.Extensions;

namespace sp2mic.WebAPI.Domain.Enumerations;

public enum TipoBancoDeDadosEnum
{
  //ORACLE,

  //POSTGRESQL,

  //MYSQL,

  [EnumInformation(1, "Tipo não mapeado")]
  TIPO_NAO_MAPEADO = 1,

  [EnumInformation(2, "MOCK", "MOCK - Banco de teste")]
  MOCK = 2,

  [EnumInformation(3, "Microsoft SQL SERVER", "SQLSERVER - Banco Microsoft SQL SERVER")]
  SQLSERVER = 3,

  [EnumInformation(4, "Microsoft SQL SERVER - File Upload", "SQLSERVER_FILE - Leitura em arquivos")]
  SQLSERVER_FILE = 4
}
