using System.ComponentModel.DataAnnotations;
using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Modulo1.Carga.Dtos;


public class CargaDto
{

  [Required(ErrorMessage = "Database type must be filled.")]
  public TipoBancoDeDadosEnum? TipoBancoDeDados {get; set;}

  public string? NomeProcedure  {get; set;}

  public string Schema {get; set;} = null!;

  public List<ParDto>? NomesProcedures;

  public DadosConexaoDto? DadosConexao {get; set;} = new();
  //public IFormFile? FileSp {get; set;}
  public bool? SnCarregada {get; set;}
}
/*
 {
  "nomeTipoBanco": "SQLSERVER",
  "host": "192.168.15.11",
  "port": "1433",
  "databaseName": "dbp_54808_sig2000",
  "userName": "sig2000",
  "password": "sig2000123",
  "schema": "dbo"
}
 {
    "dadosConexao": {
        "nomeTipoBanco": "SQLSERVER",
        "host": "localhost",
        "port": "1433",
        "databaseName": "dbp_54808_sig2000",
        "userName": "sig2000",
        "password": "sig2000123",
        "schema":"dbo"
        },
    "nomesProcedures": [
        {
            "id": 31,
            "nome": "EXCLUIR_AGRUPAMENTO_PROGRAMA_ACAO"
        },
        {
        "id": 56,
        "nome": "spa_PACApoioAdicionaAtualizaFiltro"
        },
        {
            "id": 82,
            "nome": "spa_PACApoioSetGerarCodigoEmpreendimento"
        },
        {
        "id": 377,
        "nome": "spe_PACMonitoramentoInterSetOrgaoRestricao"
        }
    ]
}
*/
