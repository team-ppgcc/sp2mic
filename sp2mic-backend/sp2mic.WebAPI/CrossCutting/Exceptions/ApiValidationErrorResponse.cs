namespace sp2mic.WebAPI.CrossCutting.Exceptions;

public class ApiValidationErrorResponse : ApiResponse
{
  public ApiValidationErrorResponse ()
    : base(400) { }

  public IEnumerable<string>? Errors {get; set;}
}
