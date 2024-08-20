using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using sp2mic.WebAPI.CrossCutting.Exceptions;
using sp2mic.WebAPI.Modulo3.Geracao.Dtos;
using sp2mic.WebAPI.Modulo3.Geracao.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo3.Geracao.Controllers;

public class GeracaoController : ControllerBase
{
  private readonly ILogger<GeracaoController> _logger;
  private readonly IGeracaoMicrosservicos<GeracaoDto> _geracaoMicrosservicosService;

  public GeracaoController(IGeracaoMicrosservicos<GeracaoDto> geracaoMicrosservicosService,
    ILogger<GeracaoController> logger)
  {
    _geracaoMicrosservicosService = geracaoMicrosservicosService ??
      throw new ArgumentNullException(nameof (geracaoMicrosservicosService));
    _logger = logger ?? throw new ArgumentNullException(nameof (logger));
  }

  [HttpGet("api/generation/ping")]
  public IActionResult Ping() => NoContent();

  [HttpPost("api/generation/projetos")]
  public IActionResult Project(GeracaoDto dto)
  {
    _logger.LogInformation("GeracaoController -> Project");
    var mensagem = _geracaoMicrosservicosService.GerarTodosProjetos(dto);

    return mensagem.Equals("No microservices pending generation were found.") ?
      StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, mensagem)) :
      StatusCode(StatusCodes.Status200OK, new ApiResponse(StatusCodes.Status200OK, mensagem));
  }

  [HttpGet("api/generation/download"), DisableRequestSizeLimit]
  public async Task<IActionResult> Download([FromQuery] string fileName)
  {
    _logger.LogInformation("GeracaoController -> Download {Name}", fileName);
    //var zipFileName = "generated-microservices.zip";

    var dirArquivoZip = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "generated-microservices-files");
    if(!Directory.Exists(dirArquivoZip))
    {
      Directory.CreateDirectory(dirArquivoZip);
    }

    var zipFileNameRelativePath = Path.Combine("Resources", "generated-microservices-files", "generated-microservices.zip");
    var zipFileNameAbsolutPath = Path.Combine(Directory.GetCurrentDirectory(), zipFileNameRelativePath);
    if (System.IO.File.Exists(zipFileNameAbsolutPath))
    {
      System.IO.File.Delete(zipFileNameRelativePath);
    }

    var diretorioDestinoClasses = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "generated-microservices");

    ZipFile.CreateFromDirectory(diretorioDestinoClasses, zipFileNameRelativePath,
      CompressionLevel.Optimal, false);

    if (!System.IO.File.Exists(zipFileNameAbsolutPath))
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, $"Error generating file."));
    }

    var memory = new MemoryStream();
    await using (var stream = new FileStream(zipFileNameAbsolutPath, FileMode.Open))
    {
      await stream.CopyToAsync(memory);
    }
    memory.Position = 0;

    return File(memory, GetContentType(zipFileNameAbsolutPath), "generated-microservices.zip");
  }

  private static string GetContentType(string path)
  {
    var provider = new FileExtensionContentTypeProvider();

    if (!provider.TryGetContentType(path, out var contentType))
    {
      contentType = "application/octet-stream";
    }

    return contentType;
  }
}
