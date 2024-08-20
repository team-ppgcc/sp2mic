using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class EndpointRepository : IEndpointRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public EndpointRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public Endpoint? FindById(int id)
    => _dbContext.Endpoints
      // .Include(x => x.Parametros)
     .Include(x => x.IdDtoClasseNavigation)
     .Include(x => x.IdStoredProcedureNavigation)
     .Include(x => x.IdMicrosservicoNavigation)
      // .ThenInclude(sp => sp.DtoClasses)
     .Include(x => x.IdVariavelRetornadaNavigation)
     .Include(x => x.TabelasAssociadas)
     .SingleOrDefault(x => x.Id == id);

  public async Task<Endpoint?> FindByIdAsync(int id)
    => await _dbContext.Endpoints
      // .Include(x => x.Parametros)
     .Include(x => x.IdDtoClasseNavigation)
     .Include(x => x.IdStoredProcedureNavigation)
     .Include(x => x.IdMicrosservicoNavigation)
      // .ThenInclude(sp => sp.DtoClasses)
     .Include(x => x.IdVariavelRetornadaNavigation)
     .Include(x => x.TabelasAssociadas)
     .SingleOrDefaultAsync(x => x.Id == id);

  public IEnumerable<Endpoint> FindAll()
    => _dbContext.Endpoints
      // .Include(x => x.Parametros)
      // .Include(x => x.IdDtoClasseNavigation)
      // .Include(x => x.IdStoredProcedureNavigation)
      // .ThenInclude(sp => sp.DtoClasses)
      // .Include(x => x.IdVariavelRetornadaNavigation)
     .Include(x => x.TabelasAssociadas)
     .OrderBy(x => x.NoMetodoEndpoint).ToHashSet();

  public async Task<IEnumerable<Endpoint>> FindAllAsync()
    => await _dbContext.Endpoints
      // .Include(x => x.Parametros)
      // .Include(x => x.IdDtoClasseNavigation)
      // .Include(x => x.IdStoredProcedureNavigation)
      // .ThenInclude(sp => sp.DtoClasses)
      // .Include(x => x.IdVariavelRetornadaNavigation)
     .Include(x => x.TabelasAssociadas)
     .OrderBy(x => x.NoMetodoEndpoint).ToListAsync();

  public IEnumerable<Endpoint> FindByFilter(EndpointFilterDto filter)
    => _dbContext.Endpoints.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoMetodoEndpoint != null && x.NoMetodoEndpoint != null &&
        x.NoMetodoEndpoint.ToLower().Contains(filter.NoMetodoEndpoint.ToLower())).Where(x
        => filter.NoPath != null && x.NoPath != null &&
        x.NoPath.ToLower().Contains(filter.NoPath.ToLower()))
      //.Where(x => filter.NoPath == null || x.NoPath == null ? false : x.NoPath.ToLower().Contains(filter.NoPath.ToLower()))
     .Where(x => filter.TxEndpoint == null ||
        x.TxEndpoint.ToLower().Contains(filter.TxEndpoint.ToLower()))
     .Where(x => filter.TxEndpointTratado == null ||
        x.TxEndpointTratado.ToLower().Contains(filter.TxEndpointTratado.ToLower()))
     .Where(x => filter.CoTipoSqlDml == null || x.CoTipoSqlDml == filter.CoTipoSqlDml)
     .Where(x => filter.CoTipoDadoRetorno == null ||
        x.CoTipoDadoRetorno == filter.CoTipoDadoRetorno)
     .Where(x => filter.SnRetornoLista == null || x.SnRetornoLista == filter.SnRetornoLista)
     .Where(x => filter.SnAnalisado == null || x.SnAnalisado == filter.SnAnalisado)
     .Where(x => filter.IdMicrosservico == null || x.IdMicrosservico == filter.IdMicrosservico)
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     .Where(x => filter.IdDtoClasse == null || x.IdDtoClasse == filter.IdDtoClasse).Where(x
        => filter.IdVariavelRetornada == null ||
        x.IdVariavelRetornada == filter.IdVariavelRetornada)
      // .Include(x => x.Parametros)
      // .Include(x => x.IdDtoClasseNavigation)
      // .Include(x => x.IdMicrosservicoNavigation)
      // .Include(x => x.IdStoredProcedureNavigation)
      // .Include(x => x.IdVariavelRetornadaNavigation)
     .Include(x => x.TabelasAssociadas)
     .OrderBy(x => x.NoMetodoEndpoint).ToHashSet();

  public async Task<IEnumerable<Endpoint>> FindByFilterAsync(EndpointFilterDto filter)
    => await _dbContext.Endpoints.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoMetodoEndpoint != null && x.NoMetodoEndpoint != null &&
        x.NoMetodoEndpoint.ToLower().Contains(filter.NoMetodoEndpoint.ToLower())).Where(x
        => filter.NoPath != null && x.NoPath != null &&
        x.NoPath.ToLower().Contains(filter.NoPath.ToLower()))
      //.Where(x => filter.NoPath == null || x.NoPath == null ? false : x.NoPath.ToLower().Contains(filter.NoPath.ToLower()))
     .Where(x => filter.TxEndpoint == null ||
        x.TxEndpoint.ToLower().Contains(filter.TxEndpoint.ToLower()))
     .Where(x => filter.TxEndpointTratado == null ||
        x.TxEndpointTratado.ToLower().Contains(filter.TxEndpointTratado.ToLower()))
     .Where(x => filter.CoTipoSqlDml == null || x.CoTipoSqlDml == filter.CoTipoSqlDml)
     .Where(x => filter.CoTipoDadoRetorno == null ||
        x.CoTipoDadoRetorno == filter.CoTipoDadoRetorno)
     .Where(x => filter.SnRetornoLista == null || x.SnRetornoLista == filter.SnRetornoLista)
     .Where(x => filter.SnAnalisado == null || x.SnAnalisado == filter.SnAnalisado)
     .Where(x => filter.IdMicrosservico == null || x.IdMicrosservico == filter.IdMicrosservico)
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     .Where(x => filter.IdDtoClasse == null || x.IdDtoClasse == filter.IdDtoClasse).Where(x
        => filter.IdVariavelRetornada == null ||
        x.IdVariavelRetornada == filter.IdVariavelRetornada)
      // .Include(x => x.Parametros)
      // .Include(x => x.TabelasAssociadas)
      // .Include(x => x.IdDtoClasseNavigation)
      // .Include(x => x.IdMicrosservicoNavigation)
      // .Include(x => x.IdStoredProcedureNavigation)
      // .Include(x => x.IdVariavelRetornadaNavigation)
     .Include(x => x.TabelasAssociadas)
     .OrderBy(x => x.NoMetodoEndpoint).ToListAsync();

  public void Add(Endpoint obj) => _dbContext.Endpoints.Add(obj);

  public void Update(Endpoint obj) => _dbContext.Endpoints.Update(obj);

  public void Delete(Endpoint obj) => _dbContext.Endpoints.Remove(obj);

  public void DeleteRange(IEnumerable<Endpoint> range) => _dbContext.Endpoints.RemoveRange(range);

  public bool JaExiste(Endpoint obj)
  {
    var commandText
      = Concat($"SELECT * FROM {Schema}.\"Endpoint\" WHERE  \"No_MetodoEndpoint\" = \'{obj.NoMetodoEndpoint}\'",
        $" and \"Id_Microsservico\" = {obj.IdMicrosservico} and \"Id\" != {obj.Id}");
    var result = _dbContext.Endpoints.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }

  public void UpdateRange(IEnumerable<Endpoint> obj) => _dbContext.Endpoints.UpdateRange(obj);

  public IEnumerable<Endpoint> FindByIdDtoClasse(int? idDtoClasse)
    => _dbContext.Endpoints.Where(x => x.IdDtoClasse == idDtoClasse).OrderBy(x => x.NoMetodoEndpoint)
     .ToHashSet();

  public IEnumerable<Endpoint> FindByIdMicrosservico(int idMicrosservico)
    => _dbContext.Endpoints.Where(x => x.IdMicrosservico == idMicrosservico)
      // .Include(x => x.Comandos)
      // .Include(x => x.Operandos)
      // .Include(x => x.Parametros)
      // .Include(x => x.IdDtoClasseNavigation)
      // .Include(x => x.IdMicrosservicoNavigation)
      // .Include(x => x.IdStoredProcedureNavigation)
      // .Include(x => x.IdVariavelRetornadaNavigation)
     .OrderBy(x => x.NoMetodoEndpoint).ToHashSet();

  public IEnumerable<Endpoint> FindByIdStoredProcedure(int idStoredProcedure)
    => _dbContext.Endpoints.Where(x => x.IdStoredProcedure == idStoredProcedure)
      // .Include(x => x.Comandos)
      // .Include(x => x.Operandos)
      // .Include(x => x.Parametros)
      // .Include(x => x.IdDtoClasseNavigation)
      // .Include(x => x.IdMicrosservicoNavigation)
      // .Include(x => x.IdStoredProcedureNavigation)
      // .Include(x => x.IdVariavelRetornadaNavigation)
     .OrderBy(x => x.NoMetodoEndpoint).ToHashSet();

  public IEnumerable<Endpoint>? FindByIdProcedure(int idStoredProcedure)
    => _dbContext.Endpoints
     .Where(x => x.IdStoredProcedure == idStoredProcedure)
     .ToList();

  public void AjustarRetornoEndpoints()
  {
    const string query = $"update {Schema}.\"Endpoint\" ep set \"Co_TipoDadoRetorno\" = (select ex.\"Co_TipoDadoRetorno\" from {Schema}.\"Expressao\" ex where ex.\"Id_OperandoEsquerda\" = (select op.\"Id\" from {Schema}.\"Operando\" op where ep.\"Id\" = op.\"Id_Endpoint\") or ex.\"Id_OperandoDireita\" = (select op.\"Id\" from {Schema}.\"Operando\" op where ep.\"Id\" = op.\"Id_Endpoint\")) where ep.\"Id\" in (select \"Id_Endpoint\" from {Schema}.\"Operando\" where \"Id_Endpoint\" is not null)";
    _dbContext.Database.ExecuteSqlRaw(query);
  }

  public void AjustarNomesEndpoints()
  {
    const string query = $"update {Schema}.\"Endpoint\" set \"No_MetodoEndpoint\" = concat(\"No_MetodoEndpoint\", cast(\"Id\" as varchar));";
    _dbContext.Database.ExecuteSqlRaw(query);
  }

  public void AjustarPathsEndpoints()
  {
    const string query = $"update {Schema}.\"Endpoint\" set \"No_Path\" = concat(\"No_Path\", cast(\"Id\" as varchar));";
    _dbContext.Database.ExecuteSqlRaw(query);
  }
}
