using Microsoft.SqlServer.Management.Smo;
using sp2mic.WebAPI.Modulo1.Carga.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using StoredProcedure = sp2mic.WebAPI.Domain.Entities.StoredProcedure;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class SqlServerParser : AbstractSqlServerParser
{
  private Database? _db;

  public SqlServerParser(ITabelaService tabelaService, IVariavelService variavelService)
    : base(tabelaService, variavelService) {}

  public override List<ParDto> FetchNomesProcedures(CargaDto dto)
    => dto.Schema == "IdentificarEmpreendimentos" ?
      RecuperarSPsIdentificarEmpreendimentos() : RecuperarDoBanco(dto);

  private static List<ParDto> RecuperarSPsIdentificarEmpreendimentos()
  {
    List<ParDto> arquivos = new()
    {
      new ParDto(877, "sps_PACApoioGetOrgaoSalaSituacao"),
      new ParDto(878, "sps_PACApoioGetOrgaosComAcao"),
      new ParDto(889, "sps_PACApoioGetOrgaoUsuario"),
      new ParDto(1136, "sps_PACCadastroAnoVigente"),
      new ParDto(661, "sps_PAC2CadastroGetEmpreendimentoIdentificar"),
      new ParDto(669, "sps_PAC2CadastroGetSegmentos"),
      new ParDto(665, "sps_PAC2CadastroGetFuncionaisAnoAnterior"),
      new ParDto(666, "sps_PAC2CadastroGetHistoricoEmpreendimento"),
      new ParDto(1311, "sps_PACCadastroGetUnidadesOrcamentariasPorOrgao"),
      new ParDto(656, "sps_PAC2CadastroGetAcoesPac"),
      new ParDto(908, "sps_PACApoioGetSetores"),
      new ParDto(658, "sps_PAC2CadastroGetAnosFuncional"),
      new ParDto(785, "sps_PACApoioGetEmpreendimentoEmEdicao"),
      new ParDto(660, "sps_PAC2CadastroGetDetalheAcao"),
      new ParDto(671, "sps_PAC2CadastroGetTotalAutorizacao"),
      new ParDto(37, "spa_PAC2CadastroSalvarEmpreendimento"),
      new ParDto(38, "spa_PAC2CadastroVincularFuncional"),
      new ParDto(664, "sps_PAC2CadastroGetFuncionais"),
      new ParDto(323, "spe_PAC2CadastroExcluiFuncional"),
      new ParDto(659, "sps_PAC2CadastroGetCodigoEmpreendimento"),
      new ParDto(662, "sps_PAC2CadastroGetEmpreendimentos"),
      new ParDto(35, "spa_PAC2CadastroCopiaEmpreendimentoMomento"),
      new ParDto(325, "spe_PAC2CadastroSetExcluirCadastro"),
      new ParDto(667, "sps_PAC2CadastroGetLinhaAutorizacao"),
      new ParDto(668, "sps_PAC2CadastroGetMonitoramento"),
      new ParDto(443, "spi_PACApoioSetIncluirLog"),
      new ParDto(324, "spe_PAC2CadastroExcluirEmpreendimento"),
      new ParDto(36, "spa_PAC2CadastroReativarEmpreendimento"),
      new ParDto(912, "sps_PACApoioGetSiglaOrgao"),
      new ParDto(687, "sps_PACAcessoGetUsuarioEditandoEmpreendimento"),
      new ParDto(1336, "sps_PACCadastroGetVerificaValoresReferencia"),
      new ParDto(754, "sps_PACApoioGetCicloVigente"),
      new ParDto(884, "sps_PACApoioGetOrgaosFiltroIdentificar"),
      new ParDto(1172, "sps_PACCadastroGetDadosFontesRecurso"),
      new ParDto(677, "sps_PACAcessoGetPermissao"),
      new ParDto(655, "sps_PAC2CadastroConsultaEmpreendimentos")
    };
    return arquivos;
  }

  // private static List<ParDto> RecuperarSPsValidacao(CargaDto dto)
  // {
  //   List<ParDto> arquivos = new()
  //   {
  //     new ParDto(656, "sps_PAC2CadastroGetAcoesPac"),
  //     new ParDto(661, "sps_PAC2CadastroGetEmpreendimentoIdentificar"),
  //     new ParDto(665, "sps_PAC2CadastroGetFuncionaisAnoAnterior"),
  //     new ParDto(666, "sps_PAC2CadastroGetHistoricoEmpreendimento"),
  //     new ParDto(669, "sps_PAC2CadastroGetSegmentos"),
  //     new ParDto(877, "sps_PACApoioGetOrgaoSalaSituacao"),
  //     new ParDto(889, "sps_PACApoioGetOrgaoUsuario"),
  //     new ParDto(1136, "sps_PACCadastroAnoVigente"),
  //     new ParDto(1311, "sps_PACCadastroGetUnidadesOrcamentariasPorOrgao")
  //   };
  //   return arquivos;
  // }

  private List<ParDto> RecuperarDoBanco(CargaDto dto)
  {
    List<ParDto> pars = new();
    ValidarDtoAntesDeFetchNomesProcedures(dto);
    var s = new Server(dto.DadosConexao!.Host);
    var db = s.Databases[dto.DadosConexao!.DatabaseName];
    for (var id = 0; id < db.StoredProcedures.Count; id++)
    {
      if (db.StoredProcedures[id].Schema != dto.Schema ||
        db.StoredProcedures[id].Name[..3] == "dt_")
      {
        continue;
      }
      var sp = db.StoredProcedures[id];
      pars.Add(new ParDto(id, sp.Name));
    }
    return pars;
  }

  protected override StoredProcedure CriarProcedure(CargaDto dto, ParDto procedureDto)
  {
    var s = new Server(dto.DadosConexao!.Host);
    _db = s.Databases[dto.DadosConexao!.DatabaseName];
    var sp = _db.StoredProcedures[procedureDto.Id];
    return new StoredProcedure(sp.Schema, sp.Name, string.Concat(sp.TextHeader, sp.TextBody));
  }

  protected override void ValidarDtoAntesDeFetchNomesProcedures(CargaDto dto)
  {
    if (dto.DadosConexao!.Host == null || dto.DadosConexao!.Port == null ||
      dto.DadosConexao!.DatabaseName == null || dto.DadosConexao!.UserName == null ||
      dto.DadosConexao!.Password == null || dto.Schema == null)
    {
      throw new ApplicationException("Please fill in the required fields.");
    }
  }

  protected override void PrepararConexao(CargaDto dto)
  {
    var s = new Server(dto.DadosConexao!.Host);
    _db = s.Databases[dto.DadosConexao!.DatabaseName];
  }
}
