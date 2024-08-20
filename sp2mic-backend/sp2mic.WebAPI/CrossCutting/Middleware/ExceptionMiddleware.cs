using System.Data.Common;
using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using sp2mic.WebAPI.CrossCutting.Exceptions;
using sp2mic.WebAPI.CrossCutting.Extensions;

namespace sp2mic.WebAPI.CrossCutting.Middleware;

public class ExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionMiddleware> _logger;

  public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError("Unexpected error: {S}", ex.Message);
      context.Response.ContentType = "application/json";
      var apiException
        = new ApiException(StatusCodes.Status400BadRequest)
        {
          Instance = context.Request.HttpContext.Request.Path, Message = ex.Message
        };
      switch (ex)
      {
        case ApplicationException applicationException:
          apiException.Message = applicationException.Message;
          break;
        case DbException:
          apiException.Message
            = $"An error occurred while trying to access the database schema {Constantes.Schema}";
          break;
        case DbUpdateException dbUpdateException:
          const string? textoBase = "An error occurred when trying to access the database.";
          var texto = VerificarErroBanco(dbUpdateException, textoBase);
          apiException.Message = texto;
          if (textoBase.Equals(texto))
          {
            _logger.LogError("Unexpected error: {S}", ex.Demystify().ToString());
          }
          break;
        case BadHttpRequestException badHttpRequestException:
          apiException.Status = StatusCodes.Status400BadRequest;
          apiException.Message = badHttpRequestException.Message;
          break;
        default:
          apiException.Status = (int) HttpStatusCode.InternalServerError;
          apiException.Details = ex.Demystify().ToString();
          break;
      }
      context.Response.StatusCode = apiException.Status;
      var json = JsonConvert.SerializeObject(apiException,
        SerializerSettings.JsonSerializerSettings);
      await context.Response.WriteAsync(json);
    }
  }

  private static string? VerificarErroBanco(DbUpdateException e, string? texto)
  {
    if (e.InnerException == null)
    {
      return texto;
    }
    if (e.InnerException.Message.Contains("uk_nodtoclasse_idmicrosservico"))
    {
      return "ClassDTO already exists in this microservice. Please choose another name.";
    }
    if (e.InnerException.Message.Contains("uk_nomicrosservico"))
    {
      return "Microservice name already existing.";
    }
    if (e.InnerException.Message.Contains("uk_notabela"))
    {
      return "Table name already existing.";
    }
    if (e.InnerException.Message.Contains("uk_noatributo_iddtoclasse"))
    {
      return "Attribute name already existing in DtoClasse.";
    }
    if (e.InnerException.Message.Contains("uk_noschema_nostoredprocedure"))
    {
      return "Schema + Stored Procedure Name already existing.";
    }
    if (e.InnerException.Message.Contains("pk_storedprocedure_tabela"))
    {
      return "Existing association.";
    }
    if (e.InnerException.Message.Contains("pk_endpoint_tabela"))
    {
      return "Existing association.";
    }
    if (e.InnerException.Message.Contains("pk_endpoint_variavel"))
    {
      return "Existing association.";
    }
    if (e.InnerException.Message.Contains("pk_comando_variavel"))
    {
      return "Existing association.";
    }
    return e.InnerException.Message.Contains("violates not-null constraint") ?
      "Operation Denied. Form has some mandatory field not filled out." : texto;
  }
}
