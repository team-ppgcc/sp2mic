using Microsoft.AspNetCore.Mvc.Filters;

namespace sp2mic.WebAPI.CrossCutting.Filter;

public class ApiLoggingFilter : IActionFilter
{
  private readonly ILogger<ApiLoggingFilter> _logger;

  public ApiLoggingFilter (ILogger<ApiLoggingFilter> logger)
    => _logger = logger ?? throw new ArgumentNullException(nameof (logger));

  //Executa ANTES da Action
  public void OnActionExecuting (ActionExecutingContext context)
  {
    //_logger.LogInformation("\n##################################################");
    //_logger.LogInformation("### OnActionExecuting: {DateTimeNow}", DateTime.Now.ToLongTimeString());
    //_logger.LogInformation("############################################\n");
  }

  //Executa DEPOIS da Action
  public void OnActionExecuted (ActionExecutedContext context)
  {
    //_logger.LogInformation("\n##################################################");
    //_logger.LogInformation("### OnActionExecuted: {DateTimeNow}", DateTime.Now.ToLongTimeString());
    //_logger.LogInformation("##################################################\n");
  }
}
