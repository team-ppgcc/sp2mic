using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo1.Carga.Dtos;
using sp2mic.WebAPI.Modulo1.Carga.Services.Parser.Mock;
using sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Interfaces;

public interface ITradutorBanco : IApplicationService
{
  IEnumerable<ParDto> FetchNomesProcedures(CargaDto dto);
  IEnumerable<StoredProcedure> FetchProceduresSelecionadas(CargaDto dto);

  static ITradutorBanco GetTradutor(TipoBancoDeDadosEnum tipoBanco, ITabelaService tabelaService,
    IVariavelService variavelService)
  {
    ITradutorBanco tradutor = tipoBanco switch
    {
      TipoBancoDeDadosEnum.MOCK => MockParser.GetInstance(),
      TipoBancoDeDadosEnum.SQLSERVER => new SqlServerParser(tabelaService, variavelService),
      TipoBancoDeDadosEnum.SQLSERVER_FILE => new SqlServerArquivosParser(tabelaService, variavelService),
      TipoBancoDeDadosEnum.TIPO_NAO_MAPEADO => throw new BadHttpRequestException(
        "Database type not accepted."),
      _ => throw new BadHttpRequestException("Database type not accepted.")
    };
    return tradutor;
  }
}
