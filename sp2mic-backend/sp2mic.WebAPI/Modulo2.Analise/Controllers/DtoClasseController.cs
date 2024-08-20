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
public class DtoClasseController : ControllerBase
{
  private readonly IDtoClasseService _service;
  private readonly IMapper _mapper;

  public DtoClasseController(IDtoClasseService service, IMapper mapper)
  {
    _service = service ?? throw new ArgumentNullException(nameof (service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof (mapper));
  }

  [HttpGet("api/analysis/dto-classes/ping")]
  public IActionResult Ping() => NoContent();

  [HttpGet("api/analysis/dto-classes/find-by-id/{id:int:min(1)}")]
  public ActionResult<DtoClasseDto> FindById(int id)
  {
    var obj = _service.FindById(id);
    var dto = _mapper.Map<DtoClasseDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/dto-classes/find-by-id-async/{id:int:min(1)}", Name = "GetDtoClasse")]
  public async Task<ActionResult<DtoClasseDto>> FindByIdAsync(int id)
  {
    var obj = await _service.FindByIdAsync(id);
    var dto = _mapper.Map<DtoClasseDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/dto-classes/find-all")]
  public ActionResult<IEnumerable<DtoClasseDto>> FindAll()
  {
    var objs = _service.FindAll();
    return Ok(_mapper.Map<IEnumerable<DtoClasseDto>>(objs));
  }

  [HttpGet(
    "api/analysis/dto-classes/find-by-id-procedure-for-combo/{idStoredProcedure:int:min(1)}")]
  public IActionResult FindByIdProcedureForCombo(int idStoredProcedure)
    => Ok(_service.FindByIdProcedureForCombo(idStoredProcedure));

  [HttpGet("api/analysis/dto-classes/find-all-async")]
  public async Task<ActionResult<IEnumerable<DtoClasseDto>>> FindAllAsync()
  {
    var objs = await _service.FindAllAsync();
    return Ok(_mapper.Map<IEnumerable<DtoClasseDto>>(objs));
  }

  [HttpGet("api/analysis/dto-classes/find-by-filter")]
  public ActionResult<IEnumerable<DtoClasseDto>> FindByFilter(
    [FromQuery] DtoClasseFilterDto? filter)
  {
    //var obj = _mapper.Map<DtoClasse>(dto);
    var objs = _service.FindByFilter(filter);
    // var soUma = objs.Where(x => x.IdStoredProcedure > Constantes.ID_PROCEDURE_ANALISADA).ToList();
    // return Ok(_mapper.Map<IEnumerable<DtoClasseDto>>(soUma));
    return Ok(_mapper.Map<IEnumerable<DtoClasseDto>>(objs));
  }

  [HttpGet("api/analysis/dto-classes/find-by-filter-async")]
  public async Task<ActionResult<IEnumerable<DtoClasseDto>>> FindByFilterAsync(
    [FromQuery] DtoClasseFilterDto? filter)
  {
    //var obj = _mapper.Map<DtoClasse>(dto);
    var objs = await _service.FindByFilterAsync(filter);
    // var soUma = objs.Where(x => x.IdStoredProcedure > Constantes.ID_PROCEDURE_ANALISADA).ToList();
    // return Ok(_mapper.Map<IEnumerable<DtoClasseDto>>(soUma));
    return Ok(_mapper.Map<IEnumerable<DtoClasseDto>>(objs));
  }

  [HttpPost("api/analysis/dto-classes/add")]
  public async Task<ActionResult> Add([FromBody] DtoClasseDto dto)
  {
    try
    {
      var novo = _mapper.Map<DtoClasse>(dto);
      await _service.AddAsync(novo);
      return new CreatedAtRouteResult("GetDtoClasse", new {id = novo.Id}, novo);
    }
    catch (Exception e)
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
    }
  }

  [HttpPut("api/analysis/dto-classes/update/{id:int:min(1)}")]
  public async Task<IActionResult> Update(int id, DtoClasseDto dto)
  {
    try
    {
      var atual = _mapper.Map<DtoClasse>(dto);
      await _service.UpdateAsync(id, atual);
      return NoContent();
    }
    catch (Exception e)
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
    }
  }

  [HttpDelete("api/analysis/dto-classes/delete/{id:int:min(1)}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _service.DeleteAsync(id);
    return NoContent();
  }
}
