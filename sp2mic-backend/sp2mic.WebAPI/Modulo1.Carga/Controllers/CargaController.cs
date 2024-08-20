using Microsoft.AspNetCore.Mvc;
using sp2mic.WebAPI.CrossCutting.Exceptions;
using sp2mic.WebAPI.Modulo1.Carga.Dtos;
using sp2mic.WebAPI.Modulo1.Carga.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo1.Carga.Controllers;

public class CargaController : ControllerBase
{
  private readonly ICargaService _service;

  public CargaController(ICargaService cargaService)
    => _service = cargaService ?? throw new ArgumentNullException(nameof (cargaService));

  // [HttpGet("api/load/calcular-metricas")]
  // public async void CalcularMetricas() { await _service.CalcularMetricas(); }

  [HttpGet("api/load/ping")]
  public IActionResult Ping() => NoContent();

  [HttpGet("api/load/supported-database")]
  public IActionResult ListarBancosSuportados()
  {
    var result = _service.ListarBancosSuportados();
    return Ok(result);
  }

  [HttpPost("api/load/procedures-names")]
  public async Task<IActionResult> ListarNomesProcedures([FromBody] CargaDto dto)
  {
    var result = await _service.ListarNomesProcedures(dto);
    return Ok(result);
  }

  [HttpPost("api/load/load-procedures")]
  public IActionResult CarregarProceduresSelecionadas([FromBody] CargaDto dto)
  {
    _service.CarregarProceduresSelecionadas(dto);
    return StatusCode(StatusCodes.Status201Created,
      new ApiResponse(StatusCodes.Status201Created,
        "Stored Procedure(s) successfully loaded and processed."));
  }

  [HttpPost("api/load/include-procedure-file"), DisableRequestSizeLimit]
  public async Task<IActionResult> IncluirArquivo()
  {
    var formCollection  = await Request.ReadFormAsync();
    await _service.IncluirArquivoProcedure(formCollection);
  // var file = formCollection.Files[0];
  // var folderName = Path.Combine(Constantes.DiretorioDestinoClassesResources,
  //   Constantes.UploadDirecotroy);
  // var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
  // var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName
  // ?.Trim('"');
  // if (fileName == null)
  // {
  //   var responde = new ApiResponse(400, "Error loading Stored Procedure from file.",
  //     $"api/Carga/IncluirArquivoProcedure");
  //   return BadRequest(responde);
  // }
  // if (!fileName[fileName.LastIndexOf(".", StringComparison.Ordinal)..]
  //  .Equals(Constantes.SufixoArquivo))
  // {
  //   return BadRequest(new ApiResponse(400, "Invalid file type", $"api/Carga/IncluirArquivoProcedure"));
  // }
  // var fullPath = Path.Combine(pathToSave, fileName);
  // await using (var stream = new FileStream(fullPath, FileMode.Create))
  // {
  //   await file.CopyToAsync(stream);
  // }
  return StatusCode(StatusCodes.Status201Created,
    new ApiResponse(StatusCodes.Status201Created,
      "File uploaded successfully."));
  }
}
