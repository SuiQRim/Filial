using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PrinterFil.Api.DataBase;
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
builder.Services.AddDbContext<FilialServerContext>(options =>
	   options.UseSqlServer(builder.Configuration.GetConnectionString("PrinterFilServer")));

builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();
builder.Services.AddScoped<IInstallationsRepository, InstallationsRepository>();
builder.Services.AddScoped<IFilialsRepository, FilialsRepository>();
builder.Services.AddScoped<IPrintersRepository, PrintersRepository>();
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
