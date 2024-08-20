using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Controllers;

[ApiController]
[Produces("application/json")]
public class VariavelController : ControllerBase
{
  private readonly IVariavelService _service;
  private readonly IMapper _mapper;

  public VariavelController (IVariavelService service, IMapper mapper)
  {
    _service = service ?? throw new ArgumentNullException(nameof (service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof (mapper));
  }

  [HttpGet("api/analysis/variaveis/find-by-id/{id:int:min(1)}")]
  public IActionResult FindById(int id)
  {
    var obj = _service.FindById(id);
    var dto = _mapper.Map<VariavelDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/variaveis/find-by-id-async/{id:int:min(1)}", Name="GetVariavel")]
  public async Task<IActionResult> FindByIdAsync(int id)
  {
    var obj = await _service.FindByIdAsync(id);
    var dto = _mapper.Map<VariavelDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/variaveis/find-all")]
  public IActionResult FindAll()
  {
    var objs = _service.FindAll();
    return Ok(_mapper.Map<IEnumerable<VariavelDto>>(objs));
  }

  [HttpGet("api/analysis/variaveis/find-all-async")]
  public async Task<IActionResult> FindAllAsync()
  {
    var objs = await _service.FindAllAsync();
    return Ok(_mapper.Map<IEnumerable<VariavelDto>>(objs));
  }

  [HttpGet("api/analysis/variaveis/find-by-filter")]
  public IActionResult FindByFilter([FromQuery] VariavelFilterDto? filter)
  {
    var objs = _service.FindByFilter(filter);
    return Ok(_mapper.Map<IEnumerable<VariavelDto>>(objs));
  }

  [HttpGet("api/analysis/variaveis/find-by-filter-async")]
  public async Task<IActionResult> FindByFilterAsync([FromQuery] VariavelFilterDto? filter)
  {
    var objs = await _service.FindByFilterAsync(filter);
    return Ok(_mapper.Map<IEnumerable<VariavelDto>>(objs));
  }

  [HttpPost("api/analysis/variaveis/add")]
  public async Task<IActionResult> Add([FromBody] VariavelDto dto)
  {
    var novo = _mapper.Map<Variavel>(dto);
    await _service.AddAsync(novo);
    return new CreatedAtRouteResult("GetVariavel", new {id = novo.Id}, novo);
  }

  [HttpPut("api/analysis/variaveis/update/{id:int:min(1)}")]
  public async Task<IActionResult> Update(int id, VariavelDto dto)
  {
    var atual = _mapper.Map<Variavel>(dto);
    await _service.UpdateAsync(id, atual);
    return NoContent();
  }

  [HttpDelete("api/analysis/variaveis/delete/{id:int:min(1)}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _service.DeleteAsync(id);
    return NoContent();
  }
}
