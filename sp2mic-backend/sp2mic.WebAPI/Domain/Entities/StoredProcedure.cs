
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Enumerations;

namespace sp2mic.WebAPI.Domain.Entities;

public class StoredProcedure
{
  public StoredProcedure()
  {
    SnAnalisada = false;
    Comandos = new HashSet<Comando>();
    DtoClasses = new HashSet<DtoClasse>();
    Endpoints = new HashSet<Endpoint>();
    Variaveis = new HashSet<Variavel>();
    TabelasAssociadas = new HashSet<Tabela>();
  }
  public StoredProcedure(string schema, string noStoredProcedure, string txDefinicao)
  {
    NoSchema = schema;
    NoStoredProcedure = noStoredProcedure;
    TxDefinicao = txDefinicao;
    TxDefinicaoTratada = TxDefinicao.TratarProcedure();
    SnAnalisada = false;
    Comandos = new HashSet<Comando>();
    DtoClasses = new HashSet<DtoClasse>();
    Endpoints = new HashSet<Endpoint>();
    Variaveis = new HashSet<Variavel>();
    TabelasAssociadas = new HashSet<Tabela>();
  }

  public int Id {get; set;}
  public string NoStoredProcedure {get; set;} = null!;
  public string NoSchema {get; set;} = null!;
  public string TxDefinicao {get; set;} = null!;
  public string TxDefinicaoTratada {get; set;} = null!;
  public TipoDadoEnum? CoTipoDadoRetorno {get; set;}
  public bool? SnRetornoLista {get; set;}
  public int? IdDtoClasse {get; set;}
  public virtual DtoClasse? IdDtoClasseNavigation {get; set;}
  public string TxResultadoParser {get; set;} = null!;
  public bool SnSucessoParser {get; set;}
  public bool SnAnalisada {get; set;}


  public virtual ICollection<Comando> Comandos {get; set;}
  public virtual ICollection<DtoClasse> DtoClasses {get; set;}
  public virtual ICollection<Endpoint> Endpoints {get; set;}
  public virtual ICollection<Variavel> Variaveis {get; set;}
  public virtual ICollection<Tabela> TabelasAssociadas {get; set;}

  public bool Equals(StoredProcedure other)
    => NoStoredProcedure == other.NoStoredProcedure && NoSchema == other.NoSchema;

  public Variavel? VariavelPorNome(string variavelName)
  {
    var consulta = Variaveis.Where(variavel
        => variavel.NoVariavel.Equals(variavelName, StringComparison.CurrentCultureIgnoreCase))
     .ToHashSet();
    if (consulta.Count != 0)
    {
      return consulta.Count == 0 ? null : consulta.First();
    }
    var comandosPorTipo = ComandosPorTipo(TipoComandoEnum.DECLARACAO);
    if (comandosPorTipo.Any())
    {
      consulta =
        comandosPorTipo
         .SelectMany(comando => comando.AsVariaveisDesseComando)
         .Where(cv => cv != null)
         .Select(cv => cv.IdVariavelNavigation)
         .Where(variavel => variavel.NoVariavel.Equals(variavelName,
              StringComparison.CurrentCultureIgnoreCase))
         .ToHashSet();
    }
    return consulta.Count == 0 ? null : consulta.First();
  }

  private HashSet<Comando> ComandosPorTipo(TipoComandoEnum tipoComandoEnum)
  {
    HashSet<Comando>? retorno = null;
    if (!Comandos.Any())
    {
      retorno = Comandos.Where(comando => comando.CoTipoComando.Equals(tipoComandoEnum.GetCodigo()))
       .ToHashSet();
    }
    return retorno ?? new HashSet<Comando>();
  }

  public Variavel? ComandoVariavelPorNome(string variavelName)
    => (from c in Comandos from cv in c.AsVariaveisDesseComando
      where cv.IdVariavelNavigation.NoVariavel.Equals(variavelName,
        StringComparison.CurrentCultureIgnoreCase) select cv.IdVariavelNavigation).FirstOrDefault();

  // public Tabela? TabelaPorNome(string tabelaName)
  // {
  //   foreach (var t in TabelasAssociadas.Where(t
  //     => t.NoTabela.Equals(tabelaName, StringComparison.CurrentCultureIgnoreCase)))
  //   {
  //     return t;
  //   }
  //   return Endpoints.SelectMany(e => e.TabelasAssociadas.Where(te
  //       => te.NoTabela.Equals(tabelaName, StringComparison.CurrentCultureIgnoreCase)))
  //    .FirstOrDefault();
  // }

  public HashSet<Variavel> GetParametrosStoredProcedure()
  {
    return !Variaveis.Any() ? new HashSet<Variavel>() : Variaveis
     .Where(ep => ep.CoTipoEscopo == TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE)
     .OrderBy(v => v.NoVariavel).ToHashSet();
  }
}
