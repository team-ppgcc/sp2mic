using Microsoft.AspNetCore.Mvc;
using Scrutor;
using sp2mic.WebAPI.Context;
using sp2mic.WebAPI.CrossCutting.Exceptions;
using sp2mic.WebAPI.CrossCutting.Filter;
using sp2mic.WebAPI.CrossCutting.Notifications;
using sp2mic.WebAPI.Modulo2.Analise.Repositories.Interfaces;
using sp2mic.WebAPI.Modulo2.Analise.Services.Interfaces;

namespace sp2mic.WebAPI.CrossCutting.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddApplicationDbContext (this IServiceCollection services)
    => services.AddDbContext<DbContextSp2Mic>(ServiceLifetime.Scoped);

  // public static IServiceCollection AddApplicationDbContext (this IServiceCollection services)
  //   => services.AddScoped<DbContext, DbContextSp2Mic>().AddDbContext<DbContextSp2Mic>(options =>
  //   {
  //     options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
  //   });

  public static IServiceCollection AddApplicationLoggingFilter (this IServiceCollection services)
    => services.AddScoped<ApiLoggingFilter>();

  /*public static IServiceCollection
    AddApplicationHandlerExceptionFilter (this IServiceCollection services)
    => services.AddScoped<HandlerExceptionFilterAttribute>();*/

  public static IServiceCollection
    AddApplicationNotificationContext (this IServiceCollection services)
    => services.AddScoped<NotificationContext>();

  public static IServiceCollection AddApplicationServices (this IServiceCollection services)
    => services.Scan(selector => selector.FromAssemblies(Application.Assemblies)
     .AddClasses(filter => filter.AssignableTo(typeof (IApplicationService)))
     .UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces()
     .WithScopedLifetime());

  public static IServiceCollection AddApplicationRepositories (this IServiceCollection services)
    => services.Scan(selector => selector.FromAssemblies(Application.Assemblies)
     .AddClasses(filter => filter.AssignableTo(typeof (IApplicationRepository)))
     .UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces()
     .WithScopedLifetime());

  public static IServiceCollection ConfigureApiExceptionResponse (this IServiceCollection services)
    => services.Configure<ApiBehaviorOptions>(options =>
    {
      options.InvalidModelStateResponseFactory = actionContext =>
      {
        var errors = actionContext.ModelState
         .Where(e => e.Value!.Errors.Count > 0)
         .SelectMany(x => x.Value!.Errors)
         .Select(x => x.ErrorMessage).ToArray();

        var errorResponse = new ApiValidationErrorResponse {Errors = errors};

        return new BadRequestObjectResult(errorResponse);
      };
    });

  // public static IServiceCollection ConfigureMapperProfiles(this IServiceCollection services)
  // {
  //   services.AddAutoMapper(typeof(AtributoProfile).Assembly);
  //   services.AddAutoMapper(typeof(DtoClasseProfile).Assembly);
  //   services.AddAutoMapper(typeof(Endpoint).Assembly);
  //   services.AddAutoMapper(typeof(MicrosservicoProfile).Assembly);
  //   services.AddAutoMapper(typeof(StoredProcedureProfile).Assembly);
  //   services.AddAutoMapper(typeof(VariavelProfile).Assembly);
  // }
}
