using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class MicrosservicoRepository : IMicrosservicoRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public MicrosservicoRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public Microsservico? FindById(int id)
    => _dbContext.Microsservicos
      // .Include(x => x.DtoClasses)
      // .Include(x => x.Endpoints)
      // .ThenInclude(e => e.IdDtoClasseNavigation)
      // .Include("Endpoints.IdStoredProcedureNavigation")
     .SingleOrDefault(x => x.Id == id);

  public async Task<Microsservico?> FindByIdAsync(int id)
    => await _dbContext.Microsservicos
      // .Include(x => x.DtoClasses)
     .Include(x => x.Endpoints).ThenInclude(e => e.IdDtoClasseNavigation)
     .Include("Endpoints.IdStoredProcedureNavigation").SingleOrDefaultAsync(x => x.Id == id);

  public IEnumerable<Microsservico> FindAll()
    => _dbContext.Microsservicos
      // .Include(x => x.DtoClasses)
      // .Include(x => x.Endpoints)
      // .ThenInclude(e => e.IdDtoClasseNavigation)
      // .Include("Endpoints.IdStoredProcedureNavigation")
     .OrderBy(x => x.NoMicrosservico).ToHashSet();

  public async Task<IEnumerable<Microsservico>> FindAllAsync()
    => await _dbContext.Microsservicos
      // .Include(x => x.DtoClasses)
      // .Include(x => x.Endpoints)
      // .ThenInclude(e => e.IdDtoClasseNavigation)
      // .Include("Endpoints.IdStoredProcedureNavigation")
     .OrderBy(x => x.NoMicrosservico).ToListAsync();

  public IEnumerable<Microsservico> FindByFilter(MicrosservicoFilterDto filter)
    => _dbContext.Microsservicos.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoMicrosservico == null ||
        x.NoMicrosservico.ToLower().Contains(filter.NoMicrosservico.ToLower())).Where(x
        => filter.SnProntoParaGerar == null || x.SnProntoParaGerar == filter.SnProntoParaGerar)
      // .Include(x => x.DtoClasses)
      // .Include(x => x.Endpoints)
      // .ThenInclude(e => e.IdDtoClasseNavigation)
      // .Include("Endpoints.IdStoredProcedureNavigation")
     .OrderBy(x => x.NoMicrosservico).ToHashSet();

  public async Task<IEnumerable<Microsservico>> FindByFilterAsync(MicrosservicoFilterDto filter)
    => await _dbContext.Microsservicos.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoMicrosservico == null ||
        x.NoMicrosservico.ToLower().Contains(filter.NoMicrosservico.ToLower()))
     .Where(x => filter.SnProntoParaGerar == null ||
        x.SnProntoParaGerar == filter.SnProntoParaGerar)
     //.Include(x => x.DtoClasses) Microsservico nao tem mais classe
     .Include(x => x.Endpoints)
      // .ThenInclude(e => e.IdDtoClasseNavigation)
      // .Include("Endpoints.IdStoredProcedureNavigation")
     .OrderBy(x => x.NoMicrosservico).ToListAsync();

  public void Add(Microsservico obj) => _dbContext.Microsservicos.Add(obj);

  public void Update(Microsservico obj) => _dbContext.Microsservicos.Update(obj);

  public void Delete(Microsservico obj) => _dbContext.Microsservicos.Remove(obj);

  public void DeleteRange(IEnumerable<Microsservico> range) => _dbContext.Microsservicos.RemoveRange(range);

  public bool JaExiste(Microsservico obj)
  {
    var commandText
      = Concat(
        $"SELECT * FROM {Schema}.\"Microsservico\" WHERE  \"No_Microsservico\" = \'{
          obj.NoMicrosservico}\'", $" and \"Id\" != {obj.Id}");
    var result = _dbContext.Microsservicos.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }

  public IEnumerable<Microsservico> FindProntosParaGerar()
  {
    return _dbContext.Microsservicos.Where(x => x.SnProntoParaGerar == true)
     //.Include(m => m.DtoClasses).ThenInclude(c => c.Atributos) Microsservico nao tem mais classe
     .Include("Endpoints")
      // Parametros
     .Include("Endpoints.Parametros")
     .Include("Endpoints.Parametros.IdStoredProcedureNavigation")
     .Include("Endpoints.Parametros.OsComandosQueContemEssaVariavel")
     .Include("Endpoints.Parametros.Operandos")
     //.Include("Endpoints.Parametros.Operandos.IdVariavelNavigation")
     .Include("Endpoints.Parametros.Operandos.IdExpressaoNavigation")
     .Include("Endpoints.Parametros.Operandos.IdEndpointNavigation")
     .Include("Endpoints.Parametros.Operandos.ExpressaoIdOperandoDireitaNavigations")
     .Include("Endpoints.Parametros.Operandos.ExpressaoIdOperandoEsquerdaNavigations")
      // Operandos
     .Include("Endpoints.Operandos")
     //.Include("Endpoints.Operandos.IdVariavelNavigation")
     .Include("Endpoints.Operandos.IdExpressaoNavigation")
     //.Include("Endpoints.Operandos.IdEndpointNavigation")
     .Include("Endpoints.Operandos.ExpressaoIdOperandoDireitaNavigations")
     .Include("Endpoints.Operandos.ExpressaoIdOperandoEsquerdaNavigations")
      // Comandos
     .Include("Endpoints.Comandos")
     .Include("Endpoints.Comandos.IdStoredProcedureNavigation")
     .Include("Endpoints.Comandos.IdComandoOrigemNavigation")
     //.Include("Endpoints.Comandos.IdEndpointNavigation")
     .Include("Endpoints.Comandos.IdExpressaoNavigation")
     .Include("Endpoints.Comandos.AsVariaveisDesseComando")
     .Include("Endpoints.Comandos.ComandosInternos")
     .Include("Endpoints.Comandos.AsVariaveisDesseComando")
     //.Include("Endpoints.Comandos.AsVariaveisDesseComando.IdComandoNavigation")
     .Include("Endpoints.Comandos.AsVariaveisDesseComando.IdVariavelNavigation")
     .Include("Endpoints.Comandos.IdExpressaoNavigation.IdOperandoEsquerdaNavigation")
     .Include("Endpoints.Comandos.IdExpressaoNavigation.IdOperandoDireitaNavigation")
     .Include("Endpoints.Comandos.IdExpressaoNavigation.Comandos")
     .Include("Endpoints.Comandos.IdExpressaoNavigation.Operandos")
      // //.Include("Endpoints.Comandos.IdVariavelNavigation")nao tem mais a referencia
      //  //// .Include("Endpoints.IdMicrosservicoNavigation")
     .Include("Endpoints.IdDtoClasseNavigation")
     .Include("Endpoints.IdDtoClasseNavigation.Atributos")
     .OrderBy(x => x.NoMicrosservico)
     .ToHashSet();
  }
}
