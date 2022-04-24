using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformsService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
  if (builder.Environment.IsDevelopment())
  {
    Console.WriteLine("Using InMemoryDatabase");
    options.UseInMemoryDatabase("InMem");
  }
  else
  {
    Console.WriteLine("Using SQLServer");
    options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"));
  }

});
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
  {
    Version = "v1",
    Title = "Platforms API",
    Description = "An ASP.NET Core REST API for managing Platforms"
  });

  // Add code comments to swagger
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

PrepDb.PrepPopulation(app);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
