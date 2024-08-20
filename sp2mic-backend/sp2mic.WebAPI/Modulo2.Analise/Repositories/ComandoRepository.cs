using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class ComandoRepository : IComandoRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public ComandoRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public Comando? FindById(int id) => _dbContext.Comandos.Find(id);

  public async Task<Comando?> FindByIdAsync(int id) => await _dbContext.Comandos.FindAsync(id);

  public IEnumerable<Comando> FindAll() => _dbContext.Comandos.ToHashSet();

  public async Task<IEnumerable<Comando>> FindAllAsync()
    => await _dbContext.Comandos.OrderBy(x => x.NuOrdemExecucao)
     .ToListAsync();

  public IEnumerable<Comando> FindByFilter(ComandoFilterDto filter)
    => _dbContext.Comandos
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.TxComandoTratado == null ||
        x.TxComandoTratado.ToLower().Contains(filter.TxComandoTratado.ToLower()))
     .Where(x => filter.CoTipoComando == null || x.CoTipoComando == filter.CoTipoComando)
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     .Where(x => filter.IdEndpoint == null || x.IdEndpoint == filter.IdEndpoint)
     // .Include(x => x.IdEndpointNavigation)
     .OrderBy(x => x.NuOrdemExecucao)
     .ToHashSet();

  public async Task<IEnumerable<Comando>> FindByFilterAsync(ComandoFilterDto filter)
    => await _dbContext.Comandos
     .Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.TxComandoTratado == null ||
        x.TxComandoTratado.ToLower().Contains(filter.TxComandoTratado.ToLower()))
     .Where(x => filter.CoTipoComando == null || x.CoTipoComando == filter.CoTipoComando)
     .Where(x => filter.IdStoredProcedure == null ||
        x.IdStoredProcedure == filter.IdStoredProcedure)
     .Where(x => filter.IdEndpoint == null || x.IdEndpoint == filter.IdEndpoint)
     // .Include(x => x.IdEndpointNavigation)
     .OrderBy(x => x.NuOrdemExecucao)
     .ToListAsync();

  public Comando? FindByIdExpressao(int idExpressao)
    => _dbContext.Comandos.FirstOrDefault(x => x.IdExpressao == idExpressao);

  public IEnumerable<Comando> FindByIdStoredProcedure(int idStoredProcedure, bool isInterno)
  {
    return _dbContext.Comandos
      //.Include(x => x.IdExpressaoNavigation)
      //.Include(x => x.VariaveisComando)
      //.ThenInclude(cv => cv.IdVariavelNavigation)
      //.Include(x => x.VariaveisComando)
      //.ThenInclude(cv => cv.IdComandoNavigation)
     // .Include("IdExpressaoNavigation.IdOperandoEsquerdaNavigation")
     // .Include("IdExpressaoNavigation.IdOperandoDireitaNavigation")
      //.Include("VariaveisComando.IdVariavelNavigation")
      //.Include(x => x.IdEndpointNavigation)
      //.Include(x => x.IdStoredProcedureNavigation)
     // .Include(x => x.ComandosInternos)
     .Where(x => isInterno ? x.IdComandoOrigem != null : x.IdComandoOrigem == null)
     .Where(x => x.IdStoredProcedure == idStoredProcedure)
     .OrderBy(x => x.NuOrdemExecucao)
     .ToList();
  }

  public IEnumerable<Comando> RecuperarComandosIf(Comando comando)
    => _dbContext.Comandos
     // .Include(x => x.IdEndpointNavigation)
     // .ThenInclude(x => x.IdVariavelRetornadaNavigation)
     // .Include(x => x.IdEndpointNavigation)
     // .ThenInclude(x => x.Parametros)
     // .Include(x => x.IdEndpointNavigation)
     // .ThenInclude(e => e.IdMicrosservicoNavigation)
     .Where(x => x.IdComandoOrigem == comando.Id)
     .Where(x => x.SnCondicaoOrigem == true)
     .OrderBy(x => x.NuOrdemExecucao)
     .ToList();

  public IEnumerable<Comando> RecuperarComandosElse(Comando comando)
    => _dbContext.Comandos
     // .Include(x => x.IdEndpointNavigation)
     // .ThenInclude(x => x.IdVariavelRetornadaNavigation)
     // .Include(x => x.IdEndpointNavigation)
     // .ThenInclude(x => x.Parametros)
     // .Include(x => x.IdEndpointNavigation)
     // .ThenInclude(e => e.IdMicrosservicoNavigation)
     .Where(x => x.IdComandoOrigem == comando.Id)
     .Where(x => x.SnCondicaoOrigem == false)
     .OrderBy(x => x.NuOrdemExecucao)
     .ToList();

  public void Add(Comando obj) => _dbContext.Comandos.Add(obj);

  public void Update(Comando obj) => _dbContext.Comandos.Update(obj);

  public void Delete(Comando obj) => _dbContext.Comandos.Remove(obj);

  public void DeleteRange(IEnumerable<Comando> range) => _dbContext.Comandos.RemoveRange(range);

  public bool JaExiste(Comando obj)
  {
    var commandText
      = Concat(
        $"SELECT * FROM {Schema}.\"Comando\" WHERE  \"Tx_Comando\" = \'{obj.TxComando}\'",
        $" and \"Id_StoredProcedure\" = {obj.IdStoredProcedure} and \"Id\" != {obj.Id}");
    var result = _dbContext.Comandos.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }

  public IEnumerable<Comando> FindByIdProcedure(int idStoredProcedure)
    => _dbContext.Comandos
     .Where(x => x.IdStoredProcedure == idStoredProcedure)
     .ToList();
}
