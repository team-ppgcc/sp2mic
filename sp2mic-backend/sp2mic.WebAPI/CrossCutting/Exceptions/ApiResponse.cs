namespace sp2mic.WebAPI.CrossCutting.Exceptions;

public class ApiResponse
{
  public ApiResponse (int statusCode, string? message = null, string? instance = null)
  {
    Status = statusCode;
    Message = message ?? GetDefaultMessageForStatusCode(statusCode);
    Instance = instance;
  }

  private static string? GetDefaultMessageForStatusCode (int statusCode)
  {
    return statusCode switch
    {
      400 => "Request not fulfilled.", 401 => "Without authorization.",
      404 => "Resource not found.", 500 => "Internal server error.", _ => null
    };
  }

  public int Status {get; set;}
  public string? Message {get; set;}
  public string? Instance {get; set;}
}
