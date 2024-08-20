using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo1.Carga.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public class SqlServerArquivosParser : AbstractSqlServerParser
{
  public SqlServerArquivosParser(ITabelaService tabelaService, IVariavelService variavelService)
    : base(tabelaService, variavelService) { }

  public override List<ParDto> FetchNomesProcedures(CargaDto dto)
  {
    var filtroNome = dto.NomesProcedures?.Count > 0 ? dto.NomesProcedures[0].Nome : null;
    ValidarDtoAntesDeFetchNomesProcedures(dto);
    var path = Path.Combine("Resources", UploadDirecotroy);
    if (!Directory.Exists(path))
    {
      throw new ApplicationException("Directory with stored procedure files does not exist.");
    }
    var arquivos = Directory.GetFiles(path, "*");
    return arquivos
     .Select(t => t.Remove(0, t.LastIndexOf('\\')))
     .Select(t => t.Remove(t.LastIndexOf('.'))
       .Replace("/", "").Replace("\\", ""))
     .Select((nomeArquivo, id) => new ParDto(id, nomeArquivo))
     .Where(x => filtroNome == null || x.Nome.Contains(filtroNome))
     .ToList();
  }

  protected override StoredProcedure CriarProcedure(CargaDto dto, ParDto procedureDto)
  {
    var nomeMaisSufixo = $"{procedureDto.Nome}{SufixoArquivo}";
    var path = Path.Combine("Resources", UploadDirecotroy, nomeMaisSufixo);
    var txDefinicao = File.ReadAllText(path);
    var procedure = new StoredProcedure(dto.Schema, procedureDto.Nome, txDefinicao);
    return procedure;
  }

  protected override void ValidarDtoAntesDeFetchNomesProcedures(CargaDto dto)
  {
    // para listar os arquivos não pecisa faz nada
  }

  protected override void PrepararConexao(CargaDto dto) { }
}
