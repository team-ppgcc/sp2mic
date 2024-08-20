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
public class MicrosservicoController : ControllerBase
{
  private readonly IMicrosservicoService _service;
  private readonly IMapper _mapper;

  public MicrosservicoController(IMicrosservicoService service, IMapper mapper)
  {
    _service = service ?? throw new ArgumentNullException(nameof (service));
    _mapper = mapper ?? throw new ArgumentNullException(nameof (mapper));
  }

  [HttpGet("api/analysis/microsservicos/ping")]
  public IActionResult Ping() => NoContent();

  [HttpGet("api/analysis/microsservicos/find-by-id/{id:int:min(1)}")]
  public IActionResult FindById(int id)
  {
    var obj = _service.FindById(id);
    var dto = _mapper.Map<MicrosservicoDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/microsservicos/find-by-id-async/{id:int:min(1)}",
    Name = "GetMicrosservico")]
  public async Task<IActionResult> FindByIdAsync(int id)
  {
    var obj = await _service.FindByIdAsync(id);
    var dto = _mapper.Map<MicrosservicoDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/microsservicos/find-all")]
  public IActionResult FindAll()
  {
    var objs = _service.FindAll();
    return Ok(_mapper.Map<IEnumerable<MicrosservicoDto>>(objs));
  }

  [HttpGet("api/analysis/microsservicos/find-all-for-combo")]
  public IActionResult FindAllForCombo() => Ok(_service.FindAllForCombo());

  [HttpGet("api/analysis/microsservicos/find-all-async")]
  public async Task<IActionResult> FindAllAsync()
  {
    var objs = await _service.FindAllAsync();
    return Ok(_mapper.Map<IEnumerable<MicrosservicoDto>>(objs));
  }

  [HttpGet("api/analysis/microsservicos/find-by-filter")]
  public IActionResult FindByFilter([FromQuery] MicrosservicoFilterDto? filter)
  {
    var objs = _service.FindByFilter(filter);
    // var soUmaSp = new HashSet<MicrosservicoDto>();
    // foreach (var m in dtoList)
    // {
    //   foreach (var epDem in m.Endpoints.Where(e => e.IdStoredProcedure > Constantes.ID_PROCEDURE_ANALISADA))
    //   {
    //     soUmaSp.Add(m);
    //   }
    // }
    //return Ok(dtoList.Where(m=>m.Id==26));
    return Ok(_mapper.Map<IEnumerable<MicrosservicoDto>>(objs));
  }

  [HttpGet("api/analysis/microsservicos/find-by-filter-async")]
  public async Task<IActionResult> FindByFilterAsync([FromQuery] MicrosservicoFilterDto? filter)
  {
    var objs = await _service.FindByFilterAsync(filter);
    return Ok(_mapper.Map<IEnumerable<MicrosservicoDto>>(objs));
  }

  [HttpPost("api/analysis/microsservicos/add")]
  public async Task<IActionResult> Add([FromBody] MicrosservicoAddUpdateDto dto)
  {
    try
    {
      var novo = _mapper.Map<Microsservico>(dto);
      await _service.AddAsync(novo);
      return new CreatedAtRouteResult("GetMicrosservico", new {id = novo.Id}, novo);
    }
    catch (Exception e)
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
    }
  }

  [HttpPut("api/analysis/microsservicos/update/{id:int:min(1)}")]
  public async Task<IActionResult> Update(int id, MicrosservicoDto dto)
  {
    try
    {
      var atual = _mapper.Map<Microsservico>(dto);
      await _service.UpdateAsync(id, atual);
      return NoContent();
    }
    catch (Exception e)
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
    }
  }

  [HttpDelete("api/analysis/microsservicos/delete/{id:int:min(1)}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _service.DeleteAsync(id);
    return NoContent();
  }
}
