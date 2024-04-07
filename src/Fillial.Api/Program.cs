using Microsoft.OpenApi.Models;
using PFilial.BLL.Services;
using PFilial.BLL.Services.Interfaces;
using PFilial.DAL.Middlewares;
using PFilial.DAL.Repositories;
using PFilial.DAL.Repositories.Interfaces;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

string? connection = builder.Configuration.GetConnectionString("PrinterFilServer")
	?? throw new ArgumentNullException(nameof(connection));

builder.Services.AddTransient<IEmployeesRepository, EmployeesRepository>(r => new(connection));
builder.Services.AddScoped<IEmployeesService, EmployeeService>();

builder.Services.AddTransient<IFilialsRepository, FilialsRepository>(r => new(connection));
builder.Services.AddScoped<IFilialsService, FilialsService>();

builder.Services.AddTransient<IPrintersRepository, PrintersRepository>(r => new(connection));
builder.Services.AddScoped<IPrintersService, PrintersService>();

builder.Services.AddTransient<IInstallationsRepository, InstallationsRepository>(r => new(connection));
builder.Services.AddScoped<IInstallationsService, InstallationsService>();

builder.Services.AddTransient<IPrintJobsRepository, PrintJobsRepository>(r => new(connection));
builder.Services.AddScoped<IPrintJobsService, PrintJobsService>();
builder.Services.AddTransient<IPrintJobImporter, PrintJobImporterCSV>();

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
