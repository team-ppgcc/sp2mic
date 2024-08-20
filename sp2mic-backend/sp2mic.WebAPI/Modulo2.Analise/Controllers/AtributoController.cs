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
public class AtributoController : ControllerBase
{
  private readonly IAtributoService _service;
  private readonly IMapper _mapper;

  public AtributoController(IAtributoService service, IMapper mapper)
  {
    _service = service ?? throw new ArgumentNullException(nameof (service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof (mapper));
  }

  [HttpGet("api/analysis/atributos/find-by-id/{id:int:min(1)}")]
  public ActionResult<AtributoDto> FindById(int id)
  {
    var obj = _service.FindById(id);
    var dto = _mapper.Map<AtributoDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/atributos/find-by-id-async/{id:int:min(1)}", Name = "GetAtributo")]
  public async Task<ActionResult<AtributoDto>> FindByIdAsync(int id)
  {
    var obj = await _service.FindByIdAsync(id);
    var dto = _mapper.Map<AtributoDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/atributos/find-all")]
  public ActionResult<IEnumerable<AtributoDto>> FindAll()
  {
    var objs = _service.FindAll();
    return Ok(_mapper.Map<IEnumerable<AtributoDto>>(objs));
  }

  [HttpGet("api/analysis/atributos/find-all-async")]
  public async Task<ActionResult<IEnumerable<AtributoDto>>> FindAllAsync()
  {
    var objs = await _service.FindAllAsync();
    return Ok(_mapper.Map<IEnumerable<AtributoDto>>(objs));
  }

  [HttpGet("api/analysis/atributos/find-by-filter")]
  public ActionResult<IEnumerable<AtributoDto>> FindByFilter([FromQuery] AtributoFilterDto? filter)
  {
    var objs = _service.FindByFilter(filter);
    return Ok(_mapper.Map<IEnumerable<AtributoDto>>(objs));
  }

  [HttpGet("api/analysis/atributos/find-by-filter-async")]
  public async Task<ActionResult<IEnumerable<AtributoDto>>> FindByFilterAsync(
    [FromQuery] AtributoFilterDto? filter)
  {
    var objs = await _service.FindByFilterAsync(filter);
    return Ok(_mapper.Map<IEnumerable<AtributoDto>>(objs));
  }

  [HttpPost("api/analysis/atributos/add")]
  public async Task<ActionResult> Add([FromBody] AtributoDto dto)
  {
    try
    {
      var novo = _mapper.Map<Atributo>(dto);
      await _service.AddAsync(novo);
      return new CreatedAtRouteResult("GetAtributo", new {id = novo.Id}, novo);
    }
    catch (Exception e)
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
    }
  }

  [HttpPut("api/analysis/atributos/update/{id:int:min(1)}")]
  public async Task<IActionResult> Update(int id, AtributoDto dto)
  {
    try
    {
      var atual = _mapper.Map<Atributo>(dto);
      await _service.UpdateAsync(id, atual);
      return NoContent();
    }
    catch (Exception e)
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
    }
  }

  [HttpDelete("api/analysis/atributos/delete/{id:int:min(1)}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _service.DeleteAsync(id);
    return NoContent();
  }

  [HttpGet("api/analysis/atributos/atributos-by-id-dtoclasse/{idDtoClasse:int:min(1)}")]
  public ActionResult<IEnumerable<AtributoDto>> GetAtributosByIdDtoClasse(int idDtoClasse)
  {
    var objs = _service.GetAtributosByIdDtoClasse(idDtoClasse);
    return Ok(_mapper.Map<IEnumerable<AtributoDto>>(objs));
  }
}
