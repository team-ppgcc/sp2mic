using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sp2mic.WebAPI.CrossCutting.Exceptions;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Controllers;

[ApiController]
[Produces("application/json")]
public class StoredProcedureController : ControllerBase
{
  private readonly IStoredProcedureService _service;
  private readonly IMapper _mapper;

  public StoredProcedureController(IStoredProcedureService service, IMapper mapper)
  {
    _service = service ?? throw new ArgumentNullException(nameof (service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof (mapper));
  }

  [HttpGet("api/analysis/stored-procedures/ping")]
  public IActionResult Ping() => NoContent();

  [HttpGet("api/analysis/stored-procedures/find-by-id/{id:int:min(1)}")]
  public IActionResult FindById(int id)
  {
    var obj = _service.FindById(id);
    var dto = _mapper.Map<StoredProcedureDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/stored-procedures/find-by-id-async/{id:int:min(1)}",
    Name = "GetStoredProcedure")]
  public async Task<IActionResult> FindByIdAsync(int id)
  {
    var obj = await _service.FindByIdAsync(id);
    var dto = _mapper.Map<StoredProcedureDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/stored-procedures/find-all")]
  public ActionResult<IEnumerable<StoredProcedureListagemDto>> FindAll()
  {
    var objs = _service.FindAll();
    return Ok(_mapper.Map<IEnumerable<StoredProcedureListagemDto>>(objs));
  }

  [HttpGet("api/analysis/stored-procedures/find-all-async")]
  public async Task<IActionResult> FindAllAsync()
  {
    var objs = await _service.FindAllAsync();
    return Ok(_mapper.Map<IEnumerable<StoredProcedureListagemDto>>(objs));
  }

  [HttpGet("api/analysis/stored-procedures/find-by-filter")]
  public IActionResult FindByFilter([FromQuery] StoredProcedureFilterDto? filter)
  {
    var objs = _service.FindByFilter(filter);
    // var soUma = dtoList.Where(x => x.Id > Constantes.ID_PROCEDURE_ANALISADA).ToList();
    // return Ok(soUma);
    return Ok(_mapper.Map<IEnumerable<StoredProcedureListagemDto>>(objs));
  }

  [HttpGet("api/analysis/stored-procedures/find-by-filter-async")]
  public async Task<IActionResult> FindByFilterAsync([FromQuery] StoredProcedureFilterDto? filter)
  {
    var objs = await _service.FindByFilterAsync(filter);
    return Ok(_mapper.Map<IEnumerable<StoredProcedureListagemDto>>(objs));
  }

  [HttpPost("api/analysis/stored-procedures/add")]
  public async Task<IActionResult> Add([FromBody] StoredProcedureDto dto)
  {
    var novo = _mapper.Map<StoredProcedure>(dto);
    await _service.AddAsync(novo);
    return new CreatedAtRouteResult("GetStoredProcedure", new {id = novo.Id}, novo);
  }

  [HttpPut("api/analysis/stored-procedures/update/{id:int:min(1)}")]
  public async Task<IActionResult> Update(int id, StoredProcedureDto dto)
  {
    try
    {
      var atual = _mapper.Map<StoredProcedure>(dto);
      await _service.UpdateAsync(id, atual);
      return NoContent();
    }
    catch (Exception e)
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
    }
  }

  [HttpDelete("api/analysis/stored-procedures/delete/{id:int:min(1)}")]
  public IActionResult Delete(int id)
  {
    _service.Delete(id);
    return NoContent();
  }

  [HttpDelete("api/analysis/stored-procedures/delete-all")]
  public IActionResult DeleteAll()
  {
    _service.DeleteAll();
    return NoContent();
  }

  [HttpGet("api/analysis/stored-procedures/get-definicao-by-id/{id:int:min(1)}")]
  public IActionResult GetDefinicaoById(int id)
  {
    var obj = _service.FindById(id);
    var dto = _mapper.Map<StoredProcedureViewDto>(obj);
    return Ok(dto);
  }
}
