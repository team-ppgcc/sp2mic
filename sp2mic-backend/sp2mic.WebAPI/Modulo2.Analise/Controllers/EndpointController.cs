using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sp2mic.WebAPI.CrossCutting.Exceptions;
using sp2mic.WebAPI.Modulo2.Analise.Dtos;
using sp2mic.WebAPI.Modulo2.Analise.Dtos.filter;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;
using Endpoint = sp2mic.WebAPI.Domain.Entities.Endpoint;

namespace sp2mic.WebAPI.Modulo2.Analise.Controllers;

[ApiController]
[Produces("application/json")]
public class EndpointController : ControllerBase
{
  private readonly IEndpointService _service;
  private readonly IMapper _mapper;

  public EndpointController(IEndpointService service, IMapper mapper)
  {
    _service = service;
    _mapper = mapper ?? throw new ArgumentNullException(nameof (mapper));
  }

  [HttpGet("api/analysis/endpoints/find-by-id/{id:int:min(1)}")]
  public IActionResult FindById(int id)
  {
    var obj = _service.FindById(id);
    var dto = _mapper.Map<EndpointDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/endpoints/find-by-id-async/{id:int:min(1)}", Name = "GetEndpoint")]
  public async Task<IActionResult> FindByIdAsync(int id)
  {
    var obj = await _service.FindByIdAsync(id);
    var dto = _mapper.Map<EndpointDto>(obj);
    return Ok(dto);
  }

  [HttpGet("api/analysis/endpoints/find-all")]
  public IActionResult FindAll()
  {
    var objs = _service.FindAll();
    return Ok(_mapper.Map<IEnumerable<EndpointListagemDto>>(objs));
  }

  [HttpGet("api/analysis/endpoints/find-all-async")]
  public async Task<IActionResult> FindAllAsync()
  {
    var objs = await _service.FindAllAsync();
    return Ok(_mapper.Map<IEnumerable<EndpointListagemDto>>(objs));
  }

  [HttpGet("api/analysis/endpoints/find-by-filter")]
  public IActionResult FindByFilter([FromQuery] EndpointFilterDto? filter)
  {
    var objs = _service.FindByFilter(filter);
    //var soUma = dtoList.Where(x => x.IdStoredProcedure > Constantes.ID_PROCEDURE_ANALISADA).ToList();
    return Ok(_mapper.Map<IEnumerable<EndpointListagemDto>>(objs));
  }

  [HttpGet("api/analysis/endpoints/find-by-filter-async")]
  public async Task<IActionResult> FindByFilterAsync([FromQuery] EndpointFilterDto? filter)
  {
    var objs = await _service.FindByFilterAsync(filter);
    return Ok(_mapper.Map<IEnumerable<EndpointListagemDto>>(objs));
  }

  [HttpPost("api/analysis/endpoints/add")]
  public async Task<IActionResult> Add([FromBody] EndpointDto dto)
  {
    var novo = _mapper.Map<Endpoint>(dto);
    await _service.AddAsync(novo);
    return new CreatedAtRouteResult("GetEndpoint", new {id = novo.Id}, novo);
  }

  [HttpPut("api/analysis/endpoints/update/{id:int:min(1)}")]
  public async Task<IActionResult> Update(int id, EndpointUpdateDto dto)
  {
    try
    {
      var novo = _mapper.Map<Endpoint>(dto);
      await _service.UpdateAsync(id, novo);
      return NoContent();
    }
    catch (Exception e)
    {
      return StatusCode(StatusCodes.Status400BadRequest,
        new ApiResponse(StatusCodes.Status400BadRequest, e.Message));
    }
  }

  [HttpDelete("api/analysis/endpoints/delete/{id:int:min(1)}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _service.DeleteAsync(id);
    return NoContent();
  }

  // [HttpGet("{idProcedure:int:min(1)}", Name = "FindByStoredProcedureId")]
  // public ActionResult<Endpoint> FindByStoredProcedureId (int idProcedure)
  // {
  //   var obj = _service.FindByStoredProcedureId(idProcedure);
  //
  //   if (obj == null)
  //   {
  //     return NotFound(new ApiResponse(404));
  //   }
  //
  //   return Ok(obj);
  // }
}
