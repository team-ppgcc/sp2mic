using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using static System.String;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class VariavelService : IVariavelService
{
  private readonly IUnitOfWork _uow;

  public VariavelService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public Variavel FindById(int id)
  {
    var obj = _uow.VariavelRepository.FindById(id);
    return obj ?? throw new BadHttpRequestException("Variable not found.");
  }

  public async Task<Variavel> FindByIdAsync(int id)
  {
    var obj = await _uow.VariavelRepository.FindByIdAsync(id);
    return obj ?? throw new BadHttpRequestException("Variable not found.");
  }

  public ISet<Variavel> FindAll()
    => _uow.VariavelRepository.FindAll().ToHashSet();

  public async Task<IEnumerable<Variavel>> FindAllAsync()
    => await _uow.VariavelRepository.FindAllAsync();

  public IEnumerable<Variavel> FindByFilter(VariavelFilterDto? filter)
    => filter is null ? FindAll() : _uow.VariavelRepository.FindByFilter(filter);

  public async Task<IEnumerable<Variavel>> FindByFilterAsync(VariavelFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.VariavelRepository.FindByFilterAsync(filter);

  public async Task<Variavel> AddAsync(Variavel? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Variable must be informed.");
    }
    _uow.VariavelRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, Variavel? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("Variable must be informed.");
    }
    var existente = await _uow.VariavelRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Variable not found.");
    }
    if (_uow.VariavelRepository.JaExiste(existente))
    {
      throw new BadHttpRequestException("Already existing attribute.");
    }
    var atualizado = Sincronizar(atual, existente);
    _uow.VariavelRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static Variavel Sincronizar(Variavel atual, Variavel existente)
  {
    //var atualizado = new Variavel();
    if (!Equals(atual.NoVariavel, existente.NoVariavel))
    {
      existente.NoVariavel = atual.NoVariavel;
    }
    if (!Equals(atual.CoTipoDado, existente.CoTipoDado))
    {
      existente.CoTipoDado = atual.CoTipoDado;
    }
    if (!Equals(atual.CoTipoEscopo, existente.CoTipoEscopo))
    {
      existente.CoTipoEscopo = atual.CoTipoEscopo;
    }
    if (!Equals(atual.NuTamanho, existente.NuTamanho))
    {
      existente.NuTamanho = atual.NuTamanho;
    }
    if (!Equals(atual.IdStoredProcedure, existente.IdStoredProcedure))
    {
      existente.IdStoredProcedure = atual.IdStoredProcedure;
    }
    return existente;
  }

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.VariavelRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Variable not found.");
    }
    _uow.VariavelRepository.Delete(existente);
    await _uow.SaveAsync();
  }

  public IEnumerable<Variavel?> FindByIdStoredProcedure(int idStoredProcedure)
    => _uow.VariavelRepository.FindByIdStoredProcedure(idStoredProcedure);

  public void TratarNomesDasVariaveis()
  {
    var todasAsVariaveis = _uow.VariavelRepository.FindAll().ToList();
    foreach (var v in todasAsVariaveis)
    {
      if (IsNullOrEmpty(v.NoVariavel))
      {
        continue;
      }
      var temp = v.NoVariavel.ToLower();
      v.NoVariavel = temp;
      Console.WriteLine(v.NoVariavel);
    }
    _uow.VariavelRepository.UpdateRange(todasAsVariaveis);
    _uow.SaveAsync();
  }

  // private readonly IUnitOfWork _uow;
  // private readonly IMapper _mapper;
  // private readonly DbContextSp2Mic _dbContext;
  // private readonly DbSet<Variavel> _dbEntity;
  // private readonly ILogger<VariavelService> _logger;
  // private readonly IComandoService _comandoService;
  //
  // public VariavelService(IUnitOfWork uow, IMapper mapper, DbContextSp2Mic contexto,
  //   IComandoService comandoService, ILogger<VariavelService> logger)
  // {
  //   _uow = uow;
  //   _mapper = mapper;
  //   _dbContext = contexto;
  //   _dbEntity = contexto.Set<Variavel>();
  //   _comandoService = comandoService;
  //   _logger = logger ?? throw new ArgumentNullException(nameof (logger));
  // }
  //
  //
  //

  //
  // public IEnumerable<Variavel> FindByFilter(Variavel variavel)
  // {
  //   //_logger.LogInformation(" VariavelService -> FindByFilter ######");
  //   return _dbEntity.AsNoTracking()
  //     //.IncludeAll()
  //     //.Include(x => x.Variavels)
  //    .Where(x => variavel.NoVariavel == null ||
  //       x.NoVariavel.ToLower().Contains(variavel.NoVariavel.ToLower()))
  //    .OrderBy(x => x.NoVariavel)
  //    .ToList();
  // }
  //
  // /* public IEnumerable<Variavel> FindByEndpointId (int idEndpoint)
  //   => _dbEntity.AsNoTracking()
  //    .Where(x => x.IdEndpoint == idEndpoint)
  //    .OrderBy(x => x.NoVariavel)
  //    .ToList();*/
  //
  // public Variavel? FindById(int? id)
  // {
  //   //_logger.LogInformation(" FindById id = {Id} ######", id);
  //
  //   var obj = _dbEntity.AsNoTracking()
  //    .SingleOrDefault(x => x.Id == id);
  //
  //   var dto = _mapper.Map<Variavel>(obj);
  //
  //   return id is null or 0 ? null : dto;
  // }
  //
  // public Variavel? ObterPorId(int? id)
  // {
  //   //_logger.LogInformation(" ObterVariavelPorId id = {Id} ######", id);
  //
  //   return id is null or 0 ? null : _dbEntity.AsNoTracking()
  //    .SingleOrDefault(x => x.Id == id);
  // }
  //
  // public void SalvarOuAtualizar(Variavel obj)
  // {
  //   //_logger.LogInformation(" SalvarOuAtualizarVariavel obj.Id = {ObjId} ######", obj.Id);
  //
  //   if (obj.Id == 0)
  //   {
  //     _dbContext.Add(obj);
  //   }
  //   else
  //   {
  //     _dbContext.Update(obj);
  //   }
  //
  //   obj.NoVariavel.TrimAndRemoveWhiteSpaces();
  //
  //   _dbContext.SaveChanges();
  // }
  //
  // public Variavel Atualizar(int id, Variavel entity) => null!;
  //
  // public void Excluir(int id)
  // {
  //   //_logger.LogInformation(" ExcluirVariavel id = {Id} ######", id);
  //   var obj = _dbEntity.AsNoTracking()
  //    .SingleOrDefault(x => x.Id == id);
  //
  //   //var obj = _dbContext.Find(id);
  //   if (obj == null)
  //   {
  //     throw new ApplicationException($"Error finding id: {id}.");
  //   }
  //
  //   _dbContext.Remove(obj);
  //   _dbContext.SaveChanges();
  // }
  //
  // public Variavel SalvarOuAtualizarComRetorno(Variavel entity)
  //   => throw new NotImplementedException();
  //
  // /**************************************************************************************************/
  // public async Task<IEnumerable<Variavel>> FindByFilterAsync(Variavel? filter)
  // {
  //   //_logger.LogInformation(" FindByFilterAsync {Filter} ######", SerializeObject(filter));
  //   var Variaveis = await _uow.VariavelRepository.FindByFilterAsync(filter);
  //
  //   var dto = _mapper.Map<IEnumerable<Variavel>>(Variaveis);
  //
  //   return dto;
  // }
  //
  // public async Task<Variavel?> FindByIdAsync(int id)
  // {
  //   var variavel = await _uow.VariavelRepository.FindByIdAsync(id);
  //
  //   if (variavel is null)
  //   {
  //     throw new BadHttpRequestException("Variable not found.");
  //   }
  //
  //  // var dto = _mapper.Map<Variavel>(variavel);
  //
  //   return dto;
  // }
  //
  // public async Task<Variavel> AddAsync(VariavelAddUpdateDto addDto)
  // {
  //   var novo = _mapper.Map<Variavel>(addDto);
  //   _uow.VariavelRepository.Add(novo);
  //   await _uow.SaveAsync();
  //   return _mapper.Map<Variavel>(novo);
  // }
  //
  // public async Task UpdateAsync(int id, VariavelAddUpdateDto dto)
  // {
  //   var variavelDoBanco = await _uow.VariavelRepository.FindByIdAsync(id);
  //
  //   if (variavelDoBanco is null)
  //   {
  //     throw new BadHttpRequestException("Variable not found.");
  //   }
  //
  //   _mapper.Map(dto, variavelDoBanco);
  //   _uow.VariavelRepository.Update(variavelDoBanco);
  //   await _uow.SaveAsync();
  // }
  //
  // public async Task DeleteAsync(int id)
  // {
  //   var variavelDoBanco = await _uow.VariavelRepository.FindByIdAsync(id);
  //
  //   if (variavelDoBanco is null)
  //   {
  //     throw new BadHttpRequestException("Variable not found.");
  //   }
  //
  //   _uow.VariavelRepository.Delete(variavelDoBanco);
  //   await _uow.SaveAsync();
  // }
  //

}
