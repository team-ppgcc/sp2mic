namespace sp2mic.WebAPI.CrossCutting.Notifications;

public class Result<T> where T : class
{
  public bool Succeeded {get; set;}
  public T? Data {get; set;}
  public List<string>? Errors {get; set;}
}
