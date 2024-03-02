using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PrinterFil.Api.DataBase;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddSwaggerGen(options =>
{
	string xmlFilename =
	$"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
	xmlFilename));

	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Сервис"
	});
});


builder.Services.AddDbContext<FiilalServerContext>(options =>
	   options.UseSqlServer(builder.Configuration.GetConnectionString("PrinterFilServer")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
