using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using sp2mic.WebAPI.CrossCutting.Extensions;
using sp2mic.WebAPI.CrossCutting.Filter;
using sp2mic.WebAPI.CrossCutting.Middleware;
using sp2mic.WebAPI.Modulo2.Analise.MapperProfiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddApplicationLoggingFilter();

//builder.Services.AddApplicationHandlerExceptionFilter();
//builder.Services.//AddApplicationException();
builder.Services.AddApplicationNotificationContext();
builder.Services.AddApplicationDbContext();
// builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

builder.Services.AddAutoMapper(typeof(AtributoProfile).Assembly);
builder.Services.AddAutoMapper(typeof(DtoClasseProfile).Assembly);
builder.Services.AddAutoMapper(typeof(EndpointProfile).Assembly);
builder.Services.AddAutoMapper(typeof(MicrosservicoProfile).Assembly);
builder.Services.AddAutoMapper(typeof(StoredProcedureProfile).Assembly);
builder.Services.AddAutoMapper(typeof(VariavelProfile).Assembly);
builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomCors("CorsPolicy", "http://localhost:4401","http://localhost:4200", "http://localhost",
  "https://sp2mic.stacktecnologia.com.br", "http://sp2mic.stacktecnologia.com.br");

builder.Services.AddMvc(o =>
{
  o.EnableEndpointRouting = true;
  o.Filters.Add<NotificationFilter>();
}).AddNewtonsoftJson(o =>
{
  o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
  o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
  //o.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
  o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});

builder.Services.ConfigureApiExceptionResponse();

builder.Services.Configure<FormOptions>(o =>
{
  o.ValueLengthLimit = int.MaxValue;
  o.MultipartBodyLengthLimit = int.MaxValue;
  o.MemoryBufferThreshold = int.MaxValue;
});

if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Resources")))
{
  Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Resources"));
}

if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "stored-procedures-upload")))
{
  Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "stored-procedures-upload"));
}

if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "generated-microservices")))
{
  Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "generated-microservices"));
}

if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "generated-microservices-files")))
{
  Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "generated-microservices-files"));
}

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
//app.UseApiExceptionHandler(app.Services.GetRequiredService<ILoggerFactory>());
app.UseStatusCodePagesWithReExecute("/errors/{0}");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseAuthorization();

app.UseCors("CorsPolicy");

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
  FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
  RequestPath = new PathString("/Resources")
});

app.MapControllers();

app.Run();
