using System.Diagnostics;
using System.Net.Http.Headers;
using sp2mic.WebAPI.CrossCutting;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.Domain.Enumerations;
using sp2mic.WebAPI.Modulo1.Carga.Dtos;
using sp2mic.WebAPI.Modulo1.Carga.Services.Interfaces;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo1.Carga.Services;

public class CargaService : ICargaService
{
  private readonly IStoredProcedureService _storedProcedureService;
  private readonly IEndpointService _endpointService;
  private readonly IDtoClasseService _dtoClasseService;
  private readonly ITabelaService _tabelaService;
  private readonly IVariavelService _variavelService;
  private readonly ILogger<CargaService> _logger;

  public CargaService(IStoredProcedureService storedProcedureService,
    IEndpointService endpointService, IDtoClasseService dtoClasseService,
    ITabelaService tabelaService,
    IVariavelService variavelService, ILogger<CargaService> logger)
  {
    _storedProcedureService = storedProcedureService;
    _endpointService = endpointService;
    _dtoClasseService = dtoClasseService;
    _tabelaService = tabelaService;
    _variavelService = variavelService;
    _logger = logger ?? throw new ArgumentNullException(nameof (logger));
  }

  public IEnumerable<ParDto> ListarBancosSuportados()
    => (from tipo in Enum<TipoBancoDeDadosEnum>.GetValues()
      where tipo != TipoBancoDeDadosEnum.MOCK && tipo != TipoBancoDeDadosEnum.TIPO_NAO_MAPEADO
      select new ParDto(tipo.GetCodigo(), tipo.GetNome())).ToList();

  public async Task<IEnumerable<ParDto>> ListarNomesProcedures(CargaDto dto)
  {
    if (dto.TipoBancoDeDados is null)
    {
      throw new BadHttpRequestException("Database type must be filled.");
    }
    ValidarNomeTipoBanco(dto.TipoBancoDeDados.Value);
    var tratutor = ITradutorBanco.GetTradutor(dto.TipoBancoDeDados.Value, _tabelaService, _variavelService);
    var stopWatch = MarcarMomentoInicioExecucao();
    var listaProcedures = tratutor.FetchNomesProcedures(dto).OrderBy(i => i.Nome).ToList();
    listaProcedures = dto.NomeProcedure is null ? listaProcedures :
      listaProcedures.Where(r => r.Nome.Contains(dto.NomeProcedure)).ToList();
    listaProcedures = await IdentificarSeJaFoiCarregada(listaProcedures);
    MarcarMomentoFinalExecucao(stopWatch, _logger);
    return dto.SnCarregada is null ? listaProcedures :
      listaProcedures.Where(r => r.SnCarregada == dto.SnCarregada).ToList();
  }

  private async Task<List<ParDto>> IdentificarSeJaFoiCarregada(List<ParDto> retorno)
  {
    var proceduresJaCarregadas = await _storedProcedureService.FindAllAsync();
    var proceduresJaCarregadasNomes
      = proceduresJaCarregadas.Select(s => s.NoStoredProcedure).ToList();
    foreach (var sp in retorno)
    {
      sp.SnCarregada = proceduresJaCarregadasNomes.Contains(sp.Nome);
    }
    return retorno;
  }

  public void CarregarProceduresSelecionadas(CargaDto dto)
  {
    if (dto.TipoBancoDeDados is null)
    {
      throw new BadHttpRequestException("Database type must be filled.");
    }
    var tratutor = ITradutorBanco.GetTradutor(dto.TipoBancoDeDados.Value, _tabelaService, _variavelService);
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var listSPs = tratutor.FetchProceduresSelecionadas(dto).ToList();
    _storedProcedureService.SaveAll(listSPs);
    _endpointService.AjustarNomesEndpoints();
    _endpointService.AjustarPathsEndpoints();
    _dtoClasseService.AjustarNomesClasses();
    _endpointService.AjustarRetornoEndpoints();
    stopWatch.Stop();
    _logger.LogInformation(
      @"Elapsed time to loading and processing selected stored procedures: {T0:hh\:mm\:ss\.fff}",
      stopWatch.Elapsed);
  }

  public async Task IncluirArquivoProcedure(IFormCollection formCollection)
  {
    var file = formCollection.Files[0];
    var folderName = Path.Combine("Resources", Constantes.UploadDirecotroy);
    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
    if (fileName == null)
    {
      throw new BadHttpRequestException("Error loading Stored Procedure from file.");
    }
    if (!fileName[fileName.LastIndexOf(".", StringComparison.Ordinal)..]
     .Equals(Constantes.SufixoArquivo))
    {
      throw new BadHttpRequestException("Invalid file type");
    }
    var fullPath = Path.Combine(pathToSave, fileName);
    await using var stream = new FileStream(fullPath, FileMode.Create);
    await file.CopyToAsync(stream);
  }

  private static void ValidarNomeTipoBanco(TipoBancoDeDadosEnum nomeTipoBanco)
  {
    if (!Enum<TipoBancoDeDadosEnum>.IsDefined(nomeTipoBanco))
    {
      throw new ApplicationException("Invalid Database Type.");
    }
  }

  private static Stopwatch MarcarMomentoInicioExecucao()
  {
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    return stopWatch;
  }

  private static void MarcarMomentoFinalExecucao(Stopwatch stopWatch, ILogger logger)
  {
    stopWatch.Stop();
    logger.LogInformation(
      @"Elapsed time to fetch stored procedures from database connection: {T0:hh\:mm\:ss\.fff}",
      stopWatch.Elapsed);
  }

  // public List<TabelasProcedureDto> IdentificarTabelasProcedures(CargaDto dto)
  // {
  //   //GetTableList
  //   var retorno = new List<TabelasProcedureDto>();
  //   var todasProcedures = ConexaoUtil.RecuperarTodasProcedures(dto);
  //   if (!todasProcedures.Any())
  //   {
  //     return retorno;
  //   }
  //   retorno.AddRange(from par in todasProcedures let tabelasDaProcedure
  //       = ConexaoUtil.IdentificarTabelasProcedures(dto, par.Nome)
  //     select new TabelasProcedureDto(par.Nome, tabelasDaProcedure));
  //   return retorno;
  // }

  // public async Task CalcularMetricas()
  // {
  // var procedures = await _storedProcedureService.FindByFilterAsync(null);
  // // TODO Salvar dados em banco
  // foreach (var sp in procedures)
  // {
  //   var metricas
  //     = new SQLProcedureParser(sp.NoSchema, sp.TxDefinicaoTratada, sp.NoStoredProcedure);
  //   metricas.ShowParsedProcedure();
  //   metricas.PrintParsedProcedure();
  //   SaveFile(
  //     Path.Combine(Constantes.DiretorioDestinoClassesResources, "generated-microservices"),
  //     "ShowParsedProcedure_ID=" + sp.Id + ".txt", metricas.ShowParsedProcedure().ToString());
  //   SaveFile(
  //     Path.Combine(Constantes.DiretorioDestinoClassesResources, "generated-microservices"),
  //     "PrintParsedProcedure_ID=" + sp.Id + ".txt", metricas.PrintParsedProcedure());
  // }
  //}
  // public static int levenshteinDistance( String s1, String s2 ) {
  //   return dist( s1.toCharArray(), s2.toCharArray() );
  // }
  //
  // public static int dist( char[] s1, char[] s2 ) {
  //
  //   // memoize only previous line of distance matrix
  //   int[] prev = new int[ s2.length + 1 ];
  //
  //   for( int j = 0; j < s2.length + 1; j++ ) {
  //     prev[ j ] = j;
  //   }
  //
  //   for( int i = 1; i < s1.length + 1; i++ ) {
  //
  //     // calculate current line of distance matrix
  //     int[] curr = new int[ s2.length + 1 ];
  //     curr[0] = i;
  //
  //     for( int j = 1; j < s2.length + 1; j++ ) {
  //       int d1 = prev[ j ] + 1;
  //       int d2 = curr[ j - 1 ] + 1;
  //       int d3 = prev[ j - 1 ];
  //       if ( s1[ i - 1 ] != s2[ j - 1 ] ) {
  //         d3 += 1;
  //       }
  //       curr[ j ] = Math.min( Math.min( d1, d2 ), d3 );
  //     }
  //
  //     // define current line of distance matrix as previous
  //     prev = curr;
  //   }
  //   return prev[ s2.length ];
  // }
}
