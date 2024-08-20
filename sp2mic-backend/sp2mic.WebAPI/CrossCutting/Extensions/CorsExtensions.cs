namespace sp2mic.WebAPI.CrossCutting.Extensions;

public static class CorsExtension
{
  public static void AddCustomCors (this IServiceCollection services, string policyName,
    string origin1, string origin2, string origin3, string origin4, string origin5)
  {
    services.AddCors(options =>
    {
      options.AddPolicy(policyName,
        builder => builder.WithOrigins(origin1, origin2, origin3, origin4, origin5).AllowAnyMethod()
         .AllowAnyHeader().AllowCredentials());
    });
  }
}
