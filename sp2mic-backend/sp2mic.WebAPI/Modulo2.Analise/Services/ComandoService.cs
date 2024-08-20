using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class ComandoService : IComandoService
{
  private readonly IUnitOfWork _uow;

  public ComandoService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public Comando FindById(int id)
  {
    var obj = _uow.ComandoRepository.FindById(id);
    return obj ?? throw new BadHttpRequestException("Command not found.");
  }

  public async Task<Comando> FindByIdAsync(int id)
  {
    var obj = await _uow.ComandoRepository.FindByIdAsync(id);
    return obj ?? throw new BadHttpRequestException("Command not found.");
  }

  public IEnumerable<Comando> FindAll() => _uow.ComandoRepository.FindAll();

  public async Task<IEnumerable<Comando>> FindAllAsync()
    => await _uow.ComandoRepository.FindAllAsync();

  public IEnumerable<Comando> FindByFilter(ComandoFilterDto? filter)
    => filter is null ? FindAll() : _uow.ComandoRepository.FindByFilter(filter);

  public async Task<IEnumerable<Comando>> FindByFilterAsync(ComandoFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.ComandoRepository.FindByFilterAsync(filter);

  public async Task<Comando> AddAsync(Comando? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Command must be informed.");
    }
    _uow.ComandoRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public async Task UpdateAsync(int id, Comando? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("Command must be informed.");
    }
    var existente = await _uow.ComandoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Command not found.");
    }
    if (_uow.ComandoRepository.JaExiste(existente))
    {
      throw new BadHttpRequestException("Already existing attribute.");
    }
    var atualizado = Sincronizar(atual, existente);
    _uow.ComandoRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static Comando Sincronizar(Comando atual, Comando existente)
  {
    //var atualizado = new Comando();
    if (!Equals(atual.TxComando, existente.TxComando))
    {
      existente.TxComando = atual.TxComando;
    }
    if (!Equals(atual.TxComandoTratado, existente.TxComandoTratado))
    {
      existente.TxComandoTratado = atual.TxComandoTratado;
    }
    if (!Equals(atual.CoTipoComando, existente.CoTipoComando))
    {
      existente.CoTipoComando = atual.CoTipoComando;
    }
    if (!Equals(atual.NuOrdemExecucao, existente.NuOrdemExecucao))
    {
      existente.NuOrdemExecucao = atual.NuOrdemExecucao;
    }
    if (!Equals(atual.VlAtribuidoVariavel, existente.VlAtribuidoVariavel))
    {
      existente.VlAtribuidoVariavel = atual.VlAtribuidoVariavel;
    }
    if (!Equals(atual.IdStoredProcedure, existente.IdStoredProcedure))
    {
      existente.IdStoredProcedure = atual.IdStoredProcedure;
    }
    if (!Equals(atual.IdComandoOrigem, existente.IdComandoOrigem))
    {
      existente.IdComandoOrigem = atual.IdComandoOrigem;
    }
    if (!Equals(atual.IdEndpoint, existente.IdEndpoint))
    {
      existente.IdEndpoint = atual.IdEndpoint;
    }
    if (!Equals(atual.IdExpressao, existente.IdExpressao))
    {
      existente.IdExpressao = atual.IdExpressao;
    }
    if (!Equals(atual.SnCondicaoOrigem, existente.SnCondicaoOrigem))
    {
      existente.SnCondicaoOrigem = atual.SnCondicaoOrigem;
    }
    return existente;
  }

  public async Task DeleteAsync(int id)
  {
    var existente = await _uow.ComandoRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Command not found.");
    }
    _uow.ComandoRepository.Delete(existente);
    await _uow.SaveAsync();
  }

  public IEnumerable<Comando> FindByIdStoredProcedure(int idStoredProcedure, bool isInterno)
    => _uow.ComandoRepository.FindByIdStoredProcedure(idStoredProcedure, isInterno);

  public Comando FindByIdExpressao(int idExpressao)
  {
    var comando = _uow.ComandoRepository.FindByIdExpressao(idExpressao);
    if (comando is null)
    {
      throw new BadHttpRequestException("Command not found.");
    }
    return comando;
  }

  // listar todos comandos cuja ORIGEM Seja ESSE If e tenha condição verdadeira
  public IEnumerable<Comando> RecuperarComandosIf(Comando comando)
    => _uow.ComandoRepository.RecuperarComandosIf(comando);

  // listar todos comandos cuja ORIGEM Seja ESSE If e tenha condição falsa
  public IEnumerable<Comando> RecuperarComandosElse(Comando comando)
    => _uow.ComandoRepository.RecuperarComandosElse(comando);
}
