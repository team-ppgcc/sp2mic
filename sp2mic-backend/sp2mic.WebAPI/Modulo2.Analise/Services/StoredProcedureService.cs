using Microsoft.VisualBasic;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Services;

public class StoredProcedureService : IStoredProcedureService
{
  private readonly IUnitOfWork _uow;

  public StoredProcedureService(IUnitOfWork uow)
    => _uow = uow ?? throw new ArgumentNullException(nameof (uow));

  public StoredProcedure FindById(int id)
    => _uow.StoredProcedureRepository.FindById(id) ??
      throw new BadHttpRequestException("Stored procedure not found.");

  public async Task<StoredProcedure> FindByIdAsync(int id)
    => await _uow.StoredProcedureRepository.FindByIdAsync(id) ??
      throw new BadHttpRequestException("Stored procedure not found.");

  public IEnumerable<StoredProcedure> FindAll() => _uow.StoredProcedureRepository.FindAll();

  public async Task<IEnumerable<StoredProcedure>> FindAllAsync()
    => await _uow.StoredProcedureRepository.FindAllAsync();

  public IEnumerable<StoredProcedure> FindByFilter(StoredProcedureFilterDto? filter)
    => filter is null ? FindAll() : _uow.StoredProcedureRepository.FindByFilter(filter);

  public async Task<IEnumerable<StoredProcedure>>
    FindByFilterAsync(StoredProcedureFilterDto? filter)
    => filter is null ? await FindAllAsync() :
      await _uow.StoredProcedureRepository.FindByFilterAsync(filter);

  //public StoredProcedure GetDefinicaoById(int idStoredProcedure)
  //  => _uow.StoredProcedureRepository.GetDefinicaoById(idStoredProcedure)
  // ?? throw new BadHttpRequestException("Stored procedure not found.");

  public StoredProcedure? FindBySchemaNome(string spSchema, string spName)
    => _uow.StoredProcedureRepository.FindBySchemaNome(spSchema, spName);

  public IEnumerable<StoredProcedure> RecuperarAnalisadas()
    => _uow.StoredProcedureRepository.RecuperarAnalisadas();

  public async Task<StoredProcedure> AddAsync(StoredProcedure? obj)
  {
    if (obj is null)
    {
      throw new BadHttpRequestException("Stored procedure must be informed.");
    }
    _uow.StoredProcedureRepository.Add(obj);
    await _uow.SaveAsync();
    return obj;
  }

  public void SaveOne(StoredProcedure obj)
  {
    _uow.StoredProcedureRepository.SaveOne(obj);
    _uow.Save();
  }

  public async Task UpdateAsync(int id, StoredProcedure? atual)
  {
    if (atual is null)
    {
      throw new BadHttpRequestException("Stored procedure must be informed.");
    }
    var existente = await _uow.StoredProcedureRepository.FindByIdAsync(id);
    if (existente is null)
    {
      throw new BadHttpRequestException("Stored procedure not found.");
    }
    if (_uow.StoredProcedureRepository.JaExiste(existente))
    {
      throw new BadHttpRequestException("Already existing Stored procedure.");
    }
    var atualizado = Sincronizar(atual, existente);
    _uow.StoredProcedureRepository.Update(atualizado);
    await _uow.SaveAsync();
  }

  private static StoredProcedure Sincronizar(StoredProcedure atual, StoredProcedure existente)
  {
    if (!Equals(atual.NoSchema, existente.NoSchema))
    {
      existente.NoSchema = atual.NoSchema;
    }
    if (!Equals(atual.NoStoredProcedure, existente.NoStoredProcedure))
    {
      existente.NoStoredProcedure = atual.NoStoredProcedure;
    }
    if (!Equals(atual.SnRetornoLista, existente.SnRetornoLista))
    {
      existente.SnRetornoLista = atual.SnRetornoLista;
    }
    if (!Equals(atual.SnAnalisada, existente.SnAnalisada))
    {
      existente.SnAnalisada = atual.SnAnalisada;
    }
    if (!Equals(atual.CoTipoDadoRetorno, existente.CoTipoDadoRetorno))
    {
      existente.CoTipoDadoRetorno = atual.CoTipoDadoRetorno;
    }
    if (!Equals(atual.IdDtoClasse, existente.IdDtoClasse))
    {
      existente.IdDtoClasse = atual.IdDtoClasse;
    }
    return existente;
  }

  public void Delete(int idStoredProcedure)
  {
    _uow.StoredProcedureRepository.Delete(idStoredProcedure);
    _uow.Save();
  }

  // public void Delete(int idStoredProcedure)
  // {
  //   var existente = _uow.StoredProcedureRepository.FindById(idStoredProcedure);
  //   if (existente is null)
  //   {
  //     throw new BadHttpRequestException("Stored procedure not found.");
  //   }
  //
  //   var comandosDaSp = _uow.ComandoRepository.FindByIdProcedure(existente.Id).ToList();
  //   if (comandosDaSp != null && comandosDaSp.Count != 0)
  //   {
  //     var expressoesDosComanodos = _uow.ExpressaoRepository.FindByComandos(comandosDaSp).ToList();
  //
  //     if (expressoesDosComanodos != null && expressoesDosComanodos.Count != 0)
  //     {
  //       var idsOperandosDasExpressoes = new List<int>();
  //       foreach (var expressao in expressoesDosComanodos)
  //       {
  //         if (expressao.IdOperandoEsquerda != null)
  //         {
  //           idsOperandosDasExpressoes.Add(expressao.IdOperandoEsquerda.Value);
  //         }
  //         if (expressao.IdOperandoDireita != null)
  //         {
  //           idsOperandosDasExpressoes.Add(expressao.IdOperandoDireita.Value);
  //         }
  //       }
  //       foreach (var idOperando in idsOperandosDasExpressoes)
  //       {
  //         _uow.OperandoRepository.Delete(idOperando);
  //       }
  //       _uow.ExpressaoRepository.DeleteRange(expressoesDosComanodos);
  //     }
  //   }
  //
  //   _uow.StoredProcedureRepository.Delete(existente);
  //   _uow.Save();
  // }

  public void DeleteAll()
  {
    _uow.StoredProcedureRepository.DeleteAll();
    _uow.Save();
  }

  public void DeleteRange()
  {
    var storedProcedures = _uow.StoredProcedureRepository.FindAll().ToList();
    if (storedProcedures == null || storedProcedures.Count == 0)
    {
      return;
    }
    foreach (var procedure in storedProcedures)
    {
      var comandosDaSp = _uow.ComandoRepository.FindByIdProcedure(procedure.Id).ToList();
      if (comandosDaSp != null && comandosDaSp.Count != 0)
      {
        var expressoesDosComanodos = _uow.ExpressaoRepository.FindByComandos(comandosDaSp).ToList();

        if (expressoesDosComanodos != null && expressoesDosComanodos.Count != 0)
        {
          var idsOperandosDasExpressoes = new List<int>();
          foreach (var expressao in expressoesDosComanodos)
          {
            if (expressao.IdOperandoEsquerda != null)
            {
             idsOperandosDasExpressoes.Add(expressao.IdOperandoEsquerda.Value);
            }
            if (expressao.IdOperandoDireita != null)
            {
              idsOperandosDasExpressoes.Add(expressao.IdOperandoDireita.Value);
            }
          }
          foreach (var idOperando in idsOperandosDasExpressoes)
          {
            _uow.OperandoRepository.Delete(idOperando);
          }
          _uow.ExpressaoRepository.DeleteRange(expressoesDosComanodos);
        }
      }
      _uow.StoredProcedureRepository.Delete(procedure);
    }
    // var tabelas = _uow.TabelaRepository.FindAll();
    // _uow.TabelaRepository.DeleteRange(tabelas);
    // var microsservicos = _uow.MicrosservicoRepository.FindAll();
    // _uow.MicrosservicoRepository.DeleteRange(microsservicos);
    _uow.Save();
  }

  public void SaveAll(IEnumerable<StoredProcedure> entityRange)
  {
    _uow.StoredProcedureRepository.SaveAll(entityRange);
    _uow.Save();
  }
}
