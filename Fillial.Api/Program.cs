using Microsoft.OpenApi.Models;
using PrinterFil.Api.Middlewares;
using PrinterFil.Api.Repositories;
using PrinterFil.Api.Repositories.IRepositories;
using PrinterFil.Api.Services;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddSwaggerGen(options =>
{
	string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Сервис"
	});
});
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

string? connection = builder.Configuration.GetConnectionString("PrinterFilServer") ?? throw new ArgumentNullException("Connection is Null");

builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>(provider => new (connection));
builder.Services.AddScoped<IInstallationsRepository, InstallationsRepository>(provider => new (connection));
builder.Services.AddScoped<IFilialsRepository, FilialsRepository>(provider => new (connection));
builder.Services.AddScoped<IPrintersRepository, PrintersRepository>(provider => new(connection));
builder.Services.AddScoped<IPrintJobsRepository, PrintJobsRepository>();

builder.Services.AddTransient<IPrintingJobImporter, PrintingJobImporterCSV>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
