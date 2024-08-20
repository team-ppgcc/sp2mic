using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sp2mic.WebAPI.Domain.Entities;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.Modulo2.Analise.Controllers;

[ApiController]
[Produces("application/json")]
public class TabelaController : ControllerBase
{
  private readonly ITabelaService _service;
  private readonly IMapper _mapper;

  public TabelaController(ITabelaService service, IMapper mapper)
  {
    _service = service ?? throw new ArgumentNullException(nameof (service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof (mapper));
  }

  [HttpGet("api/analysis/tabelas/find-by-id/{id:int:min(1)}")]
  public IActionResult FindById(int id)
  {
    var obj = _service.FindById(id);
    var dto = _mapper.Map<TabelaDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/tabelas/find-by-id-async/{id:int:min(1)}", Name="GetTabela")]
  public async Task<IActionResult> FindByIdAsync(int id)
  {
    var obj = await _service.FindByIdAsync(id);
    var dto = _mapper.Map<TabelaDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/tabelas/find-all")]
  public IActionResult FindAll()
  {
    var objs = _service.FindAll();
    return Ok(_mapper.Map<IEnumerable<TabelaDto>>(objs));
  }

  [HttpGet("api/analysis/tabelas/find-all-async")]
  public async Task<IActionResult> FindAllAsync()
  {
    var objs = await _service.FindAllAsync();
    return Ok(_mapper.Map<IEnumerable<TabelaDto>>(objs));
  }

  [HttpGet("api/analysis/tabelas/find-by-filter")]
  public IActionResult FindByFilter([FromQuery] TabelaFilterDto? filter)
  {
    var objs = _service.FindByFilter(filter);
    return Ok(_mapper.Map<IEnumerable<TabelaDto>>(objs));
  }

  [HttpGet("api/analysis/tabelas/find-by-filter-async")]
  public async Task<IActionResult> FindByFilterAsync([FromQuery] TabelaFilterDto? filter)
  {
    var objs = await _service.FindByFilterAsync(filter);
    return Ok(_mapper.Map<IEnumerable<TabelaDto>>(objs));
  }

  [HttpPost("api/analysis/tabelas/add")]
  public async Task<IActionResult> Add([FromBody] TabelaDto dto)
  {
    var novo = _mapper.Map<Tabela>(dto);
    await _service.AddAsync(novo);
    return new CreatedAtRouteResult("GetTabela", new {id = novo.Id}, novo);
  }

  [HttpPut("api/analysis/tabelas/update/{id:int:min(1)}")]
  public async Task<IActionResult> Update(int id, TabelaDto dto)
  {
    var atual = _mapper.Map<Tabela>(dto);
    await _service.UpdateAsync(id, atual);
    return NoContent();
  }

  [HttpDelete("api/analysis/tabelas/delete/{id:int:min(1)}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _service.DeleteAsync(id);
    return NoContent();
  }
}
