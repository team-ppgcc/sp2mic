using sp2mic.WebAPI.Modulo1.Carga.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Interfaces;

public interface ICargaService : IApplicationService
{
  public IEnumerable<ParDto> ListarBancosSuportados();
  public Task<IEnumerable<ParDto>> ListarNomesProcedures (CargaDto dto);
  public void CarregarProceduresSelecionadas (CargaDto dto);
  public Task IncluirArquivoProcedure(IFormCollection formCollection);
  //public Task CalcularMetricas ();
  //public List<TabelasProcedureDto> IdentificarTabelasProcedures (CargaDto dto);
}
