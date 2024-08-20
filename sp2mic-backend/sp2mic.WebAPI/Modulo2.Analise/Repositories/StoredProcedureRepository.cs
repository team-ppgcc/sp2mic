using Microsoft.EntityFrameworkCore;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using static System.String;
using static sp2mic.WebAPI.CrossCutting.Constantes;

namespace sp2mic.WebAPI.Modulo2.Analise.Repositories;

public class StoredProcedureRepository : IStoredProcedureRepository
{
  private readonly DbContextSp2Mic _dbContext;

  public StoredProcedureRepository(DbContextSp2Mic dbContext) => _dbContext = dbContext;

  public StoredProcedure? FindById(int id)
    => _dbContext.StoredProcedures
      // .Include(x => x.DtoClasses)
      // .Include(x => x.IdDtoClasseNavigation)
      // .Include(x => x.Endpoints)
      // .Include("Endpoints.IdMicrosservicoNavigation")
      // .Include("Endpoints.IdDtoClasseNavigation")
      // .Include("Endpoints.IdVariavelRetornadaNavigation")
     .Include(x => x.TabelasAssociadas)
     .SingleOrDefault(x => x.Id == id);

  //    .Include(x => x.Comandos).AsNoTracking()
  //     //.Include("Comandos.VariaveisComando")
  //    .Include("Comandos.VariaveisComando.IdVariavelNavigation").AsNoTracking()
  //    .Include("Comandos.ComandosInternos").AsNoTracking()
  //    .Include(x => x.Endpoints).AsNoTracking()
  //    .Include("Endpoints.Parametros").AsNoTracking()
  //    .Include("Endpoints.Comandos").AsNoTracking()
  //    .Include("Endpoints.Operandos").AsNoTracking()
  //    .Include("Variaveis.VariaveisComando").AsNoTracking()
  //    .Include("Variaveis.Comandos").AsNoTracking()
  //    .Include("Variaveis.Comandos.IdExpressaoNavigation").AsNoTracking()
  //    .Include("Variaveis.VariaveisOperando").AsNoTracking()
  //    .Include("Variaveis.Operandos").AsNoTracking()
  //    .SingleOrDefault(x => x.Id == id);

  public async Task<StoredProcedure?> FindByIdAsync(int id)
  {
    // await _dbContext.Endpoints.LoadAsync();
    // await _dbContext.Endpoints.Include(x => x.IdMicrosservicoNavigation).LoadAsync();
    // await _dbContext.Endpoints.Include(x => x.IdDtoClasseNavigation).LoadAsync();
    // await _dbContext.Endpoints.Include(x => x.IdVariavelRetornadaNavigation).LoadAsync();

    var storedProcedures = await _dbContext.StoredProcedures
      // .Include(x => x.DtoClasses)
     .Include(x => x.IdDtoClasseNavigation)
     .Include(x => x.TabelasAssociadas)
      // .Include(x => x.Endpoints)
      //  //.ThenInclude(e => e.IdMicrosservicoNavigation)
     .Include("Endpoints.IdMicrosservicoNavigation")
     .Include("Endpoints.IdDtoClasseNavigation")
     .Include("Endpoints.IdVariavelRetornadaNavigation")
     .Include("Endpoints.TabelasAssociadas")
     .SingleOrDefaultAsync(x => x.Id == id);
    return storedProcedures;
  }
  //User usr = dbContext.Users.Include(a => a.UserDetails.Select(ud => ud.Address)).FirstOrDefault(a => a.UserId == userId);

  //  User usr = dbContext.Users.FirstOrDefault(a => a.UserId == userId);
  //UserDeatils ud = usr.UserDetails; // UserDetails are loaded here

  //User usr = dbContext.Users.FirstOrDefault(a => a.UserId == userId);
  //dbContext.Entry(usr).Reference(usr => usr.UserDetails).Load();
  public IEnumerable<StoredProcedure> FindAll()
    => _dbContext.StoredProcedures
      // .Include(x => x.DtoClasses)
      .Include(x => x.IdDtoClasseNavigation)
     .Include(x => x.TabelasAssociadas)
      // .Include(x => x.Endpoints)
      // .Include("Endpoints.IdMicrosservicoNavigation")
      // .Include("Endpoints.IdDtoClasseNavigation")
      // .Include("Endpoints.IdVariavelRetornadaNavigation")
     .OrderBy(x => x.NoStoredProcedure).ToList();

  public async Task<IEnumerable<StoredProcedure>> FindAllAsync()
    => await _dbContext.StoredProcedures
      // .Include(x => x.DtoClasses)
     .Include(x => x.IdDtoClasseNavigation)
     .Include(x => x.TabelasAssociadas)
      // .Include(x => x.Endpoints)
      // .ThenInclude(e => e.IdMicrosservicoNavigation)
      //.Include(x => x.Endpoints)
      //.ThenInclude(e => e.IdDtoClasseNavigation)
      // .Include(x => x.Endpoints)
      //.ThenInclude(e => e.IdVariavelRetornadaNavigation)
     .OrderBy(x => x.NoStoredProcedure).ToListAsync();

  public IEnumerable<StoredProcedure> FindByFilter(StoredProcedureFilterDto filter)
    => _dbContext.StoredProcedures.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoStoredProcedure == null ||
        x.NoStoredProcedure.ToLower().Contains(filter.NoStoredProcedure.ToLower()))
     .Where(x => filter.NoSchema == null ||
        x.NoSchema.ToLower().Contains(filter.NoSchema.ToLower()))
     .Where(x => filter.CoTipoDadoRetorno == null ||
        x.CoTipoDadoRetorno == filter.CoTipoDadoRetorno)
     .Where(x => filter.SnRetornoLista == null || x.SnRetornoLista == filter.SnRetornoLista)
     .Where(x => filter.IdDtoClasse == null || x.IdDtoClasse == filter.IdDtoClasse)
     .Where(x => filter.TxResultadoParser == null ||
        x.TxResultadoParser.ToLower().Contains(filter.TxResultadoParser.ToLower()))
     .Where(x => filter.SnSucessoParser == null || x.SnSucessoParser == filter.SnSucessoParser)
     .Where(x => filter.SnAnalisada == null || x.SnAnalisada == filter.SnAnalisada)
      // .Include(x => x.DtoClasses)
      .Include(x => x.IdDtoClasseNavigation)
     .Include(x => x.TabelasAssociadas)
      // .Include(x => x.Endpoints)
      // .Include("Endpoints.IdMicrosservicoNavigation")
      // .Include("Endpoints.IdDtoClasseNavigation")
      // .Include("Endpoints.IdVariavelRetornadaNavigation")
     .OrderBy(x => x.NoStoredProcedure).ToHashSet();

  public async Task<IEnumerable<StoredProcedure>> FindByFilterAsync(StoredProcedureFilterDto filter)
    => await _dbContext.StoredProcedures.Where(x => filter.Id == null || x.Id == filter.Id)
     .Where(x => filter.NoStoredProcedure == null ||
        x.NoStoredProcedure.ToLower().Contains(filter.NoStoredProcedure.ToLower()))
     .Where(x => filter.NoSchema == null ||
        x.NoSchema.ToLower().Contains(filter.NoSchema.ToLower()))
     .Where(x => filter.CoTipoDadoRetorno == null ||
        x.CoTipoDadoRetorno == filter.CoTipoDadoRetorno)
     .Where(x => filter.SnRetornoLista == null || x.SnRetornoLista == filter.SnRetornoLista)
     .Where(x => filter.IdDtoClasse == null || x.IdDtoClasse == filter.IdDtoClasse)
     .Where(x => filter.TxResultadoParser == null ||
        x.TxResultadoParser.ToLower().Contains(filter.TxResultadoParser.ToLower()))
     .Where(x
        => filter.SnSucessoParser == null || x.SnSucessoParser == filter.SnSucessoParser)
     .Where(x
        => filter.SnAnalisada == null || x.SnAnalisada == filter.SnAnalisada)
      // .Include(x => x.DtoClasses)
     .Include(x => x.IdDtoClasseNavigation)
     .Include(x => x.TabelasAssociadas)
      // .Include(x => x.Endpoints)
      // .Include("Endpoints.IdMicrosservicoNavigation")
      // .Include("Endpoints.IdDtoClasseNavigation")
      //.Include("Endpoints.IdVariavelRetornadaNavigation")
     .OrderBy(x => x.NoStoredProcedure).ToListAsync();

  public void Add(StoredProcedure obj) => _dbContext.StoredProcedures.Add(obj);

  public void Update(StoredProcedure obj) => _dbContext.StoredProcedures.Update(obj);

  public void Delete(StoredProcedure obj) => _dbContext.StoredProcedures.Remove(obj);

  public void DeleteRange(IEnumerable<StoredProcedure> storedProcedures)
    => _dbContext.StoredProcedures.RemoveRange(storedProcedures);

  public bool JaExiste(StoredProcedure obj)
  {
    var commandText
      = Concat(
        $"SELECT * FROM {Schema}.\"StoredProcedure\" WHERE  \"No_StoredProcedure\" = \'{
          obj.NoStoredProcedure}\'",
        $" and \"No_Schema\" = \'{obj.NoSchema}\' and \"Id\" != {obj.Id}");
    var result = _dbContext.StoredProcedures.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 1;
  }

  public StoredProcedure? FindBySchemaNome(string spSchema, string spName)
    => _dbContext.StoredProcedures
     .Where(x => x.NoSchema.ToLower().Equals(spSchema.ToLower()))
     .SingleOrDefault(x => x.NoStoredProcedure.ToLower().Equals(spName.ToLower()));

  public void UpdateRange(IEnumerable<StoredProcedure> obj)
    => _dbContext.StoredProcedures.UpdateRange(obj);

  //public StoredProcedure? GetDefinicaoById(int id)
  //  => _dbContext.StoredProcedures.SingleOrDefault(x => x.Id == id);

  public IEnumerable<StoredProcedure> RecuperarAnalisadas()
    => _dbContext.StoredProcedures
      .Include("IdDtoClasseNavigation")
      .Include("Comandos")
      .Include("Comandos.AsVariaveisDesseComando.IdVariavelNavigation")
      .Include("Comandos.ComandosInternos")
      .Include("Comandos.ComandosInternos.IdEndpointNavigation")
      .Include("Comandos.ComandosInternos.IdEndpointNavigation.IdMicrosservicoNavigation")
      .Include("Comandos.IdEndpointNavigation")
      .Include("Comandos.IdEndpointNavigation.IdDtoClasseNavigation")
      .Include("Comandos.IdEndpointNavigation.Parametros")
      .Include("Comandos.IdEndpointNavigation.IdVariavelRetornadaNavigation")
      .Include("Comandos.IdEndpointNavigation.IdMicrosservicoNavigation")
      .Include("Comandos.IdExpressaoNavigation")
      .Include("Comandos.IdExpressaoNavigation.IdOperandoEsquerdaNavigation")
      .Include("Comandos.IdExpressaoNavigation.IdOperandoEsquerdaNavigation.IdVariavelNavigation")
      .Include("Comandos.IdExpressaoNavigation.IdOperandoDireitaNavigation")
      .Include("Comandos.IdExpressaoNavigation.IdOperandoDireitaNavigation.IdVariavelNavigation")
      .Include("Endpoints")
      .Include("Endpoints.Parametros")
      .Include("Endpoints.IdDtoClasseNavigation")
      .Include("Endpoints.Parametros")
      .Include("Endpoints.IdVariavelRetornadaNavigation")
      .Include("Endpoints.IdMicrosservicoNavigation")
      .Include("Endpoints.Operandos")
      .Include("Endpoints.Comandos")
      .Include("Variaveis")
      .Include("Variaveis.OsComandosQueContemEssaVariavel")
      .Include("Variaveis.Operandos")
      //.Include("Variaveis.VariaveisOperando")
     .Where(x => x.SnAnalisada == true).OrderBy(x => x.NoStoredProcedure).ToList();

  public void SaveAll (IEnumerable<StoredProcedure> entityRange)
  {
    var listaProcedures = entityRange.Where(NaoExiste).ToList();
    _dbContext.StoredProcedures.AddRange(listaProcedures);
  }

  public void SaveOne (StoredProcedure obj)
  {
    if (NaoExiste(obj))
    {
      _dbContext.StoredProcedures.Update(obj);
    }
  }

  private bool NaoExiste(StoredProcedure obj)
  {
    var commandText
      = $"SELECT * FROM {Schema}.\"StoredProcedure\" WHERE \"No_Schema\" = \'{obj.NoSchema}\' and \"No_StoredProcedure\" = \'{obj.NoStoredProcedure}\'";
    var result = _dbContext.StoredProcedures.FromSqlRaw(commandText).ToHashSet();
    return result.Count == 0;
  }

  public void DeleteAll()
  {
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Comando_Variavel\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Comando\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Expressao\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Operando\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Endpoint_Variavel\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Endpoint_Tabela\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Endpoint\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Variavel\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"StoredProcedure_Tabela\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"StoredProcedure\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Atributo\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"DtoClasse\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Tabela\""));
    _dbContext.Database.ExecuteSqlRaw(Concat("DELETE FROM ", Schema, ".\"Microsservico\""));


    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Atributo_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Comando_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Comando_Variavel_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"DtoClasse_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Endpoint_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Expressao_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Microsservico_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Operando_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"StoredProcedure_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Tabela_Id_seq\" RESTART WITH 1"));
    _dbContext.Database.ExecuteSqlRaw(Concat("ALTER SEQUENCE ", Schema, ".\"Variavel_Id_seq\" RESTART WITH 1"));
  }

  public void Delete(int idStoredProcedure)
  {
    const string queryOperandoDireita = $"delete from {Schema}.\"Operando\" o where o.\"Id\" in (select e.\"Id_OperandoDireita\" from {Schema}.\"Expressao\" e where e.\"Id_OperandoDireita\" is not null and e.\"Id\" in (select c.\"Id_Expressao\" from {Schema}.\"Comando\" c where c.\"Id_StoredProcedure\" = 5 and c.\"Id_Expressao\" is not null))";
    _dbContext.Database.ExecuteSqlRaw(queryOperandoDireita);

    const string queryOperandoEsquerda = $"delete from {Schema}.\"Operando\" o where o.\"Id\" in (select e.\"Id_OperandoEsquerda\" from {Schema}.\"Expressao\" e where e.\"Id_OperandoEsquerda\" is not null and e.\"Id\" in (select c.\"Id_Expressao\" from {Schema}.\"Comando\" c where c.\"Id_StoredProcedure\" = 5 and c.\"Id_Expressao\" is not null))";
    _dbContext.Database.ExecuteSqlRaw(queryOperandoEsquerda);

    const string queryExpressao = $"delete from {Schema}.\"Expressao\" e where e.\"Id\" in (select c.\"Id_Expressao\" from {Schema}.\"Comando\" c where c.\"Id_StoredProcedure\" = 5 and c.\"Id_Expressao\" is not null)";
    _dbContext.Database.ExecuteSqlRaw(queryExpressao);

    var queryComando = $"delete from {Schema}.\"Comando\" c where c.\"Id_StoredProcedure\" = {idStoredProcedure}";
    _dbContext.Database.ExecuteSqlRaw(queryComando);

    var queryEndpoint = $"delete from {Schema}.\"Endpoint\" e where e.\"Id_StoredProcedure\" = {idStoredProcedure}";
    _dbContext.Database.ExecuteSqlRaw(queryEndpoint);

    var queryVariavel = $"delete from {Schema}.\"Variavel\" v where v.\"Id_StoredProcedure\" = {idStoredProcedure}";
    _dbContext.Database.ExecuteSqlRaw(queryVariavel);

    var queryTabela = $"delete from {Schema}.\"Tabela\" t where t.\"Id\" in (select spt.\"Id_Tabela\" from {Schema}.\"StoredProcedure_Tabela\" spt where spt.\"Id_StoredProcedure\" = {idStoredProcedure})";
    _dbContext.Database.ExecuteSqlRaw(queryTabela);

    var queryStoredProcedureTabela = $"delete from {Schema}.\"StoredProcedure_Tabela\" spt where spt.\"Id_StoredProcedure\" = {idStoredProcedure}";
    _dbContext.Database.ExecuteSqlRaw(queryStoredProcedureTabela);

    var queryStoredProcedure = $"delete from {Schema}.\"StoredProcedure\" sp where sp.\"Id\" = {idStoredProcedure}";
    _dbContext.Database.ExecuteSqlRaw(queryStoredProcedure);
  }
}
