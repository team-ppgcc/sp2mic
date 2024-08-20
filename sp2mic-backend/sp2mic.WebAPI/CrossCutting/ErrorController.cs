using Microsoft.AspNetCore.Mvc;
using sp2mic.WebAPI.CrossCutting.Exceptions;

namespace sp2mic.WebAPI.CrossCutting;

[Route("errors/{code}")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
  [HttpGet]
  public IActionResult Error (int code) => new ObjectResult(new ApiResponse(code));
}
