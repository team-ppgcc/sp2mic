using Microsoft.SqlServer.TransactSql.ScriptDom;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo1.Carga.Dtos;
using sp2mic.WebAPI.Modulo1.Carga.Services.Interfaces;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using static System.String;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;
using Literal = Microsoft.SqlServer.TransactSql.ScriptDom.Literal;

namespace sp2mic.WebAPI.Modulo1.Carga.Services.Parser.SqlServer;

public abstract class AbstractSqlServerParser : ITradutorBanco
{
  private readonly ITabelaService _tabelaService;
  private readonly IVariavelService _variavelService;
  private static ISet<Tabela> _todasAsTabelas = new HashSet<Tabela>();
  private static ISet<Variavel> _variaveisDaProcedure = new HashSet<Variavel>();
  private static ISet<Tabela> _tabelasDaProcedure = new HashSet<Tabela>();
  private static string _txEndpoint;

  protected AbstractSqlServerParser(ITabelaService tabelaService, IVariavelService variavelService)
  {
    _tabelaService = tabelaService ?? throw new ArgumentNullException(nameof (tabelaService));
    _variavelService = variavelService ?? throw new ArgumentNullException(nameof (variavelService));
  }

  public abstract IEnumerable<ParDto> FetchNomesProcedures(CargaDto dto);

  public IEnumerable<StoredProcedure> FetchProceduresSelecionadas(CargaDto dto)
  {
    ValidarDtoAntesDeFetchProceduresSelecionadas(dto);
    PrepararConexao(dto);
    List<StoredProcedure> listaProcedures = new();
    _todasAsTabelas = _tabelaService.FindAll();
    //_variaveisDaProcedure = _variavelService.FindAll();
    foreach (var procedureDto in dto.NomesProcedures!)
    {
      if (procedureDto == null)
      {
        throw new BadHttpRequestException("Stored Procedure must be informed.");
      }
      //_logger.LogInformation("Carregando: {Procedure}: ", procedureDto.Nome);
      _tabelasDaProcedure = new HashSet<Tabela>();
      _variaveisDaProcedure = new HashSet<Variavel>();
      var procedure = CriarProcedure(dto, procedureDto);
      ComplementarProcedure(procedure);
      listaProcedures.Add(procedure);
    }
    return listaProcedures;
  }

  private void ValidarDtoAntesDeFetchProceduresSelecionadas(CargaDto dto)
  {
    ValidarDtoAntesDeFetchNomesProcedures(dto);
    if (dto.NomesProcedures is null || !dto.NomesProcedures.Any())
    {
      throw new ApplicationException("Please select at least one stored procedure.");
    }
  }

  protected abstract void ValidarDtoAntesDeFetchNomesProcedures(CargaDto dto);

  protected abstract void PrepararConexao(CargaDto dto);

  protected abstract StoredProcedure CriarProcedure(CargaDto dto, ParDto procedureDto);

  private void ComplementarProcedure(StoredProcedure procedure)
  {
    var fragment = Parser.GetFragment(procedure.TxDefinicao);
    var procStatement = ExtrairProcedure(procedure, fragment);
    if (procStatement is null)
    {
      return;
    }
    procedure.SnSucessoParser = true;
    ExtrairParametrosStoredProcedure(procedure, procStatement.Parameters);
    var statements = procStatement.StatementList.Statements;
    ExtrairComandos(procedure, statements, procedure.Comandos, true);
    //ExtrairParametrosStoredProcedure(procedure, procStatement.Parameters);
    //ExtrairTabelasProcedure(procedure, fragment);
    if (procedure.SnSucessoParser)
    {
      procedure.TxResultadoParser = "Stored Procedure parser successfully.";
    }
  }

  // private void ExtrairTabelasProcedure(StoredProcedure procedure, TSqlFragment fragment)
  // {
  //   var tabelas = Parser.GetTableList(fragment);
  //   _logger.LogInformation("Tabelas {Tabelas}: ", tabelas.ToString());
  // }

  private static CreateProcedureStatement? ExtrairProcedure(StoredProcedure procedure,
    TSqlFragment fragment)
  {
    var statements = Parser.GetProcedures(fragment);
    if (statements.Count != 1)
    {
      // Espera-se que toda procedure tenha apenas um Create Procedure
      procedure.SnSucessoParser = false;
      procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
        $"Stored Procedure text unrecognized: \"{procedure.NoStoredProcedure}\".\n");
      //throw new ApplicationException("Stored Procedure text" + procedure.NoStoredProcedure + " unrecognized.");
      return null;
    }
    var proc = statements[0];
    var fragmentSql = Parser.GetFragmentSqlTratado(proc);
    procedure.TxDefinicao = fragmentSql;
    procedure.TxDefinicaoTratada = fragmentSql.TratarProcedure();
    return proc;
  }

  private static void ExtrairParametrosStoredProcedure(StoredProcedure procedure,
    IEnumerable<ProcedureParameter> parametros)
  {
    foreach (var parametro in parametros)
    {
      // var variavel = CriarVariavel(procedure, parametro.VariableName.Value,
      //   IdentificarTipo(procedure, parametro.DataType));
      var variavel = RecuperarVariavel(procedure, parametro.VariableName.Value,
        IdentificarTipo(procedure, parametro.DataType), TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE);
      //variavel.CoTipoEscopo = TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE;
      variavel.IdStoredProcedure = procedure.Id;
      variavel.IdStoredProcedureNavigation = procedure;
      procedure.Variaveis.Add(variavel);
    }
  }

  private void ExtrairComandos(StoredProcedure procedure, IEnumerable<TSqlStatement> statements,
    ICollection<Comando> comandos, bool condicaoOrigem)
  {
    var ordemExecucao = 0;
    foreach (var statement in statements)
    {
      ordemExecucao++;
      Comando comando = new();
      switch (statement)
      {
        case PredicateSetStatement:
          ordemExecucao--;
          comando.NuOrdemExecucao = ordemExecucao;
          comando.CoTipoComando = TipoComandoEnum.TIPO_NAO_MAPEADO;
          continue;
        case BeginEndBlockStatement bloco:
          ordemExecucao--;
          ExtrairComandos(procedure, bloco.StatementList.Statements, comandos, condicaoOrigem);
          comando.NuOrdemExecucao = ordemExecucao;
          continue;
        case DeleteStatement:
        case UpdateStatement:
        case InsertStatement:
        case SelectStatement:
        case CreateTableStatement:
        case AlterTableStatement:
        case DropTableStatement:
        case ExecuteStatement:
          CriarEndpoint(comando, procedure, statement);
          comando.NuOrdemExecucao = ordemExecucao;
          break;
        case DeclareVariableStatement declareVariableStatement:
          CriarDeclaracao(comando, procedure, declareVariableStatement);
          comando.NuOrdemExecucao = ordemExecucao;
          break;
        case SetVariableStatement setVariableStatement:
          CriarAtribuicao(comando, procedure, setVariableStatement);
          comando.NuOrdemExecucao = ordemExecucao;
          break;
        case IfStatement ifStatement:
          CriarIf(comando, procedure, ifStatement);
          comando.NuOrdemExecucao = ordemExecucao;
          break;
        case BeginTransactionStatement:
          comando.CoTipoComando = TipoComandoEnum.TIPO_NAO_MAPEADO;
          procedure.SnSucessoParser = false;
          procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
            $"Statement unhandled: \"{statement}\".\n");
          // throw new ApplicationException(
          //   $"Stored Procedure \"{procedure.NoStoredProcedure}\" has unhandled statement by this tool {Converter.TipoComando(statement).GetNome()}");
          break;
        default:
          //      Command Type not yet mapped: Microsoft.SqlServer.TransactSql.ScriptDom.DeclareCursorStatement
          comando.CoTipoComando = TipoComandoEnum.TIPO_NAO_MAPEADO;
          procedure.SnSucessoParser = false;
          procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
            $"Statement unhandled: \"{statement}\".\n");
          //throw new ApplicationException("Command Type not yet mapped: " + Converter.TipoComando(statement));
          break;
      }
      var fragmentSql = Parser.GetFragmentSqlTratado(statement);
      comando.TxComando = fragmentSql;
      comando.TxComandoTratado = fragmentSql.TratarComando();
      comando.IdStoredProcedure = procedure.Id;
      comando.IdStoredProcedureNavigation = procedure;
      comando.SnCondicaoOrigem = condicaoOrigem;
      comandos.Add(comando);
    }
  }

  //Parameter 'ordemExecucao' is only passed to itself
  //private static Expressao ExtrairExpressao(StoredProcedure procedure, TipoDadoEnum tipoDadoEnum, int ordemExecucao, TSqlFragment fragment)
  private Expressao ExtrairExpressao(StoredProcedure procedure, TipoDadoEnum tipoDadoEnum,
    TSqlFragment fragment)
  {
    Operando operandoEsquerda = new() {CoTipoOperando = TipoOperandoEnum.TIPO_NAO_MAPEADO};
    Operando operandoDireita = new() {CoTipoOperando = TipoOperandoEnum.TIPO_NAO_MAPEADO};
    Expressao expressao = new()
    {
      CoTipoDadoRetorno = tipoDadoEnum,
      NuOrdemExecucao = 1,
      IdOperandoEsquerda = operandoEsquerda.Id,
      IdOperandoEsquerdaNavigation = operandoEsquerda,
      IdOperandoDireita = operandoDireita.Id,
      IdOperandoDireitaNavigation = operandoDireita
    };
    switch (fragment)
    {
      case ExistsPredicate predicate:
        var endpoint = CriarEndpoint(procedure,
          (QuerySpecification) predicate.Subquery.QueryExpression);
        operandoEsquerda.CoTipoOperando = TipoOperandoEnum.ENDPOINT;
        operandoEsquerda.IdEndpoint = endpoint.Id;
        operandoEsquerda.IdEndpointNavigation = endpoint;
        operandoDireita.CoTipoOperando = TipoOperandoEnum.CONSTANTE;
        operandoDireita.TxValor = "0";
        expressao.CoTipoDadoRetorno = TipoDadoEnum.BOOLEAN;
        endpoint.SnRetornoLista = null;
        expressao.CoOperador = TipoOperadorEnum.DIFERENTE;
        break;
      case BooleanComparisonExpression booleanComparison:
        var operador = Converter.TipoOperadorBooleano(procedure, booleanComparison.ComparisonType);
        operandoEsquerda = CriarOperando(booleanComparison.FirstExpression, procedure);
        operandoDireita = CriarOperando(booleanComparison.SecondExpression, procedure);
        expressao.CoTipoDadoRetorno = TipoDadoEnum.BOOLEAN;
        expressao.CoOperador = operador;
        expressao.IdOperandoEsquerda = operandoEsquerda.Id;
        expressao.IdOperandoEsquerdaNavigation = operandoEsquerda;
        expressao.IdOperandoDireita = operandoDireita.Id;
        expressao.IdOperandoDireitaNavigation = operandoDireita;
        break;
      case BooleanParenthesisExpression booleanParenthesisExpression:
        var expressaoInterna = ExtrairExpressao(procedure, tipoDadoEnum,
          booleanParenthesisExpression.Expression);
        //var expressaoInterna = ExtrairExpressao(procedure, tipoDadoEnum, ordemExecucao, booleanParenthesisExpression.Expression);
        operandoEsquerda.CoTipoOperando = TipoOperandoEnum.EXPRESSAO;
        operandoEsquerda.IdExpressao = expressaoInterna.Id;
        operandoEsquerda.IdExpressaoNavigation = expressaoInterna;
        expressao.CoTipoDadoRetorno = TipoDadoEnum.BOOLEAN;
        expressao.IdOperandoEsquerda = operandoEsquerda.Id;
        expressao.IdOperandoEsquerdaNavigation = operandoEsquerda;
        expressao.IdOperandoDireita = null;
        expressao.IdOperandoDireitaNavigation = null;
        break;
      //case BooleanBinaryExpression: // booleanBinaryExpression:
      // ExtrairVariavel(procedure, booleanBinaryExpression.FirstExpression, variaveis);
      // ExtrairVariavel(procedure, booleanBinaryExpression.SecondExpression, variaveis);
      // variavel = ExtrairVariavel(procedure, booleanBinaryExpression, new List<Variavel>());
      // operandoEsquerda.CoTipoOperando = TipoOperandoEnum.VARIAVEL;
      //  operandoEsquerda.IdVariavel = variavel.Id;
      //  operandoEsquerda.IdExpressaoNavigation = variavel;
      // operandoDireita.CoTipoOperando = TipoOperandoEnum.CONSTANTE_NULL;
      // expressao.CoTipoDadoRetorno = TipoDadoEnum.BOOLEAN;
      //  expressao.IdOperandoEsquerda = operandoEsquerda.Id;
      //  expressao.IdOperandoEsquerdaNavigation = operandoEsquerda;
      //  expressao.CoOperador = TipoOperadorEnum.IGUAL;
      //  expressao.IdOperandoDireita = operandoDireita.Id;
      //  expressao.IdOperandoDireitaNavigation = operandoDireita;
      // operandoEsquerda.CoTipoOperando = TipoOperandoEnum.VARIAVEL;
      //  operandoEsquerda.IdVariavel = variavel.Id;
      //  operandoEsquerda.IdExpressaoNavigation = variavel;
      // operandoDireita.CoTipoOperando = TipoOperandoEnum.CONSTANTE_NULL;
      // expressao.CoTipoDadoRetorno = TipoDadoEnum.BOOLEAN;
      // expressao.IdOperandoEsquerda = operandoEsquerda.Id;
      // expressao.IdOperandoEsquerdaNavigation = operandoEsquerda;
      // expressao.CoOperador = TipoOperadorEnum.IGUAL;
      // expressao.IdOperandoDireita = operandoDireita.Id;
      // expressao.IdOperandoDireitaNavigation = operandoDireita;
      // break;
      case BooleanIsNullExpression booleanIsNullExpression:
        if (booleanIsNullExpression.Expression is VariableReference variableReference)
        {
          var variavel = RecuperarVariavel(procedure, variableReference.Name);
          operandoEsquerda.CoTipoOperando = TipoOperandoEnum.VARIAVEL;
          operandoEsquerda.IdVariavel = variavel.Id;
          operandoEsquerda.IdVariavelNavigation = variavel;
          operandoDireita.CoTipoOperando = TipoOperandoEnum.CONSTANTE_NULL;
          expressao.CoTipoDadoRetorno = TipoDadoEnum.BOOLEAN;
          expressao.IdOperandoEsquerda = operandoEsquerda.Id;
          expressao.IdOperandoEsquerdaNavigation = operandoEsquerda;
          expressao.CoOperador = TipoOperadorEnum.IGUAL;
          expressao.IdOperandoDireita = operandoDireita.Id;
          expressao.IdOperandoDireitaNavigation = operandoDireita;
        }
        else
        {
          procedure.SnSucessoParser = false;
          procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
            $"Statement unhandled: \"{booleanIsNullExpression.Expression}\".\n");
        }
        break;
      case BinaryExpression binaryExpression:
        var operador1
          = Converter.TipoOperadorBinario(procedure, binaryExpression.BinaryExpressionType);
        operandoEsquerda = CriarOperando(binaryExpression.FirstExpression, procedure, tipoDadoEnum);
        operandoDireita = CriarOperando(binaryExpression.SecondExpression, procedure, tipoDadoEnum);
        expressao.CoTipoDadoRetorno = tipoDadoEnum;
        expressao.CoOperador = operador1;
        expressao.IdOperandoEsquerda = operandoEsquerda.Id;
        expressao.IdOperandoEsquerdaNavigation = operandoEsquerda;
        expressao.IdOperandoDireita = operandoDireita.Id;
        expressao.IdOperandoDireitaNavigation = operandoDireita;
        break;
      //case BooleanNotExpression booleanNotExpression: if not exists(select 1 from pacfuncional where
      case BooleanNotExpression booleanNotExpression:
        var exprInterna
          = ExtrairExpressao(procedure, tipoDadoEnum, booleanNotExpression.Expression);
        operandoEsquerda.CoTipoOperando = TipoOperandoEnum.EXPRESSAO;
        operandoEsquerda.IdExpressao = exprInterna.Id;
        operandoEsquerda.IdExpressaoNavigation = exprInterna;
        expressao.CoTipoDadoRetorno = TipoDadoEnum.BOOLEAN;
        expressao.IdOperandoEsquerda = operandoEsquerda.Id;
        expressao.IdOperandoEsquerdaNavigation = operandoEsquerda;
        expressao.IdOperandoDireita = null;
        expressao.IdOperandoDireitaNavigation = null;
        break;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{fragment}\".\n");
        //throw new ApplicationException("Expression Type not yet mapped: " + fragment);
        break;
    }
    return expressao;
  }

  private void CriarIf(Comando comando, StoredProcedure procedure, IfStatement ifStatement)
  {
    //var expressao = ExtrairExpressao(procedure, TipoDadoEnum.BOOLEAN, 1, ifStatement.Predicate);
    var expressao = ExtrairExpressao(procedure, TipoDadoEnum.BOOLEAN, ifStatement.Predicate);
    comando.CoTipoComando = TipoComandoEnum.IF;
    comando.IdExpressao = expressao.Id;
    comando.IdExpressaoNavigation = expressao;
    switch (ifStatement.ThenStatement)
    {
      case SetVariableStatement setVariableStatement:
        CriarAtribuicao(comando, procedure, setVariableStatement);
        break;
      case SelectStatement selectStatement:
        CriarEndpoint(comando, procedure, selectStatement);
        break;
      case BeginEndBlockStatement beginEndThen:
        //var beginEnd = (BeginEndBlockStatement) ifStatement.ThenStatement;
        ExtrairComandos(procedure, beginEndThen.StatementList.Statements, comando.ComandosInternos,
          true);
        break;
      default:
        comando.CoTipoComando = TipoComandoEnum.TIPO_NAO_MAPEADO;
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{ifStatement.ThenStatement}\".\n");
        //throw new ApplicationException("Expression Type not yet mapped: " + ifStatement.ThenStatement);
        break;
    }

    foreach (var comandoInterno in comando.ComandosInternos)
    {
      comandoInterno.IdComandoOrigem = comando.Id;
      comandoInterno.IdComandoOrigemNavigation = comando;
    }
    if (ifStatement.ElseStatement is null)
    {
      return;
    }
    switch (ifStatement.ElseStatement)
    {
      case SetVariableStatement setVariableStatement:
        CriarAtribuicao(comando, procedure, setVariableStatement);
        break;
      case SelectStatement selectStatement:
        CriarEndpoint(comando, procedure, selectStatement);
        break;
      case BeginEndBlockStatement beginEndElse:
        //var beginEndElse = (BeginEndBlockStatement) ifStatement.ElseStatement;
        // if (beginEndElse != null)
        // {
        ExtrairComandos(procedure, beginEndElse.StatementList.Statements, comando.ComandosInternos,
          false);
        foreach (var comandoInterno in comando.ComandosInternos)
        {
          comandoInterno.IdComandoOrigem = comando.Id;
          comandoInterno.IdComandoOrigemNavigation = comando;
        }
        //}
        break;
      default:
        comando.CoTipoComando = TipoComandoEnum.TIPO_NAO_MAPEADO;
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{ifStatement.ElseStatement}\".\n");
        //throw new ApplicationException("Expression Type not yet mapped: " + ifStatement.ElseStatement);
        break;
    }
  }

  /* private static Variavel ClonarVariavel (StoredProcedure procedure, VariableReference variavel)
   {
     var variavelRecuperada = RecuperarVariavel(procedure, variavel);
     Variavel param = new() {NoVariavel = variavel.Name, CoTipoDado = variavelRecuperada.CoTipoDado};
     return param;
   }*/
  private static void ExtrairVariavel(StoredProcedure procedure,
    BooleanExpression booleanExpression, List<Variavel> variaveis, ISet<Tabela> tabelasDoEndpoint)
  {
    switch (booleanExpression)
    {
      case BooleanBinaryExpression booleanBinaryExpression:
        ExtrairVariavel(procedure, booleanBinaryExpression.FirstExpression, variaveis,
          tabelasDoEndpoint);
        ExtrairVariavel(procedure, booleanBinaryExpression.SecondExpression, variaveis,
          tabelasDoEndpoint);
        break;
      case BooleanComparisonExpression booleanComparisonExpression:
        ExtrairVariavel(procedure, booleanComparisonExpression.FirstExpression, variaveis,
          tabelasDoEndpoint);
        ExtrairVariavel(procedure, booleanComparisonExpression.SecondExpression, variaveis,
          tabelasDoEndpoint);
        break;
    }
  }

  private static void ExtrairVariavel(StoredProcedure procedure, ScalarExpression expressao,
    List<Variavel> variaveis, ISet<Tabela> tabelasDoEndpoint)
  {
    switch (expressao)
    {
      case VariableReference variavel:
      {
        variaveis.Add(RecuperarVariavel(procedure, variavel.Name));
        break;
      }
      case ParenthesisExpression parenthesis:
        ExtrairVariavel(procedure, parenthesis.Expression, variaveis, tabelasDoEndpoint);
        break;
      case BinaryExpression binaryExpression:
        ExtrairVariavel(procedure, binaryExpression.FirstExpression, variaveis, tabelasDoEndpoint);
        ExtrairVariavel(procedure, binaryExpression.SecondExpression, variaveis, tabelasDoEndpoint);
        break;
      case ConvertCall convert:
        ExtrairVariavel(procedure, convert.Parameter, variaveis, tabelasDoEndpoint);
        break;
      case LeftFunctionCall:
      case FunctionCall:
      case ColumnReferenceExpression:
        // TODO tipo do parametro pela referencia da coluna
        //ExtrairTabela(procedure, columnReferenceExpression.MultiPartIdentifier);
        break;
      case IntegerLiteral:
      case CastCall:
      case StringLiteral:
        break;
      case SearchedCaseExpression searchedCaseExpression:
        foreach (var searchedWhenClause in searchedCaseExpression.WhenClauses)
        {
          ExtrairVariaveis(procedure, searchedWhenClause.WhenExpression, variaveis,
            tabelasDoEndpoint);
          ExtrairVariavel(procedure, searchedWhenClause.ThenExpression, variaveis,
            tabelasDoEndpoint);
        }
        ExtrairVariavel(procedure, searchedCaseExpression.ElseExpression, variaveis,
          tabelasDoEndpoint);
        break;
      case ScalarSubquery subquery:
        var query = (QuerySpecification) subquery.QueryExpression;
        var selectElements = query.SelectElements;
        var whereClause = query.WhereClause;
        if (whereClause != null)
        {
          //
          // switch (whereClause)
          // {
          //   case null when !selectElements.IsNullOrEmpty():
          //     procedure.SnSucessoParser = false;
          //     procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          //       $"Statement unhandled: \"{selectElements}\".\n");
          //     return;
          //   case null:
          //     return;
          // }
          switch (whereClause.SearchCondition)
          {
            case BooleanBinaryExpression binaria:
              variaveis.AddRange(
                ExtrairVariaveisBooleanBinaryExpression(procedure, binaria, tabelasDoEndpoint));
              break;
            case BooleanComparisonExpression comparacao:
              variaveis.AddRange(
                ExtrairVariaveisBooleanComparisonExpression(procedure, comparacao,
                  tabelasDoEndpoint));
              break;
            default:
              procedure.SnSucessoParser = false;
              procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
                $"Statement unhandled: \"{whereClause.SearchCondition}\".\n");
              //throw new ApplicationException("Expression Type not yet mapped: " + whereClause.SearchCondition);
              break;
          }
        }
        break;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{expressao}\".\n");
        //throw new ApplicationException("Expression Type not yet mapped: " + expressao);
        break;
    }
  }

  private void CriarAtribuicao(Comando comando, StoredProcedure procedure,
    SetVariableStatement atribuicao)
  {
    var variavel = RecuperarVariavel(procedure, atribuicao.Variable.Name);
    //variavel.CoTipoEscopo = TipoEscopoEnum.LOCAL;
    ICollection<ComandoVariavel> cvList = new List<ComandoVariavel>();
    ComandoVariavel cv = new();
    cvList.Add(cv);
    var operando = CriarOperando(atribuicao.Expression, procedure, variavel.CoTipoDado);
    Expressao expressao = new()
    {
      CoTipoDadoRetorno = variavel.CoTipoDado,
      NuOrdemExecucao = 1,
      IdOperandoEsquerda = operando.Id,
      IdOperandoEsquerdaNavigation = operando
    };
    comando.CoTipoComando = TipoComandoEnum.ATRIBUICAO;
    comando.AsVariaveisDesseComando = cvList;
    comando.IdExpressao = expressao.Id;
    comando.IdExpressaoNavigation = expressao;
    procedure.Comandos.Add(comando);
    cv.IdComando = comando.Id;
    cv.IdComandoNavigation = comando;
    cv.IdVariavel = variavel.Id;
    cv.IdVariavelNavigation = variavel;
    cv.NuOrdem = 1;
  }

  private Operando CriarOperando(ScalarExpression valor, StoredProcedure procedure,
    TipoDadoEnum tipoDadoEnum = TipoDadoEnum.TIPO_NAO_MAPEADO)
  {
    Operando operando = new() {CoTipoOperando = TipoOperandoEnum.TIPO_NAO_MAPEADO};
    switch (valor)
    {
      case FunctionCall funcao:
        operando.CoTipoOperando = TipoOperandoEnum.METODO;
        operando.TxValor = funcao.FunctionName.Value;
        break;
      case StringLiteral stringLiteral:
        operando.CoTipoOperando = TipoOperandoEnum.CONSTANTE_STRING;
        operando.TxValor = stringLiteral.Value;
        break;
      case Literal literal:
        operando.CoTipoOperando = TipoOperandoEnum.CONSTANTE;
        operando.TxValor = literal.Value;
        break;
      case ScalarSubquery subquery:
        var endpoint = CriarEndpoint(procedure, (QuerySpecification) subquery.QueryExpression);
        operando.CoTipoOperando = TipoOperandoEnum.ENDPOINT;
        operando.IdEndpoint = endpoint.Id;
        operando.IdEndpointNavigation = endpoint;
        break;
      case VariableReference variableReference:
        var variavel = RecuperarVariavel(procedure, variableReference.Name);
        operando.CoTipoOperando = TipoOperandoEnum.VARIAVEL;
        operando.IdVariavel = variavel.Id;
        operando.IdVariavelNavigation = variavel;
        break;
      case BinaryExpression binaryExpression:
        //var expressao = ExtrairExpressao(procedure, tipoDadoEnum, 0, binaryExpression);
        var expressao = ExtrairExpressao(procedure, tipoDadoEnum, binaryExpression);
        operando.CoTipoOperando = TipoOperandoEnum.EXPRESSAO;
        operando.IdExpressao = expressao.Id;
        operando.IdExpressaoNavigation = expressao;
        break;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{valor}\".\n");
        //throw new ApplicationException("Expression Type not yet mapped: " + valor);
        break;
    }
    return operando;
  }

  // private static Variavel RecuperarVariavel(StoredProcedure procedure, VariableReference variavel)
  // {
  //   var nomeVariavel = variavel.Name.SemArroba().ToLower();
  //   var variavelRecuperada = procedure.VariavelPorNome(nomeVariavel) ??
  //     procedure.ComandoVariavelPorNome(nomeVariavel);
  //   return variavelRecuperada ?? CriarVariavel(procedure, nomeVariavel, SqlDataTypeOption.VarChar);
  // }

  // private static Variavel RecuperarVariavel(StoredProcedure procedure, string nome)
  // {
  //   var nomeVariavel = nome.ToLower();
  //   var variavelRecuperada = procedure.VariavelPorNome(nomeVariavel) ??
  //     procedure.ComandoVariavelPorNome(nomeVariavel);
  //   return variavelRecuperada ?? CriarVariavel(procedure, nomeVariavel, SqlDataTypeOption.VarChar);
  // }

  // private static Variavel CriarVariavel(StoredProcedure procedure, string nome,
  //   SqlDataTypeOption tipo)
  // {
  //   var tipoDado = Converter.TipoDado(procedure, tipo);
  //   Variavel variavel = new()
  //   {
  //     NoVariavel = nome.SemArroba().ToLower(),
  //     CoTipoDado = tipoDado,
  //     IdStoredProcedureNavigation = procedure
  //   };
  //   return variavel;
  // }

 private static Variavel RecuperarVariavel(StoredProcedure procedure, string nomeVariabel,
    SqlDataTypeOption tipo = SqlDataTypeOption.VarChar, TipoEscopoEnum escopo = TipoEscopoEnum.LOCAL)
  {
    var nome = nomeVariabel.SemArroba().ToLower();
    var tipoDado = Converter.TipoDado(procedure, tipo);
    foreach (var variavelExistente in _variaveisDaProcedure.Where(v
      => MesmaVariavel(v, nome, procedure)))
    {
      return variavelExistente;
    }
    return CriarVariavel(procedure, nome, tipoDado, escopo);
  }

  private static Variavel CriarVariavel(StoredProcedure procedure, string nome, TipoDadoEnum tipo,
    TipoEscopoEnum escopo)
  {
    Variavel variavel = new()
    {
      NoVariavel = nome,
      CoTipoDado = tipo,
      IdStoredProcedure = procedure.Id,
      IdStoredProcedureNavigation = procedure,
      CoTipoEscopo = escopo
    };
    _variaveisDaProcedure.Add(variavel);
    return variavel;
  }

  private static bool
    MesmaVariavel(Variavel v, string nome, StoredProcedure procedure)
    => Equals(v.NoVariavel, nome) &&
      Equals(v.IdStoredProcedureNavigation, procedure);

  private static void CriarDeclaracao(Comando comando, StoredProcedure procedure,
    DeclareVariableStatement declareVariable)
  {
    ICollection<ComandoVariavel> cvList = new List<ComandoVariavel>();
    comando.CoTipoComando = TipoComandoEnum.DECLARACAO;
    comando.AsVariaveisDesseComando = cvList;
    procedure.Comandos.Add(comando);
    var nuOrdem = 0;
    foreach (var declare in declareVariable.Declarations)
    {
      var variavel = RecuperarVariavel(procedure, declare.VariableName.Value, IdentificarTipo(procedure, declare.DataType));
      // var variavel = CriarVariavel(procedure, declare.VariableName.Value,
      //   IdentificarTipo(procedure, declare.DataType));
      //variavel.CoTipoEscopo = TipoEscopoEnum.LOCAL;
      variavel.IdStoredProcedure = procedure.Id;
      variavel.IdStoredProcedureNavigation = procedure;
      ComandoVariavel cv = new();
      cvList.Add(cv);
      nuOrdem++;
      cv.IdComando = comando.Id;
      cv.IdComandoNavigation = comando;
      cv.IdVariavel = variavel.Id;
      cv.IdVariavelNavigation = variavel;
      cv.NuOrdem = nuOrdem;
    }
  }

  private static bool? GetIsRetornoLista(string fragmentSql)
  {
    if (fragmentSql.Contains("top 1") || fragmentSql.Contains("TOP 1") ||
      fragmentSql.Contains("Top 1"))
    {
      return false;
    }
    return null;
  }

  private void CriarEndpoint(Comando comando, StoredProcedure procedure, TSqlStatement statement)
  {
    Endpoint endpoint = new();
    var fragmentSql = Parser.GetFragmentSqlTratado(statement);
    endpoint.NoMetodoEndpoint = "nomeAindaNaoDefinido";
    endpoint.NoPath = "/path-ainda-nao-definido";
    endpoint.TxEndpoint = fragmentSql;
    _txEndpoint = endpoint.TxEndpoint;
    endpoint.TxEndpointTratado = fragmentSql.TratarEndpoint();
    endpoint.IdStoredProcedure = procedure.Id;
    endpoint.IdStoredProcedureNavigation = procedure;
    endpoint.SnRetornoLista = GetIsRetornoLista(fragmentSql);
    endpoint.SnAnalisado = false;

    List<Variavel> parametros = new List<Variavel>();
    var tabelas = new HashSet<Tabela>();
    ISet<Tabela> tabelasDoEndpoint = new HashSet<Tabela>();
    //BooleanBinaryExpression where = new();
    //BooleanComparisonExpression whereComparisonExpression = new();
    switch (statement)
    {
      case CreateTableStatement createTableStatement:
        endpoint.NoMetodoEndpoint
          = Concat("create", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
        endpoint.NoPath
          = Concat("/", "create", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
        endpoint.CoTipoSqlDml = TipoEndpointEnum.CREATE;
        endpoint.CoTipoDadoRetorno = TipoDadoEnum.VOID;
        endpoint.SnRetornoLista = false;
        foreach (var ientifier in createTableStatement.SchemaObjectName.Identifiers)
        {
          var nome = ientifier.Value;
          var tabela = RecuperarTabela(nome);
          _tabelasDaProcedure.Add(tabela);
          tabelasDoEndpoint.Add(tabela);
        }
        break;
      case AlterTableStatement alterTableStatement:
        endpoint.NoMetodoEndpoint = Concat("alter", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
        endpoint.NoPath = Concat("/", "alter", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
        endpoint.CoTipoSqlDml = TipoEndpointEnum.ALTER;
        endpoint.CoTipoDadoRetorno = TipoDadoEnum.VOID;
        endpoint.SnRetornoLista = false;
        foreach (var ientifier in alterTableStatement.SchemaObjectName.Identifiers)
        {
          var nome = ientifier.Value;
          var tabela = RecuperarTabela(nome);
          _tabelasDaProcedure.Add(tabela);
          tabelasDoEndpoint.Add(tabela);
        }
        break;
      case DropTableStatement dropTableStatement:
        endpoint.NoMetodoEndpoint = Concat("drop", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
        endpoint.NoPath = Concat("/", "drop", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
        endpoint.CoTipoSqlDml = TipoEndpointEnum.DROP;
        endpoint.CoTipoDadoRetorno = TipoDadoEnum.VOID;
        endpoint.SnRetornoLista = false;
        foreach (var objects in dropTableStatement.Objects)
        {
          foreach (var ientifier in objects.Identifiers)
          {
            var nome = ientifier.Value;
            var tabela = RecuperarTabela(nome);
            _tabelasDaProcedure.Add(tabela);
            tabelasDoEndpoint.Add(tabela);
          }
        }
        break;
      case ExecuteStatement executeStatement:
        endpoint.NoMetodoEndpoint
          = Concat("execute", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
        endpoint.NoPath
          = Concat("/", "execute", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
        endpoint.CoTipoSqlDml = TipoEndpointEnum.EXEC;
        endpoint.CoTipoDadoRetorno = TipoDadoEnum.VOID;
        endpoint.SnRetornoLista = false;
        break;
      case DeleteStatement deleteStatement:
        endpoint.NoMetodoEndpoint
          = Concat("delete", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
        endpoint.NoPath
          = Concat("/", "delete", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
        endpoint.CoTipoSqlDml = TipoEndpointEnum.DELETE;
        endpoint.CoTipoDadoRetorno = TipoDadoEnum.VOID;
        endpoint.SnRetornoLista = false;
        parametros = ExtrairVariaveis(deleteStatement.DeleteSpecification.WhereClause, procedure,
          tabelasDoEndpoint);
        //
        //   where = (BooleanBinaryExpression) deleteStatement.DeleteSpecification.WhereClause
        //    .SearchCondition;
        //
        //   variaveis = ExtrairVariaveis(procedure, where);
        foreach (var parametro in parametros)
        {
          parametro.EndpointsQueContemEssaVariavelComoParametro.Add(endpoint);
          // parametro.CoTipoEscopo
          //   = parametro.CoTipoEscopo == TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE ?
          //     parametro.CoTipoEscopo : TipoEscopoEnum.LOCAL;
          endpoint.Parametros.Add(parametro);
        }
        ExtrairTabelas(procedure, deleteStatement.DeleteSpecification.Target, tabelasDoEndpoint);
        break;
      case UpdateStatement updateStatement:
        endpoint.NoMetodoEndpoint
          = Concat("update", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
        endpoint.NoPath
          = Concat("/", "update", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
        endpoint.CoTipoSqlDml = TipoEndpointEnum.UPDATE;
        endpoint.CoTipoDadoRetorno = TipoDadoEnum.VOID;
        endpoint.SnRetornoLista = false;
        // where = (BooleanBinaryExpression) updateStatement.UpdateSpecification.WhereClause
        //  .SearchCondition;
        //
        // variaveis = ExtrairVariaveis(procedure, where);
        parametros = ExtrairVariaveis(updateStatement.UpdateSpecification.WhereClause, procedure,
          tabelasDoEndpoint);
        foreach (var setClause1 in updateStatement.UpdateSpecification.SetClauses)
        {
          var setClause = (AssignmentSetClause) setClause1;
          ExtrairVariavel(procedure, setClause.NewValue, parametros, tabelasDoEndpoint);
        }
        foreach (var parametro in parametros)
        {
          parametro.EndpointsQueContemEssaVariavelComoParametro.Add(endpoint);
          // parametro.CoTipoEscopo
          //   = parametro.CoTipoEscopo == TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE ?
          //     parametro.CoTipoEscopo : TipoEscopoEnum.LOCAL;
          endpoint.Parametros.Add(parametro);
        }
        ExtrairTabelas(procedure, updateStatement.UpdateSpecification.Target, tabelasDoEndpoint);
        break;
      case InsertStatement insertStatement:
        endpoint.NoMetodoEndpoint
          = Concat("insert", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
        endpoint.NoPath
          = Concat("/", "insert", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
        endpoint.CoTipoSqlDml = TipoEndpointEnum.INSERT;
        endpoint.CoTipoDadoRetorno = TipoDadoEnum.VOID;
        endpoint.SnRetornoLista = false;
        switch (insertStatement.InsertSpecification.InsertSource)
        {
          case SelectInsertSource:
            break;
          case ValuesInsertSource valuesInsertSource:
            foreach (var rowValue in valuesInsertSource.RowValues)
            {
              foreach (var scalarExpression in rowValue.ColumnValues)
              {
                ExtrairVariavel(procedure, scalarExpression, parametros, tabelasDoEndpoint);
              }
            }
            foreach (var parametro in parametros)
            {
              parametro.EndpointsQueContemEssaVariavelComoParametro.Add(endpoint);
              // parametro.CoTipoEscopo
              //   = parametro.CoTipoEscopo == TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE ?
              //     parametro.CoTipoEscopo : TipoEscopoEnum.LOCAL;
              endpoint.Parametros.Add(parametro);
            }
            break;
          default:
            procedure.SnSucessoParser = false;
            procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
              $"Statement unhandled: \"{insertStatement.InsertSpecification.InsertSource}\".\n");
            //throw new ApplicationException("Endpoint Type not yet mapped: " + statement);
            break;
        }
        ExtrairTabelas(procedure, insertStatement.InsertSpecification.Target, tabelasDoEndpoint);
        break;
      case SelectStatement selectStatement:
        endpoint.NoMetodoEndpoint
          = Concat("select", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
        endpoint.NoPath
          = Concat("/", "select", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
        endpoint.CoTipoSqlDml = TipoEndpointEnum.SELECT;
        endpoint.CoTipoDadoRetorno = TipoDadoEnum.DTO;
        endpoint.SnRetornoLista = null;
        //Console.WriteLine(procedure.NoStoredProcedure);
        QuerySpecification specification = null;
        // try
        // {
        //   specification = (QuerySpecification) selectStatement.QueryExpression;
        // }
        // catch (InvalidCastException  e)
        // {
        //   var queryParenthesisExpression
        //     = (QueryParenthesisExpression) selectStatement.QueryExpression;
        //   specification = (QuerySpecification) queryParenthesisExpression.QueryExpression;
        //   Console.WriteLine(e);
        // }
        if (selectStatement.QueryExpression.GetType() == typeof (BinaryQueryExpression))
        {
          procedure.SnSucessoParser = false;
          procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
            $"Statement unhandled: \"{selectStatement.QueryExpression}\".\n");
          break;
        }
        if (selectStatement.QueryExpression.GetType() == typeof(QueryParenthesisExpression))
        {
          var queryParenthesisExpression
            = (QueryParenthesisExpression) selectStatement.QueryExpression;
          specification = (QuerySpecification) queryParenthesisExpression.QueryExpression;
        }
        else
        {
          specification = (QuerySpecification) selectStatement.QueryExpression;
        }
        //specification = (QuerySpecification) selectStatement.QueryExpression;
        parametros = ExtrairVariaveis(specification.WhereClause, procedure, tabelasDoEndpoint);
        ExtrairVariaveisDoSelect(specification.SelectElements, procedure, parametros, endpoint);
        ExtrairVariaveis(specification.FromClause, procedure, parametros, tabelasDoEndpoint);
        foreach (var parametro in parametros)
        {
          parametro.EndpointsQueContemEssaVariavelComoParametro.Add(endpoint);
          // parametro.CoTipoEscopo
          //   = parametro.CoTipoEscopo == TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE ?
          //     parametro.CoTipoEscopo : TipoEscopoEnum.LOCAL;
          endpoint.Parametros.Add(parametro);
        }
        var classe = ExtrairClasse(specification.SelectElements, procedure);
        if (classe is not null)
        {
          endpoint.IdDtoClasse = classe.Id;
          endpoint.IdDtoClasseNavigation = classe;
          endpoint.CoTipoDadoRetorno = TipoDadoEnum.DTO;
          endpoint.SnRetornoLista = null;
        }
        ExtrairTabelas(specification.FromClause, procedure, tabelasDoEndpoint);
        break;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{statement}\".\n");
        //throw new ApplicationException("Endpoint Type not yet mapped: " + statement);
        break;
    }
    foreach (var parametro in parametros)
    {
      parametro.EndpointsQueContemEssaVariavelComoParametro.Add(endpoint);
      // parametro.CoTipoEscopo = parametro.CoTipoEscopo == TipoEscopoEnum.PARAMETRO_STORED_PROCEDURE ?
      //   parametro.CoTipoEscopo : TipoEscopoEnum.LOCAL;
      endpoint.Parametros.Add(parametro);
    }
    foreach (var tabela in _tabelasDaProcedure)
    {
      procedure.TabelasAssociadas.Add(tabela);
      tabela.StoredProceduresAssociadas.Add(procedure);
    }
    foreach (var tabela in tabelasDoEndpoint)
    {
      tabela.EndpointsAssociados.Add(endpoint);
      endpoint.TabelasAssociadas.Add(tabela);
    }
    comando.CoTipoComando = TipoComandoEnum.ENDPOINT;
    comando.IdEndpoint = endpoint.Id;
    comando.IdEndpointNavigation = endpoint;
  }

  private static Endpoint CriarEndpoint(StoredProcedure procedure, QuerySpecification query)
  {
    Endpoint endpoint = new();
    var fragmentSql = Parser.GetFragmentSqlTratado(query);
    endpoint.TxEndpoint = fragmentSql;
    _txEndpoint = endpoint.TxEndpoint;
    endpoint.TxEndpointTratado = fragmentSql.TratarEndpoint();
    endpoint.IdStoredProcedure = procedure.Id;
    endpoint.IdStoredProcedureNavigation = procedure;
    endpoint.NoMetodoEndpoint = Concat("select", procedure.NoStoredProcedure.InicialMaiuscula(), endpoint.Id);
    endpoint.NoPath = Concat("/", "select", "-", procedure.NoStoredProcedure.InicialMinuscula(), endpoint.Id);
    endpoint.CoTipoSqlDml = TipoEndpointEnum.SELECT;
    endpoint.CoTipoDadoRetorno = TipoDadoEnum.VOID;
    endpoint.SnRetornoLista = false;
    endpoint.SnAnalisado = false;
    ISet<Tabela> tabelasDoEndpoint = new HashSet<Tabela>();
    //var selectElements = query.SelectElements;
    var whereClause = query.WhereClause;
    if (whereClause == null)
    {
      return endpoint;
    }
    // switch (whereClause)
    // {
    //   case null when !selectElements.IsNullOrEmpty():
    //     procedure.SnSucessoParser = false;
    //     procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
    //       $"Statement unhandled: \"{selectElements}\".\n");
    //     return endpoint;
    //   case null:
    //     return endpoint;
    // }

    var variaveis = new List<Variavel>();
    switch (whereClause.SearchCondition)
    {
      case BooleanBinaryExpression binaria:
        //variaveis.AddRange(ExtrairVariaveis(procedure, binaria));
        ExtrairVariaveis(procedure, binaria, variaveis, tabelasDoEndpoint);
        break;
      case BooleanComparisonExpression comparacao:
        //variaveis.AddRange(ExtrairVariaveis(procedure, comparacao));
        ExtrairVariaveis(procedure, comparacao, variaveis, tabelasDoEndpoint);
        break;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{whereClause.SearchCondition}\".\n");
        //throw new ApplicationException("Expression Type not yet mapped: " + whereClause.SearchCondition);
        break;
    }
    //var where = (BooleanBinaryExpression) whereClause.SearchCondition;
    //var variaveis = ExtrairVariaveis(procedure, where);
    foreach (var variavel in variaveis)
    {
      variavel.EndpointsQueContemEssaVariavelComoParametro.Add(endpoint);
      endpoint.Parametros.Add(variavel);
    }
    foreach (var tabela in tabelasDoEndpoint)
    {
      tabela.EndpointsAssociados.Add(endpoint);
      endpoint.TabelasAssociadas.Add(tabela);
    }
    return endpoint;
  }

  private static List<Variavel> ExtrairVariaveisBooleanBinaryExpression(StoredProcedure procedure,
    BooleanBinaryExpression expressao, ISet<Tabela> tabelasDoEndpoint)
  {
    var variaveis = new List<Variavel>();
    ExtrairVariaveis(procedure, expressao.FirstExpression, variaveis, tabelasDoEndpoint);
    ExtrairVariaveis(procedure, expressao.SecondExpression, variaveis, tabelasDoEndpoint);
    return variaveis;
  }

  private static List<Variavel> ExtrairVariaveisBooleanComparisonExpression(
    StoredProcedure procedure, BooleanComparisonExpression expressao,
    ISet<Tabela> tabelasDoEndpoint)
  {
    var variaveis = new List<Variavel>();
    ExtrairVariavel(procedure, expressao.FirstExpression, variaveis, tabelasDoEndpoint);
    ExtrairVariavel(procedure, expressao.SecondExpression, variaveis, tabelasDoEndpoint);
    return variaveis;
  }

  private static void ExtrairVariaveis(StoredProcedure procedure, BooleanExpression expressao,
    List<Variavel> variaveis, ISet<Tabela> tabelasDoEndpoint)
  {
    switch (expressao)
    {
      case BooleanBinaryExpression binaria:
        variaveis.AddRange(
          ExtrairVariaveisBooleanBinaryExpression(procedure, binaria, tabelasDoEndpoint));
        break;
      case BooleanComparisonExpression comparacao:
        variaveis.AddRange(
          ExtrairVariaveisBooleanComparisonExpression(procedure, comparacao, tabelasDoEndpoint));
        break;
      case ExistsPredicate:
        break;
      case BooleanIsNullExpression booleanIsNullExpression:
        ExtrairVariavel(procedure, booleanIsNullExpression.Expression, variaveis,
          tabelasDoEndpoint);
        break;
      case InPredicate inPredicate:
        ExtrairVariavel(procedure, inPredicate.Expression, variaveis, tabelasDoEndpoint);
        foreach (var scalarExpression in inPredicate.Values)
        {
          ExtrairVariavel(procedure, scalarExpression, variaveis, tabelasDoEndpoint);
        }
        break;
      case BooleanParenthesisExpression parenthesisExpression:
        ExtrairVariaveis(procedure, parenthesisExpression.Expression, variaveis, tabelasDoEndpoint);
        break;
      case BooleanNotExpression notExpression:
        ExtrairVariaveis(procedure, notExpression.Expression, variaveis, tabelasDoEndpoint);
        break;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{expressao}\".\n");
        //throw new ApplicationException("Expression Type not yet mapped: " + expressao);
        break;
    }
  }

  private static void ExtrairVariaveis(FromClause? fromClause, StoredProcedure procedure,
    List<Variavel> variaveis, ISet<Tabela> tabelasDoEndpoint)
  {
    if (fromClause is null)
    {
      return;
    }
    foreach (var tableReference in fromClause.TableReferences)
    {
      switch (tableReference)
      {
        case OpenXmlTableReference openXml:
          ExtrairVariavel(procedure, openXml.Variable, variaveis, tabelasDoEndpoint);
          break;
        case QualifiedJoin qualifiedJoin:
          ExtrairVariavel(procedure, qualifiedJoin.FirstTableReference, variaveis,
            tabelasDoEndpoint);
          ExtrairVariavel(procedure, qualifiedJoin.SecondTableReference, variaveis,
            tabelasDoEndpoint);
          break;
        case NamedTableReference namedTableReference:
        {
          foreach (var ientifier in namedTableReference.SchemaObject.Identifiers)
          {
            var nome = ientifier.Value;
            var tabela = RecuperarTabela(nome);
            _tabelasDaProcedure.Add(tabela);
          }
        }
          break;
        default:
          procedure.SnSucessoParser = false;
          procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
            $"Statement unhandled: \"{tableReference}\".\n");
          //throw new ApplicationException("Table Reference not yet mapped: " + tableReference);
          break;
      }
    }
  }

  private static void ExtrairVariavel(StoredProcedure procedure, TableReference? tableReference,
    List<Variavel> variaveis, ISet<Tabela> tabelasDoEndpoint)
  {
    if (tableReference is null)
    {
      return;
    }
    switch (tableReference)
    {
      case NamedTableReference namedTableReference:
      {
        foreach (var ientifier in namedTableReference.SchemaObject.Identifiers)
        {
          var nome = ientifier.Value;
          var tabela = RecuperarTabela(nome);
          _tabelasDaProcedure.Add(tabela);
        }
      }
        break;
      case QualifiedJoin qualifiedJoin:
        ExtrairVariavel(procedure, qualifiedJoin.FirstTableReference, variaveis, tabelasDoEndpoint);
        ExtrairVariavel(procedure, qualifiedJoin.SecondTableReference, variaveis,
          tabelasDoEndpoint);
        ExtrairVariavel(procedure, qualifiedJoin.SearchCondition, variaveis, tabelasDoEndpoint);
        break;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{tableReference}\".\n");
        //throw new ApplicationException($"TableReference Type {tableReference} not yet mapped");
        break;
    }
  }

  /* *********************************** EXTRAIR TABELAS **************************************** */

  private void ExtrairTabelas(FromClause? fromClause, StoredProcedure procedure,
    ISet<Tabela> tabelas)
  {
    if (fromClause is null)
    {
      return;
    }
    foreach (var tableReference in fromClause.TableReferences)
    {
      switch (tableReference)
      {
        // case OpenXmlTableReference openXml:
        //ExtrairTabelas(procedure, openXml.Variable, tabelas);
        // break;
        case QualifiedJoin qualifiedJoin:
          ExtrairTabelas(procedure, qualifiedJoin.FirstTableReference, tabelas);
          ExtrairTabelas(procedure, qualifiedJoin.SecondTableReference, tabelas);
          break;
        case NamedTableReference namedTableReference:
        {
          var tabelaEncontrada = RecuperarTabela(namedTableReference);
          tabelas.Add(tabelaEncontrada);
          break;
        }
        default:
          procedure.SnSucessoParser = false;
          procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
            $"Statement unhandled: \"{tableReference}\".\n");
          //throw new ApplicationException("Table Reference not yet mapped: " + tableReference);
          break;
      }
    }
  }

  private static void ExtrairTabelas(StoredProcedure procedure, TableReference tableReference,
    ISet<Tabela> tabelas)
  {
    switch (tableReference)
    {
      case NamedTableReference namedTableReference:
      {
        var tabelaEncontrada = RecuperarTabela(namedTableReference);
        tabelas.Add(tabelaEncontrada);
        break;
      }
      case QualifiedJoin qualifiedJoin:
        ExtrairTabelas(procedure, qualifiedJoin.FirstTableReference, tabelas);
        ExtrairTabelas(procedure, qualifiedJoin.SecondTableReference, tabelas);
        break;
      default:
        procedure.SnSucessoParser = false;
        procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
          $"Statement unhandled: \"{tableReference}\".\n");
        //throw new ApplicationException($"TableReference Type {tableReference} not yet mapped");
        break;
    }
  }

  // private static void ExtrairTabela(StoredProcedure procedure,
  //   MultiPartIdentifier multiPartIdentifier, HashSet<Tabela> tabelas)
  // {
  //   var tabela = new Tabela();
  //   foreach (var identifier in multiPartIdentifier.Identifiers)
  //   {
  //     tabela.NoTabela = identifier.Value;
  //   }
  //   tabelas.Add(tabela);
  // }

  private static Tabela RecuperarTabela(NamedTableReference tabela)
  {
    var nome = tabela.SchemaObject.BaseIdentifier.Value.ToLower();

    foreach (var tabelaExistente in _todasAsTabelas.Where(t
      => t.NoTabela.Equals(nome, StringComparison.CurrentCultureIgnoreCase)))
    {
      return tabelaExistente;
    }

    return CriarTabela(nome);
  }

  private static Tabela RecuperarTabela(string nome)
  {
    foreach (var tabelaExistente in _todasAsTabelas.Where(t
      => t.NoTabela.Equals(nome, StringComparison.CurrentCultureIgnoreCase)))
    {
      return tabelaExistente;
    }
    return CriarTabela(nome);
  }

  private static Tabela CriarTabela(string name)
  {
    var tabela = new Tabela {NoTabela = name};
    _todasAsTabelas.Add(tabela);
    return tabela;
  }

  /* ******************************************************************************************** */

  private static DtoClasse? ExtrairClasse(IEnumerable<SelectElement> selectElements,
    StoredProcedure procedure)
  {
    var selectElementsList = selectElements.ToList();
    if (selectElementsList.Any(element => element is not SelectScalarExpression))
    {
      return null;
    }
    var classe = new DtoClasse();
    //{
    classe.NoDtoClasse = Concat(procedure.NoStoredProcedure, "DTO", classe.Id); //,
    classe.IdStoredProcedure = procedure.Id;//,
    classe.IdStoredProcedureNavigation = procedure;
    //};
    foreach (var selectScalarExpression in selectElementsList.Cast<SelectScalarExpression>())
    {
      string nomeColuna;
      switch (selectScalarExpression.Expression)
      {
        // case BinaryExpression:
        // case CoalesceExpression:
        // case ScalarSubquery:
        // case ParenthesisExpression:
        // case FunctionCall:
        //
        //   nomeColuna = selectScalarExpression.ColumnName.Value;
        //   break;
        case ColumnReferenceExpression columnReferenceExpression:
          if (columnReferenceExpression.MultiPartIdentifier.Identifiers.Count == 2)
          {
            nomeColuna = Concat(columnReferenceExpression.MultiPartIdentifier.Identifiers[0].Value,
              columnReferenceExpression.MultiPartIdentifier.Identifiers[1].Value
               .InicialMaiuscula());
          }
          else if (selectScalarExpression.ColumnName is not null)
          {
            nomeColuna = selectScalarExpression.ColumnName.Value;
          }
          else
          {
            nomeColuna = columnReferenceExpression.MultiPartIdentifier.Identifiers[0].Value;
          }
          break;
        default:
          nomeColuna = selectScalarExpression.ColumnName is not null ?
            selectScalarExpression.ColumnName.Value : "nomeAtributoAindaNaoDefinido";
          break;
      }
      //var atributo = new Atributo(nomeColuna, TipoDadoEnum.OBJECT, classeDTO);
      var atributo = new Atributo(nomeColuna, TipoDadoEnum.STRING, classe.Id);
      classe.Atributos.Add(atributo);
    }
    return classe;
  }

  private static void ExtrairVariaveisDoSelect(IEnumerable<SelectElement> selectElements,
    StoredProcedure procedure, List<Variavel> variaveis, Endpoint endpoint)
  {
    foreach (var selectElement in selectElements)
    {
      switch (selectElement)
      {
        case SelectSetVariable selectSetVariable:
          var variavelRetornada = RecuperarVariavel(procedure, selectSetVariable.Variable.Name);
          // var variavelRetornada = RecuperarVariavel(procedure, selectSetVariable.Variable) ??
          // CriarVariavel(procedure, selectSetVariable.Variable.Name, SqlDataTypeOption.VarChar);
          variavelRetornada.IdStoredProcedure = procedure.Id;
          endpoint.IdVariavelRetornada = variavelRetornada.Id;
          endpoint.IdVariavelRetornadaNavigation = variavelRetornada;
          endpoint.CoTipoDadoRetorno = variavelRetornada.CoTipoDado;
          endpoint.SnRetornoLista = false;
          break;
        case SelectScalarExpression selectScalarExpression:
          switch (selectScalarExpression.Expression)
          {
            case VariableReference variableReference:
              // variaveis.Add(RecuperarVariavel(procedure, variableReference) ??
              //   CriarVariavel(procedure, variableReference.Name, SqlDataTypeOption.VarChar));
              variaveis.Add(RecuperarVariavel(procedure, variableReference.Name));
              break;
          }
          break;
      }
    }
    // if (selectElements.Any(element => element is not SelectScalarExpression))
    // {
    //   return;
    // }
    // foreach (var selectElement in selectElements)
    // {
    //   var selectScalarExpression = (SelectScalarExpression) selectElement;
    //   switch (selectScalarExpression.Expression)
    //   {
    //     case VariableReference variableReference:
    //       variaveis.Add(RecuperarVariavel(procedure, variableReference));
    //       break;
    //   }
    // }
  }

  private static List<Variavel> ExtrairVariaveis(WhereClause? whereClause,
    StoredProcedure procedure, ISet<Tabela> tabelasDoEndpoint)
  {
    var variaveis = new List<Variavel>();
    if (whereClause is null)
    {
      return variaveis;
    }
    variaveis = whereClause.SearchCondition switch
    {
      BooleanBinaryExpression booleanBinaryExpression => ExtrairVariaveisBooleanBinaryExpression(
        procedure, booleanBinaryExpression, tabelasDoEndpoint),
      BooleanComparisonExpression booleanComparisonExpression =>
        ExtrairVariaveisBooleanComparisonExpression(procedure, booleanComparisonExpression,
          tabelasDoEndpoint),
      _ => variaveis
    };
    return variaveis;
  }

  private static SqlDataTypeOption IdentificarTipo(StoredProcedure procedure,
    DataTypeReference tipoDado)
  {
    try
    {
      return ((SqlDataTypeReference) tipoDado).SqlDataTypeOption;
    }
    catch (InvalidCastException e)
    {
      procedure.SnSucessoParser = false;
      procedure.TxResultadoParser = Concat(procedure.TxResultadoParser,
        $"Statement unhandled: \"{e.Message}\".\n");
      //throw new ApplicationException("Data Type not yet mapped for conversion. " + e.Message);
      return SqlDataTypeOption.None;
    }
  }

  // private static IEnumerable<TSqlStatement> RecuperarStatements(
  //   ProcedureStatementBodyBase procStatement)
  // {
  //   var statements = procStatement.StatementList.Statements;
  //   if (statements.Count != 1)
  //   {
  //     // Espera-se que o corpo principal só tenha um Statement Do tipo BeginEnd
  //     throw new ApplicationException(
  //       "Unexpected situation. Main body has more than one statement.");
  //   }
  //   try
  //   {
  //     var statement = (BeginEndBlockStatement) statements[0];
  //     return statement.StatementList.Statements;
  //   }
  //   catch (Exception e)
  //   {
  //     throw new ApplicationException("Unexpected situation. Main body has no BeginEnd. " +
  //       e.Message);
  //   }
  // }

  // private static IEnumerable<StoredProcedure> RecuperarStoredProcedures(string connectionString,
  //   string schema)
  // {
  //   List<StoredProcedure> storedProcedures = new();
  //   var queryString = GetSelectStoredProcedures(schema);
  //   using var connection = new SqlConnection(connectionString);
  //   var command = new SqlCommand(queryString, connection);
  //   connection.Open();
  //   var reader = command.ExecuteReader();
  //   // Call Read before accessing data.
  //   while (reader.Read())
  //   {
  //     var storedProcedure = ReadSingleRow(reader);
  //     storedProcedure.SnSucessoParser = true;
  //     storedProcedures.Add(storedProcedure);
  //   }
  //   // Call Close when done reading.
  //   reader.Close();
  //   return storedProcedures;
  // }

  // private static StoredProcedure ReadSingleRow(IDataRecord dataRecord)
  // {
  //   //_logger.LogInformation("{dataRecord[0]}, {dataRecord[1]}, {dataRecord[2]}, {dataRecord[3]}",
  //   //  dataRecord[0], dataRecord[1], dataRecord[2], dataRecord[3]);
  //
  //   var schemaName = /*(string) dataRecord[0] == null ? "Schema não encontrado." :*/
  //     (string) dataRecord[0];
  //   var procedureName = /* (string) dataRecord[1] == null ? "Nome não encontrado." :*/
  //     (string) dataRecord[1];
  //   var definition = (string) dataRecord[3];
  //   var storedProcedure = new StoredProcedure(schemaName, procedureName, definition);
  //   var parameters = dataRecord[2];
  //   var variaveis = GetVariveis(parameters, storedProcedure);
  //   storedProcedure.NoSchema = schemaName;
  //   storedProcedure.NoStoredProcedure = procedureName;
  //   storedProcedure.TxDefinicao = definition;
  //   storedProcedure.TxDefinicaoTratada = definition.TratarProcedure();
  //   storedProcedure.Variaveis = variaveis;
  //   return storedProcedure;
  // }

  // private static List<Variavel> GetVariveis(object parameters, StoredProcedure storedProcedure)
  // {
  //   //@AGRCod char, @PRGAno char, @ORGCod char
  //   var nomeP1 = "@AGRCod";
  //   var tipoP1 = TipoDadoEnum.STRING; // "char";
  //   var nomeP2 = "@PRGAno";
  //   var tipoP2 = TipoDadoEnum.STRING; // "char";
  //   var nomeP3 = "@ORGCod";
  //   var tipoP3 = TipoDadoEnum.STRING; // "char";
  //   var variaveis = new List<Variavel>();
  //   var v1 = new Variavel
  //   {
  //     NoVariavel = nomeP1,
  //     CoTipoDado = tipoP1,
  //     NuTamanho = 0,
  //     IdStoredProcedure = storedProcedure.Id,
  //     IdStoredProcedureNavigation = storedProcedure
  //   };
  //   variaveis.Add(v1);
  //   var v2 = new Variavel
  //   {
  //     NoVariavel = nomeP2,
  //     CoTipoDado = tipoP2,
  //     NuTamanho = 0,
  //     IdStoredProcedure = storedProcedure.Id,
  //     IdStoredProcedureNavigation = storedProcedure
  //   };
  //   variaveis.Add(v2);
  //   var v3 = new Variavel
  //   {
  //     NoVariavel = nomeP3,
  //     CoTipoDado = tipoP3,
  //     NuTamanho = 0,
  //     IdStoredProcedure = storedProcedure.Id,
  //     IdStoredProcedureNavigation = storedProcedure
  //   };
  //   variaveis.Add(v3);
  //   return variaveis;
  // }
  //
  // private static string GetSelectStoredProcedures(string schema)
  //   => "SELECT schema_name(obj.schema_id) AS schema_name, " +
  //     "       obj.name as procedure_name, " +
  //     "        substring(par.parameters, 0, len(par.parameters)) AS parameters, " +
  //     "        mod.definition " + "FROM sys.objects obj " + "JOIN sys.sql_modules mod " +
  //     "     ON mod.object_id = obj.object_id " +
  //     "CROSS apply (SELECT p.name + ' ' + TYPE_NAME(p.user_type_id) + ', ' " +
  //     "             FROM sys.parameters p " + "             WHERE p.object_id = obj.object_id " +
  //     "                   AND p.parameter_id != 0 " +
  //     "             FOR XML PATH ('') ) par (parameters) " +
  //     "WHERE obj.type IN ('P', 'X') and schema_name(obj.schema_id) = '" + schema + "'" +
  //     "ORDER BY schema_name, procedure_name ";
}
